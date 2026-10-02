# Combat Workshop v0.1

## Abrir

Menu `Tools > Combat Workshop`. A abertura procura um loadout de maos e o objeto `Player` da cena como ponto de partida. Tambem aceita um prefab de personagem no campo Preview character e qualquer WeaponLoadoutSO no campo superior.

Socket registry aceita o WeaponSocketRegistry mesmo quando ele esta fora do personagem. Na cena atual, o Workshop detecta o registro do MN Weapons e remapeia suas referencias para a copia visual do Player. Materials alterna entre o material neutro de inspecao e os materiais originais, sem modificar os assets.

## Fluxo

1. Carregue um loadout ou use `+` na barra superior para criar um rascunho novo.
2. Em Loadout, configure ID, nome, controller base, state de idle e clip de idle.
3. Em Weapons, adicione prefabs e escolha os sockets. Posicao, rotacao e escala sao locais a cada socket. O ponto azul move a arma; o modo de rotacao usa um anel. Os campos numericos permitem ajustes nos tres eixos.
4. Em Attacks, selecione o state, o clip e os dados de cada ataque. Ao substituir o clip, seus eventos de combate sao lidos; eventos do clip anterior nao sao transportados automaticamente.
5. Adicione os marcadores Hit, Open Combo, Close Combo e Finish. Arraste-os na linha do tempo, edite o tempo em segundos ou use `@` para colocar no cursor. Default markers fornece somente um ponto de partida, nao detecta o impacto da animacao.
6. Arraste o ponto amarelo da hitbox para mover e o ponto branco para redimensionar. Os handles operam no plano da camera; use os campos numericos para profundidade. Arraste com o botao direito para orbitar e use a roda para zoom.
7. Use Play, os botoes de frame, Loop e Combo para visualizar. Velocidade da previa usa porcentagem: 100 e a velocidade normal.
8. Valide em Checks e salve pelo icone de disquete.

## Dados e seguranca

- Tudo roda no Editor, sem adicionar dependencia ao build do jogo.
- A previa usa copias isoladas, sem scripts de gameplay, eventos de animacao, audio ou fisica ativos.
- Edicoes dos dados e marcadores ficam no rascunho ate salvar; ha Undo/Redo e confirmacao de descarte.
- Register Addressable e uma acao separada que altera imediatamente o cadastro Addressables do modelo. Nao faz parte do rascunho do loadout.
- Salvar atualiza o WeaponLoadoutSO e seu ComboAttackSetSO. Se esse conjunto for compartilhado por outros loadouts, eles tambem receberao as alteracoes. Duplique o conjunto antes caso deseje variantes independentes.
- O Workshop cria uma pasta propria ao lado do loadout, identificada pelo nome e parte do GUID. Ali ficam o AnimatorOverrideController e os clips editaveis Attack_1.anim, Attack_2.anim etc.
- Clips de origem nao sao modificados. Eventos que nao pertencem aos quatro eventos de combate sao preservados nas copias; loops sao desligados nos ataques.
- Salvar novamente atualiza as mesmas referencias. Remover ataques nao apaga automaticamente os arquivos antigos da pasta.
- O controller base nao e reestruturado: precisa ter os states desejados, no layer zero, com clips-base distintos. Blend Trees nao sao aceitos como slots de ataque/idle nesta versao.
- Nao cria encantamentos nem altera o catalogo ou o equipamento atual da cena.

## Previa e limites

A hitbox usa a origem e a opcao de rotacao do AttackHitbox do personagem. Direcao usa a configuracao do PlayerDirection. Avanco reproduz a curva, distancia, duracao e delay existentes. A caixa verde e um alvo geometrico de teste: fica laranja quando um marcador de impacto coincide com a sobreposicao. Nao representa resistencias, layers, regras de dano ou colisao real do gameplay.

Combo reproduz os clips em sequencia ate seus marcadores Finish. Nao simula buffer de input, transicoes do Animator, cancelamentos, hit reactions nem o teste completo contra IA. Nao gera animacoes ou modelos, nao edita poses e nao implementa root motion. A fonte autoritativa para o feeling final continua sendo o Play Mode.

Os estados com rotacoes personalizadas, root motion e armaduras incompativeis podem diferir da previa. Teste o loadout no personagem real depois de salvar. Para nova arma, a ferramenta precisa receber modelos e clips existentes, com rig/avatar compativeis.

## Verificacao

`Tools > Combat Workshop Checks` executa verificacoes de criacao, salvamento, reabertura, referencias, eventos e geometria. As fixtures sao criadas em uma pasta temporaria dentro de Assets e removidas pelas APIs da Unity ao terminar.
