# Encantamentos V1

## Escopo desta etapa

- Ofertas de level-up usam o `_defaultDeckConfig`.
- Tipos permitidos sao configurados por lista em `EnchantmentsDeckConfig`.
- Armas e habilidades substituidas voltam a ser elegiveis no catalogo.
- Uma oferta tem no maximo uma arma e uma habilidade.
- Habilidades ativas sempre passam pela escolha de slot na selecao de encantamentos.
- Skip encerra a oferta sem aquisicao e concede rerolls extras para a proxima oferta padrao.
- Pedidos de ofertas recebidos durante uma selecao ficam em fila.
- Tiers ponderados, botoes de reroll e loja ainda nao fazem parte desta etapa.

## Configuracao no Inspector

### MN_Enchantments

1. Mantenha Player Object apontando para o jogador.
2. Configure Options Per Level Up e a lista Available Enchantments.
3. No Default Deck Config, preencha Enchantments Types Available com STATUS, WEAPON e ABILITY, conforme o conteudo desejado. O campo passou de enum unico para lista: revise sua configuracao serializada.
4. Configure Rolls e habilite Allow Skip.
5. Configure Skip Additional Rolls com o bonus concedido por skip.
6. Confira Enchantment Type em cada SO. O filtro usa esse campo; o Tier ainda nao altera probabilidades.
7. Adicione o SO dos Punhos ao catalogo quando ele estiver pronto, apontando para o loadout correto.

Uma lista de tipos vazia nao permite nenhum encantamento. Ofertas sem candidatos nao abrem a HUD nem consomem o bonus de rerolls.

### EnchantmentSelectionUI

Mantenha este componente em um objeto ativo fora dos paineis que ele liga e desliga.

1. Preserve Panel Object, List Cards e Pause Game While Selecting.
2. Crie um botao de skip e atribua em Skip Button.
3. Crie um painel filho da selecao para escolher o slot e atribua em Ability Slot Panel. Ele nao deve ser o proprio Panel Object nem um ancestral do componente.
4. Crie um botao por slot e preencha List Ability Slot Buttons na mesma ordem dos slots do MN_PlayerAbilities: elemento 0 escolhe slot 0, elemento 1 escolhe slot 1.
5. Atribua um botao de voltar em Cancel Slot Selection Button. Ele volta para as cartas sem consumir a escolha.
6. Configure os textos dos botoes, por exemplo Slot 1, Slot 2 e Voltar.

Os listeners sao conectados pelo script. Nao adicione chamadas duplicadas no OnClick do Inspector. Alternativamente, uma UI personalizada pode usar SelectAbilitySlot(int), CancelSlotSelection() e Skip().

Durante a escolha do slot e o carregamento, as cartas ficam indisponiveis. O jogo so retoma quando a oferta termina. Em falha de carregamento/setup, a oferta e a habilidade anterior sao preservadas; o jogador pode tentar novamente ou voltar.

### MN_PlayerAbilities e prefabs

- Preserve os inputs e os slots ja configurados.
- Cada prefab de habilidade ativa deve conter exatamente um AbilityBase. Os modulos de cooldown, setup e demais componentes continuam separados.
- O manager guarda a identidade do prefab, a instancia e o handle Addressable. Prefabs iguais nao podem ser adquiridos duas vezes enquanto estiverem presentes.
- A preparacao da nova instancia ocorre antes da remocao da antiga.
- A remocao desativa os componentes, limpa listas/eventos/slot e libera o handle.
- Projeteis ja lancados continuam com seu ciclo proprio; futuros efeitos vinculados, como orbes, devem ser encerrados pela habilidade em OnDeactivate.

## Ciclo de vida de AbilityBase

`Disable()` bloqueia temporariamente. `Cancel()` interrompe a execucao atual. `Deactivate()` encerra o vinculo da instancia com o personagem.

Deactivate nao e mais virtual: o fluxo fica centralizado e executa uma unica vez, inclusive se OnDestroy o chamar novamente. A ordem e bloquear novas execucoes, cancelar, notificar OnAbilityDeactivating e executar OnDeactivate. As rotinas do proprio AbilityBase sao interrompidas ao final.

As subclasses devem sobrescrever `protected override void OnDeactivate()` para remover inscricoes, buffs e recursos proprios. As passivas existentes foram migradas. Componentes separados sao responsaveis pela propria limpeza; AbilityCooldown agora escuta o evento para parar sua rotina e remover inscricoes.

Futuros upgrades devem escutar OnAbilityDeactivating da instancia a que pertencem, remover os proprios efeitos e inscricoes e encerrar sua existencia. Seus registros de aquisicao tambem deverao ser removidos pelo futuro sistema de upgrades. Nenhuma classe de upgrade foi criada nesta etapa.

## Ofertas e historico

List Available Enchantments e o catalogo, nao uma pilha da qual itens sao removidos. CanAcquire verifica o loadout atual para armas e os prefabs atualmente adquiridos para habilidades. Outros encantamentos seguem CanRepeat e o historico.

HasEnchantment e List Applied Enchantments continuam representando historico. Nao devem ser usados como prova de que uma habilidade ainda esta equipada. Use CanAcquire ou MN_PlayerAbilities.HasAbility para as regras de posse atuais.

OfferEnchantments(config) pode enfileirar pedidos. TryOfferEnchantments(config) retorna false se houver uma oferta ativa, sendo adequado para uma futura loja que nao deve cobrar por uma oferta que nao abriu.

GetRollsAmount e chamado uma vez por oferta valida. CurrentOfferRolls guarda o total inicial por carta para a futura implementacao de reroll. A HUD nao deve chamar GetRollsAmount de novo. Um skip adiciona o bonus ao default, mesmo que uma configuracao personalizada permita skip. Configuracoes separadas da loja nao consomem o bonus do default.

## Conferencia manual

1. Subir de nivel: aparecem apenas os tipos permitidos e o jogo pausa.
2. Escolher status: aplicar uma vez, fechar a oferta e retomar o jogo.
3. Escolher ativa: abrir slots, voltar para as cartas e escolher novamente.
4. Substituir uma ativa: somente a nova permanece no slot e a antiga pode voltar ao sorteio.
5. Trocar arma: a equipada nao aparece, mas a anterior volta a ser elegivel.
6. Dar skip: nao aplicar encantamento e conferir o bonus em CurrentOfferRolls na proxima oferta.
7. Abrir uma oferta com Allow Skip desativado: nao mostrar skip e recusar TrySkipOffer.
8. Solicitar duas ofertas: concluir a primeira antes de abrir a segunda.
9. Simular prefab invalido: manter a habilidade anterior e permitir voltar/tentar novamente.

As cenas, prefabs e ScriptableObjects nao foram reconfigurados automaticamente nesta etapa.
