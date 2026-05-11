# Промпт

Реализуй в проекте `circuits.core` (директория `circuits.core/GraphGenerator/`)
класс `CliqueGraphGenerator`, реализующий интерфейс `IGraphGenerator`.

Генератор строит полный неориентированный граф `K_n`: для каждой пары вершин
`(i, j)` c `1 ≤ i < j ≤ verticesCount` добавляется ребро.

Параметры конструктора:

- `int verticesCount` — количество вершин, ≥ 0.

Если `verticesCount < 0`, выбрасывай `ArgumentException` с понятным сообщением
на русском языке.

Используй соглашения по коду и доменные типы из `../context.md`.
