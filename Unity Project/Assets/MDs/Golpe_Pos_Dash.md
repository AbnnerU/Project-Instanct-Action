# Golpe pos-dash

`PostDashAttackController` guarda um unico pedido de ataque durante o dash. Ao
terminar normalmente, executa o `PostDashAttack` do loadout equipado. Dash
cancelado, dano recebido, troca de arma ou desativacao descartam o pedido.

O golpe usa `AttackHitbox`, incluindo dano, resistencias, critico e knockback.
Ele nao consome um indice do combo normal. As passivas que escutam os eventos
de execucao do combo normal nao contam esse golpe separado.

## Configuracao

- Player: `PostDashAttackController`, `ComboController` e `Dash_ComboControllerHandler`.
- Animator: `ComboAnimationEventReceiver`, que encaminha os eventos ao ataque correto.
- O handler do dash deve ter `Maintain Combo Index` ligado para preservar o combo.
- No Combat Workshop: `Attacks > Post-dash > Enabled`.
- Crie o slot pelo botao `Create post-dash Animator slot`, se necessario.
- Escolha o clip e configure dano, hitbox, knockback e marcadores `HIT`/`FINISH`.
- Salvar gera `PostDashAttack.anim` e registra o clip no Animator Override da arma.
- Armas existentes ficam sem esse golpe ate `HasPostDashAttack` ser habilitado.

## Retencao e interrupcoes

O prazo continua correndo durante o dash. Durante o golpe pos-dash, a retencao
fica pausada. Ao terminar, os segundos restantes voltam a correr. Um prazo ja
expirado nao e recuperado. Receber dano descarta o indice retido.

`ComboControllerBase` expoe `PauseComboRetention`, `ResumeComboRetention` e
`ComboRetentionRemaining`. Os bloqueios de input pertencem a quem os solicita;
o fim do dash nao remove o bloqueio aplicado pelo golpe especial.

Os eventos de animacao continuam sendo `Attack_ExecuteHitbox` e `Attack_Finish`.
Nao ha janela de combo no golpe especial. Eventos de outros estados sao
ignorados. Se `FINISH` faltar, o fim do estado libera os bloqueios; uma transicao
para outro estado interrompe o golpe sem forcar Idle.
