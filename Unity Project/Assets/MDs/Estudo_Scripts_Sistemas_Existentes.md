# Estudo dos Scripts e Sistemas Existentes

Documento baseado na leitura de `Assets/Script` e no guia `Assets/MDs/Guia_Padroes_Unity.md`.

Este estudo identifica o que ja existe no projeto, quais sistemas podem ser aproveitados para `Instanct Action`, quais precisam de ajustes e quais ainda nao atendem diretamente ao escopo da PoC.

## Visao geral da base atual

A pasta `Assets/Script` possui uma base reutilizavel com sistemas de:

- Vida, dano simples e eventos de ciclo de vida.
- Projetil com movimento, gravidade e colisao por `SphereCastNonAlloc`.
- Moedas de partida.
- Requisitos/desbloqueios.
- Loading por Addressables.
- Object Pool.
- Singleton e SceneSingleton.
- Acoes genericas acionaveis.
- Interacao.
- Tiers/evolucao visual por ScriptableObjects.
- Interfaces para contratos gerais.

A base e util, mas ainda nao representa o gameplay especifico da PoC. Ela deve ser tratada como infraestrutura inicial, nao como arquitetura completa do jogo.

## Aderencia ao Guia de Padroes Unity

Pontos alinhados:

- Existem `Singleton<T>` e `SceneSingleton<T>` separados.
- Managers locais usam prefixo `MN_`, como `MN_Coins` e `MN_Requirements`.
- Managers globais usam prefixo `SIS_`, como `SIS_LoadingSceneChannel` e `SIS_GameSession`.
- Ha uso de interfaces para reduzir dependencia direta.
- Ha uso de ScriptableObjects para dados (`RequirementSO`, `LoadingTaskSOBase`, evolucoes visuais).
- Ha uso de Addressables no sistema de loading e em alguns dados visuais.
- Eventos `Action` aparecem em vida, moedas, requisitos e loading.

Pontos a ajustar:

- Nem todos os scripts estao dentro de namespace `Scripts.*`.
- Alguns nomes de variaveis nao seguem o padrao privado com `_`.
- Existem comentarios em portugues dentro de scripts; o guia pede codigo em ingles.
- Alguns metodos publicos usam camelCase (`warmPool`, `spawnObject`, `releaseObject`) em vez de PascalCase.
- Existem dois modelos de Singleton: um em `Assets/Script/Singleton` e outro em `Assets/Script/Scripts Pool/Singleton.cs`.
- Alguns scripts usam referencias diretas sem validacao forte de null.
- Alguns scripts usam tags por string configuravel, o que pode ser aceitavel, mas precisa ser bem controlado.

## Sistema de vida

Arquivos principais:

- `Abstract/Health/HealthBasics.cs`
- `Health/HealthManager.cs`
- `Health/HealthBar.cs`
- `Health/DynamicHealthRepairUI.cs`
- `Health/OnDeath_UnityEvent.cs`
- `Health/OnRespawn_UnityEvent.cs`
- `Health/OnHit_Particles.cs`
- `Health/OnHit_VisualEffect.cs`
- `Health/KillAfterTime_OnEnable.cs`
- `Health/LifeCycle_HandleCanvasActiveState.cs`
- `Health/StatusType.cs`

### O que existe

`HealthBasics` guarda:

- Bloqueio.
- Vida maxima.
- Estado hittable.
- Estado killable.
- Vida atual.
- Estado vivo/morto.

`HealthEvents` adiciona eventos:

- `OnDeath`.
- `OnRespawn`.
- `OnHealthUpdate`.
- `OnMaxHealthValueUpdated`.
- `OnBlockUpdate`.

`HealthManager` implementa:

- `IHittable`.
- `IHealable`.
- Receber dano.
- Curar.
- Matar.
- Resetar vida.
- Soltar objeto no pool ao morrer, se configurado.

### Pontos fortes

- E um bom ponto de partida para vida de player, inimigos e objetos destrutiveis.
- Ja tem eventos de vida, morte, hit e respawn.
- Ja permite integrar UI, feedback visual e pooling.
- Usa uma abstracao `IHittable`, evitando depender diretamente de `HealthManager`.

### Limites para a PoC

O sistema atual recebe dano como `float` ou `int`. Ele ainda nao entende:

- Dano fisico.
- Dano magico.
- Dano real.
- Resistencias.
- Critico.
- Fonte do dano.
- Escalamento com status.
- Roubo de vida.
- Vampirismo magico.

Recomendacao:

- Nao colocar toda essa logica dentro de `HealthManager`.
- Criar um `DamageResolver` ou equivalente para calcular dano final.
- O `HealthManager` deve receber o valor final ou um pacote de dano ja resolvido.
- Considerar uma nova interface, como `IDamageable`, para dano tipado.

### Cuidados identificados

- `HealthBar.Awake()` tenta buscar componente, mas nao atribui o resultado porque chama `healthObject.GetComponent<HealthBasicsEvents>()` sem salvar.
- Alguns scripts assumem que referencias existem e podem gerar null reference.
- `HealthUnityEvent` e declarado vazio e talvez nao seja necessario.
- `StatusType.cs` existe, mas ainda nao parece conectado ao sistema de status da PoC.

## Sistema de projetil

Arquivo principal:

- `Projectile/Projectile.cs`

### O que existe

`Projectile` controla:

- Ativacao/desativacao.
- Gravidade.
- Velocidade.
- Dano inteiro.
- Tag do alvo.
- LayerMask de colisao.
- LayerMask de chao.
- Raio do projetil.
- `SphereCastNonAlloc` para detectar impactos.
- Release por Object Pool.
- Evento `OnHitDamageableTarget`.

### Pontos fortes

- Ja evita alocacoes com `SphereCastNonAlloc`.
- Ja e compativel com pool.
- Ja pode ser usado para Fire Ball e inimigo ranged.
- Ja permite gravidade customizada.

### Limites para a PoC

Hoje o projetil:

- Usa `int _damage`.
- Chama `IHittable.Hit(_damage)`.
- Filtra alvo por tag string.
- Nao recebe tipo de dano.
- Nao recebe dados de escalamento.
- Nao diferencia alvo principal, area, pierce ou multi hit.

Recomendacao:

- Adaptar para receber um `DamageData` ou `ResolvedDamage`.
- Trocar `_damage` por um pacote de dano.
- Manter suporte a pool.
- Considerar interface de alvo em vez de depender apenas de tag.
- Para Fire Ball da PoC, o projetil ja serve como base com pequenas alteracoes.

## Sistema de moedas

Arquivo principal:

- `Coins/MN_Coins.cs`

### O que existe

`MN_Coins` e um `SceneSingleton`.

Funcionalidades:

- Guardar moedas.
- Adicionar moedas.
- Remover moedas.
- Verificar se pode pagar.
- Controlar inflacao.
- Disparar eventos de atualizacao, adicao, remocao e inflacao.

### Pontos fortes

- Esta alinhado com manager local de cena (`MN_`).
- Eventos permitem atualizar UI sem acoplamento forte.
- Inflacao pode ser util para loja e rerolls.

### Limites para a PoC

A PoC exige principalmente XP e upgrades; moedas aparecem no projeto completo e podem entrar se a loja de atributos for testada cedo.

Recomendacao:

- Reaproveitar se moedas entrarem na PoC.
- Separar moedas de XP.
- Nao usar moedas como requisito obrigatorio para level up.
- Revisar `RemoveCoins`, pois `OnRemove` recebe o valor original, nao o valor final com inflacao.

## Sistema de requisitos

Arquivos principais:

- `Requeriments/MN_Requirements.cs`
- `Requeriments/Scriptable Base/RequirementSO.cs`
- `Requeriments/Saver/RequirementsSaveData.cs`

### O que existe

`RequirementSO` possui:

- `ID`.
- `DisplayName`.
- `Description`.

`MN_Requirements` controla:

- Requisitos desbloqueados.
- Verificacao de requisito unico.
- Verificacao de lista de requisitos.
- Evento ao desbloquear.
- Exportacao/importacao de lista salva.

### Pontos fortes

- Muito util para regras de upgrades.
- O conceito de `ID` combina com o PDF do projeto completo.
- Pode apoiar bloqueios como "upgrade X impede Y" ou "upgrade de habilidade so aparece se habilidade existir".

### Limites para a PoC

Na PoC nao ha regras avancadas de semi aleatoriedade nem tiers. Mesmo assim, vale preservar esse sistema para evolucao futura.

Recomendacao:

- Usar como base conceitual para validadores de upgrade.
- Corrigir grafia de pasta/classe em algum momento futuro: `Requeriments` deveria ser `Requirements`.
- Evitar acoplar todo upgrade a `MN_Requirements`; alguns requisitos podem ser runtime, como arma equipada ou habilidade possuida.

## Sistema de loading e Addressables

Arquivos principais:

- `Loading Scene/SIS_LoadingSceneChannel.cs`
- `Loading Scene/LoadingSceneHandler.cs`
- `Loading Scene/LoadingTaskSOBase.cs`
- `Loading Scene/AssetReferenceLoadingTaskSOBase.cs`
- `Loading Scene/LoadingSceneHandler_LoadingBar.cs`
- `Loading Scene/LoadingSceneHandler_CleanSession.cs`
- `Loading Scene/LoadScene_TryLoad.cs`
- `Loading Scene/SIS_GameSession.cs`
- `Loading Scene/GameSessionCleaner.cs`

### O que existe

`SIS_LoadingSceneChannel`:

- E um Singleton persistente.
- Guarda a cena Addressable a carregar.
- Guarda fila de tarefas Addressables.
- Abre a cena de loading.
- Permite carregar cena limpando sessao.

`LoadingSceneHandler`:

- Carrega tarefas Addressables.
- Separa tarefas sync/async.
- Executa tarefas.
- Carrega a cena final por Addressables.
- Atualiza progresso.
- Ativa a cena ao final.

`LoadingTaskSOBase`:

- Base para tarefas de loading por ScriptableObject.
- Permite execucao async ou sync.
- Permite restricao de tipo de carregamento.

### Pontos fortes

- Excelente encaixe com a PoC, que pede Addressables desde o inicio.
- Permite preparar dados antes de entrar na cena.
- Pode evoluir para load de waves, inimigos, armas, habilidades e upgrades.
- Ja considera diferencas de WebGL.

### Limites e cuidados

- `UpdateAddressableProgress` aguarda o fim, mas nao usa percentual real do handle.
- A barra de loading recebe progresso por etapas, nao por progresso real de assets.
- `SIS_GameSession` ainda esta bem vazio.
- Alguns `using` parecem nao usados.

Recomendacao:

- Manter esse sistema como base de carregamento.
- Para PoC, usar cena direta se necessario, mas nao abandonar Addressables.
- Evoluir loading tasks para carregar bancos de dados de armas, inimigos, habilidades e upgrades.

## Object Pool

Arquivos principais:

- `Scripts Pool/ObjectPool.cs`
- `Scripts Pool/ObjectPoolContainer.cs`
- `Scripts Pool/PoolManager.cs`
- `Scripts Pool/Singleton.cs`

### O que existe

`ObjectPool<T>`:

- Guarda containers.
- Cria itens por factory.
- Reutiliza itens livres.
- Cria novos se a pool estiver cheia.

`PoolManager`:

- Gera pools por prefab.
- Spawna objetos.
- Desativa e devolve objetos.
- Expoe API estatica.

### Pontos fortes

- Muito util para projeteis, inimigos, particulas e efeitos.
- Ja integrado com `Projectile` e `HealthManager`.
- Pode reduzir custo de `Instantiate`/`Destroy`.

### Limites e cuidados

- Metodos publicos internos usam camelCase.
- Existe um `Singleton<T>` dentro da pasta `Scripts Pool`, diferente do `Singleton<T>` global.
- `PoolManager` herda de `ClassInstance<PoolManager>`, que pode criar objeto automaticamente se nenhum existir.
- Isso pode fugir do controle de hierarquia recomendado no guia, pois managers deveriam estar em `_System` ou `_Global`.

Recomendacao:

- Usar na PoC, especialmente para projeteis.
- Considerar padronizar o singleton usado pelo pool.
- Garantir que o `PoolManager` esteja em objeto claro na hierarquia.

## Singleton e SceneSingleton

Arquivos principais:

- `Singleton/Singleton.cs`
- `Singleton/SceneSingleton.cs`

### O que existe

`Singleton<T>`:

- Garante instancia unica.
- Destroi duplicatas.
- Usa `DontDestroyOnLoad`.

`SceneSingleton<T>`:

- Garante instancia unica por cena.
- Nao persiste entre cenas.
- Limpa `Instance` no `OnDestroy`.

### Pontos fortes

- Esta alinhado ao guia.
- Deve ser a base preferencial para managers `SIS_` e `MN_`.

### Recomendacao

- Preferir estes singletons aos da pasta `Scripts Pool`.
- Usar `Singleton` apenas quando o sistema realmente precisa sobreviver entre cenas.
- Usar `SceneSingleton` para sistemas de partida, como waves, XP, upgrades e moedas.

## Do Action

Arquivos principais:

- `Abstract/Do Action/DoActionBase.cs`
- `Do Action/General/DoAction_DefineActiveState.cs`
- `Do Action/General/DoAction_DefineActiveStateOfCanvas.cs`
- `Do Action/Animation/DoAction_AnimSetBool.cs`
- `Do Action/Animation/DoAction_RandomAnimation.cs`

### O que existe

`DoActionBase` define um contrato simples:

- `Execute()`.

Implementacoes atuais:

- Ativar/desativar GameObject.
- Ativar/desativar Canvas.
- Setar bool em Animator.
- Tocar animacao aleatoria.

### Pontos fortes

- Pode ser util para botoes, eventos simples e feedbacks.
- Segue ideia de encapsular uma acao.

### Limites para a PoC

Nao deve virar o principal sistema de combate. Ataques, habilidades e upgrades precisam de modelos proprios, dados e validacao.

Recomendacao:

- Usar para acoes simples de UI/feedback.
- Evitar usar `DoAction` para orquestrar regras grandes de gameplay.

## Interactable

Arquivos principais:

- `Interactable/Interactable.cs`
- `Interface/Interactable/IInteractable.cs`

### O que existe

`Interactable`:

- Dispara `OnInteract`.
- Retorna o proprio GameObject.
- Loga a interacao.

### Uso possivel

Pode ser base para:

- Baus.
- Coletaveis.
- Objetos de loja.
- Interacoes futuras.

Na PoC, provavelmente nao e prioridade, a menos que baus/moedas entrem cedo.

## Tiers e evolucao visual

Arquivos principais:

- `Tier/Enum/Tier.cs`
- `Tier/Scriptable Base/Visual/CreatureEvolutionDataSO.cs`
- `Tier/Scriptable Base/Visual/EvolutionPathSO.cs`
- `Tier/Scriptable Base/Visual/TextureVariantSO.cs`
- `Interface/Tier/IHasTier.cs`
- `Interface/Tier/Visual/IReadOnlyEvolutionData.cs`
- `Interface/Tier/Visual/IHasEvolutionVisualInitialization.cs`

### O que existe

Ha um sistema voltado para evolucao visual:

- `Tier` de 1 a 10.
- Caminho de evolucao.
- Modelo visual inicial.
- Conexoes de evolucao por tier.
- Variantes de textura e material via Addressables.

### Pontos fortes

- Ja usa ScriptableObjects.
- Ja usa Addressables.
- Pode servir como referencia de estrutura data-driven.

### Limites para Instanct Action

O tier do PDF do projeto completo e tier de aprimoramento:

- Comum.
- Normal.
- Epico.
- Lendario.

Isso nao e a mesma coisa que `Tier_1` a `Tier_10` de evolucao visual.

Recomendacao:

- Nao reutilizar diretamente esse enum para upgrades.
- Criar um enum proprio para raridade/tier de upgrade quando o sistema completo for implementado.
- Reaproveitar apenas o padrao de ScriptableObject e Addressables.

## Interfaces gerais

Pastas principais:

- `Interface/General`
- `Interface/Spawner`
- `Interface/Camera`
- `Interface/Animation`
- `Interface/Effects`
- `Interface/Nameable`
- `Interface/Selectable`

### O que existe

Contratos identificados:

- Estado ativo.
- Validacao de acao.
- Identificacao por ID.
- Timer salvavel.
- Spawner.
- Spawner Addressable com preview.
- Dados de item posicionavel.
- Camera follow.
- Animacao/velocidade.
- Efeitos.
- Nomeavel.
- Selecionavel.

### Pontos fortes

- Mostram uma direcao boa de arquitetura por contratos pequenos.
- Algumas interfaces podem ser reaproveitadas em sistemas de upgrades, spawners e UI.

### Cuidados

- Nem todas usam namespace.
- Algumas possuem pequenos erros de grafia (`OnResquestValidation`, `ICursorVisibilyHandler`, `OnReadyToEnvolve`).
- Algumas parecem vir de outro projeto ou de outro dominio.

Recomendacao:

- Reaproveitar apenas quando o contrato fizer sentido real para a PoC.
- Corrigir grafia apenas se o script for usado ativamente, para evitar refatoracao ampla sem necessidade.

## Sistemas mais aproveitaveis para a PoC

Usar quase diretamente:

- `Singleton<T>`.
- `SceneSingleton<T>`.
- `LoadingSceneHandler`.
- `SIS_LoadingSceneChannel`.
- `LoadingTaskSOBase`.
- `PoolManager`, com atencao ao singleton proprio.

Usar com adaptacao:

- `HealthManager`.
- `HealthBasics`.
- `IHittable`.
- `Projectile`.
- `HealthBar`.
- `OnHit_Particles`.
- `OnHit_VisualEffect`.
- `MN_Coins`.

Usar como inspiracao:

- `MN_Requirements`.
- `RequirementSO`.
- `DoActionBase`.
- Sistema de tier/evolucao visual.
- Interfaces de spawner e identificacao.

Baixa prioridade para PoC:

- `DynamicHealthRepairUI`.
- `Interactable`, se nao houver baus/interacao.
- Evolucao visual por tier.
- Save/load de requisitos.

## Sistemas ausentes para a PoC

Ainda sera necessario criar:

- Sistema de status do jogador.
- Modelo de dano tipado.
- Resolvedor de dano.
- Resistencias de inimigos.
- Sistema de armas.
- Sistema de combo.
- Sistema de hitbox por ataque.
- Controle de direcao do player.
- Dash com cooldown por lado.
- Sistema de habilidades.
- Loadout e substituicao de habilidades.
- Passivas por eventos.
- Sistema de XP.
- Sistema de level up.
- Sistema de selecao de upgrades.
- Upgrades de status.
- Upgrades de troca de arma.
- Sistema de waves.
- Behaviour Tree dos inimigos.
- Dados de inimigos.

## Recomendacoes de integracao

### Dano

Criar tipos novos:

- `DamageType`.
- `DamageData`.
- `DamageScalingData`.
- `ResolvedDamage`.
- `DamageResolver`.

Manter `HealthManager` simples:

- Recebe dano final.
- Atualiza vida.
- Dispara eventos.

### Status

Criar um componente central:

- `PlayerStats`.

Ele deve:

- Guardar valores runtime.
- Aplicar modificadores absolutos.
- Aplicar modificadores percentuais.
- Emitir eventos de alteracao.
- Permitir modificadores temporarios.

### Armas

Criar:

- `WeaponSO`.
- `WeaponAttackData`.
- `WeaponHandler`.
- `AttackHitbox`.

Usar eventos:

- `OnAttackStarted`.
- `OnAttackHit`.
- `OnComboAdvanced`.
- `OnComboReset`.

### Habilidades

Criar:

- `AbilitySO`.
- `AbilityRuntime`.
- `AbilityLoadout`.
- Interfaces para habilidade ativa/passiva.

Habilidades passivas devem ouvir eventos, nao ficar presas em `Update`.

### Upgrades

Criar:

- `UpgradeSO`.
- `StatUpgradeSO`.
- `WeaponUpgradeSO`.
- `UpgradeSelectionManager`.

Para o futuro:

- Requisitos.
- Bloqueios.
- Raridade/tier.
- Regras de repeticao.

## Riscos tecnicos observados

- Evoluir `IHittable` diretamente pode quebrar muitos scripts existentes.
- Alterar `Projectile` sem preservar API atual pode quebrar prefabs configurados.
- Dois singletons diferentes podem gerar comportamento inesperado.
- Scripts sem namespace podem colidir com nomes futuros.
- Tags por string podem gerar erro silencioso se configuradas errado.
- Sistemas vindos de outro contexto podem carregar conceitos que nao pertencem ao core da PoC.

## Plano de limpeza gradual

Nao e recomendado refatorar tudo agora. Melhor caminho:

1. Criar sistemas novos da PoC em namespaces `Scripts.Combat`, `Scripts.Player`, `Scripts.Enemies`, `Scripts.Progression` e `Scripts.Upgrades`.
2. Integrar com scripts existentes por adaptadores pequenos.
3. Preservar `HealthManager` enquanto o modelo de dano nasce.
4. Adaptar `Projectile` apenas quando Fire Ball ou inimigo ranged forem implementados.
5. Padronizar singletons quando o `PoolManager` for usado em gameplay real.
6. Corrigir nomes/grafias apenas em arquivos tocados pelo desenvolvimento da PoC.

## Conclusao

A base existente e boa como kit inicial de infraestrutura, principalmente para vida, pooling, loading e Addressables. O jogo, porem, ainda precisa dos sistemas especificos de combate, progressao e upgrades descritos na PoC.

O melhor uso desses scripts e aproveita-los como fundacao, mantendo a arquitetura do guia: sistemas pequenos, dados por ScriptableObject, managers apenas quando fizer sentido, contratos por interface e eventos para comunicacao entre gameplay, UI e feedbacks.
