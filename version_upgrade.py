import json
import os
import sys
import re

source_dir = r"D:\oru\OstranautsRu\Ostranauts_Data\StreamingAssets\data"

target_dir = r"C:\Program Files (x86)\Steam\steamapps\common\Ostranauts\Ostranauts_Data\StreamingAssets\data"

fields_to_translate = ["strNameShort", "strNameFriendly", "strDesc", "strTitle"]

SPECIAL_FILE = "conditions_simple.json"

stats = {
    "processed": 0,
    "special": 0,
    "skipped_no_target": 0,
    "skipped_read_error": 0,
    "skipped_write_error": 0,
    "skipped_bad_structure": 0,
    "warnings": 0,
    "total_objects_updated": 0,
    "errors": 0,
}


def warn(msg):
    stats["warnings"] += 1
    # print(f"WARN: {msg}")


def error(msg):
    print(f"❌ ERROR: {msg}", file=sys.stderr)


def load_json_stripping_comments(filepath):
    """Читает JSON-файл, удаляя строки, начинающиеся с // (комментарии)."""
    with open(filepath, "r", encoding="utf-8-sig") as f:
        text = f.read()
    cleaned = re.sub(r"^\s*//.*$", "", text, flags=re.MULTILINE)
    return json.loads(cleaned)


def process_conditions_simple(rel_path, source_path, target_path):
    """
    Обрабатывает conditions_simple.json:
    """
    print(f"\n=== Специальная обработка: {rel_path} ===")

    try:
        source_json = load_json_stripping_comments(source_path)
    except Exception as e:
        error(f"Ошибка чтения/парсинга source {rel_path}: {e}")
        return False

    if not isinstance(source_json, list) or len(source_json) == 0:
        error(f"Source {rel_path}: ожидался непустой список.")
        return False
    source_obj = source_json[0]
    if not isinstance(source_obj, dict) or "aValues" not in source_obj:
        error(f"Source {rel_path}: первый элемент не содержит 'aValues'.")
        return False

    src_values = source_obj["aValues"]
    translations = {}
    i = 0
    while i + 6 < len(src_values):
        name = str(src_values[i]).strip()
        friendly = str(src_values[i + 1]).strip()
        desc = str(src_values[i + 2]).strip()
        translations[name] = (friendly, desc)
        i += 7

    print(f"Загружено переводов из source: {len(translations)}")

    try:
        target_json = load_json_stripping_comments(target_path)
    except Exception as e:
        error(f"Ошибка чтения/парсинга target {rel_path}: {e}")
        return False

    if not isinstance(target_json, list) or len(target_json) == 0:
        error(f"Target {rel_path}: ожидался непустой список.")
        return False
    target_obj = target_json[0]
    if not isinstance(target_obj, dict) or "aValues" not in target_obj:
        error(f"Target {rel_path}: первый элемент не содержит 'aValues'.")
        return False

    tgt_values = target_obj["aValues"]

    updated = []
    i = 0
    missing = 0
    while i < len(tgt_values):
        if i + 6 < len(tgt_values):
            name = str(tgt_values[i]).strip()
            if name in translations:
                friendly_new, desc_new = translations[name]

                updated.append(tgt_values[i])
                updated.append(friendly_new)
                updated.append(desc_new)

                updated.extend(tgt_values[i + 3 : i + 7])
            else:
                missing += 1
                updated.extend(tgt_values[i : i + 7])
            i += 7
        else:
            updated.extend(tgt_values[i:])
            break

    if missing:
        warn(f"{rel_path}: для {missing} записей перевод не найден.")

    target_obj["aValues"] = updated
    output_data = [target_obj]
    try:
        os.makedirs(os.path.dirname(source_path), exist_ok=True)
        with open(source_path, "w", encoding="utf-8") as f:
            json.dump(output_data, f, ensure_ascii=False, indent=2)
        print(f"✔ Файл {rel_path} успешно обновлён ({len(updated)} элементов).")
        return True
    except Exception as e:
        error(f"Ошибка записи {source_path}: {e}")
        return False


for root, dirs, files in os.walk(source_dir):
    if "schemas" in root:
        print(f"Пропущена папка: {root}")
        continue

    for filename in files:
        if "verbs.json" in filename:
            print(f"Пропущен файл: {filename}")
            continue
        if not filename.lower().endswith(".json"):
            continue

        source_path = os.path.join(root, filename)
        rel_path = os.path.relpath(source_path, source_dir)
        target_path = os.path.join(target_dir, rel_path)

        if not os.path.exists(target_path):
            error(f"Пропущен {rel_path}: нет соответствующего файла в {target_dir}")
            stats["skipped_no_target"] += 1
            continue

        if filename == SPECIAL_FILE:
            success = process_conditions_simple(rel_path, source_path, target_path)
            if success:
                stats["special"] += 1
            else:
                stats["errors"] += 1
            continue

        try:
            with open(source_path, "r", encoding="utf-8-sig") as f:
                source_data = json.load(f)
        except Exception as e:
            error(f"Ошибка чтения {rel_path}: {e}")
            stats["skipped_read_error"] += 1
            continue

        if not isinstance(source_data, list):
            error(f"Некорректная структура {rel_path}: ожидался список. Skip.")
            stats["skipped_bad_structure"] += 1
            continue

        old_translations = {}
        for idx, item in enumerate(source_data):
            if not isinstance(item, dict):
                warn(f"{rel_path}: элемент #{idx} не словарь, skip")
                continue
            name = item.get("strName")
            if not name:
                continue
            trans = {}
            for field in fields_to_translate:
                if field in item:
                    val = item[field]
                    if not isinstance(val, str):
                        warn(
                            f"{rel_path}: поле '{field}' для '{name}' не строка, приведено к строке"
                        )
                        val = str(val)
                    trans[field] = val
            if trans:
                old_translations[name] = trans

        try:
            with open(target_path, "r", encoding="utf-8-sig") as f:
                target_data = json.load(f)
        except Exception as e:
            error(f"Ошибка чтения целевого файла {target_path}: {e}")
            stats["skipped_read_error"] += 1
            continue

        if not isinstance(target_data, list):
            error(f"Некорректная структура целевого файла {rel_path}. Skip.")
            stats["skipped_bad_structure"] += 1
            continue

        updated_objects = []
        missing = 0
        for item in target_data:
            if not isinstance(item, dict):
                updated_objects.append(item)
                continue
            new_item = item.copy()
            name = new_item.get("strName")
            if name and name in old_translations:
                trans = old_translations[name]
                for field in fields_to_translate:
                    if field in new_item and field in trans:
                        new_item[field] = trans[field]
            else:
                if name:
                    missing += 1
            updated_objects.append(new_item)

        if missing:
            warn(f"{rel_path}: для {missing} объектов не найдено переводов")

        try:
            os.makedirs(os.path.dirname(source_path), exist_ok=True)
            with open(source_path, "w", encoding="utf-8") as f:
                json.dump(updated_objects, f, ensure_ascii=False, indent=2)
        except Exception as e:
            error(f"Ошибка записи {rel_path}: {e}")
            stats["skipped_write_error"] += 1
            continue

        stats["processed"] += 1

print(f"Обработано файлов:    {stats['processed']}")
print(f"Обработано специальных файлов:        {stats['special']}")
print(f"Пропущено (нет целевого файла):       {stats['skipped_no_target']}")
print(f"Пропущено (ошибка чтения):            {stats['skipped_read_error']}")
print(f"Пропущено (ошибка записи):            {stats['skipped_write_error']}")
print(f"Пропущено (неверная структура):       {stats['skipped_bad_structure']}")
print(f"Всего предупреждений:                 {stats['warnings']}")
