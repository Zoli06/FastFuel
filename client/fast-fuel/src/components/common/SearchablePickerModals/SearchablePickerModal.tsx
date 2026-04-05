import { Modal, ScrollArea, Stack, Text, TextInput, UnstyledButton } from '@mantine/core';
import { IconSearch } from '@tabler/icons-react';
import type { Key, ReactNode } from 'react';

type SearchablePickerModalProps<TItem> = {
  opened: boolean;
  title: string;
  searchValue: string;
  searchPlaceholder: string;
  onSearchChange: (value: string) => void;
  onClose: () => void;
  items: readonly TItem[];
  getItemKey: (item: TItem) => Key;
  isItemSelected: (item: TItem) => boolean;
  onSelectItem: (item: TItem) => void;
  renderItem: (item: TItem, selected: boolean) => ReactNode;
  emptyMessage?: ReactNode;
};

export function SearchablePickerModal<TItem>({
  opened,
  title,
  searchValue,
  searchPlaceholder,
  onSearchChange,
  onClose,
  items,
  getItemKey,
  isItemSelected,
  onSelectItem,
  renderItem,
  emptyMessage = 'No items found',
}: SearchablePickerModalProps<TItem>) {
  return (
    <Modal opened={opened} onClose={onClose} title={title} centered size="md">
      <Stack gap="sm">
        <TextInput
          placeholder={searchPlaceholder}
          leftSection={<IconSearch size={15} />}
          value={searchValue}
          onChange={(e) => onSearchChange(e.currentTarget.value)}
          autoFocus
        />
        <ScrollArea h={320}>
          <Stack gap={4}>
            {items.length === 0 && (
              <Text c="dimmed" size="sm" ta="center" py="md">
                {emptyMessage}
              </Text>
            )}
            {items.map((item) => {
              const selected = isItemSelected(item);

              return (
                <UnstyledButton
                  key={getItemKey(item)}
                  onClick={() => onSelectItem(item)}
                  style={{
                    padding: '10px 14px',
                    borderRadius: 8,
                    width: '100%',
                    textAlign: 'left',
                    background: selected
                      ? 'var(--mantine-color-orange-5)'
                      : 'var(--mantine-color-blue-9)',
                    border: selected
                      ? '2px solid var(--mantine-color-blue-4)'
                      : '2px solid var(--mantine-color-blue-8)',
                    transition: 'all 0.12s',
                  }}
                >
                  {renderItem(item, selected)}
                </UnstyledButton>
              );
            })}
          </Stack>
        </ScrollArea>
      </Stack>
    </Modal>
  );
}
