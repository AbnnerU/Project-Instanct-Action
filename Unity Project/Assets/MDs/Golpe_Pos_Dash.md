# Golpe pos-dash

O Player usa somente `PostDashAttackController : ComboControllerBase`.
Ele substitui o componente `ComboController`, sem alterar essa classe.
Nao ha referencia para um segundo controlador.

## Funcionamento

- Fora do dash, os inputs de esquerda/direita viram e iniciam o combo normal.
- Durante cada golpe, a direcao fica bloqueada. A ultima direcao aceita na janela
  de combo e aplicada somente quando o proximo ataque comeca.
- Durante o dash, esses inputs guardam um unico pedido de golpe especial e a
  ultima direcao escolhida, sem virar durante o dash.
- No fim de um dash nao cancelado, executa o `PostDashAttack` da arma.
- A lista de ataques normais permanece intacta. Apenas `CurrentAttack` seleciona
  temporariamente o golpe especial.
- Animacao, hitbox, eventos e cancelamento usam a execucao da classe base.
- Durante o especial, novos pedidos de ataque sao ignorados.
- Ao terminar, restaura o indice e o tempo restante que existiam antes do especial.
- Dano recebido, troca de arma e desativacao descartam pedido e retencao.
- Um novo dash pode preservar a retencao anterior sem contar o especial como
  um passo do combo normal.

## Configuracao

- Player: somente `PostDashAttackController` como controlador de combate.
- Nao adicionar `ComboController` nem `Dash_ComboControllerHandler` ao mesmo
  personagem. O pos-dash ja cuida do cancelamento no inicio do dash.
- Configure `Left Attack Input`, `Right Attack Input`, `Direction Handler`,
  `Dash`, `Weapon Manager` e as referencias herdadas.
- `DirectionHandler` nao le inputs. O pos-dash aplica a direcao antes de iniciar
  cada ataque e mantem seu proprio bloqueio ate o termino ou cancelamento.
- Nao use `Combo_DirectionHandler` adicional neste personagem: o bloqueio ja e
  responsabilidade do controlador. `Dash_DirectionHandler` pode permanecer.
- `Maintain Combo Index On Dash` controla a preservacao ao iniciar um dash.
- Armas, habilidades, hit reaction e `ComboAnimationEventReceiver` devem
  referenciar esse mesmo componente.
- O prazo de retencao continua correndo durante o dash e pausa apenas no especial.
- No Combat Workshop: `Attacks > Post-dash > Enabled`, clip, dano, hitbox,
  knockback e marcadores `HIT`/`FINISH`.
- Os eventos continuam `Attack_ExecuteHitbox` e `Attack_Finish`.
- Armas sem golpe especial continuam executando o combo normal.

Os eventos herdados agora pertencem a um unico controlador, inclusive os de
execucao da hitbox. Sistemas que escutam esses eventos tambem recebem o especial;
`IsPostDashAttack` permite distinguir os dois casos.
