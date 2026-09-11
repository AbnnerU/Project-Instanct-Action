# Project Instanct Action: entendimento do projeto e estado da PoC

Data: 11/09/2026.

## 1. Objetivo e limites deste estudo

Consolidar o entendimento do jogo, da implementacao atual e das preocupacoes que motivam uma possivel PoC parte 2. Este documento nao define seu escopo nem autoriza mudancas de gameplay.

Fontes principais:

- `C:/Users/abnne/OneDrive/Geral/Desktop/Nova pasta/ProjectInstanctAction.html`: visao completa, recorte da PoC e consideracoes finais.
- `Assets/MDs/Guia_Padroes_Unity.md`: convencoes de organizacao e programacao.
- Scripts atuais em `Assets/Script`, dados de input e organizacao dos assets.
- Leitura do contexto, hierarquia e componentes da `Assets/Scenes/SampleScene.unity` pelo KitWright MCP.
- Historico fornecido pelo autor, como contexto de decisoes, sem presumir que toda implementacao antiga continua igual.

Legenda usada no texto:

- **Confirmado:** encontrado no codigo, configuracao ou documento indicado.
- **Relato:** comportamento percebido pelo autor, ainda sem reproducao nesta analise.
- **Hipotese:** explicacao tecnica plausivel que precisa de teste.
- **Proposta:** consideracao de design ainda nao implementada/aprovada como proxima tarefa.

Nao entrei em Play Mode neste levantamento. Portanto, nao afirmo ter medido latencia, reproduzido os bugs relatados ou avaliado pessoalmente a sensacao dos golpes. A consulta inicial do Editor nao mostrou erros de compilacao; uma tentativa de inspecao por `execute_code` falhou por timeout do compilador da ferramenta. As consultas dedicadas de hierarquia e propriedades funcionaram. Nenhum script, prefab, cena ou balanceamento foi alterado.

## 2. Identidade do jogo

O nucleo e sobreviver a inimigos que chegam dos dois lados, usando ataques direcionais, dash e habilidades, enquanto a build muda ao longo da partida.

O personagem e 3D, mas a apresentacao e lateral, como um jogo 2D. **Nao existe caminhada livre do player.** O dash e a locomocao principal; deslocamentos de ataques e habilidades sao movimentos associados a acoes, nao um controle de andar convencional.

O interesse do jogo esta na combinacao de posicionamento, decisao de quando atacar/esquivar, leitura dos inimigos e sinergias de aprimoramentos. Melhorar apenas a quantidade de conteudo nao valida necessariamente esse nucleo.

O documento aponta PC/Steam primeiro, navegador/Crazy Games posteriormente e mobile em uma etapa futura. A referencia de publico e de jogadores de soulslike, MOBA, hack and slash e roguelike, aproximadamente entre 15 e 24 anos.

### Visao completa, alem da PoC

- Inimigos corpo a corpo e a distancia, com variacoes de atributos, alcance, resistencias e combos.
- Chefes periodicos e progressao de dificuldade.
- Armas com identidade propria, diferentes sequencias, animacoes, alcances e escalonamentos.
- Tres slots de habilidades ativas, uma ultimate e passivas sem um limite equivalente de slots.
- Ultimate carregada por dano, nao apenas por recarga convencional.
- Aprimoramentos de status, armas, habilidades, modificacoes especificas de armas/habilidades e economia.
- Tiers, rerolls, restricoes entre ofertas e regras de elegibilidade conforme a build.
- Moedas da partida, compras e drops; nao tratar a existencia de `MN_Coins` como prova de uma loja completa.
- Modo normal e uma ideia futura de Pantheon.
- Defesa, perfect block e stamina aparecem como possibilidade, nao como mecanica obrigatoria ja decidida.

## 3. Recorte da primeira PoC

O objetivo original era provar o ciclo de combate e crescimento: bater/esquivar, eliminar inimigos, ganhar XP, escolher aprimoramento e continuar.

Inclui danos fisico/magico/real, resistencias, atributos, dois tipos de inimigo, armas com ritmos diferentes, combos, dash, aprimoramentos de status, troca de armas, habilidades e spawn continuo. Addressables faz parte da arquitetura desde cedo. Menus completos e acabamento de produto nao eram a prioridade.

As quatro habilidades de referencia sao:

| Habilidade de design | Comportamento |
| --- | --- |
| Magic Attack, passiva | Dano adicional magico a cada terceiro ataque, com base e escalonamentos. |
| Heavy Attack, passiva | Buff temporario de PF associado ao dash, renovando a duracao. |
| Fire ball, ativa | Projetil na direcao de disparo, com dano configuravel. |
| Estocada Perfurante, ativa | Avanco ofensivo atravessando inimigos e causando dano fisico. |

Os scripts foram generalizados em vez de receberem obrigatoriamente os nomes dessas habilidades. Essa decisao deve ser preservada quando fizer sentido: dados e prefabs configuram o efeito concreto.

## 4. Cena que existe hoje

Unity 6000.5.4f1, produto `Project Instanct Action`, cena ativa `SampleScene`, inspecionada em Edit Mode e inicialmente sem alteracoes pendentes na cena.

Estrutura observada:

- `_Main`: camera, luz e volume.
- `_Scenario`: chao de teste.
- `_System`: managers, Cinemachine, NavMesh e apoio de UI.
- `_Canvas`: XP/nivel, atributos, dois indicadores de dash, dois slots visuais de habilidades e tres cards de encantamentos.
- `Player`: fisica, direcao, combate, dash, vida, recepcao de dano, reacoes, cura e aplicadores de status.
- `Player/Y Bot@Punching`: Animator e `ComboAnimationEventReceiver`.
- Filhos do player para origem da hitbox, pontos de spawn e containers separados de habilidades passivas/ativas.
- Containers para popups, inimigos e projeteis.

Configuracoes confirmadas no Editor:

| Item | Configuracao |
| --- | --- |
| Dash | Distancia 3; duracao 0.465 s; cooldown esquerdo/direito 0.5 s. |
| Movimento do combo | Transform de movimento e referencia de direcao apontam para o Player. |
| Animator do modelo | Root motion desligado; Update Mode Normal; Cull Update Transforms. |
| Arma inicial | `Hand_WeaponLoadoutSO`, equipada no Start pelo `MN_PlayerWeapon`. |
| Combo serializado no player | `HeavyHammer_ComboAttackSetSO`; nao confundir com o conjunto efetivo apos equipar o loadout inicial. |
| Ondas | Duas referencias de inimigos; dois pontos; maximo 10 vivos; 3 iniciais; intervalo 3.5 s. |
| Pool de ondas | Aquecimento inicial desativado; reset de vida ao spawn ativado. |

O asset `Assets/Input/Inputs.inputactions` possui os seguintes bindings:

| Acao | Input |
| --- | --- |
| Attack Right / Attack Left | Botao direito / esquerdo do mouse. |
| Dash Right / Dash Left | D / A. |
| Ability 1 / Ability 2 | Q / E. |

Portanto, A/D para virar e toque duplo para dash ainda sao propostas de alteracao desse esquema, nao uma descricao do controle atual.

## 5. Mapa dos sistemas

### Combate, direcao e animacao

`ComboControllerBase` contem o fluxo compartilhado entre player e inimigos: ataque atual, janela de combo, proximo ataque solicitado, animacao, hitbox, avanco e cancelamento. `PlayerComboController` adiciona inputs direcionais; o inimigo solicita ataques pela IA.

Fluxo simplificado:

1. Input/IA solicita ataque.
2. A direcao e escolhida antes de iniciar o golpe.
3. O controller inicia a rotina de avanco e chama a animacao.
4. Animation Events executam a hitbox, abrem/fecham a janela e finalizam o golpe.
5. Havendo continuacao aceita, inicia o proximo indice; caso contrario encerra o combo e solicita idle.

Eventos usados: `Attack_ExecuteHitbox`, `Attack_OpenComboWindow`, `Attack_CloseComboWindow` e `Attack_Finish`, encaminhados por `ComboAnimationEventReceiver`.

O input de continuacao nao muda a direcao do golpe em andamento. A direcao armazenada passa a valer ao iniciar o proximo golpe. Depois de haver um ataque enfileirado, novos comandos podem atualizar essa direcao ate a transicao.

### Hitbox e dano

`AttackHitbox` consulta `Physics.OverlapBoxNonAlloc` no instante do evento. Nao cria necessariamente um GameObject com collider e nao acompanha automaticamente todo o arco da arma ao longo de varios frames.

O volume usa origem, offset, meia-extensao, rotacao opcional da origem, mascara e limite de alvos do ataque. A deduplicacao vale para cada chamada de execucao da hitbox. Ja existem desenhos de depuracao da configuracao e do golpe executado.

Fluxo principal: `DamageData` + informacoes do atacante -> `DamageRequest` -> `IDamageReceiver`/`DamageReceiver` -> `DamageResolver` -> `IHittable`/`HealthManager` -> `DamageResult` e eventos.

`DamageReceiver` preserva a separacao entre receber um dano contextualizado e simplesmente administrar vida. Ha um caminho legado de fallback para `IHittable` no ataque, mas ele nao e equivalente a passar por toda a recepcao de dano tipado.

O calculo combina dano base, PF e PM, considera critico e aplica resistencia correspondente. Dano real ignora resistencia; status de resistencia ausente nao deve reduzir o dano.

`HealOnDamageDealt` escuta `IDamageDealer`, filtra tipos e cura uma porcentagem de `DamageResult.FinalDamage`. Ha dois componentes no player, permitindo separar life steal e vamp. Esse valor e o resultado informado pelo pipeline, nao uma garantia automatica de ser exatamente a diferenca de HP antes/depois em casos como overkill.

### Status e apresentacao

`MN_PlayerStatus` centraliza atributos. `StatusInfoSourceBase` desacopla consumidores da origem; `StatusInfoByMN_PlayerStatus` adapta o manager. `StatusData` fornece notificacoes de alteracao.

Aplicadores conectam atributos a vida maxima e velocidade de ataque. `StatusInfoTextView` conecta atributos a textos. Popups, barras e indicadores de recarga sao consumidores visuais, nao os responsaveis pelo calculo de combate.

As unidades atuais nao sao todas iguais:

| Dado | Convencao observada |
| --- | --- |
| PF, PM, vida e dano base | Quantidades numericas. |
| Escalonamento de PF/PM | Multiplicador: 1 = 100%, 0.1 = 10%. |
| Chance critica | 0 a 100. |
| Dano critico no resolver | Multiplicador; fallback 2x quando ausente/invalido. |
| Resistencias | Porcentagem limitada a 0..100 no calculo. |
| Life steal e vamp | Valor dividido por 100 para calcular a cura. |
| Attack speed | Multiplicador enviado ao Animator; 1 e a referencia normal. |

O documento de design cita critico inicial de 125% e limite visual de attack speed de 0.3x a 3x. O fallback atual do resolver e 2x, e `AttackSpeedByStatus` aplica minimo configuravel sem teto 3x. Sao diferencas a reconciliar, nao alteracoes feitas neste estudo.

### Armas e autoria de conteudo

`WeaponLoadoutSO` contem ID, nome, `ComboAttackSetSO`, `RuntimeAnimatorController` e lista de modelos. Cada modelo possui referencia Addressable, SocketID, offset local, Euler local e escala.

`WeaponSocketRegistry` associa identificadores a Transforms. `MN_PlayerWeapon` libera instancias anteriores, troca conjunto de ataques/controller, atualiza cache de parametros e instancia os modelos nas referencias escolhidas. Maos vazias seguem o mesmo caminho por um loadout inicial.

Pastas de conteudo encontradas: Hands, Heavy Hammer Sword e Two Swords. A troca atual aceita Animator Override Controller previamente montado; nao exige reconstruir a maquina de estados por codigo.

Criar uma arma ainda significa manter varios elementos coerentes: modelo/pivo, sockets, offsets, controller/overrides, clips e seus eventos, dados de cada ataque, hitboxes, avanco e encantamento que oferece o loadout. A dificuldade relatada de autoria e compativel com essa quantidade de pontos de configuracao, mesmo quando cada sistema isolado funciona.

### Habilidades

`AbilityBase` e MonoBehaviour; dados de encantamento ficam em SO. `MN_PlayerAbilities` instancia prefabs via Addressables, escolhe container, cria `AbilityContext`, chama os `AbilityAutoSetupBase` e inicializa os componentes.

O contexto pertence a integracao com este jogo; as habilidades recebem apenas as referencias/dados necessarios pelos seus setups. Os dados de dano e buff sao entregues por interfaces com `IReadOnlyList`. Essa exposicao restringe operacoes da lista, mas nao torna os objetos contidos profundamente imutaveis.

- `AttackBonusDamageAbility`: bonus em todos os ataques ou a cada N execucoes, com modo de aplicacao e dano configuravel.
- `TemporaryStatusBuffOnDashAbility`: buff configuravel, gatilho no inicio/fim do dash e renovacao/remocao temporal.
- `LauchProjectileAbility`: cast, animacao, spawn via pool e envio de `DamageRequest` ao projetil.
- `PiercingThrustAbility`: implementacao dedicada da estocada.
- `AbilityCooldown`: modulo separado, iniciado no comeco ou termino da habilidade.

Slots ativos tem input, habilidade, icone, estado de recarga e contador de bloqueios. Uma habilidade em execucao bloqueia os outros slots; dash bloqueia todos. Com slots cheios, a atribuicao atual escolhe o primeiro. Isso ainda nao e uma tela de escolha de slot pelo jogador, nem prova de descarte completo da instancia substituida.

A Fireball atual referencia um prefab diretamente e faz spawn pelo `PoolManager`. Nao confundir o prefab da habilidade ser Addressable com cada projetil ser carregado individualmente por Addressables. Sua animacao atual usa nome de State e `CrossFade`; uma solucao para centenas de animacoes nao esta automaticamente resolvida por essa implementacao.

### Inimigos, reacoes e ondas

`EnemyBrain` integra BT, alvo, NavMesh e combo. Ha fluxo de preparacao para reutilizacao e parada da BT ao desativar. Reacoes foram separadas: animacao, bloqueio de NavMesh, cancelamento/bloqueio de combo, dash e collider conforme o objeto.

Morte tem componentes para animacao e parada/liberacao atrasada. O ranged reaproveita a base com `DamageRequestProjectileShooter` e evento de disparo. `DamageKnockbackReceiver` e o receptor dedicado para recuo associado aos dados de ataque.

`MN_Waves` carrega prefabs Addressable, usa pooling, sorteia inimigo/ponto e tenta repor enquanto houver capacidade. A morte remove o inimigo da contagem de vivos. Nao espera todos morrerem para iniciar outra onda, e nao implementa progressao de dificuldade por si so.

### XP, encantamentos e descricoes

`OnDeath_AddXp` alimenta `MN_Xp`; o level up aumenta o requisito por incremento fixo e avisa `MN_Enchantments`. O XP atual e zerado na subida, descartando excedente no comportamento presente.

`MN_Enchantments` sorteia ofertas e evita repetir aprimoramentos nao repetiveis ja aplicados. Nao equivale ainda ao conjunto de tiers, rerolls, exclusoes e restricoes de build da visao completa.

`EnchantmentSelectionUI` pausa o jogo, prepara cards e restaura o timeScale ao fechar; a selecao possui `finally` para fechamento. A exibicao espera conclusoes de carregamento dos icones. O relato de card ausente precisa ser reproduzido considerando esse fluxo assincrono, nao atribuido automaticamente ao sorteio.

`EnchantmentDescriptionFormatter` resolve marcadores a partir dos dados, incluindo modificadores de status e danos/escalonamentos. Emite tags TMP com `sprite name`, evitando depender de indices visuais para identificar icones. A descricao acompanha o balanceamento dos dados em vez de repetir manualmente todos os valores.

## 6. Por que a sensacao pode continuar crua

### Input e continuidade

**Confirmado:** o primeiro pedido de continuacao so e aceito dentro da janela aberta. Um clique antecipado nao vira um buffer geral para essa janela. Isso e um candidato concreto para explicar parte da sensacao de comando perdido, mas precisa de teste com os tempos reais dos clips.

**Confirmado:** cancelamento e fim de combo zeram o indice. O documento completo e as consideracoes finais sugerem preservar a sequencia por um tempo e permitir retomar/reexecutar o golpe cancelado. Essa regra ainda diverge do controller atual.

### Avanco e ritmo da animacao

**Confirmado:** o avanco e uma coroutine dentro do combo, com delay e duracao em segundos, curva e interpolacao do Transform. Ela e cancelada com o ataque, conforme a decisao anterior do autor.

**Confirmado:** o attack speed altera o parametro da animacao, mas essa rotina nao ajusta delay/duracao pela mesma velocidade. Existe a possibilidade de os pes, impacto e deslocamento deixarem de coincidir ao mudar velocidade ou clip.

**Confirmado:** `CrossFade` recebe duracao normalizada; nao e a mesma unidade de uma duracao fixa em segundos. Isso importa ao comparar ataques com clips de comprimentos diferentes. Fonte: [Unity Animator.CrossFade](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Animator.CrossFade.html).

**Hipotese:** o deslizamento percebido pode resultar da combinacao de curva generica, distancia, blend entre poses e animacoes que nao foram produzidas para o mesmo ritmo. Trocar so a forma de mover nao garante resolver a qualidade dos clips.

### Interrupcoes e multiplos sistemas

Ataque, dash, habilidade, hit reaction e knockback influenciam disponibilidade, animacao e/ou movimento. Os slots ja tem contador de bloqueios, enquanto outros componentes usam Enable/Disable e rotinas que reabilitam ao terminar.

**Hipotese a testar:** um sistema pode liberar algo que outro ainda precisa bloquear; ou mais de uma rotina pode tentar mover o mesmo personagem. Nao e justificativa para reescrever tudo, mas e um ponto de observacao importante ao reproduzir interrupcoes.

### Um golpe visual nem sempre e um unico pacote de dano

O bonus passivo chama `ReceiveDamage` novamente. Portanto, golpe base e bonus podem gerar dois eventos de recepcao. Alem disso, seu contador acompanha o evento de execucao da hitbox: varios eventos de hitbox no mesmo clip contam como varias execucoes, nao necessariamente como um unico golpe visual.

Isso e relevante para o relato de feedback repetido. **Nao foi demonstrado que seja a causa do bug relatado**, mas existe uma diferenca objetiva entre ataque iniciado, hitbox executada, alvo atingido e pacote de dano aplicado.

## 7. Root motion por script

Sim, a ideia lembrada pelo autor existe. O Unity oferece `OnAnimatorMove` para tratar movimento de animacao por script. Tambem permite ler deslocamento do Animator e, em outra abordagem, usar curvas de animacao para orientar movimento. Fonte: [Unity: Scripting Root Motion](https://docs.unity3d.com/6000.0/Documentation/Manual/ScriptingRootMotion.html) e [OnAnimatorMove](https://docs.unity3d.com/kr/current/ScriptReference/MonoBehaviour.OnAnimatorMove.html).

Aplicacao possivel aqui, ainda como hipotese de experimento: obter o deslocamento da animacao e aplica-lo ao objeto principal sob as regras do jogo, mantendo plano de movimento, direcao, cancelamento e tratamento de colisoes. O modelo filho nao deveria simplesmente se afastar da raiz que representa o personagem.

Isso pode ajudar a aproximar movimento e passada do golpe, mas exige avaliar os clips e sua importacao. Um clip in-place nao passa a ter um avanco util apenas por ligar uma opcao. Rotacao extraida da animacao tambem nao precisa ser aceita como direcao de gameplay.

Nao recomendo ativar `Apply Root Motion` indiscriminadamente nem substituir todos os movimentos agora. Antes da decisao, comparar o mesmo ataque com avanco atual e movimento orientado pela animacao, mantendo controles e dano constantes, seria uma forma de investigar a origem da diferenca. O planejamento desse teste fica para a conversa da PoC 2.

## 8. Consideracoes finais do HTML, preservadas para a proxima conversa

Estas sao propostas e relatos do autor, nao uma lista de implementacoes ja aprovadas:

1. A/D passar a determinar a direcao.
2. Um unico input de ataque para a direcao atual.
3. Dash por toque duplo em A/D.
4. Projeteis com Addressables.
5. Avanco durante ataques: ja existe uma versao, mas sua qualidade continua em discussao.
6. Melhorar a janela de continuacao do combo.
7. Preservar indice por X tempo depois de cancelar.
8. Tornar a ordem de execucao do combo previsivel.
9. Melhorar o processo de montar e testar conteudo.
10. Permitir que a animacao influencie o movimento do ataque.
11. Investigar card/slot de aprimoramento que as vezes nao aparece.
12. Padronizar valores em 0..100, ainda precisando delimitar quais unidades.
13. Remover aprimoramentos percentuais de PF e PM.
14. Usar valores flat para vamp/life steal; esclarecer se significa incremento flat do atributo percentual ou cura fixa por hit.
15. Remover aprimoramentos de resistencia magica/fisica, sem presumir remocao das resistencias do combate.
16. Investigar feedback de hit repetido para um unico golpe.
17. Investigar atraso da animacao Hitted do ranged.

## 9. Convencoes que vou manter

- Sistemas simples e componentes com responsabilidade clara; evitar abstrair possibilidades que ainda nao existem.
- Managers `MN_` com SceneSingleton quando sao servicos da cena.
- Classes base reutilizaveis, integracao especifica por contexto/setup e interfaces pequenas.
- Namespaces `Scripts.*`, classes/metodos em ingles e pastas coerentes com a responsabilidade.
- Convencao atual de colecoes: `_list_...`, `_dc_...`, `_hashSet_...`; publicas `List_...` etc.
- `#region` por responsabilidade quando ajuda a navegar, sem fragmentar artificialmente codigo curto.
- SOs para configuracao; estado temporal de execucao nos componentes/instancias.
- Preservar mudancas manuais do autor; nao substituir sua abordagem sem demonstrar necessidade.
- Assets Unity alterados pelo Editor/MCP, nao por remendos de YAML.

## 10. Conclusao de onboarding

A primeira PoC ja tem uma cadeia funcional extensa e integrada. O ponto aberto nao parece ser simplesmente adicionar a proxima mecanica: e validar se o controle, o ritmo dos golpes, as interrupcoes e o trabalho de criar conteudo sustentam o jogo desejado.

Minha leitura e que uma PoC parte 2 pode ter valor como validacao de qualidade do combate e facilidade de autoria, sem confundir isso com polimento visual superficial ou expansao de escopo. Ainda nao ha um plano fechado: faltam escolher os comportamentos alvo, reproduzir os problemas e comparar alternativas em um recorte pequeno.

Para a proxima conversa, os principais assuntos sao input/janelas, continuidade do combo, sincronizacao entre movimento e animacao, configuracao de uma arma nova e os bugs concretos anotados pelo autor. Nao preciso reintroduzir caminhada livre, morte do player ou sistemas completos de produto para tratar essas questoes.
