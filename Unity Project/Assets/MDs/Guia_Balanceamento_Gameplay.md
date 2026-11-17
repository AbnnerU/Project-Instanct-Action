# Guia de gameplay e balanceamento

Atualizado em 14/11/2026. Documento de contexto para discutir balanceamento fora do projeto Unity. Nao e um guia de programacao.

## 1. Objetivo e limites deste documento

O projeto e um jogo de combate em uma arena, com movimentacao lateral, armas corpo a corpo, dash, habilidades e escolhas de aprimoramentos durante a partida. A versao V1 pretende ser uma demonstracao jogavel para buscar investimento.

O jogador enfrenta inimigos continuamente, recebe XP e moedas e construi uma build durante a partida. A morte leva ao game over, com opcao de recomecar.

As regras abaixo combinam comportamento atual conferido no projeto e decisoes de design. Valores de exemplos NAO sao automaticamente valores aplicados no jogo. Configuracoes de prefabs e cenas podem mudar; este documento nao e um inventario completo dos valores atuais.

League of Legends, ARAM: Desordem e Arena sao referencias desejadas para o balanceamento. Nao importar seus numeros diretamente sem considerar as diferencas de combate, quantidade de alvos e duracao da partida.

## 2. Loop da partida

1. Comecar com status e arma iniciais.
2. Enfrentar inimigos melee e ranged que continuam aparecendo enquanto houver capacidade.
3. Receber XP e moedas por mortes, conforme recompensas configuradas.
4. Escolher encantamentos ao subir de nivel e comprar novas selecoes com moedas.
5. Trocar armas e habilidades, acumular bonus e enfrentar uma pressao crescente.
6. Morrer, ver o painel de game over e reiniciar pelo loading.

O game over nao pausa automaticamente o tempo. O evento de game over deve parar o spawner. Os inimigos deixam de perseguir/atacar um alvo morto. Essas ligacoes dependem da configuracao da cena.

Ainda precisam ser definidos como metas de balanceamento: duracao desejada de uma partida, nivel esperado ao final, ritmo de escolhas e eventual condicao de vitoria.

## 3. Combate e armas

- A arma define um conjunto de ataques em combo e pode ter um ataque especial pos-dash.
- Uma animacao pode conter varios hits. Cada hit tem dano, escalonamentos, area e knockback proprios.
- O dano de um combo deve somar todos os hits que realmente atingem o alvo, nao apenas contar animacoes.
- Ha ajuda de aproximacao e influencia do movimento das animacoes. Alcance, deslocamento e recuperacao tambem fazem parte da forca da arma.
- A direcao do ataque e escolhida pelos inputs direcionais de ataque; nao deve mudar no meio do golpe.
- Apertar ataque durante o dash pode guardar um ataque pos-dash e sua direcao. Ele e executado ao terminar um dash valido.
- O golpe pos-dash preserva a continuidade do combo conforme a regra de retencao; seu tempo nao deve consumir o prazo reservado para retomar o combo.
- Cancelar depois de executar uma hitbox pode considerar aquele ataque como realizado para a continuidade do combo.

Armas discutidas no projeto incluem punhos, machado e martelo de duas maos. Nao assumir que todas possuem valores ou identidade de balanceamento finalizados.

Comparar por arma: dano por hit, dano por combo, tempo ate o primeiro hit, duracao total, recuperacao, DPS pratico, alcance, cobertura de grupo, knockback, compromisso de posicionamento e ataque pos-dash. Uma arma lenta nao precisa ter o mesmo DPS de uma rapida se oferecer outras vantagens relevantes.

## 4. Unidades dos status

Percentuais sao configurados na escala 0-100: 10 significa 10%, 100 significa 100%, 150 significa 150%. Nao usar 0,1 para representar 10%.

| Status | Significado para balanceamento |
| --- | --- |
| Vida | Quantidade absoluta de vida maxima |
| Poder fisico (PF) | Valor absoluto que contribui para danos com escalonamento fisico |
| Poder magico (PM) | Valor absoluto que contribui para danos com escalonamento magico |
| Velocidade de ataque | 100 representa a velocidade normal; avaliar o impacto no tempo real dos combos |
| Chance critica | Percentual de chance; o calculo limita a faixa efetiva a 0-100 |
| Dano critico | Multiplicador TOTAL em percentual: 125 significa 1,25x, nao +125% |
| Resistencia fisica | Percentual direto de reducao de dano fisico |
| Resistencia magica | Percentual direto de reducao de dano magico |
| Roubo de vida | Percentual de cura sobre dano aplicado elegivel |
| Vamp | Percentual de cura sobre dano aplicado elegivel |
| Stamina | Status existente; seu papel efetivo precisa ser confirmado antes de balancear |

Vamp e roubo de vida dependem dos tipos de dano aceitos pela configuracao. Nao presumir automaticamente a mesma distincao entre ataques/habilidades usada no LoL. Atualmente a elegibilidade da cura pode ser filtrada pelo TIPO de dano.

## 5. Formulas de dano e cura

```text
Dano antes do critico = dano base + PF * escala PF / 100 + PM * escala PM / 100
Dano critico = dano anterior * dano critico percentual / 100
Dano fisico final = dano apos critico * (1 - resistencia fisica / 100)
Dano magico final = dano apos critico * (1 - resistencia magica / 100)
Dano real final = dano apos critico, sem reducao por resistencia
Cura = dano final elegivel * percentual de cura / 100
```

Resistencias sao limitadas entre 0% e 100% no calculo. Isso NAO e a formula de armadura do LoL. O multiplicador critico nao fica abaixo de 1x; se o status estiver ausente, existe fallback de 2x, que nao deve ser confundido com o valor inicial configurado do jogador.

Por padrao, apenas dano fisico pode critar. Um encantamento permite criticos magicos e reais, usando a chance e o multiplicador existentes. Permitir critico nao concede chance critica por si so.

Tipo de dano e fonte de escalonamento sao independentes: dano magico pode escalar com PF, por exemplo. Aumentar PF/PM nao aumenta uma parcela de dano base fixa sem escalonamento correspondente.

Exemplo didatico: 20 de dano base + 100% de PF, com PF 60, gera 80 antes de critico/resistencia. Com resistencia de 25%, gera 60. Com critico de 150%, gera 90 apos essa resistencia.

## 6. Encantamentos e decks

Encantamentos podem dar status, armas, habilidades ou outras regras especiais. As categorias tambem incluem economia e upgrades especificos de arma/habilidade; estes ultimos ainda sao excluidos pelo construtor padrao de ofertas.

- A selecao usa um deck/configuracao que determina categorias permitidas, rerolls e permissao de skip.
- Armas e habilidades atualmente equipadas nao devem ser oferecidas novamente.
- Ao substituir uma arma ou habilidade, a anterior pode voltar a ser elegivel no deck.
- A habilidade removida limpa seus efeitos e dependencias. Upgrades associados a ela, quando existirem, devem sair junto.
- Encantamentos de status podem se repetir quando configurados para isso.
- O historico impede a repeticao dos encantamentos marcados como nao repetiveis.
- Na mesma oferta, nao ha duplicatas da mesma carta, nem mais de uma carta de arma, nem mais de uma carta de habilidade. A restricao de habilidades abrange ativas e passivas juntas.
- Se faltarem candidatos validos, a oferta pode ter menos cartas que o solicitado.

### Tiers e probabilidades

Existem quatro tiers. Os pesos encontrados no asset padrao sao:

| Tier | Peso | Chance quando todos os tiers tem candidatos |
| --- | ---: | ---: |
| 1 | 50 | 50% |
| 2 | 30 | 30% |
| 3 | 15 | 15% |
| 4 | 5 | 5% |

Primeiro sorteia-se o tier; depois uma carta elegivel daquele tier, com chances iguais entre as cartas candidatas. Sao PESOS relativos, nao chances fixas independentes. Tiers vazios sao excluidos e os pesos restantes sao normalizados novamente a cada escolha.

Exemplo: se so os tiers 1 e 2 estiverem disponiveis, suas chances passam a 62,5% e 37,5%. Peso zero desabilita um tier. As restricoes de cartas ja sorteadas tambem mudam o conjunto elegivel das proximas cartas.

Nao ha aqui uma curva confirmada de raridade por nivel. Nao assumir que tier 4 so aparece tarde ou que cada oferta garante um tier especifico.

### Reroll, skip e compra

- Reroll substitui uma carta, respeitando as outras cartas da oferta e excluindo a carta substituida.
- Sem substituto valido, nao realiza o reroll.
- Skip concede rerolls adicionais para uma proxima abertura da configuracao correspondente; o bonus e consumido na abertura, nao fica sendo somado permanentemente.
- A selecao comprada nao permite skip pela regra de design. O preco e o ganho de moedas precisam ser balanceados juntos.
- Quantidade de cartas, rerolls basicos, bonus por skip e custo da compra devem ser conferidos na configuracao usada antes de simular a economia.

### Bonus de status

- FLAT adiciona unidades. Em um status percentual, adiciona pontos percentuais.
- Percentual do atual adiciona uma fracao do valor atual, podendo produzir crescimento composto com repeticoes.
- Exemplo: velocidade 100 + 10 flat = 110. Mais 10% do atual resulta em 121.
- Vamp e roubo de vida foram planejados com bonus flat de pontos percentuais.
- Encantamentos de resistencia fisica/magica foram retirados do escopo solicitado; resistencias continuam existindo nos inimigos.
- Bonus mistos devem oferecer valores individuais menores que bonus dedicados, sem assumir que a simples soma dos numeros mede seu poder.

## 7. Habilidades para comparar

| Habilidade | Papel e funcionamento |
| --- | --- |
| Bola de fogo | Projetil na direcao de conjuracao; dano base e escalonamentos, atraso de disparo e recarga |
| Estocada perfurante | Avanco atravessando inimigos, com dano em area ao longo da execucao; combina mobilidade e ofensiva |
| Ruptura arcana | Marca fixa a frente, atraso e explosao em area; jogador fica livre apos conjurar; remover a habilidade limpa marcas pendentes |
| Orbes | Tres objetos orbitam o jogador por uma duracao; dano por contato, intervalo por alvo por orbe; reutilizar substitui o conjunto; nao bloqueia nem cancela combo |
| Ataque magico passivo | Bonus de dano em ataques conforme uma cadencia configuravel, originalmente a cada terceiro ataque |
| Buff apos dash | Bonus temporario de status, originalmente PF; novo acionamento renova a duracao |
| Recuperacao de recarga passiva | Acertos de hitbox recuperam segundos das ativas equipadas; uma vez por execucao de hitbox e com intervalo minimo |

A recuperacao de recarga inclui o pos-dash no fluxo atual, mas nao projeteis, dano adicional de passivas, orbes ou dano das proprias habilidades. Nao afeta o dash nem reduz permanentemente a recarga base. Multi-hit pode gerar varias oportunidades, limitadas pelo intervalo minimo.

Ativas ocupam slots escolhidos pelo jogador. Algumas execucoes bloqueiam outros slots; durante o dash todas ficam bloqueadas pela regra adotada. Recarga e bloqueio de execucao sao conceitos distintos. Passivas nao ocupam os slots ativos.

Para cada habilidade registrar: dano por alvo, numero pratico de alvos, numero de ticks/hits, duracao, tempo de conjuracao, recarga, cobertura espacial, mobilidade, possibilidade de critico e cura gerada. Nos orbes, considerar que tres orbes podem atingir o mesmo alvo.

## 8. Inimigos e progressao

Ha inimigos corpo a corpo e ranged. Cada tipo possui status-base proprios e pode ter resistencias diferentes.

O crescimento de status por nivel e linear sobre a BASE:

```text
Multiplicador percentual = 100 + crescimento por nivel * (nivel do jogador - 1)
Status do inimigo = status-base * multiplicador percentual / 100
```

Exemplo: crescimento de 10 por nivel resulta em 120% no nivel 3, nao 121%. Vida, PF e PM sao os status inicialmente previstos para crescimento. O dano fixo dos ataques nao cresce automaticamente junto com PF/PM.

Os valores sao capturados ao spawn/reativacao. Inimigos vivos mantem os valores anteriores; os novos usam o nivel atualizado. Reutilizar um inimigo do pool deve recalcular a partir da base, sem acumular bonus de spawns anteriores.

O spawn e continuo, nao exige matar todos para iniciar uma onda seguinte. Marcos por nivel configuram intervalo e limite de inimigos vivos. Vale o maior marco ja alcancado, ate chegar ao proximo. Diminuir o limite nao remove inimigos existentes. Uma espera ja iniciada pode terminar com o intervalo anterior.

Pontos ocupados sao evitados: tenta outros pontos e aguarda quando todos estao bloqueados. Portanto, intervalo nominal nao equivale necessariamente a quantidade real de inimigos gerada por segundo.

## 9. Referencia numerica discutida

Levantamento anterior de 20 campeoes do LoL, nivel 1, sem itens/runas/modificadores de modos, usando Data Dragon 16.22.1:

- Vida media: 620,75.
- AD medio: 58,2.
- AP base: 0; habilidades possuem dano base independente de AP.
- Referencia arredondada sugerida para teste: 620 de vida e 60 de PF. NAO e confirmacao dos valores atualmente aplicados no projeto.
- Fonte dos atributos: https://ddragon.leagueoflegends.com/cdn/16.22.1/data/en_US/champion.json

## 10. Ordem de trabalho sugerida

1. Fixar status iniciais do jogador e de um inimigo de referencia, sem progressao.
2. Balancear armas contra esse alvo: golpes para matar e tempo real para matar.
3. Balancear habilidades em relacao as armas e ao seu custo de oportunidade.
4. Balancear encantamentos, tiers e sinergias. Incluir o nerf de velocidade de ataque.
5. Ajustar crescimento dos inimigos, ritmo de spawn e economia para builds de varios niveis.

Velocidade de ataque merece atencao especial: aumenta DPS, frequencia de efeitos por ataque, oportunidades de cura e recuperacao de recarga. Nao avaliar seu valor apenas pelo dano de uma arma isolada.

Testar builds fisica, magica, critica, velocidade/multi-hit e sustentacao. Comparar builds medianas, fracas e muito favorecidas pelo sorteio, nao apenas a melhor combinacao.

## 11. Registro recomendado para os testes

Registrar por teste: versao, nivel, arma, habilidades, encantamentos, status totais, inimigo, resistencias, tempo para matar, dano recebido, cura, inimigos vivos, mortes por minuto e percepcao do jogador.

Manter separados: valor atual confirmado, proposta, justificativa, resultado observado e decisao. DPS teorico nao inclui deslocamento, erros de posicionamento, interrupcoes e tempo sem alvo.

Pendencias que precisam de verificacao antes de calcular curvas finais:

- Valores reais de cada arma/hit, habilidade, carta e prefab de inimigo.
- Status iniciais efetivamente configurados e quantidades de recompensas.
- Curva de XP e preservacao do excedente: houve uma decisao de preservar, mas uma leitura anterior ainda mostrava zerar XP ao subir de nivel. Nao presumir resolvido sem conferir.
- Se a cura considera dano excedente a vida restante do alvo; isso pode inflar sustentacao e deve ser medido.
- Qual configuracao de tiers/deck esta realmente ligada na cena de teste.
- Ritmo alvo da partida e identidade desejada de cada arma.

Nao alterar automaticamente o projeto a partir desta analise externa. As propostas devem voltar ao desenvolvedor para aplicacao e teste no jogo.
