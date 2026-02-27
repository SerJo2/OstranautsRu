def count_changed_lines(diff_file):
    added = 0
    removed = 0

    with open(diff_file, 'r', encoding='utf-8') as f:
        for line in f:
            if line.startswith(('diff ', 'index ', '--- ', '+++ ', '@@')):
                continue
            if line.startswith('+'):
                added += 1
            elif line.startswith('-'):
                removed += 1

    return added, removed

if __name__ == '__main__':
    added, removed = count_changed_lines('diff.txt')
    total = added + removed
    print(f'Добавлено строк: {added}')
    print(f'Удалено строк: {removed}')
    print(f'Всего изменено строк: {total}')