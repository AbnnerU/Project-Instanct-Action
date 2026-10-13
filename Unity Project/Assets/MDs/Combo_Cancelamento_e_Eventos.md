# Combo: cancelamento e eventos

## Janela de input

- A janela abre no inicio de cada ataque, sem Animation Event de abertura.
- Um input durante a janela guarda somente um proximo ataque.
- `Attack_CloseComboWindow` fecha a janela, mas preserva o input ja guardado.
- `Attack_Finish` inicia o ataque seguinte se houver input guardado; caso contrario encerra o combo.
- `Attack_OpenComboWindow` permanece como metodo vazio para compatibilidade com clips antigos. O Workshop nao gera mais esse marcador.

## Dash e cancelamento

Ao iniciar o dash, `Dash_ComboControllerHandler` bloqueia novos ataques e chama `CancelAttack(false)`, sem forcar Idle sobre a animacao de dash. O indice e o input enfileirado sao descartados. Ao terminar o dash, o proximo input inicia o primeiro ataque do combo.

Por padrao, cancelamentos descartam o indice. Para preservar explicitamente o golpe atual, use `CancelAttack(playIdle: false, maintainComboIndex: true)`. O proximo `RequestAttack()` reinicia esse golpe desde o inicio, sem recuperar tempo de animacao ou input enfileirado. Nao ha prazo de retomada.

`CancelAttack(bool playIdle = true, bool maintainComboIndex = false)` permite controlar Idle e preservacao independentemente. Os handlers existentes continuam usando o cancelamento completo. Troca de conjunto de ataques, desativacao e termino normal do combo tambem zeram o indice.

## Estado de execucao

O indice indica qual golpe iniciar. `ResetExecution()` nao altera esse indice: apenas invalida callbacks da execucao anterior, retorna a fase para `IDLE` e limpa o input guardado.

Uma unica fase substitui os booleanos de ataque, janela e finalizacao: `IDLE`, `ACCEPTING_INPUT`, `RECOVERY` e `FINISHING`. As propriedades publicas de estado sao calculadas a partir dela. O controlador continua responsavel pelo Animator, sem outro componente de animacao.

O nome do state vem do `CurrentAttack`. O Guid da hitbox e criado localmente, mantendo um identificador por execucao de hitbox. A versao de execucao protege contra cancelamentos ou substituicoes ocorridos dentro dos eventos.

## Protecao contra travamento

- O Animator e o state sao validados antes de iniciar o ataque.
- O trigger de Idle pendente e limpo antes do CrossFade. Triggers de dash e hit reaction nao sao apagados.
- O receiver valida o Animator e o state de origem do evento. Em transicoes, somente o destino pode controlar o ataque atual.
- Se o Animator sair do ataque ou chegar ao fim sem `Attack_Finish`, o controlador cancela o estado inconsistente. Nao executa hitboxes nem avanca o combo artificialmente.
- A verificacao usa o progresso normalizado do Animator, nao um timeout fixo: acompanha a velocidade da animacao.

## Verificacao

`Tools > Combat > Run Combo Checks (Play Mode)` testa o fluxo em um objeto temporario, sem alterar prefabs ou a cena salva. Entra em Play Mode quando necessario e restaura a configuracao de execucao em segundo plano ao terminar. O resultado pode ser consultado em `ComboControllerChecks.Result`.

`Tools > Combat Workshop Checks` valida os marcadores e o salvamento de loadouts em Edit Mode.
