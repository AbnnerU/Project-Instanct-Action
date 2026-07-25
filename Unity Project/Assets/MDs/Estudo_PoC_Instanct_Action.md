# Estudo da PoC - Instanct Action

Documento baseado no PDF `PoC.pdf`.

Este estudo traduz a Prova de Conceito em escopo pratico de implementacao. A PoC deve validar a ideia geral do jogo com foco em programacao, sem tentar entregar toda a complexidade do projeto final.

## Objetivo da PoC

A PoC precisa provar que o nucleo do jogo funciona:

- Jogador parado no centro.
- Ataques direcionais.
- Dash para esquerda e direita.
- Inimigos vindos dos lados.
- Dano fisico, magico e real.
- Status do jogador.
- Armas com combo.
- Habilidades ativas e passivas.
- XP progressivo.
- Escolha de aprimoramentos.
- Escalada infinita de poder.

O documento original deixa claro que algumas solucoes poderao ser refeitas/refatoradas depois. Mesmo assim, a PoC ja deve nascer pensando no jogo completo, principalmente no modelo de dados e na arquitetura por Addressables.

## Escopo incluido

A PoC deve incluir:

- Dano fisico, magico e real.
- Status do jogador.
- 2 tipos de inimigos.
- 2 armas diferentes.
- Combo de ataques.
- Dash.
- Encantamento de status.
- Aprimoramento de armas.
- Modo ondas normal.
- Sistema de XP progressivo.
- 4 habilidades.
- Substituicao de habilidades.
- Arquitetura baseada em Addressables.

Fora do escopo inicial:

- Menu.
- Tiers de aprimoramento.
- Defesa e perfect block.
- Override por golpe individual de combo.
- Modo Pantheon.
- Monetizacao.
- Mobile/web refinado.

## Status da PoC

Status exigidos para o jogador:

- Vida.
- Poder Fisico.
- Poder Magico.
- Roubo de vida.
- Velocidade de ataque.

Recomendacao:

- Criar um sistema de status runtime capaz de receber modificadores absolutos e percentuais.
- Mesmo que a PoC use poucos status, o formato deve permitir novos status depois.
- Evitar hard code em cada habilidade; habilidades devem consultar status por contrato ou por um componente central do player.

Status que podem ficar preparados para depois:

- Chance de critico.
- Dano critico.
- Vampirismo magico.
- Stamina.

## Sistema de dano da PoC

A PoC ja deve diferenciar:

- Dano Fisico.
- Dano Magico.
- Dano Real.

Necessario implementar:

- Tipo de dano.
- Valor base.
- Escalamento com Poder Fisico.
- Escalamento com Poder Magico.
- Resistencia do alvo.
- Aplicacao em alvos com vida.

Na PoC, ataques e habilidades podem usar:

- Dano base da arma/habilidade.
- Escalamento base da arma/habilidade.
- Output de dano final configurado no dado principal.

Nao precisa implementar ainda:

- Override de dano por golpe individual.
- Override de escalamento por golpe individual.
- Critico.
- Dano hibrido muito avancado.
- Regras complexas de mitigacao.

## Inimigos da PoC

Devem existir 2 inimigos:

- Corpo a corpo.
- A distancia com projetil.

Todo inimigo deve ter:

- Vida.
- Dano.
- Distancia de ataque.
- Velocidade de movimento.
- Resistencias.

Comportamento:

- Ir ate o jogador.
- Tentar atacar quando estiver em distancia.
- Exibir feedback quando estiver prestes a atacar.
- Se receber dano durante a preparacao do ataque, cancelar o ataque.
- Ao parar de receber dano, o ataque reinicia do comeco se o player ainda estiver em alcance.
- O comportamento deve ser controlado por Behaviour Tree.

Recomendacao:

- Comecar com uma Behaviour Tree pequena: `Approach`, `PrepareAttack`, `Attack`, `Recover`.
- O inimigo ranged deve reaproveitar o sistema de projetil existente, mas com dano tipado ou adaptado.
- Separar dados do inimigo em ScriptableObject para facilitar balanceamento.

## Armas da PoC

Devem existir 2 armas:

- Uma arma rapida.
- Uma arma lenta.

Cada arma precisa configurar:

- Dano base.
- Escalamento com Poder Fisico.
- Escalamento com Poder Magico.
- Tipo/output de dano final.
- Combo de ataques.
- Area/hitbox de ataque.
- Momento da animacao em que a hitbox aparece.
- Animacoes da arma.

Regras:

- Velocidade de ataque influencia a velocidade da animacao.
- A PoC nao precisa de override por ataque individual.
- O mesmo dano base e escalamento da arma pode valer para todos os golpes.
- Trocar arma deve trocar tambem animacoes.

Recomendacao:

- Usar `WeaponSO` para dados da arma.
- Usar uma lista de ataques/steps de combo, mesmo que todos herdem o mesmo dano por enquanto.
- O runtime da arma deve controlar combo, tempo de reset, input e janela de hitbox.
- Hitbox deve ser disparada por evento de animacao ou por janelas configuradas.

## Dash da PoC

Dash obrigatorio:

- Dash para esquerda.
- Dash para direita.
- Cooldown independente por lado.
- Velocidade configuravel.
- Tempo de recarga configuravel.

Interacoes:

- Dash pode cancelar ataques.
- Dash serve como principal deslocamento.
- Uma habilidade passiva da PoC depende de dash (`Heavy attack`).

Recomendacao:

- Criar eventos como `OnDashStarted`, `OnDashFinished`, `OnDashSideUsed`.
- Separar input de execucao do dash.
- Registrar cooldowns por direcao.

## XP e level up

O modo ondas da PoC deve ter XP progressivo:

- Inimigos derrotados concedem XP.
- Ao atingir o XP necessario, o jogador sobe de nivel.
- Ao subir de nivel, aparecem aprimoramentos.
- A quantidade de XP necessaria aumenta apos cada nivel.

Recomendacao:

- Criar um `MN_LevelProgression`.
- Emitir eventos `OnXPUpdated` e `OnLevelUp`.
- O sistema de upgrade deve ouvir o level up e abrir a escolha de aprimoramentos.

## Aprimoramentos da PoC

A PoC nao precisa de tier.

Tipos exigidos:

- Encantamento de status.
- Aprimoramento de armas.

Encantamento de status:

- Deve permitir criar qualquer upgrade que altera um status especifico.
- Pode alterar por numero absoluto.
- Pode alterar por porcentagem.
- Percentual deve ser baseado no valor atual do status.

Aprimoramento de armas:

- Deve mostrar uma arma disponivel na pool.
- Deve trocar a arma atual do jogador.
- Deve ser implementado depois que armas e encantamentos de status estiverem prontos.
- Deve trocar tambem animacoes da arma.

Recomendacao:

- Criar uma base `UpgradeSO`.
- Criar especializacoes como `StatUpgradeSO` e `WeaponUpgradeSO`.
- Cada upgrade deve ter `ID`, nome, descricao, icone e metodo de aplicacao.
- Para a PoC, a selecao pode ser simples, sem tiers e sem regras avancadas.

## Habilidades da PoC

Devem existir 4 habilidades:

### Magic Attack

Tipo:

- Passiva.

Comportamento:

- A cada 3 ataques, o terceiro causa dano magico adicional.
- Escala com dano base + percentual de Poder Fisico e/ou Poder Magico.

Recomendacao:

- Ouvir evento de ataque acertado.
- Contar acertos.
- Aplicar dano adicional no terceiro acerto.

### Heavy Attack

Tipo:

- Passiva.

Comportamento:

- Depois de usar dash, ganha X% de Poder Fisico por X segundos.

Recomendacao:

- Ouvir evento de dash.
- Aplicar modificador temporario de status.
- Remover modificador ao fim do tempo.

### Fire Ball

Tipo:

- Ativa.

Comportamento:

- Lanca uma bola de fogo na direcao em que o jogador esta olhando.
- Causa dano configurado por base + escalamento de Poder Fisico/Poder Magico.

Recomendacao:

- Reaproveitar `Projectile`.
- Adaptar o projetil para receber um pacote de dano tipado, nao apenas `int`.
- Usar Object Pool para instancias frequentes.

### Estocada Perfurante

Tipo:

- Ativa.

Comportamento:

- O personagem avanca um curto espaco para frente.
- Atravessa inimigos daquele lado.
- Causa dano fisico configurado por base + escalamento.

Recomendacao:

- Pode compartilhar parte da logica com dash, mas deve ser habilidade propria.
- Precisa de hitbox durante o avanco.
- Deve aplicar dano uma vez por alvo atingido durante a estocada.

## Substituicao de habilidades

A PoC ja deve permitir substituir habilidades.

Recomendacao:

- Criar um `AbilityLoadout` com slots.
- Separar habilidade ativa de passiva.
- Permitir equipar, remover e substituir.
- Cada habilidade deve ter ID estavel.

Possivel limite:

- O projeto completo fala em ate 3 habilidades ativas, 1 suprema e passivas ilimitadas.
- Na PoC, pode haver slots suficientes para testar substituicao sem implementar suprema.

## Addressables

O PDF da PoC exige arquitetura baseada em Addressables desde o inicio.

Recomendacao:

- Usar Addressables para cenas.
- Usar Addressables para armas, habilidades, upgrades, inimigos e projeteis quando fizer sentido.
- Evitar referencias diretas a prefabs de conteudo expansivel.
- Usar `AssetReferenceGameObject` e tipos especificos quando possivel.
- Aproveitar a base de loading existente no projeto.

## Sistemas minimos sugeridos

Sistemas de gameplay:

- `MN_GameState`
- `MN_Waves`
- `MN_LevelProgression`
- `MN_UpgradeSelection`
- `MN_Coins`, se moedas entrarem cedo
- `PlayerStats`
- `PlayerCombat`
- `PlayerDash`
- `WeaponHandler`
- `AbilityLoadout`
- `DamageResolver`
- `EnemyBrain`

Dados:

- `DamageData`
- `StatModifierData`
- `WeaponSO`
- `AbilitySO`
- `UpgradeSO`
- `EnemySO`
- `WaveSO`

Contratos:

- `IDamageable` ou evolucao de `IHittable`.
- `IStatModifierReceiver`.
- `IAbility`.
- `IUpgradeApplier`.
- `IWeaponUser`.
- `IXPRewardProvider`.

## Aproveitamento dos scripts existentes

Pode ser usado como base:

- `HealthManager` e `HealthBasics` para vida e eventos de hit/morte.
- `Projectile` para Fire Ball e inimigo ranged, com adaptacao para dano tipado.
- `PoolManager` para projeteis e inimigos.
- `SIS_LoadingSceneChannel` e `LoadingSceneHandler` para cenas Addressables.
- `MN_Coins` para economia de partida, se moedas entrarem na PoC.
- `RequirementSO` e `MN_Requirements` como inspiracao para IDs/requisitos de upgrades.
- Interfaces existentes como referencia de padrao.

Precisa adaptar:

- `IHittable` hoje recebe apenas `float`/`int`; PoC precisa de dano com tipo e escalamento.
- `Projectile` hoje usa `int _damage` e `string _targetTag`; deve receber dados mais ricos.
- Alguns scripts nao usam namespace `Scripts.*` e devem ser ajustados quando forem promovidos a base oficial.

## Ordem recomendada de implementacao

1. Criar modelo de status do jogador.
2. Criar modelo de dano tipado e resolvedor de dano.
3. Adaptar/apoiar vida e hit para aceitar dano final.
4. Criar player parado com direcao esquerda/direita.
5. Implementar dash com cooldown por lado.
6. Implementar arma inicial, combo e hitbox por animacao.
7. Criar 2 armas configuraveis.
8. Criar inimigo melee simples.
9. Criar inimigo ranged e projetil.
10. Criar sistema de waves basico.
11. Criar XP e level up.
12. Criar upgrades de status.
13. Criar troca de arma por upgrade.
14. Criar habilidades passivas e ativas.
15. Criar substituicao de habilidades.
16. Integrar Addressables nos conteudos principais.

## Criterios de sucesso

A PoC estara validada quando for possivel:

- Entrar direto em uma cena jogavel, sem menu.
- Atacar inimigos dos dois lados.
- Usar dash para os dois lados com cooldown separado.
- Matar inimigos melee e ranged.
- Receber dano dos inimigos.
- Diferenciar dano fisico, magico e real.
- Ver resistencias afetando dano.
- Trocar entre duas armas.
- Executar combos.
- Usar Fire Ball e Estocada Perfurante.
- Ter Magic Attack e Heavy Attack funcionando como passivas.
- Ganhar XP, subir de nivel e escolher aprimoramentos.
- Aplicar upgrades de status absolutos e percentuais.
- Trocar arma por aprimoramento.
- Substituir habilidades.
- Continuar escalando poder indefinidamente.

## Decisoes a adiar

Adiar ate a PoC dar sinais claros:

- Defesa.
- Perfect block.
- Stamina.
- Critico completo.
- Suprema.
- Tiers de upgrade.
- Reroll.
- Loja de atributos completa.
- Modo Pantheon.
- Regras avancadas de exclusao entre upgrades.

## Conclusao

A PoC deve ser pequena no conteudo, mas forte na arquitetura. O objetivo nao e ter muitos inimigos, armas ou habilidades; e provar que o nucleo modular funciona. Se dano, status, arma, dash, inimigo, XP e upgrade estiverem bem separados, o projeto completo podera crescer sem virar uma cadeia de excecoes.
