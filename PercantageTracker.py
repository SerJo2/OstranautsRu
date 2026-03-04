import re
from pathlib import Path
import json
import os

folder_path = Path('./Ostranauts_Data')
all_strName = 0
all_strNameFriendly = 0
all_strDesc = 0
all_strNameShort = 0

english_strName = 0
english_strNameFriendly = 0
english_strDesc = 0
english_strNameShort = 0

ru_strName = 0
ru_strNameFriendly = 0
ru_strDesc = 0
ru_strNameShort = 0


def contains_russian(text):
    return bool(re.search(r'[а-яА-ЯёЁ]', text))

for file_path in folder_path.rglob('*'):
    if file_path.is_file():
        if os.path.splitext(file_path)[1] == '.json':
            if "conditions_simple" in str(file_path):
                with open(file_path, 'r', encoding='utf-8') as f:
                    lines = f.readlines()
                    filtered_lines = [line for line in lines if not line.lstrip().startswith('//')]
                    json_str = ''.join(filtered_lines)
                    data = json.loads(json_str)
            elif "verbs" in str(file_path) or "schema" in str(file_path):
                pass
            else:
                with open(file_path, 'r', encoding='utf-8-sig') as file:
                    data = json.load(file)
                    for i in data:
                        if "strName" in i:
                            all_strName += 1
                            english_strName += 1

                        if "strNameShort" in i:
                            all_strNameShort += 1
                            if contains_russian(i["strNameShort"]):
                                ru_strNameShort += 1
                            else:
                                english_strNameShort += 1
                        if "strNameFriendly" in i:
                            all_strNameFriendly += 1
                            if contains_russian(i["strNameFriendly"]):
                                english_strNameFriendly += 1
                            else:
                                ru_strNameFriendly += 1

                        if "strDesc" in i and i["strDesc"] is not None:
                            all_strDesc += 1
                            if contains_russian(i["strDesc"]):
                                english_strDesc += 1
                            else:
                                ru_strDesc += 1


ru_progress = (ru_strDesc + ru_strNameShort + ru_strNameFriendly)
all_progress = (all_strDesc + all_strNameShort + all_strNameFriendly)
percentage_progress = round(((ru_strDesc + ru_strNameShort + ru_strNameFriendly) / (all_strDesc + all_strNameShort + all_strNameFriendly)) * 100, 2)




with open("README.MD", 'r', encoding='utf-8') as file:
    lines = file.readlines()

with open('README.MD', 'w', encoding='utf-8') as file:
    for line in lines:
        if "https://img.shields.io/badge/Прогресс" in line:
            file.write(f'![Progress](https://img.shields.io/badge/Прогресс-{ru_progress}%2F{all_progress}({percentage_progress}%25)-green)' + '\n')
        else:
            file.write(line)
