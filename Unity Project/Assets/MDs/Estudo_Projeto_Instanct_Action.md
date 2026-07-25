# Estudo do Projeto - Instanct Action

Documento baseado no PDF `Project_ Instanct Action (1).pdf`.

Este estudo resume o projeto completo, identifica os sistemas centrais e traduz a ideia em uma direcao tecnica para Unity. O objetivo nao e substituir o documento original, mas transformar o conceito em uma base de consulta para arquitetura, planejamento e implementacao.

## Visao geral

`Instanct Action` e um jogo 2D ou 2.5D de acao, sobrevivencia e progressao roguelike. O jogador fica centralizado na tela, enfrenta inimigos que surgem dos dois lados e monta builds diferentes ao longo da partida por meio de armas, habilidades, passivas, status e aprimoramentos semi aleatorios.

O jogo combina referencias de:

- `Souls Like`: leitura de ataques, timing, punicao, possivel defesa/perfect block.
- `MOBA`: habilidades ativas, suprema, cooldowns, status e sinergias.
- `Hack and Slash`: combos, cancelamentos, ataque direcional e ritmo de combate.
- `Roguelike`: builds por tentativa, escolhas semi aleatorias, escalada de poder e desafio.

## Plataformas e publico

Plataforma inicial:

- PC, com foco em Steam.

Possiveis expansoes:

- Plataformas web como Crazy Games, com monetizacao por anuncios da plataforma.
- Mobile, exigindo revisao de input, UI, performance e sessoes de jogo.

Publico alvo:

- Jovens entre 15 e 24 anos.
- Jogadores que gostam de acao responsiva, desafio crescente e combinacoes de builds.

## Fantasia de gameplay

O jogador nao anda livremente pelo cenario. Ele fica no centro da tela e interage com ameacas vindas dos lados. A movimentacao principal e feita por dash para esquerda e direita, cada lado com cooldown proprio.

O jogo deve vender a sensacao de:

- Ler inimigos chegando.
- Escolher o lado certo para atacar, desviar ou usar habilidade.
- Crescer de poder durante a partida.
- Montar sinergias novas em cada tentativa.
- Sobreviver a ondas cada vez mais intensas.

## Loop principal

Fluxo base do modo normal:

1. Jogador inicia com atributos padrao, sem armas especiais e sem habilidades avancadas.
2. Inimigos surgem dos cantos/lados em ondas.
3. Jogador ataca, usa dash e habilidades para sobreviver.
4. Dano causado e eliminacoes geram XP.
5. Ao subir de nivel, o jogador escolhe 1 entre 3 aprimoramentos.
6. Moedas obtidas na partida podem comprar pacotes de atributos.
7. O desafio escala com inimigos mais fortes, mais numerosos e bosses periodicos.
8. A partida termina quando a vida do jogador chega a zero.

## Movimento e posicionamento

O player fica centralizado. O dash e a principal ferramenta de deslocamento e evasao.

Regras de dash:

- Existe um dash para cada lado.
- Cada lado tem cooldown individual.
- O dash pode desviar de ataques e projeteis.
- Passivas e encantamentos podem alterar o comportamento do dash.
- Efeitos de dash devem ser cumulativos quando fizer sentido.
- O dash pode cancelar ataques em andamento.
- O dash nao reseta o combo; se cancelar um ataque, o proximo ataque reinicia do frame inicial daquele golpe.

## Combate

O combate gira em torno de ataques por arma, habilidades ativas, habilidades passivas e uma habilidade suprema.

O jogador pode ter:

- Ate 3 habilidades ativas.
- 1 habilidade suprema.
- Numero ilimitado de passivas/aprimoramentos.

Armas:

- Cada arma possui combos de 2 a 3 golpes.
- O "soco" funciona como arma inicial conceitual.
- Cada arma tem dano base e escalonamento com Poder Fisico e/ou Poder Magico.
- Cada golpe do combo pode, no jogo completo, sobrescrever dano base, escalonamento e tipo de dano.
- A area de ataque tende a ser definida por `Overlap` (`OverlapBox`, `OverlapSphere`, etc).
- A animacao define quando e onde a hitbox existe.

Ataques especiais:

- Algumas armas podem ter ataques especiais apos dash.
- Exemplo: dash + ataque com espada gera um giro ao final, causando dano ao redor.

## Sistema de dano

Tipos de dano:

- Dano Fisico.
- Dano Magico.
- Dano Real.

Dano Real:

- Nao e reduzido por resistencias.

O modelo final precisa separar pelo menos:

- Fonte do dano.
- Tipo do dano.
- Valor base.
- Escalamentos.
- Multiplicadores.
- Critico.
- Resistencia do alvo.
- Efeitos extras como roubo de vida ou vampirismo magico.

## Inimigos

Inimigos surgem dos lados/cantos e avancam em direcao ao jogador. Eles podem ser corpo a corpo ou a distancia.

Todo inimigo deve possuir:

- Vida.
- Dano.
- Distancia de ataque.
- Velocidade de ataque.
- Velocidade de movimento.
- Resistencias.
- Opcionalmente, um conjunto de ataques unicos.

Resistencias:

- Pequena: reduz dano fisico ou magico em 30%.
- Media: reduz dano fisico ou magico em 50%.
- Grande: reduz dano fisico ou magico em 80%.

Observacao:

- Os valores sao provisorios.
- Um inimigo pode ter resistencia diferente para dano fisico e dano magico.
- Bosses sao inimigos especiais, mais fortes, com mais vida e ataques diferenciados.

## Status do jogador

Status principais identificados:

- Vida.
- Poder Fisico.
- Poder Magico.
- Chance de critico.
- Dano critico.
- Stamina/Resistencia, caso defesa seja aprovada.
- Roubo de vida.
- Vampirismo magico.
- Velocidade de ataque.

Vida:

- Comeca em 100 no conceito.
- Ao chegar a zero, o jogador perde.

Poder Fisico:

- Influencia dano de ataques.
- Pode ser usado como escala de habilidades e passivas.

Poder Magico:

- Influencia habilidades e efeitos magicos.
- Pode tambem afetar armas, passivas ou encantamentos.

Critico:

- Dano critico padrao causa 125% do dano normal.
- Habilidades geralmente nao criticam por padrao.
- Aprimoramentos podem liberar critico para habilidades.

Roubo de vida:

- Cura com base em uma porcentagem do Dano Fisico causado.

Vampirismo magico:

- Cura com base em uma porcentagem do Dano Magico causado.

Velocidade de ataque:

- Define intervalos dos combos e pode afetar velocidade de animacao.
- Referencia conceitual: `Ronin: O ultimo samurai`.
- Para ataques com arma, faixa sugerida: minimo 0.3x, maximo 3x.
- O valor pode continuar crescendo para servir como escala de passivas e habilidades.

## Defesa e perfect block

A defesa ainda e conceitual e pode nao entrar na versao final.

Ideia:

- Jogador defende ataques/projeteis de um lado.
- Defesa reduz dano em 90% ou mais.
- Defesa consome stamina.
- Stamina zerada impede defesa e causa stun por alguns segundos.
- Perfect block nega todo dano e nao consome stamina.

Recomendacao:

- Tratar defesa como sistema opcional e isolado.
- Nao acoplar o core de dano ao perfect block antes de validar a PoC.
- Criar interfaces ou eventos para permitir encaixe futuro.

## Habilidades ativas

Cada habilidade ativa possui:

- Dano base.
- Escalamento com Poder Fisico e/ou Poder Magico.
- Tipo de dano.
- Cooldown.
- Possivel cast time.
- Possivel bloqueio de outras habilidades ou ataques.
- Possivel cancelamento de ataque.

Fluxos possiveis:

- Usar habilidade > executar > entrar em cooldown.
- Usar primeira parte > liberar segunda parte > executar proximas partes > cooldown.

Exemplos do conceito:

- Gelo: congela inimigos ao redor; primeiro ataque em inimigo congelado causa dano magico baseado na vida do alvo.
- Corte perfeito: avanca na direcao atual causando dano fisico e magico.
- Orbe/Chute devastador: projeta uma orbe, marca alvo e permite segunda ativacao para avancar ate ele.

## Habilidades passivas e encantamentos

Passivas nao precisam de input. Podem estar sempre ativas, reagir a acoes ou depender de condicoes.

Exemplos:

- Ira: acertos em combate aumentam velocidade de ataque ate um limite.
- Sede de sangue: vida baixa aumenta Poder Fisico.
- Golpes de fogo: a cada 3 ataques, dispara fogo frontal.
- Ainda nao: primeira morte restaura vida temporariamente; dano causado pode interromper sangramento.
- Ventos cortantes: apos dash, cria area de vento causando dano magico.
- Laminas magicas: a cada 5 ataques, adiciona dano escalado por Poder Magico.

Requisito estrutural:

- Aprimoramentos devem ser aditivos sempre que possivel.
- Um aprimoramento nao deve invalidar outro sem regra explicita.

## Suprema

A suprema depende de input, mas nao usa cooldown comum.

Funcionamento:

- Carrega de 0% a 100% com base no dano causado.
- Ao chegar em 100%, pode ser usada.
- Ao usar, volta a 0%.
- Deve ser mais impactante que habilidades comuns.

## Economia e loja de atributos

Durante a partida, o jogador ganha moedas.

Usos:

- Comprar pacotes de atributos.
- Comprar rerolls de aprimoramento.

Fluxo da loja de atributos:

1. Jogador abre a loja por input.
2. Compra um pacote.
3. Aparecem 3 opcoes.
4. Jogador escolhe 1 opcao.
5. O atributo ou conjunto de atributos e aplicado ao jogador.

Origem das moedas:

- Inimigos derrotados.
- Baus que aparecem em intervalos.

## Aprimoramentos

Aprimoramentos aparecem periodicamente ou ao subir de nivel.

Tiers conceituais:

- Comum.
- Normal.
- Epico.
- Lendario.

Regras:

- O jogador escolhe entre 3 opcoes.
- Pode rerollar uma opcao ate 3 vezes.
- Pode comprar novos rerolls com moedas.
- Boa parte dos aprimoramentos nao deve repetir.
- Cada card de aprimoramento precisa ter identificador proprio.
- Aprimoramentos podem ter excecoes e pre-requisitos.

Categorias:

- Status.
- Encantamentos/passivas.
- Armas.
- Encantamentos de arma.
- Habilidades.
- Encantamentos de habilidades.
- Encantamentos de economia.

Status:

- Alteram status base do jogador.

Encantamentos/passivas:

- Alteram significativamente a gameplay.
- Podem ter dano base e escalamentos.

Armas:

- Concedem nova arma.
- Ao trocar arma, encantamentos da arma anterior sao perdidos.

Encantamentos de arma:

- Afetam a arma atual.
- Podem ser exclusivos de arma.

Habilidades:

- Concedem nova habilidade.
- Se o jogador ja tiver o maximo, deve poder substituir.

Encantamentos de habilidade:

- Afetam uma habilidade especifica.
- Nao devem aparecer se o jogador nao possui a habilidade.

Encantamentos de economia:

- Afetam moedas, recompensas, drop e atributos aleatorios.

## Semi aleatoriedade

A selecao de aprimoramentos nao e aleatoria pura. Ela precisa respeitar regras de contexto.

Regras identificadas:

- Encantamento de habilidade nao aparece se o jogador nao tem a habilidade.
- Encantamento de arma nao aparece se o jogador nao tem a arma.
- No maximo uma nova arma e uma nova habilidade podem aparecer no mesmo conjunto de opcoes.
- A arma equipada nao deve aparecer como nova arma equivalente.
- Aprimoramentos podem bloquear outros aprimoramentos.
- Armas ou estados atuais podem bloquear aprimoramentos especificos.

Implicacao tecnica:

- O sistema de upgrades deve ter validadores.
- Cada upgrade deve possuir `ID`, categoria, tier, requisitos, bloqueios e regras de repeticao.
- O gerador de opcoes deve filtrar antes de sortear.

## Modos de jogo

### Modo ondas

Modo principal.

Fluxo:

- Jogador inicia basico.
- Inimigos surgem em ondas.
- Algumas ondas possuem boss.
- Dano e eliminacoes geram XP.
- Ao subir de nivel, aparecem 3 aprimoramentos.
- XP necessario aumenta a cada nivel.
- Loja de atributos permite escalar poder durante a partida.

### Modo Pantheon

Modo de bosses.

Fluxo:

- Antes de iniciar, jogador escolhe arma, 3 habilidades e 1 passiva.
- Enfrenta apenas bosses em sequencia.
- Ha um aprimoramento entre bosses.
- Nao ha novos encantamentos de arma nem novas habilidades.
- Ao vencer todos, recebe recompensa.

## Direcao de arquitetura

Pelo escopo do projeto, os sistemas mais importantes devem ser data-driven e modulares:

- Status do jogador.
- Dano.
- Armas.
- Ataques de combo.
- Habilidades.
- Passivas.
- Aprimoramentos.
- Inimigos.
- Waves.
- Bosses.
- Economia.
- XP e nivel.

Padroes recomendados:

- ScriptableObjects para dados de armas, ataques, habilidades, upgrades, inimigos e waves.
- Interfaces para alvos que recebem dano, coletaveis, sistemas ativaveis e validadores.
- Eventos para vida, dano causado, morte, XP, level up, moedas, troca de arma e troca de habilidade.
- Managers `MN_` para sistemas locais de partida.
- Managers `SIS_` apenas para sistemas globais entre cenas.
- Addressables para conteudo carregavel e expansivel.

## Sistemas provaveis

Sistemas centrais:

- `MN_GameState` ou equivalente para estado da partida.
- `MN_Waves` para ondas e bosses.
- `MN_PlayerStats` para atributos runtime.
- `MN_XP` ou `MN_LevelProgression` para XP e level up.
- `MN_UpgradeSelection` para gerar opcoes semi aleatorias.
- `MN_Coins` para moedas de partida.
- `PlayerCombat` para input e execucao de ataques.
- `PlayerDash` para dash lateral.
- `WeaponRuntime` ou `WeaponHandler` para arma atual e combo.
- `AbilityLoadout` para habilidades equipadas.
- `DamageResolver` para calcular dano final.
- `EnemyBrain` ou Behaviour Tree para comportamento.

Dados por ScriptableObject:

- `PlayerBaseStatsSO`.
- `WeaponSO`.
- `WeaponAttackSO`.
- `AbilitySO`.
- `PassiveSO`.
- `UpgradeSO`.
- `EnemySO`.
- `WaveSO`.
- `DamageConfigSO`, se necessario.

## Riscos de escopo

O projeto completo tem muitos sistemas combinatorios. Os maiores riscos sao:

- Criar upgrades especificos demais antes de estabilizar o modelo base.
- Misturar dano, hitbox, animacao e efeitos em uma unica classe grande.
- Fazer habilidades conhecerem diretamente armas, inimigos, UI e economia.
- Implementar defesa/perfect block cedo demais.
- Tratar tier visual existente como se ja fosse tier de upgrade.
- Criar muitas regras por Inspector sem identificadores e validadores.

## Prioridades tecnicas

Prioridade alta:

- Modelo de dano com tipos fisico, magico e real.
- Status runtime do jogador.
- Sistema de armas e combos configuraveis.
- Hitboxes ligadas ao timing de animacao.
- Inimigos melee/ranged simples.
- XP, level up e selecao de upgrades.
- Dash com cooldown por lado.
- Addressables desde o inicio.

Prioridade media:

- Loja de atributos.
- Moedas e baus.
- Bosses.
- Supremas.
- Reroll de upgrades.
- Regras avancadas de semi aleatoriedade.

Prioridade baixa para inicio:

- Defesa/perfect block.
- Modo Pantheon.
- Encantamentos muito especificos.
- Monetizacao web/mobile.

## Conclusao

`Instanct Action` deve ser tratado como um jogo de combate modular com progressao roguelike. A decisao mais importante para a arquitetura e separar bem dados, execucao e regras de validacao. O jogo completo depende de muitos efeitos aditivos; por isso, cada sistema deve nascer com contratos pequenos, eventos claros e dados configuraveis por ScriptableObject.

A PoC deve validar o nucleo: atacar, dar dash, matar inimigos, ganhar XP, escolher upgrades e sentir a escalada de poder. O restante deve ser preparado por arquitetura, mas nao precisa entrar completo na primeira versao.
