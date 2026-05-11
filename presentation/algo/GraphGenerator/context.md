# Контекст проекта для генераторов графа

Этот документ описывает контекст проекта, необходимый ИИ-агенту для написания реализации
любого алгоритма генерации графа в рамках данного проекта. В паре с файлом `algo.md`
конкретного алгоритма этого должно быть достаточно, чтобы получить реализацию, близкую
к существующей в проекте.

## О проекте

Проект — это .NET/C# приложение (`circuits.core`), предназначенное для решения задачи
разбиения графа на части (как правило, на две) в области микроэлектроники. Граф моделирует
схему, а разбиение — её физическое деление (например, на кристаллы). Критерий
оптимальности — минимизация числа рёбер, пересекающих границу между частями.

Для исследований алгоритмов разбиения нужны графы разной формы и плотности, поэтому в
проекте реализован набор генераторов графов, каждый из которых строит граф особого
вида (полный, цепной, двудольный, случайный по Эрдёшу–Реньи, случайный с фиксированным
числом рёбер по Фишеру–Йейтсу, регулярная решётка и т. д.).

## Технологический стек

- Язык: C# (современный, файлово‑скоупленные пространства имён не требуются — типы в
  проекте объявлены в глобальном неймспейсе).
- Платформа: .NET (см. `circuits.core/circuits.core.csproj`).
- Используются современные синтаксические конструкции C#: коллекционные литералы
  (`[]`), property‑getters со стрелкой, `out` параметры почти не используются.
- Без внешних зависимостей: только стандартная библиотека.

## Доменные типы

Ниже перечислены типы, которые ИИ‑агент должен использовать при реализации генератора.
Они уже существуют в проекте, создавать их повторно не нужно.

### `IVertex` / `Vertex`

Вершина графа идентифицируется целочисленным индексом, начинающимся с **1**.

```csharp
public interface IVertex : IComparable<IVertex>, IEquatable<IVertex>
{
    int Index { get; }
}

public class Vertex : IVertex
{
    public int Index { get; }
    public Vertex(int index) { /* index >= 1 */ }
    // Equals/GetHashCode по Index
}
```

### `IEdge` / `Edge`

Ребро — неупорядоченная пара вершин. Равенство и хеш считаются по «нормализованной»
форме (вершина с меньшим индексом — первая), поэтому ребро `(2, 5)` равно `(5, 2)`.

```csharp
public interface IEdge : IEquatable<IEdge>
{
    IVertex First { get; }
    IVertex Second { get; }
    IEdge GetNormalized();
}

public class Edge : IEdge
{
    public Edge(IVertex first, IVertex second) { /* ... */ }
}
```

### `IGraph` / `Graph`

Неориентированный граф с фиксированным набором вершин (создаются в конструкторе) и
динамически добавляемыми рёбрами. Граф «пустой» — это граф без рёбер, но **со всеми
вершинами** от `1` до `verticesCount` включительно.

```csharp
public interface IGraph : IGraphLoggable
{
    IEnumerable<IVertex> Vertices { get; }
    bool HasVertex(IVertex vertex);
    bool HasEdge(IEdge edge);
    void AddEdge(IEdge edge);
    IEnumerable<IVertex> GetAdjacencyList(IVertex vertex);
}

public class Graph : IGraph
{
    public static Graph Empty(int verticesCount); // фабричный метод
    // Конструктор приватный; вершины создаются автоматически
    // AddEdge игнорирует дубли и рёбра с несуществующими вершинами
}
```

Важно: вершины внутри `Graph.Empty(n)` уже созданы и пронумерованы от `1` до `n`.
Генератору остаётся только построить нужный набор рёбер и вызывать `graph.AddEdge(...)`.

## Контракт генератора

Все генераторы реализуют единый интерфейс:

```csharp
public interface IGraphGenerator
{
    IGraph Generate();
}
```

Параметры генератора передаются в **конструктор**. В конструкторе обязательна
валидация параметров: если параметр некорректен — выбрасывается `ArgumentException`
с понятным сообщением на русском языке. Параметры сохраняются в приватные поля.

## Соглашения по коду (важно для близости к существующему стилю)

Эти соглашения соблюдаются во всех существующих генераторах. Им нужно следовать.

1. **Класс реализует `IGraphGenerator`** напрямую, без промежуточного базового класса.
2. **Поля приватные**, без `readonly`, инициализируются в конструкторе. Если нужен
   `Random`, он создаётся в конструкторе вызовом `new()` без seed.
3. **Валидация параметров** вынесена в отдельный приватный метод
   `GetParamsError(...)`, возвращающий `Exception?`. Конструктор вызывает его и
   бросает исключение, если оно не `null`:
   ```csharp
   var paramsError = GetParamsError(...);
   if (paramsError != null)
       throw paramsError;
   ```
   Сообщения исключений — на русском языке.
4. **Метод `Generate()`** имеет шаблонный вид:
   ```csharp
   public IGraph Generate()
   {
       IGraph graph = GetEmptyGraph();
       FillGraph(graph);
       return graph;
   }
   ```
5. **`GetEmptyGraph()`** возвращает `Graph.Empty(totalVerticesCount)`.
6. **`FillGraph(IGraph graph)`** добавляет рёбра в граф. Часто внутри он
   делегирует генерацию списков рёбер другим приватным методам (например,
   `GenerateAllPossibleEdges()`, `GenerateRandomEdges()`).
7. **Вершины создаются как `new Vertex(i)`** там, где это нужно, на лету.
   Не нужно искать существующие экземпляры в `graph.Vertices` — равенство
   вершин определено по `Index`, и `Graph.AddEdge` корректно отработает.
8. **Рёбра — `new Edge(firstVertex, secondVertex)`**. Порядок концов не важен:
   равенство и хеш считаются по нормализованной форме.
9. **Нумерация вершин — с 1**, а не с 0. Это критично: `Vertex` бросает исключение,
   если `Index < 1`.
10. **Никакого логирования, никаких `Console.WriteLine`** внутри генератора.
11. **Без внешних зависимостей**: только `System.*`.

## Пример скелета (для общего понимания стиля)

```csharp
public class SomeGraphGenerator : IGraphGenerator
{
    private int verticesCount;
    private int someParam;
    private Random random;

    public SomeGraphGenerator(int verticesCount, int someParam)
    {
        var paramsError = GetParamsError(verticesCount, someParam);
        if (paramsError != null)
            throw paramsError;

        this.verticesCount = verticesCount;
        this.someParam = someParam;
        random = new();
    }

    private Exception? GetParamsError(int verticesCount, int someParam)
    {
        if (verticesCount < 0)
            return new ArgumentException("Количество вершин в графе не может быть отрицательным");
        // ... другие проверки
        return null;
    }

    public IGraph Generate()
    {
        IGraph graph = GetEmptyGraph();
        FillGraph(graph);
        return graph;
    }

    private IGraph GetEmptyGraph()
    {
        return Graph.Empty(verticesCount);
    }

    private void FillGraph(IGraph graph)
    {
        // ... основная логика конкретного алгоритма ...
    }
}
```

## Размещение файла

Файл реализации должен быть размещён в директории
`circuits.core/GraphGenerator/<Имя>GraphGenerator.cs`. Класс называется по
схеме `<Имя>GraphGenerator`.
