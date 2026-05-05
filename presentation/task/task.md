# Задача разбиения графа

В данной работе рассматривается решение задачи разбиения графа на две части методом Кернигана-Лина

## Постановка задачи

```math
G=(V,E)
\newline
g\text{ - генератор}
\newline
g(G)=(I,J)\text{, где}
\newline
I\subseteq{V},J\subseteq{V}
\newline
\text{При этом:}
\newline
I\cap{J}=\varnothing
\newline
\text{Тогда:}
\newline
C(G,g)=\{e=(u,v)\in{E}|u\in{I},v\in{J}\text{ или }u\in{J},v\in{I}\}
\newline
q(G,g)=|C(G,g)|\text{ - критерий}
\newline
\text{Задача:}
\newline
q(G,g)\rightarrow\min
```