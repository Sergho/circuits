# Промпт

Реализуй в проекте `circuits.core` (директория `circuits.core/GraphGenerator/`)
класс `BipartiteGraphGenerator`, реализующий интерфейс `IGraphGenerator`.

Генератор строит неориентированный двудольный граф с заданными размерами левой и
правой долей и заданным числом рёбер. Рёбра идут только между долями.

Параметры конструктора:

- `int leftPartitionSize` — количество вершин в левой доле, ≥ 0.
- `int rightPartitionSize` — количество вершин в правой доле, ≥ 0.
- `int edgesCount` — количество рёбер, от 0 до `leftPartitionSize * rightPartitionSize`
  включительно.

Если параметры некорректны — выбрасывай `ArgumentException` с понятным сообщением
на русском языке.

Вершины с индексами `1..leftPartitionSize` относятся к левой доле, индексы
`leftPartitionSize+1 .. leftPartitionSize+rightPartitionSize` — к правой.

Алгоритм выбора рёбер: построй полный список рёбер `(i, j)` с `1 ≤ i ≤ leftPartitionSize`
и `leftPartitionSize+1 ≤ j ≤ leftPartitionSize+rightPartitionSize`, затем выбери
из него ровно `edgesCount` рёбер по схеме Фишера–Йейтса (без повторений).

Используй соглашения по коду и доменные типы из `../context.md`.
