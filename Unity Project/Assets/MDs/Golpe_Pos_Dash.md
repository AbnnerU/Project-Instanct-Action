# Golpe pos-dash

O Player usa somente `PostDashAttackController : ComboControllerBase`.
Ele substitui o componente `ComboController`, sem alterar essa classe.
Nao ha referencia para um segundo controlador.

## Funcionamento

- Fora do dash, o input chama o combo normal herdado.
- Durante o dash, o mesmo input guarda um unico pedido de golpe especial.
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
- Configure `Attack Input`, `Dash`, `Weapon Manager` e as referencias herdadas.
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
