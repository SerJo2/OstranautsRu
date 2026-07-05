import re
from pathlib import Path
import json

# Путь изменился, а в скрипте нет... Так же strName как я понимаю являются неизменяемыми ID
# смысла подсчитывать их как-будто нет - как минимум, в наличии ни одной переведённой строки.
folder_path = Path('./src/ostranautsRu/data')
# И нам в целом не нужно знать сколько строк по отдельности, раньше они в любом случае складывались.
all_str = 0
# Ещё не вижу смысла в целом оставлять подсчёт переменных с английским, он нигде не используется.
ru_str = 0

def contains_russian(text):
    return bool(re.search(r'[а-яА-ЯёЁ]', text))
# Вместо просто укажем неизменяемое множество в каких строках искать, обновляя две переменных.
FIELDS = frozenset({
    "strNameFriendly",
    "strNameShort",
    "strDesc",
    "strTitle"
})
# К тому же, здесь было забыто strTitle, что в version_upgrade.py к слову указано.

def count_translation_strings(root):

    all_str = 0
    ru_str = 0

    stack = [root]

    while stack:

        obj = stack.pop()

        if isinstance(obj, dict):

            for key, value in obj.items():

                if key in FIELDS:

                    if isinstance(value, str):

                        value = value.strip()

                        if value and not all(char in "=-_" for char in value):

                            all_str += 1

                            if contains_russian(value):
                                ru_str += 1

                elif isinstance(value, (dict, list)):

                    stack.append(value)


        elif isinstance(obj, list):

            stack.extend(obj)


    return all_str, ru_str

for file_path in folder_path.rglob('*'):
    if file_path.is_file():
# pathlib уже используется, и может это делать сам, привлекание os излишне.
       if file_path.suffix == '.json':
# // убрались из conditions_simple.json в version_upgrade.py, повтор в этом месте излишен.
# Да и verbs так же пропустим из-за нестандартного json, a *.schema.json из-за разницы в обработке.
            try:
                with open(file_path, 'r', encoding='utf-8-sig') as file:
                    data = json.load(file)

            except (json.JSONDecodeError, UnicodeDecodeError):
                continue

            file_all, file_ru = count_translation_strings(data)

            all_str += file_all
            ru_str += file_ru
# Данные ещё были не правильные: strNameFriendly и strDesc прибавляли значения ru_str*
# если строка была НЕ переведена

# Здесь вообще была странная штука, объединение значений в краткие переменные есть,
# а их использования - нет, дублируя то же самое символ в символ. Для чего? Чёрт знает.
percentage_progress = round((ru_str / all_str) * 100, 2) if all_str else 0



with open("README.MD", 'r', encoding='utf-8') as file:
    lines = file.readlines()

with open('README.MD', 'w', encoding='utf-8') as file:
    for line in lines:
        if "https://img.shields.io/badge/Прогресс" in line:
            file.write(f'![Progress](https://img.shields.io/badge/Прогресс-{ru_str}%2F{all_str}({percentage_progress}%25)-green)' + '\n')
        else:
            file.write(line)
