# Промпт

Реализуй в проекте `circuits.core` (директория
`circuits.core/GraphPartitionGenerator/`) класс
`KernighanLinGraphPartitionGenerator`, реализующий интерфейс
`IGraphPartitionGenerator`. Это упрощённый «жадный» вариант алгоритма
Кернигана–Лина для разбиения графа на `k` частей с минимизацией числа
пересекающих границу рёбер.

Параметры конструктора:

- `int partsCount` — количество частей разбиения (обычно 2).

Публичное свойство `LastIterationsCount` (с приватным сеттером) хранит число
выполненных «полезных» свопов в последнем вызове `Generate`.

Алгоритм:

1. Создать `partition = new GraphPartition(graph, partsCount)`.
2. Заполнить кэш `Dictionary<Edge, int> cachedGains` приростами для **всех**
   пар вершин из разных частей: ключ — `new Edge(u, v)`, значение —
   `partition.CalculateGain(u, v)`.
3. Случайно «перемешать» начальное разбиение: для `i` от `1` до
   `verticesCount - 1` выбрать случайный `j ∈ [i + 1, verticesCount)`; если
   вершины `i` и `j` уже в одной части — пропустить; иначе вызвать общий
   `SwapVertices(partition, i, j)` (см. ниже). Эти свопы **не учитываются**
   в `LastIterationsCount`.
4. Сбросить `LastIterationsCount = 0`. В цикле, пока в `cachedGains`
   существует пара с положительным приростом:
   - Найти пару с максимальным `gain > 0`.
   - Вызвать `SwapVertices(partition, u, v)`.
   - `LastIterationsCount++`.
5. Вернуть `partition`.

`SwapVertices(partition, u, v)`:

1. `partition.SwapVertices(u, v)`.
2. Для каждой вершины `w` графа, отличной от `u`, формируется ключ
   `new Edge(u, w)`. Если ключ уже в кэше — удалить, иначе добавить с
   значением `partition.CalculateGain(u, w)`.
3. То же самое для `v`.

Для случайных чисел используй `Random`, инициализированный в конструкторе через
`new()` без seed. Используй `Dictionary<Edge, int>`; равенство `Edge` уже
корректно учитывает порядок концов.

Используй соглашения по коду и доменные типы из `../context.md`.
