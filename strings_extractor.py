import json
import sys


def filter_json(input_path, output_path, fields):
    with open(input_path, "r", encoding="utf-8") as f:
        data = json.load(f)

    filtered = []
    for item in data:
        new_item = {key: item[key] for key in fields if key in item}
        filtered.append(new_item)

    with open(output_path, "w", encoding="utf-8") as f:
        json.dump(filtered, f, indent=2, ensure_ascii=False)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        sys.exit(1)

    input_file = sys.argv[1]
    output_file = sys.argv[2]
    needed_fields = ["strName", "strDesc", "strNameFriendly"]

    filter_json(input_file, output_file, needed_fields)
    print(f"езультат сохранён в {output_file}")
