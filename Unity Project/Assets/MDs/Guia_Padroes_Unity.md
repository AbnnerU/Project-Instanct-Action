# Guia de Padroes de Codigo e Arquitetura Unity

Este documento consolida os padroes internos extraidos da documentacao da empresa. Ele foi escrito para ser usado por programadores e tambem como contexto para assistentes de IA antes de mexerem no projeto.

Use este guia como uma referencia normativa: quando houver duvida entre seguir uma preferencia generica de Unity/C# e seguir o padrao descrito aqui, priorize o padrao deste documento, desde que ele faca sentido para o contexto do projeto.

## Principios gerais

- Todo codigo deve ser escrito em ingles.
- Prefira codigo modular, com baixo acoplamento e responsabilidades claras.
- Evite classes "faz tudo". Uma classe deve ter uma razao clara para existir e uma razao clara para mudar.
- Sempre que um script precisar conversar com outro, prefira depender de interfaces, classes abstratas ou eventos em vez de acessar diretamente uma implementacao concreta.
- Mantenha padroes de nome, hierarquia e organizacao consistentes do inicio ao fim do projeto.
- Evite chamar muitos metodos diferentes diretamente pelo Inspector, especialmente em botoes de HUD. Prefira scripts intermediarios, interfaces ou gerenciadores quando a acao for grande.
- Prefira estruturas previsiveis e "plug and play", especialmente com ScriptableObjects quando fizer sentido.
- Imagens e assets visuais devem ficar proximos dos prefabs correspondentes quando isso facilitar manutencao.

## Nomenclatura de codigo

### Variaveis

Variaveis usam substantivos.

```csharp
private int _myVariable;
int myVariable;
public int MyVariable;
public static int MY_VARIABLE;
```

Regras:

- Variaveis privadas usam prefixo `_` e camelCase: `_myVariable`.
- Variaveis dentro de metodo usam camelCase: `myVariable`.
- Variaveis publicas usam PascalCase: `MyVariable`.
- Variaveis estaticas usam maiusculas com separacao por `_`: `MY_VARIABLE`.

### Interfaces

Interfaces usam prefixo `I`.

```csharp
public interface IMyInterface
{
}
```

### Classes abstratas

Classes abstratas usam sufixo `Base`.

```csharp
public abstract class MyClassBase
{
}
```

### Metodos

Metodos usam verbos, pois representam acoes.

```csharp
public void DoSomething()
{
}
```

### Booleanos

Prefira prefixar variaveis booleanas com `is`, ou `Is` quando forem publicas, apenas quando isso fizer sentido no contexto. O mesmo vale para metodos que retornam `bool`.

```csharp
private bool _isAlive;
public bool IsTargetNearby()
{
    return true;
}
```

Nao force `is` quando o nome ficar artificial.

### Colecoes

Use nomes no plural e prefixos claros conforme o tipo:

```csharp
private List<string> _listNames;
private Dictionary<int, string> _dcNames;
private string[] _names;
private Queue<string> _queueNames;
private HashSet<string> _hashsetNames;
```

Padroes:

- `List_` ou `_list`: listas.
- `Dc_` ou `_dc`: dicionarios.
- Plural simples para arrays.
- `Queue_` ou `_queue`: filas.
- `Hashset_` ou `_hashset`: conjuntos.

### Enums

A declaracao do enum usa substantivo no singular. A variavel pode ter prefixo `e` e os valores usam maiusculas com `_`.

```csharp
private Colors _eColors;
public Colors eColors;

private enum Colors
{
    COLOR_BLUE,
    COLOR_RED
}
```

### Prefixos de metodos e variaveis

Use prefixos que comuniquem a intencao:

- `Get`: pegar ou retornar algo.
- `Set`: definir.
- `Add`: adicionar.
- `Delete`: deletar, geralmente apagar ou destruir.
- `Create`: criar.
- `Remove`: remover, geralmente de lista ou colecao.
- `Try`: tentar realizar algo, sempre retornando `bool` ou usando parametro `out`.
- `Is`: verificar algo, normalmente retornando `bool`.
- `On`: indicar gatilho, callback ou ponto de acao.

### Sufixos de variaveis

Use sufixos para deixar claro o tipo de recurso:

- `Animation`: animacoes, por exemplo `RunAnimation`.
- `SFX`: efeitos sonoros, por exemplo `RunSFX`.
- `VFX`: particulas ou efeitos visuais, por exemplo `RunVFX`.
- `Component`: opcional para componentes quando ajuda a diferenciar nomes, por exemplo `ButtonComponent`, `ImageComponent`, `NavMeshAgentComponent`.

## Eventos

O evento padrao e `Action`. Quando necessario, tambem podem ser usados `Func<T, TResult>` e `UnityEvent`.

Ordem de preferencia:

1. `Action` e `Func`.
2. `UnityEvent`.

Prefixos:

- Eventos `Action` e `Func` usam prefixo `On`.
- Eventos `Func` podem usar prefixo `Func` quando isso melhorar a leitura.
- Eventos `UnityEvent` usam prefixo `UE`.

Exemplo:

```csharp
public event Action OnHitted;
public event Func<bool> FuncHitted;
public UnityEvent UEHitted;

OnHitted?.Invoke();
bool result = FuncHitted?.Invoke() ?? false;
UEHitted?.Invoke();
```

Sempre use `?.Invoke()` para disparar eventos.

## Hierarquia Unity

O idioma da hierarquia tambem deve ser ingles.

Objetos pais padrao devem usar prefixo `_`.

Exemplos:

- `_Setup`
- `_Canvas`
- `_System`
- `_Audio`
- `_Feel`
- `_Missions`
- `_Dialogues`
- `_Scenario`
- `_VFX`
- `_Global`

### Objetos pais principais

- `_Setup`: camera, directional light, EventSystem e scripts de setup.
- `_Canvas`: principais canvases do jogo.
- `_System`: tudo que controla ou faz parte do gameplay. No geral, scripts.
- `_Audio`: audios.
- `_Feel`: feedbacks do More Mountains Feedbacks.

Objetos opcionais:

- `_Missions`: missoes do jogo.
- `_Dialogues`: conversas e dialogos.
- `_Scenario`: parte visual 2D ou 3D.
- `_VFX`: efeitos visuais.
- `_Global`: elementos comuns entre cenas.

Se o projeto precisar de outros objetos pais, eles podem ser adicionados, desde que o padrao seja mantido em todas as cenas.

### Organizacao de `_System`

Dentro de `_System`, espera-se pelo menos:

- `_Core`: scripts do core loop do jogo.
- `_Managers`: scripts gerenciadores.

Dependendo do projeto, tambem pode existir `_Save` ou `_Load` para operacoes que nao sao responsabilidade de gerenciadores. Eles podem ficar separados ou dentro de prefabs, desde que os nomes `_Save` e `_Load` sejam preservados.

### Organizacao de `_Audio`

Filhos comuns:

- `_SFX`
- `_Music`
- `_Ambient`

### Organizacao de `_Canvas`

No canvas principal, use `_MainMenu` quando o canvas estiver no menu inicial. Em outras cenas, nomes comuns incluem:

- `_Main`
- `_Vitory`
- `_Derrota`
- `_Pause`

Mantenha a base de nomenclatura, mesmo quando o projeto exigir variacoes.

### VFX

`_VFX` pode ficar dentro de `_Scenario` ou na raiz da hierarquia, dependendo da necessidade.

### Player, mobs e objetos spawnados

Para o player, crie um objeto `_Player`. Normalmente ele nao precisa ser filho de outro objeto.

Para mobs, projeteis e objetos criados em grande quantidade durante o jogo, prefira um objeto pai com sufixo `Container`.

Exemplos:

- `Enemies Container`
- `Bullets Container`

### Nome de objetos na hierarquia

Use o formato:

```text
Nome desejado + O que e o objeto
```

Exemplos:

- `Click SFX`
- `Ambient Music`
- `Jump Particle System`
- `Upgrade Feel`

Para UI, sempre especifique o tipo do objeto:

- `Menu - Button`
- `Inventory - Scroll View`
- `Main Image - Image`
- `Game Title Label - Text (TMP)`

Para backgrounds, use `Background` ou `BG`:

- `Background - Panel`
- `Menu BG - Panel`
- `Inventory BG - Image`

Para titulos, campos ou textos similares, prefira `Label`:

- `Game Title Label - Text`
- `Name Label - Text`
- `Age Label - Text`

### `_Global`

`_Global` deve ser obrigatoriamente um prefab. Ele centraliza referencias e sistemas comuns entre cenas, evitando perda de referencias no Inspector ao trocar de cena.

Objetivos:

- Evitar "Reference Hell".
- Centralizar alteracoes compartilhadas entre cenas.
- Reutilizar sistemas comuns em multiplas fases.

Estrutura comum:

```text
_Global
  _System
  _Canvas
  _Audio
  _Feel
```

Regra de coexistencia:

- Tudo que e regra geral do jogo fica dentro de `_Global`.
- Tudo que e exclusivo da fase fica na raiz da cena, fora de `_Global`.

Exemplo:

```text
_Global
  _System
  _Canvas
  _Audio
_Setup
_Scenario
_System
_VFX
Enemies Container
```

Nesse caso, `_System` dentro de `_Global` gerencia o core loop levado entre cenas. O `_System` da cena gerencia apenas o level loop local.

## Localizacao de scripts

Evite colocar scripts fora de `_System`, especialmente em objetos aleatorios da hierarquia. Reserve a hierarquia para objetos de jogo e coloque a logica dentro de `_System`.

Crie um objeto filho dentro de `_System` para cada sistema diferente. O nome do objeto que contem um script deve ser:

1. O mesmo nome do script.
2. Um nome similar ao script.
3. Um nome descritivo do que aquele sistema contem.

Exemplo:

```text
_System
  _Core
    Traffic System
    Police System
    Inventory
```

Se um sistema tiver uma parte logica e uma parte visual:

- Pode colocar todos os scripts em um unico objeto filho de `_System`.
- Ou criar um objeto filho de `_System` com a parte logica e filhos internos para a parte visual.

Escolha um padrao e mantenha-o ate o fim do projeto.

Excecoes aceitaveis fora de `_System`:

- Scripts que ficam em prefabs.
- Scripts dependentes de eventos de HUD, como `IPointerDownHandler`, `IPointerUpHandler`, etc.
- Scripts de movimento do player, que costumam pertencer ao proprio player.

## Gerenciadores

Scripts gerenciadores coordenam, atualizam, salvam, carregam ou controlam partes do jogo de forma centralizada. Eles sao nucleos de execucao ou orquestracao e geralmente possuem maior acoplamento ao fluxo do jogo.

Objetivos:

- Controlar aspectos de gameplay ou sistemas que exigem supervisao global.
- Persistir e propagar dados entre cenas ou sistemas.
- Manter estados globais ou sensiveis durante a execucao.
- Centralizar o que nao faz sentido descentralizar.

Caracteristicas:

- Tem papel especifico e intransferivel.
- Sao dificeis de reaproveitar em outros projetos.
- Costumam ser mais "hard coded" por serem ligados ao jogo atual.
- Normalmente sao `Singleton` ou `SceneSingleton`.
- Podem ser scene-bound ou persistentes via `DontDestroyOnLoad`.

Prefixos:

- Gerenciadores `Singleton`: `SIS_`
- Gerenciadores `SceneSingleton`: `MN_`

Exemplos:

```text
SIS_Ads
MN_Map
```

### O que nao deve ser gerenciador

Nao transforme em gerenciador:

- Scripts reaproveitaveis como `Health`, `Weapon`, `InputHandler`.
- Classes auxiliares ou utilitarias como `Timer`, `ObjectPool`, `DamageCalculator`.
- Controladores locais como `EnemyController` ou `PlayerMotor`.

Esses itens pertencem a dominios genericos, modulares ou especificos de entidade, nao ao controle global do jogo.

### Tipos comuns de gerenciadores

Gerenciador Save/Load:

- Controla leitura e escrita de dados persistentes.
- Pode lidar com PlayerPrefs, JSON, banco de dados ou servicos online.
- Salva e carrega progresso, configuracoes, inventario e conquistas.

Gerenciador de status de jogo:

- Controla modos ou estados do jogo, como menu, jogando, pausa, vitoria e derrota.
- Controla transicoes de estado.
- Informa outros sistemas quando ha mudanca de estado.
- Pode lidar com input global, como ESC para pausar.

Gerenciador de acoes complexas:

- Centraliza acoes que envolvem varios sistemas ao mesmo tempo.
- Exemplo: entrar em veiculo, dormir, usar item especial, trocar modo de controle.
- Orquestra sequencias e evita que sistemas precisem se conhecer diretamente.

Gerenciador de fases:

- Controla carregamento e descarregamento de cenas.
- Controla transicoes entre fases.
- Carrega cenas async.
- Mostra loading screen.
- Reseta dados de gameplay quando necessario.

Gerenciador de score:

- Armazena pontos, combos, high score ou metricas de desempenho.
- Reseta dados ao iniciar nova partida.
- Compara e carrega dados de pontuacao.

Gerenciador visual de HUD:

- Controla exibicao de paineis, mensagens, HUD e janelas contextuais.
- Ativa ou desativa elementos de UI globalmente.
- Mostra mensagens ou efeitos visuais centralizados.
- Mantem controle do que esta visivel e quando.

Idealmente, mantenha gerenciadores separados por funcao. Um gerenciador pode acumular mais de um papel em projetos pequenos, mas isso deve ser uma decisao consciente.

## Singleton e SceneSingleton

Use `Singleton` apenas quando a instancia realmente precisa persistir entre cenas. Use `SceneSingleton` quando a instancia deve existir apenas dentro da cena atual.

### Singleton

Conceito:

- Garante uma instancia global unica.
- Sobrevive a troca de cenas.
- Usa `DontDestroyOnLoad`.

Use para:

- Gerenciadores de sistemas realmente globais, como audio, save, estado principal do jogo.
- Componentes que precisam manter estado entre fases.

Base padrao:

```csharp
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}
```

### SceneSingleton

Conceito:

- Garante uma instancia unica dentro da cena.
- Nao persiste entre cenas.
- Ao recarregar a cena, uma nova instancia sera criada.

Use para:

- Gerenciadores ou orquestradores locais da cena.
- Controles de UI locais.
- Spawners locais.
- Sistemas hard-coded que so fazem sentido naquela fase.

Base padrao:

```csharp
using UnityEngine;

public abstract class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
    }
}
```

### DataImporterBase

Use para importadores de dados que carregam informacoes ao iniciar a cena.

```csharp
using UnityEngine;

public abstract class DataImporterBase<T> : SceneSingleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        LoadData();
    }

    protected abstract void LoadData();
}
```

## Partial classes

Nao adote `partial` como padrao para scripts de gameplay.

`partial` existe em C# e pode dividir uma classe em varios arquivos, mas no Unity isso costuma quebrar a expectativa de "um script - uma classe" e pode gerar sobrecarga cognitiva.

Regras:

- Evite `partial` para separar gameplay, movimento, animacao, input ou regras de jogo.
- Se uma classe ficou grande o bastante para parecer precisar de `partial`, normalmente ela deve ser separada em componentes menores.
- Em `MonoBehaviour`, apenas o arquivo cujo nome coincide exatamente com o nome da classe sera reconhecido como componente pelo Unity.
- Use `partial` apenas em casos especiais, como codigo gerado, logica de editor ou builds multiplataforma, e sempre respeitando as regras de nomenclatura/assembly do Unity.

Exemplo de decisao:

- Ruim: `PlayerController` parcial dividido em movimento e animacao.
- Melhor: `PlayerController` para movimento e `AnimationController` para animacao, comunicando-se por interface ou evento.

## Interfaces padrao

As interfaces abaixo representam contratos comuns. Use-as quando o comportamento esperado bater com o contrato. Evite criar uma interface grande demais quando interfaces pequenas resolvem melhor.

```csharp
public interface IHealable
{
    event Action OnHeal;
    void Heal(float healValue);
    bool ItIsAtMaximumHealthValue();
}

public interface IHittable
{
    event Action<int> OnHitted;
    void ResetHealth();
    void Hit(float damage);
    void Hit(float damage, bool triggerDamageEvent);
    void WhenKilled();
}

public interface IResettable
{
    void ResetState();
}

public interface IActivable
{
    void Enable();
    void Disable();
}

public interface IInteractable
{
    void Interact();
}

public interface ICollectable
{
    void Collect();
}

public interface IUsable
{
    void Use(GameObject user);
}

public interface ICooldown
{
    bool IsOnCooldown { get; }
    float CooldownTime { get; }
    event Action OnCooldownStarted;
    event Action OnCooldownFinished;
}

public interface IMovementEvents
{
    event Action<Vector3> OnMovementUpdate;
    event Action OnMovementStopped;
}

public interface IMovableDirection
{
    void Move(Vector3 direction);
}

public interface IMovableTarget
{
    void MoveTo(Transform target);
    void MoveTo(Vector3 targetPosition);
}

public interface IShootable
{
    void OnShot();
}

public interface ITargetable
{
    bool IsTargetable { get; }
    Transform GetTargetPoint();
}

public interface ILockable
{
    bool IsLocked { get; }
    event Action OnLocked;
    event Action OnUnlocked;
    void Lock();
    void Unlock();
}

public interface ISpawner
{
    void Spawn(Vector3 position);
    void SetPrefab(GameObject prefab);
}
```

Tambem existe o contrato para objetos afetados pelo estado do jogo:

```csharp
public interface IAffectedByGameState
{
    void GameState_OnChangeGameStatus(GameStatus gameStatus);
}

public enum GameStatus
{
    NORMAL,
    BREAK,
    PAUSE
}
```

## Design patterns recomendados

### Observer

Use quando um objeto precisa notificar outros sobre mudancas sem forte acoplamento.

Uso comum:

- Eventos de gameplay.
- Vida, pontuacao, morte.
- UI.
- Sistemas de missoes.

Exemplo:

```csharp
public class Player : MonoBehaviour
{
    public static event Action OnPlayerDeath;

    private void Die()
    {
        OnPlayerDeath?.Invoke();
    }
}

public class UIManager : MonoBehaviour
{
    private void OnEnable()
    {
        Player.OnPlayerDeath += ShowGameOver;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= ShowGameOver;
    }

    private void ShowGameOver()
    {
    }
}
```

### State

Use quando um objeto muda comportamento conforme seu estado interno.

Uso comum:

- IA.
- Maquinas de estado.
- Fases do jogo.

### Strategy

Use quando existem familias de algoritmos intercambiaveis.

Uso comum:

- Diferentes movimentacoes.
- Ataques.
- IA modular.

### Command

Use para encapsular uma solicitacao como objeto.

Uso comum:

- Input.
- Undo/redo.
- IA com comandos enfileirados.

### Facade

Use para criar uma interface simples para um conjunto complexo de subsistemas.

Uso comum:

- Orquestrar sistemas como UI, audio, spawn, load de cena, score.
- Evitar que um botao, menu ou fluxo chame varios sistemas diretamente.

### Decorator

Use para adicionar funcionalidades dinamicamente sem alterar a classe base.

Uso comum:

- Buffs/debuffs.
- Power-ups.
- Habilidades em camadas.

### Mediator

Use para centralizar comunicacao entre objetos e reduzir dependencias diretas.

Uso comum:

- Sistemas de UI e gameplay onde varios componentes precisam interagir.
- HUD, pontuacao, morte do jogador, controle de fluxo.

### SOLID

Siga os principios SOLID como direcao de arquitetura:

- Single Responsibility: uma classe deve ter apenas uma razao para mudar.
- Open/Closed: aberto para extensao, fechado para modificacao.
- Liskov Substitution: subtipos devem poder substituir o tipo base.
- Interface Segregation: prefira interfaces pequenas e especificas.
- Dependency Inversion: dependa de abstracoes, nao de implementacoes concretas.

## Referencias entre classes

Quando um script precisar acessar outro, pense primeiro em uma dependencia abstrata.

Prefira:

- Interfaces.
- Classes abstratas.
- Eventos.
- Mediadores.
- Facades.

Evite:

- Buscar e chamar classes concretas sem necessidade.
- Fazer scripts conhecerem muitos detalhes uns dos outros.
- Transformar tudo em singleton por conveniencia.

Exemplo:

- Um projetil que causa dano deve procurar `IDamageable` ou `IHittable`, nao uma classe concreta como `EnemyHealth`.
- Um script de cartas deve acessar `CardBase`, nao uma classe concreta `Card`.

O objetivo e baixo acoplamento: scripts devem saber o minimo possivel uns sobre os outros.

## UnityEvent em botoes de HUD

Evite configurar botoes do HUD para chamar muitos metodos diferentes diretamente pelo `OnClick` no Inspector.

Problemas:

- Dependencias ocultas.
- Dificuldade de leitura.
- Referencias quebradas quando objetos mudam.
- Ordem de execucao dificil de entender.
- Botao vira orquestrador de regra de jogo.

Preferencias:

1. O botao chama um metodo simples no proprio script que conhece o contexto do botao.
2. O script implementa interfaces de UI como `IPointerDownHandler`, `IPointerUpHandler`, `IBeginDragHandler`, `IDragHandler`, `IEndDragHandler` quando necessario.
3. Acoes grandes devem ser encaminhadas para um gerenciador, facade ou mediator.

Excecoes aceitaveis:

- Ligar ou desligar telas.
- Trigar efeitos VFX/SFX.
- Prototipos e fases de teste.

## Random

Por padrao, use `UnityEngine.Random`. Para evitar conflito com `System.Random`, pode-se aliasar:

```csharp
using Random = UnityEngine.Random;
```

## Colliders

Quando nao houver motivo forte para especializar, use tipos genericos:

- `Collider` para 3D.
- `Collider2D` para 2D.

Evite amarrar scripts a `BoxCollider`, `SphereCollider`, `MeshCollider`, `CircleCollider2D`, `BoxCollider2D`, etc., quando o comportamento pode funcionar com qualquer collider.

## Namespaces

Cada sistema deve ter seu proprio namespace. Todo namespace deve comecar com `Scripts`.

Exemplos:

```csharp
namespace Scripts.Input
{
}

namespace Scripts.UI
{
}

namespace Scripts.Health
{
}

namespace Scripts.IA
{
}
```

Namespaces ajudam a evitar nomes duplicados e mantem o projeto organizado por dominio.

## Linha unica

Para `if`, `for` e `foreach` simples, e permitido usar linha unica sem braces quando isso melhora a leitura.

```csharp
if (booleana) DoSomething();

for (int i = 0; i < listSomething.Count; i++) listSomething[i] = i;

foreach (int number in listSomething) Debug.Log(number);
```

Use com cuidado. Se a condicao tiver efeitos colaterais, multiplas acoes ou risco de leitura ruim, prefira bloco com braces.

## Pattern JSON

Usamos JSON como formato data-driven para informacoes do jogo.

Padrao:

- Dados sao gerenciados via JSON.
- Para scripts que importam JSON, use `DataImporterBase<T>`.
- O importador deve ser `Singleton` ou `SceneSingleton` conforme o escopo.
- Os nomes das chaves no JSON devem bater exatamente com os campos da classe que recebe os dados.
- Alguns JSONs podem ser gerados a partir de arquivos de Excel.

Exemplo:

```csharp
public abstract class DataImporterBase<T> : SceneSingleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        LoadData();
    }

    protected abstract void LoadData();
}
```

Exemplo de importador:

```csharp
public class MN_Data : DataImporterBase<MN_Data>
{
    public Dictionary<int, Data> Dc_Datas { get; private set; } = new Dictionary<int, Data>();

    protected override void LoadData()
    {
        // Importa dados e preenche Dc_Datas.
    }
}
```

## Object Pool

Use Object Pool quando objetos sao criados e destruidos repetidamente.

Funcionamento:

1. Crie uma piscina de objetos pre-carregados.
2. Quando precisar de um objeto, pegue um inativo da piscina e ative.
3. Ao terminar o uso, desative e devolva para a piscina.

Use para:

- Balas.
- Explosoes.
- Particulas repetidas.
- Inimigos ou objetos temporarios criados com frequencia.

Vantagens:

- Evita custo de `Instantiate` e `Destroy`.
- Reduz travamentos.
- Melhora estabilidade de FPS.

## Update, FixedUpdate, LateUpdate e Coroutines

### Update

Use para:

- Logica geral que nao envolve fisica.
- Input do jogador.
- Movimento simples sem Rigidbody.
- Timers, cooldowns e animacoes nao controladas por fisica.

Nao use para:

- Interacao com `Rigidbody` como `AddForce`, `MovePosition` ou velocidade.

### FixedUpdate

Use para:

- Tudo relacionado a fisica.
- Movimentacao de `Rigidbody`.
- Forcas e velocidades fisicas.

Nao use para:

- Input direto do jogador, pois ele pode ser perdido entre chamadas.
- Logica visual ou nao fisica.

### LateUpdate

Use para:

- Movimento de camera seguindo alvo.
- Ajustes de animacao apos update normal.
- Logicas de inverse kinematics ou follow-up visual.

Nao use para:

- Mover o proprio alvo que a camera segue.

### Coroutines

Use para:

- Sequencias temporizadas.
- Esperas.
- Acoes periodicas.
- Interpolacoes.

Evite criar `new WaitForSeconds(...)` repetidamente dentro de loops. Cache quando o tempo for fixo.

```csharp
private WaitForSeconds _wait;

private void Awake()
{
    _wait = new WaitForSeconds(0.5f);
}

private IEnumerator FireRoutine()
{
    while (_isFiring)
    {
        FireBullet();
        yield return _wait;
    }
}
```

## Structs x Classes

Use `struct` quando:

- Representa um valor unico.
- E pequeno em memoria, idealmente ate 16 bytes.
- Deve ser imutavel.
- Nao precisa de heranca.

Exemplos:

- `Vector3`
- `Quaternion`
- `Color`

Use `class` quando:

- O objeto tem identidade e ciclo de vida proprio.
- Varias partes do codigo precisam referenciar e modificar o mesmo objeto.
- Contem referencias para outros objetos.
- E grande ou complexo.
- Precisa de heranca.
- Herda de `MonoBehaviour`.

Todo script que herda de `MonoBehaviour` e uma classe.

## Async/Await

Use `async/await` para operacoes longas que nao devem bloquear a thread principal.

Use para:

- Operacoes de rede.
- Login, download, ranking.
- Leitura e escrita de arquivos.
- Processamento pesado.
- Integracao com Addressables.

Exemplo de uso:

```csharp
private async void OnStartButtonPressed()
{
    _startButton.interactable = false;
    string resultData = await FetchDataFromServerAsync();
    StartCoroutine(StartCountdownRoutine());
}
```

## Cuidados com DontDestroyOnLoad

Use `DontDestroyOnLoad` apenas para gerenciadores globais.

Evite marcar scripts desnecessarios como persistentes. Objetos persistentes demais:

- Geram bugs ao trocar de cena.
- Sao mais dificeis de rastrear.
- Podem carregar referencias antigas.
- Aumentam acoplamento escondido.

## Cuidados com null

`null` pode ser ambiguo. Ao lidar com referencias importantes, prefira falhar rapido em desenvolvimento.

Recomendacoes:

- Valide dependencias obrigatorias no `Awake`.
- Use mensagens claras de erro.
- Use `TryGetComponent` para componentes opcionais.

Exemplo:

```csharp
[SerializeField] private GameManager _gameManager;

private void Awake()
{
    if (_gameManager == null)
    {
        Debug.LogError("GameManager nao foi atribuido no Inspector.", this);
        return;
    }
}
```

Para componentes opcionais:

```csharp
if (TryGetComponent<Armor>(out var armor))
{
    armor.ApplyBuff();
}
```

## Tags e Layers

Tags respondem a pergunta: "O que este objeto e?"

Use tags para:

- Identificacao rapida em runtime.
- Objetos singulares ou papeis centrais, como `Player`, `MainCamera`, `GameController`.
- Categorias de gameplay, como `Enemy`, `Collectible`, `Destructible`.
- Verificacoes simples com `CompareTag`.

Nao use tags para:

- Habilidades ou estados, como `CanFly` ou `IsPoisonous`.
- Filtrar fisica ou renderizacao.
- Criar dezenas de categorias como `EnemyMelee`, `EnemyRanged`, `EnemyBoss`.

Layers respondem a pergunta: "Com o que este objeto interage?"

Use layers para:

- Fisica e matriz de colisao.
- Raycasts seletivos.
- Renderizacao seletiva por camera.

Exemplos:

- Tiros do player colidem com `Enemy`, mas nao com `Player`.
- Raycast do personagem procura apenas `Ground`.
- Camera de UI renderiza apenas layer `UI`.

## Performance

### Dicas gerais

- Evite alterar escala diretamente no objeto 3D da cena. Se precisar mudar tamanho, altere `Scale Factor` no importador do modelo.
- Crie prefab do modelo 3D em vez de usar o modelo diretamente na cena.
- Ajuste graficos conforme a plataforma: anti-aliasing, render scale, sombras e far clip plane.
- Marque objetos estaticos como `Static` quando fizer sentido.
- Tire `Raycast Target` de imagens e textos de UI que nao precisam receber clique.
- Remova `Pixel Perfect` de Canvas quando nao for necessario.
- Desative efeitos visuais desnecessarios como Reflection Probes e Specular Highlights em objetos que nao precisam.
- Use shaders otimizados, como Unlit para objetos sem iluminacao.

### Static Batching, Dynamic Batching e GPU Instancing

Resumo:

- Static Batching: objetos estaticos, poucos materiais, exige marcar como Static. Nao combina com GPU Instancing.
- Dynamic Batching: objetos pequenos e moveis. Funciona melhor com menos de 300 vertices em shaders simples sem iluminacao.
- GPU Instancing: muitos objetos iguais e moveis, mesmo material e shader com suporte. Incompativel com Static Batching.

Regras para GPU Instancing:

- Use quando varias instancias compartilham a mesma malha e material.
- Nao use junto com Static Batching.
- Evite em objetos com menos de 256 vertices sem medir antes.
- Ative `Enable GPU Instancing` no material.
- Para variar propriedades entre instancias sem quebrar instancing, use `MaterialPropertyBlock`.

```csharp
var block = new MaterialPropertyBlock();
block.SetColor("_Color", Color.red);
renderer.SetPropertyBlock(block);
```

### Buscas e alocacoes

- Buscar por tag costuma ser mais rapido que buscar por tipo.
- Evite `Find`, `FindWithTag` e `FindObjectOfType` dentro de `Update`.
- Cacheie `Camera.main`, pois o acesso direto e lento internamente.
- Use `NonAlloc` quando precisar controlar alocacoes em loops intensos, por exemplo `Physics.RaycastNonAlloc`.
- Em listas e arrays, use `for` quando precisar de indice.
- Use `foreach` em colecoes como `Dictionary` e `HashSet`, que nao sao indexadas.
- Evite `foreach` em arrays dentro de `Update` em plataformas limitadas.
- Ao calcular com vetores, prefira calcular escalares antes e multiplicar pelo vetor no final.
- Prefira `GetComponent<T>()`.
- Evite `GetComponent(string)`.
- Para tipos dinamicos, use `GetComponent(typeof(T))`.
- Evite usar string como identificador de sistemas de jogo. Prefira `int`, `enum` ou `Guid`.
- Para propriedades de shader, use `Shader.PropertyToID` uma vez e guarde o inteiro.
- Use `CompareTag("TagName")` em vez de `obj.tag == "TagName"`.

## Audio

### Modos de carregamento

`Decompress On Load`:

- Descompacta o audio no carregamento e guarda em RAM.
- Toca imediatamente.
- Ideal para sons curtos e frequentes, como UI, tiros, passos e interacoes rapidas.
- Consome mais RAM.
- Evite em musicas, falas longas e muitos audios simultaneos.

`Compressed In Memory`:

- Mantem o audio comprimido em RAM e descomprime durante a reproducao.
- Economiza RAM.
- Tem pequeno custo de CPU.
- Bom para efeitos medios ou pouco frequentes.
- Pode causar stutter em audios longos.

`Streaming`:

- Le o audio direto do disco conforme toca.
- Usa pouca RAM.
- Ideal para musicas, narracoes, dialogos longos e cutscenes.
- Pode depender da velocidade do armazenamento.
- Nao use para sons curtos e frequentes.

### Formatos

`PCM`:

- Sem compressao e sem perda.
- Qualidade maxima.
- Tamanho grande.
- Ideal para sons curtos que precisam de resposta imediata.
- Evite para musicas e falas longas.

`ADPCM`:

- Compressao leve com perdas pequenas.
- Menor que PCM.
- Decodifica mais rapido que Vorbis.
- Suporta apenas mono e stereo.
- Bom para efeitos curtos e medios.
- Evite para musicas, falas longas ou alta fidelidade.

`Vorbis`:

- Compressao com perdas, semelhante a MP3.
- Formato padrao para `.ogg`.
- Unico formato compatavel com Streaming no Unity.
- Bom para musicas, dialogos e narracoes.
- Evite para sons curtos e repetidos, especialmente com `Compressed In Memory`.

### Recomendacoes praticas de audio

- Som de botao: `.wav` com PCM ou ADPCM, modo `Decompress On Load`.
- Explosao: `.wav` ADPCM, modo `Compressed In Memory`.
- Som ambiente curto: `.wav` ADPCM, modo `Decompress On Load`.
- Musica de fundo: `.ogg` Vorbis, modo `Streaming`.
- Fala de personagem: `.ogg` Vorbis, modo `Decompress On Load` ou `Streaming`, conforme tamanho e frequencia.

## Addressables

Addressables gerenciam e carregam assets por endereco unico. O codigo nao precisa saber se o asset esta no build inicial, em um servidor, em DLC ou em patch.

Vantagens:

- Gerenciamento automatico de download e cache.
- Resolucao de dependencias entre assets.
- Carregamento assincrono.
- Melhor controle de memoria.
- Suporte a conteudo remoto e atualizacoes.

Uso basico:

1. Abra `Window > Asset Management > Addressables > Groups`.
2. Crie as configuracoes quando o Unity pedir.
3. Marque assets ou objetos como Addressable.
4. Renomeie o endereco para algo legivel.
5. Carregue via `AssetReference`, nao por string.

Evite:

- Carregar Addressables por string fixa.
- Marcar scripts como Addressable.
- Referenciar prefab Addressable diretamente no Inspector sem `AssetReference`, pois isso pode carregar o asset cedo demais.

Prefira:

```csharp
[SerializeField] private AssetReference assetReference;
```

Quando houver tipo especifico:

```csharp
[SerializeField] private AssetReferenceGameObject assetReferenceGameObject;
[SerializeField] private AssetLabelReference assetLabelReference;
```

Para tipos sem `AssetReference` pronto, crie um tipo proprio:

```csharp
[System.Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid) : base(guid)
    {
    }
}
```

### Duplicacao de assets

O problema:

- Se dois grupos de Addressables dependem do mesmo asset compartilhado, o asset pode ser duplicado nos bundles.
- Isso aumenta tamanho de build/download.
- Em ScriptableObjects, duplicacao pode gerar bugs silenciosos quando duas copias divergem.

Solucao:

- Centralize assets compartilhados em um grupo compartilhado, como `Default Local Group` ou grupo estatico equivalente.
- Deixe Addressables gerenciar dependencias.
- Se qualquer asset Addressable referencia outro asset, esse asset referenciado tambem deve ser Addressable ou estar em grupo compartilhado.

Estrategia de conteudo:

- Built-in: minimo necessario para a cena inicial.
- Static Content: assets que nao mudam depois do release.
- Mutable Content: assets que podem ser atualizados, como balanceamento, textos, localizacao e configuracoes.

Workflow recomendado:

1. Limpe cache.
2. Construa Addressables.
3. Construa o jogo.
4. Para atualizacao, use `Check for Content Update Restrictions`.
5. Gere novo catalogo e bundles de atualizacao.
6. Disponibilize os arquivos para o servidor quando houver conteudo remoto.

O catalogo de Addressables funciona como um mapa: informa onde encontrar o conteudo de cada grupo. Ao carregar um asset, o sistema verifica catalogos carregados e pode atualizar o conteudo automaticamente na inicializacao.

## Scripts reutilizaveis documentados

A documentacao inclui exemplos de scripts reutilizaveis. Eles devem ser tratados como referencia de solucao e nao como regra para copiar cegamente.

Exemplos identificados:

- Sistema de idioma/localizacao.
- Ajuste de tamanho de texto TMP.
- Shader e scripts para daltonismo em URP.
- Manipulacao de input de eixo no Cinemachine.

Ao reutilizar esses scripts:

- Preserve namespaces comecando por `Scripts`.
- Ajuste nomes para seguir os padroes deste guia.
- Verifique dependencias como `TMP`, `Addressables`, `Sirenix/Odin`, `Cinemachine` e URP.
- Extraia interfaces ou classes base quando o script precisar ser usado por varios sistemas.

## Checklist antes de criar ou alterar codigo

Use esta lista antes de implementar:

- O nome do script, variaveis, metodos e eventos segue o padrao?
- A classe tem uma responsabilidade clara?
- O script pertence a `_System`, a um prefab, ao player ou a um objeto local especifico?
- Se for gerenciador, ele realmente precisa ser gerenciador?
- Se for singleton, ele realmente precisa persistir entre cenas?
- Alguma referencia concreta poderia virar interface, classe abstrata ou evento?
- O script evita `Find`/buscas pesadas em `Update`?
- Componentes obrigatorios sao validados?
- Componentes opcionais usam `TryGetComponent`?
- Botoes de HUD chamam um ponto de entrada simples, e nao uma lista grande de metodos?
- Assets compartilhados por Addressables estao em grupo compartilhado?
- Audio esta com formato e modo adequados ao uso?
- O codigo evita `partial` salvo caso especial?

## Como uma IA deve usar este guia

Antes de modificar o projeto, uma IA deve:

1. Ler este guia e tratar seus padroes como contexto de arquitetura.
2. Inspecionar o codigo existente para identificar como esses padroes ja aparecem no projeto real.
3. Preservar o estilo local quando ele nao contradizer este guia.
4. Evitar refatoracoes amplas que nao foram pedidas.
5. Preferir pequenas alteracoes consistentes a novas arquiteturas inventadas.
6. Ao criar scripts Unity, seguir namespace `Scripts.*`, nomes em ingles, responsabilidades pequenas e dependencias por abstracao quando possivel.
7. Ao lidar com cena/hierarquia, respeitar `_System`, `_Global`, `_Canvas`, `_Audio`, `_Feel`, containers e prefixos de gerenciadores.

Este documento nao substitui julgamento tecnico. Ele define o idioma arquitetural do projeto para que humanos e IAs trabalhem com a mesma base.
