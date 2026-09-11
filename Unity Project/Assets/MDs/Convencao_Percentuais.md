# Convencao de valores percentuais

Atualizacao: 11/09/2026. Substitui as unidades antigas descritas no estudo da PoC.

## Regra de configuracao

Valores percentuais de gameplay sao configurados em pontos percentuais: 100 = 100%, 150 = 150%, 200 = 200%. Valores fracionarios continuam validos: 2.5 significa 2.5%, nao 250%.

Nao ha conversao automatica baseada na magnitude: 1 sempre significa 1% em um campo percentual. O sistema nao tenta adivinhar se o autor quis dizer 100%.

| Campo | Exemplo |
| --- | --- |
| PhysicForceScaling / MagicForceScaling | 200 adiciona 200% do atributo ao dano. |
| CRITICAL_DAMAGE | 125 causa 125% do dano total, ou 1.25x internamente. Nao significa +125% de bonus. |
| ATTACK_SPEED | 100 e velocidade normal; 150 e uma vez e meia. |
| CRITICAL_CHANCE | 25 corresponde a 25% de chance; o calculo limita a 0..100. |
| PHYSICAL_RESISTANCE / MAGIC_RESISTANCE | 30 reduz 30%; o calculo limita a 0..100. |
| LIFE_STEAL / VAMP | 10 cura 10% do FinalDamage dos tipos aceitos pelo componente. |
| Modificador PERCENT_OF_CURRENT | 10 acrescenta 10% do valor atual. |

Dano base, PF, PM, vida, XP, distancias e tempos continuam em unidades absolutas. Um dano base de 85 continua sendo 85. Duracao 0.5 continua sendo meio segundo.

Valores internos exigidos pelo Unity continuam normalizados quando necessario: Image.fillAmount, curvas/interpolacoes, tempos normalizados de animacao e multiplicador do Animator. As conversoes acontecem na fronteira do calculo/API, nao no valor de status configurado pelo designer.

## Formulas

```text
Dano antes do critico = BaseDamage + PF * PhysicForceScaling / 100
                                 + PM * MagicForceScaling / 100

Dano critico = Dano antes do critico * max(100, CRITICAL_DAMAGE) / 100
Dano recebido = Dano apos critico * (1 - clamp(Resistencia, 0, 100) / 100)
Cura = FinalDamage * max(0, LifeStealOuVamp) / 100
Animator.AttackSpeed = max(MinimumAttackSpeed, AttackSpeed) / 100
```

Dano real ignora resistencia. Resistencia ausente vale zero. Critico sem o status CRITICAL_DAMAGE conserva o fallback historico de 200%; um status presente abaixo de 100% fica limitado a 100%, sem criar dano adicional escondido. A base configurada do player e 125%.

AttackSpeedByStatus usa 100 como fallback quando o valor recebido nao e positivo e 1 como minimo padrao em porcentagem. O comportamento de fallback foi preservado, apenas suas unidades mudaram. Nao foi adicionado teto de velocidade.

## Aprimoramentos flat versus relativos

FLAT soma o valor diretamente na unidade do status. Em um atributo percentual, essa unidade e ponto percentual.

```text
CRITICAL_DAMAGE 125 + FLAT 12 = 137
CRITICAL_DAMAGE 125 + PERCENT_OF_CURRENT 12 = 140

ATTACK_SPEED 100 + FLAT 4 = 104
ATTACK_SPEED 150 + PERCENT_OF_CURRENT 10 = 165

PF 10 + FLAT 2 = 12
PF 10 + PERCENT_OF_CURRENT 20 = 12
```

Os mesmos conceitos valem para BuffData. Nao multiplicar um modificador PERCENT_OF_CURRENT novamente por 100: ele ja estava usando porcentagem antes desta migracao.

## Causa do critico exagerado

A base antiga do player era 1.25, mas CriticalImpact adicionava 12 como FLAT. O resolver tratava o resultado 13.25 como multiplicador direto. Um golpe de 85 poderia virar 1126.25 antes das demais condicoes.

Com a convencao nova: base 125 + 12 = 137, multiplicador interno 1.37. O mesmo golpe de 85 causa 116.45, antes de resistencia. O calculo de soma do aprimoramento nao precisava de um caso especial para critico: precisava receber atributos na mesma unidade dos dados de balanceamento.

## Arquivos de codigo afetados

- `Combat/DamageResolver.cs`: divide escalonamentos e dano critico por 100.
- `Combat/Data/DamageData.cs`: documenta a unidade dos escalonamentos no Inspector.
- `Status/Appliers/AttackSpeedByStatus.cs`: defaults em porcentagem e conversao apenas antes de enviar ao Animator.
- `Status/StatusInfo.cs`: tooltips de unidades dos valores de status.
- `Scriptable Base/Enchantment/StatusUpgradeSO.cs` e `Combat/Data/BuffData.cs`: tooltips esclarecendo FLAT e PERCENT_OF_CURRENT. A formula relativa ja estava correta.
- `Enchantment/Description/EnchantmentDescriptionFormatter.cs`: mostra escalonamentos diretamente com %, sem multiplicar novamente por 100.

Resistencias, chance critica, cura percentual e buffs/aprimoramentos relativos ja dividiam por 100. Nao receberam uma segunda conversao. A HUD recebe os valores de status na nova unidade sem conversoes extras.

## Migracao de conteudo e verificacao

A ferramenta Editor `PercentageConventionMigration` converte somente os campos legados identificados na cena, SOs e prefabs do jogo, usando APIs do Unity. Nao faz substituicao textual de assets. O relatorio `Migracao_Percentuais.json` registra objeto, propriedade, antes e depois, e impede uma segunda aplicacao acidental.

Conversoes aplicadas: 24 valores em 15 arquivos. Escalonamentos x100, bases de critico/attack speed x100, defaults do aplicador x100 e aprimoramentos FLAT de attack speed x100. CriticalImpact (+12) e CriticalWeight (+8) ja estavam em pontos percentuais e nao foram multiplicados.

As verificacoes de regressao podem ser executadas em `Tools > Project Instanct Action > Percentages > Run Calculation Checks`. As 18 assercoes passaram no Editor, exercitando DamageResolver, modificadores e formatacao de descricoes sem alterar dados da partida. `Verify Migrated Values` le os valores dos assets/cena pelo Unity e compara com o relatorio de migracao.
