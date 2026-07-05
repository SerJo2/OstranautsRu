import re
from pathlib import Path
import json

folder_path = Path("./src/ostranautsRu/data")
all_str = 0
ru_str = 0


def contains_russian(text):
    return bool(re.search(r"[а-яА-ЯёЁ]", text))


FIELDS = frozenset({"strNameFriendly", "strNameShort", "strDesc", "strTitle"})


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


for file_path in folder_path.rglob("*"):
    if file_path.is_file():
        if file_path.suffix == ".json":
            try:
                with open(file_path, "r", encoding="utf-8-sig") as file:
                    data = json.load(file)

            except (json.JSONDecodeError, UnicodeDecodeError):
                continue

            file_all, file_ru = count_translation_strings(data)

            all_str += file_all
            ru_str += file_ru
percentage_progress = round((ru_str / all_str) * 100, 2) if all_str else 0


with open("README.MD", "r", encoding="utf-8") as file:
    lines = file.readlines()

with open("README.MD", "w", encoding="utf-8") as file:
    for line in lines:
        if "https://img.shields.io/badge/Прогресс" in line:
            file.write(
                f"![Progress](https://img.shields.io/badge/Прогресс-{ru_str}%2F{all_str}({percentage_progress}%25)-green)"
                + "\n"
            )
        else:
            file.write(line)
