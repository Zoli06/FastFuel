import { Button, Group, Select, TextInput } from '@mantine/core';
import {
  IconArrowDown,
  IconArrowUp,
  IconSearch,
  IconSortAscendingLetters,
  IconSortDescendingLetters,
} from '@tabler/icons-react';
import { ALL_STATUSES, SORT_OPTIONS, STATUS_META } from './helpers.ts';
import type { OrderStatus, SortKey } from './types.ts';

interface OrderHistoryFiltersProps {
  search: string;
  onSearchChange: (v: string) => void;
  sortKey: SortKey;
  onSortChange: (v: SortKey) => void;
  statusFilter: OrderStatus | undefined;
  onStatusChange: (v: OrderStatus | undefined) => void;
}

export const OrderHistoryFilters = ({
  search,
  onSearchChange,
  sortKey,
  onSortChange,
  statusFilter,
  onStatusChange,
}: OrderHistoryFiltersProps) => (
  <>
    <Group gap="sm" wrap="wrap">
      <TextInput
        placeholder="Search by order # or restaurant..."
        leftSection={<IconSearch size={16} color="var(--mantine-color-orange-5)" />}
        value={search}
        onChange={(e) => onSearchChange(e.currentTarget.value)}
        size="md"
        style={{ flex: 1, minWidth: 200 }}
        styles={{ input: { borderColor: 'var(--mantine-color-orange-8)' } }}
      />
      <Select
        data={SORT_OPTIONS}
        value={sortKey}
        onChange={(v) => v && onSortChange(v as SortKey)}
        size="md"
        style={{ width: 190 }}
        styles={{ input: { borderColor: 'var(--mantine-color-orange-8)' } }}
        leftSection={
          sortKey.includes('asc') ? (
            sortKey.startsWith('name') ? (
              <IconSortAscendingLetters size={16} color="var(--mantine-color-orange-5)" />
            ) : (
              <IconArrowUp size={16} color="var(--mantine-color-orange-5)" />
            )
          ) : sortKey.startsWith('name') ? (
            <IconSortDescendingLetters size={16} color="var(--mantine-color-orange-5)" />
          ) : (
            <IconArrowDown size={16} color="var(--mantine-color-orange-5)" />
          )
        }
      />
    </Group>

    <Group gap={6} wrap="wrap">
      <Button
        size="compact-sm"
        variant={statusFilter === undefined ? 'filled' : 'light'}
        color="orange"
        onClick={() => onStatusChange(undefined)}
      >
        All
      </Button>
      {ALL_STATUSES.map((s) => (
        <Button
          key={s}
          size="compact-sm"
          variant={statusFilter === s ? 'filled' : 'light'}
          color={STATUS_META[s].color}
          onClick={() => onStatusChange(statusFilter === s ? undefined : s)}
        >
          {STATUS_META[s].label}
        </Button>
      ))}
    </Group>
  </>
);
