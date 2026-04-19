import { Modal, ScrollArea, Stack, Text, TextInput, UnstyledButton } from '@mantine/core';
import { IconSearch } from '@tabler/icons-react';
import type { Key, ReactNode } from 'react';
import classes from './SearchablePickerModal.module.css';

type SearchablePickerModalProps<TItem> = {
  opened: boolean;
  title: string;
  hasSelection: boolean;
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
  hasSelection,
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
    <Modal
      opened={opened}
      onClose={onClose}
      title={title}
      withCloseButton={hasSelection}
      centered
      size="md"
    >
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
                  className={classes.item}
                  data-selected={selected || undefined}
                  py={10}
                  px={14}
                  bdrs={8}
                  w="100%"
                  ta="left"
                  bd={selected ? '2px solid var(--mantine-color-red-8)' : '1px solid transparent'}
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
