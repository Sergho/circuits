# Контекст проекта для генераторов разбиения графа

Этот документ описывает контекст проекта, необходимый ИИ‑агенту для написания
реализации любого алгоритма генерации разбиения графа на части. В паре с файлом
`algo.md` конкретного алгоритма этого должно быть достаточно, чтобы получить
реализацию, близкую к существующей в проекте.

## О проекте

Проект — это .NET/C# приложение (`circuits.core`), решающее задачу разбиения графа
на части (в основном на 2) в области микроэлектроники. Граф моделирует схему,
а разбиение — её физическое деление; критерий оптимальности — минимизация числа
**пересекающих границу разбиения рёбер** (cross edges). Постановка задачи:

```
G = (V, E)
g(G) = (I, J), I ⊆ V, J ⊆ V, I ∩ J = ∅
C(G, g) = {e = (u, v) ∈ E : u ∈ I, v ∈ J  или  u ∈ J, v ∈ I}
q(G, g) = |C(G, g)| → min
```

Чтобы исследовать качество и скорость алгоритмов разбиения, нужны реализации
разных алгоритмов (например, Кернигана–Лина), реализующих единый интерфейс.

## Технологический стек

- Язык: C# (современный, без явного `namespace`, типы — в глобальном неймспейсе).
- Платформа: .NET (см. `circuits.core/circuits.core.csproj`).
- Используются современные конструкции C#: коллекционные литералы `[]`,
  `out` параметры почти не применяются.
- Без внешних зависимостей: только стандартная библиотека.

## Доменные типы

Используются те же типы, что и в контексте для генераторов графа
(`../GraphGenerator/context.md`: вершины, рёбра, граф), плюс типы, специфичные
для разбиения.

### `IVertex` / `Vertex`

Целочисленный индекс с 1. Равенство и хеш — по `Index`.

### `IEdge` / `Edge`

Неупорядоченная пара вершин. Равенство и хеш считаются по «нормализованной»
форме (меньший индекс — первая вершина), поэтому `(2, 5)` равно `(5, 2)`.
Это важно: `Edge` можно использовать в качестве ключа словаря для пары
вершин — порядок не имеет значения.

### `IGraph` / `Graph`

Неориентированный граф. Полезные для разбиения методы:

```csharp
public interface IGraph : IGraphLoggable
{
    IEnumerable<IVertex> Vertices { get; }
    bool HasVertex(IVertex vertex);
    bool HasEdge(IEdge edge);
    void AddEdge(IEdge edge);
    IEnumerable<IVertex> GetAdjacencyList(IVertex vertex);
}
```

И свойство `VerticesCount` у конкретного класса `Graph`. Все вершины графа
имеют индексы от `1` до `VerticesCount`.

### `IGraphPart` / `GraphPart`

Одна часть разбиения. Хранит подмножество вершин родительского графа.

```csharp
public interface IGraphPart : IGraphLoggable
{
    IEnumerable<IVertex> Vertices { get; }
    bool HasVertex(IVertex vertex);
    void AssignVertex(IVertex vertex);
    void RemoveVertex(IVertex vertex);
}
```

### `IGraphPartition` / `GraphPartition`

Само разбиение графа на части. Создаётся как `new GraphPartition(graph, partsCount)`,
после чего вершины **уже автоматически распределены** по частям примерно поровну
(остаток — в первые части): первые `ceil(n/k)` вершин — в часть 0, следующие —
в часть 1, и так далее.

```csharp
public interface IGraphPartition
{
    IGraph Graph { get; }
    int PartsCount { get; }
    IEnumerable<IGraphPart> Parts { get; }
    int CrossEdgesCount { get; }                  // количество межчастьевых рёбер

    void SwapVertices(IVertex first, IVertex second); // переносит вершины между частями
    int CalculateGain(IVertex first, IVertex second); // прирост качества при свопе пары
    IGraphPart GetPart(IVertex vertex);
}
```

Особенности:

- `SwapVertices(a, b)` корректно работает только если `a` и `b` находятся в
  **разных частях** (в обратном случае результат бессмысленный — он «удалит»
  обе вершины из одной части и «вернёт» их же).
- `CalculateGain(a, b)` возвращает прирост `oldCut - newCut` при свопе двух
  вершин из разных частей. Если `a` и `b` смежны, в формулу вносится поправка
  на 2 (рёбро между ними не уходит за границу, а остаётся внутренним после
  свопа, поэтому учитывается двойственно).
- `CrossEdgesCount` — текущее количество рёбер, концы которых лежат в разных
  частях.

## Контракт генератора разбиения

```csharp
public interface IGraphPartitionGenerator
{
    int LastIterationsCount { get; }
    IGraphPartition Generate(IGraph graph);
}
```

- `Generate(IGraph graph)` строит и возвращает `IGraphPartition`.
- `LastIterationsCount` — число «полезных» итераций основного цикла последнего
  вызова `Generate`. Семантика «итерации» зависит от алгоритма (для
  Кернигана–Лина — число выполненных свопов).

## Соглашения по коду

1. **Класс реализует `IGraphPartitionGenerator`** напрямую.
2. **Параметры в конструкторе.** Количество частей `partsCount` обычно
   передаётся в конструктор.
3. **Поля приватные**, могут быть `readonly`. `Random` создаётся в конструкторе
   через `new()` без seed.
4. **`LastIterationsCount`** — публичное свойство с приватным сеттером,
   инициализируется в конструкторе нулём, обновляется во время `Generate`.
5. **Метод `Generate(IGraph graph)`** строит `GraphPartition`, инициализирует
   внутренние структуры (например, кэш приростов), запускает основной цикл и
   возвращает разбиение.
6. **Никакого логирования, никаких `Console.WriteLine`** внутри генератора.
7. **Без внешних зависимостей**: только `System.*`.

## Пример скелета

```csharp
public class SomePartitionGenerator : IGraphPartitionGenerator
{
    private readonly int partsCount;
    private readonly Random random;

    public int LastIterationsCount { get; private set; }

    public SomePartitionGenerator(int partsCount)
    {
        this.partsCount = partsCount;
        random = new();
        LastIterationsCount = 0;
    }

    public IGraphPartition Generate(IGraph graph)
    {
        var partition = new GraphPartition(graph, partsCount);
        // ... начальная инициализация ...
        LastIterationsCount = 0;
        while (/* ещё есть полезные шаги */)
        {
            // ... выбрать лучшую операцию, применить её, увеличить счётчик ...
            LastIterationsCount++;
        }
        return partition;
    }
}
```

## Размещение файла

Файл реализации должен быть размещён в директории
`circuits.core/GraphPartitionGenerator/<Имя>GraphPartitionGenerator.cs`. Класс
называется по схеме `<Имя>GraphPartitionGenerator`.

## Связанные сущности

- `GraphPartitionStats` оборачивает генератор и считает статистику
  (время генерации, число итераций, число пересекающих рёбер). Менять его
  при реализации нового генератора не требуется — достаточно соответствовать
  интерфейсу.
- `FileGraphLogger` умеет логировать `IGraphLoggable` (граф или часть) в файл;
  внутри генератора он не используется.
