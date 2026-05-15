# -*- coding: utf-8 -*-
import sys, copy
sys.stdout.reconfigure(encoding='utf-8')

from pptx import Presentation
from pptx.util import Inches, Pt
from pptx.dml.color import RGBColor
from pptx.oxml.ns import qn

SRC  = r'C:\Users\kirill2\Documents\circuits2\circuits\scrennshots\presentation (2).pptx'
DST  = r'C:\Users\kirill2\Documents\circuits2\circuits\scrennshots\presentation_updated.pptx'

prs = Presentation(SRC)

# ── helpers ──────────────────────────────────────────────────────────────────

def replace_text(shape, new_text, font_size=11):
    """Заменяет весь текст в текстбоксе, сохраняя обёртку слов."""
    tf = shape.text_frame
    tf.word_wrap = True
    # очищаем все абзацы кроме первого
    while len(tf.paragraphs) > 1:
        tf.paragraphs[-1]._p.getparent().remove(tf.paragraphs[-1]._p)
    para = tf.paragraphs[0]
    # убираем лишние run-ы
    for r in para.runs[1:]:
        r._r.getparent().remove(r._r)
    if para.runs:
        para.runs[0].text = new_text
        para.runs[0].font.size = Pt(font_size)
    else:
        run = para.add_run()
        run.text = new_text
        run.font.size = Pt(font_size)

def add_textbox(slide, left_in, top_in, w_in, h_in, text, font_size=11, bold=False, color=None):
    """Добавляет новый текстбокс с текстом."""
    tb = slide.shapes.add_textbox(Inches(left_in), Inches(top_in), Inches(w_in), Inches(h_in))
    tf = tb.text_frame
    tf.word_wrap = True
    para = tf.paragraphs[0]
    run = para.add_run()
    run.text = text
    run.font.size = Pt(font_size)
    run.font.bold = bold
    if color:
        run.font.color.rgb = RGBColor(*color)
    return tb

def add_row_to_table(shape, row_texts):
    """Добавляет строку в таблицу PowerPoint через deepcopy последней строки."""
    tbl_elem = shape.table._tbl
    all_tr   = tbl_elem.findall(qn('a:tr'))
    new_tr   = copy.deepcopy(all_tr[-1])   # копируем форматирование последней строки

    cells = new_tr.findall(qn('a:tc'))
    for cell, text in zip(cells, row_texts):
        # убираем все a:r кроме первого, первому ставим нужный текст
        all_r = cell.findall('.//' + qn('a:r'))
        for j, r in enumerate(all_r):
            t_el = r.find(qn('a:t'))
            if j == 0:
                if t_el is None:
                    from lxml import etree
                    t_el = etree.SubElement(r, qn('a:t'))
                t_el.text = text
            else:
                r.getparent().remove(r)
        # если вообще не было run-ов — создаём
        if not all_r:
            from lxml import etree
            p_el = cell.find('.//' + qn('a:p'))
            if p_el is None:
                p_el = etree.SubElement(cell, qn('a:p'))
            r_el = etree.SubElement(p_el, qn('a:r'))
            t_el = etree.SubElement(r_el, qn('a:t'))
            t_el.text = text

    tbl_elem.append(new_tr)

# ════════════════════════════════════════════════════════════════════════════
# СЛАЙД 25 — Качество разбиения (ER p=0.15): дополнить вывод
# ════════════════════════════════════════════════════════════════════════════
slide25 = prs.slides[24]
for shape in slide25.shapes:
    if shape.has_text_frame and 'Вывод:' in shape.text_frame.text and 'случайных' in shape.text_frame.text:
        replace_text(shape,
            'Вывод: FM превосходит KL по качеству разбиения на 1–12,5% на случайных графах '
            '(ER, p=0,15). Наибольшая разница при малых n: +12,5% при n=25, +7,5% при n=100; '
            'при n=600 разница сглаживается до 1,2%. Критическое преимущество — скорость: '
            'KL при n=600 работает 8 914 мс, FM — 230 мс (в 39 раз быстрее).',
            font_size=11)
        print('Слайд 25: вывод обновлён')
        break

# ════════════════════════════════════════════════════════════════════════════
# СЛАЙД 26 — Ранжирование генераторов: добавить строку «Цепочка» + вывод
# ════════════════════════════════════════════════════════════════════════════
slide26 = prs.slides[25]
table_shape26 = None
for shape in slide26.shapes:
    if shape.shape_type == 19:      # TABLE
        table_shape26 = shape
        break

if table_shape26:
    # Добавляем строку «Цепочка» — данные из замера n=200
    add_row_to_table(table_shape26, ['Цепочка', '28,7', '7,3', '+74,4%'])
    print('Слайд 26: строка Цепочка добавлена')

# Добавляем текстбокс с выводом под таблицей
# Таблица: top=2.59, height≈2.32 → после добавления строки ≈ 2.59+2.79=5.38
add_textbox(slide26, 0.44, 5.50, 12.5, 1.75,
    'Вывод: FM превосходит KL на всех типах графов — от +1,1% (двудольный) до +74,4% (цепочка). '
    'Ранжирование по числу межсоединений FM при n=200 (от лучшего к худшему): '
    '1) Цепочка (7,3) — структура близка к оптимально делимой; '
    '2) Решётка (15,0) — регулярность снижает число разрезов; '
    '3) Фишер–Йетс (1141) ≈ ER (1153) — случайные графы неразличимы; '
    '4) Двудольный (1179) — алгоритм не эксплуатирует двудольность при высокой плотности.',
    font_size=10)
print('Слайд 26: вывод добавлен')

# ════════════════════════════════════════════════════════════════════════════
# СЛАЙД 27 — Структурированные графы (цепочка): расширить вывод
# ════════════════════════════════════════════════════════════════════════════
slide27 = prs.slides[26]
for shape in slide27.shapes:
    if shape.has_text_frame and 'Вывод:' in shape.text_frame.text and 'структурированных' in shape.text_frame.text:
        replace_text(shape,
            'Вывод: Цепочечный граф — предельный случай: оптимальный разрез = 1 ребро. '
            'FM при n=200 достигает 7,3 межсоединений против 28,7 у KL (+74,4%). '
            'При n=600 разрыв сохраняется: 20,7 vs 79,3. '
            'На случайных и двудольных графах преимущество FM умеренное (1–12%). '
            'Причина: FM перемещает по одной вершине и точнее адаптируется к локальной структуре; '
            'KL застревает в локальных минимумах при обмене парами.',
            font_size=11)
        print('Слайд 27: вывод обновлён')
        break

# ════════════════════════════════════════════════════════════════════════════
# СЛАЙД 28 — Время работы: обновить вывод, убрать дублирующую заметку
# ════════════════════════════════════════════════════════════════════════════
slide28 = prs.slides[27]
to_remove = []
for shape in slide28.shapes:
    if shape.has_text_frame:
        txt = shape.text_frame.text
        if 'кривые совпадают' in txt and 'n≥300' in txt:
            to_remove.append(shape._element)
        elif 'Ключевое различие' in txt:
            replace_text(shape,
                'Вывод: Скорость FM кратно превосходит KL и разрыв нарастает: '
                'n=50 → в 11×, n=100 → в 17×, n=400 → в 38×, n=600 → в 39×, n=1000 → в 111×. '
                'Рост времени KL квадратичный — O(n²·k); FM линейный — O((n+m)·k). '
                'При n=1000: KL — 80 213 мс (>80 сек), FM — 721 мс. '
                'Качество разбиения при этом сопоставимо: разница ≤ 12% в пользу FM.',
                font_size=11)
            print('Слайд 28: вывод обновлён')

for el in to_remove:
    el.getparent().remove(el)
    print('Слайд 28: лишняя заметка удалена')

# ════════════════════════════════════════════════════════════════════════════
# СЛАЙД 29 — Графики итерации/время: добавить вывод справа от картинки
# ════════════════════════════════════════════════════════════════════════════
# Картинка занимает x=0.46…8.30 — справа есть ~5 дюймов
slide29 = prs.slides[28]
add_textbox(slide29, 8.40, 1.55, 4.70, 1.00,
    'Корреляция итераций и времени',
    font_size=13, bold=True)

add_textbox(slide29, 8.40, 2.65, 4.70, 4.55,
    '• Формы кривых «итерации» и «время» совпадают — корреляция прямая.\n\n'
    '• При n=1000: KL — 906 ходов, 80 213 мс; FM — 896 ходов, 721 мс. '
    'Число ходов почти одинаково, но каждый ход KL в ~111 раз дороже.\n\n'
    '• KL пересчитывает gain для всех пар O(n²); FM — только для соседей '
    'перемещённой вершины O(deg) через bucket-структуру.\n\n'
    '• Разрыв во времени растёт с n: при n=50 — 11×, при n=1000 — 111×.\n\n'
    '• Вывод: итерационная сложность алгоритмов одинакова, '
    'но FM побеждает за счёт дешевизны одного хода.',
    font_size=11)
print('Слайд 29: выводы добавлены')

# ════════════════════════════════════════════════════════════════════════════
# СЛАЙД 30 — Выводы: обновить данные FM до n=1000
# ════════════════════════════════════════════════════════════════════════════
slide30 = prs.slides[29]
for shape in slide30.shapes:
    if shape.has_text_frame:
        txt = shape.text_frame.text
        if 'n=600 работает ~230 мс' in txt:
            replace_text(shape,
                ' - Сложность: O((n + m) · k) за счёт пересчёта только соседей\n'
                '  - При n=600 — 230 мс (в 39× быстрее KL); при n=1000 — 721 мс (в 111× быстрее)\n'
                '  - На случайных графах: качество почти одинаково (разница 1–12%)\n'
                '  - На структурированных графах (цепочка): FM лучше на 70–74%',
                font_size=12)
            print('Слайд 30: блок FM обновлён')
            break

# ════════════════════════════════════════════════════════════════════════════
prs.save(DST)
print(f'\nГотово. Сохранено: {DST}')
