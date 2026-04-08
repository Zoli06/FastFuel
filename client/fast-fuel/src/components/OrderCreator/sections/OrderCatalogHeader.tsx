import { Button, Group, Text, TextInput } from '@mantine/core';
import {
  IconArrowsSort,
  IconArrowDown,
  IconArrowUp,
  IconSearch,
  IconSortAscendingLetters,
  IconSortDescendingLetters,
} from '@tabler/icons-react';
import type { SortKey } from '../types.ts';

type RestaurantOption = {
  id: number;
  name: string;
};

type OrderCatalogHeaderProps = {
  selectedRestaurant: RestaurantOption | undefined;
  needsRestaurantPicker: boolean;
  searchQuery: string;
  onSearchQueryChange: (value: string) => void;
  sortKey: SortKey;
  onSortKeyChange: (value: SortKey) => void;
  onOpenRestaurantPicker: () => void;
};

export const OrderCatalogHeader = ({
  selectedRestaurant,
  needsRestaurantPicker,
  searchQuery,
  onSearchQueryChange,
  sortKey,
  onSortKeyChange,
  onOpenRestaurantPicker,
}: OrderCatalogHeaderProps) => {
  return (
    <Group justify="space-between" align="flex-end" wrap="wrap" gap="sm">
      <Group gap="xs" align="center">
        {selectedRestaurant ? (
          <>
            <Text size="sm" c="dimmed">
              Ordering from:
            </Text>
            <Text fw={700} size="sm" c="darkred">
              {selectedRestaurant.name}
            </Text>
            {needsRestaurantPicker && (
              <Button
                size="compact-xs"
                variant="subtle"
                color="darkred"
                onClick={onOpenRestaurantPicker}
              >
                Change
              </Button>
            )}
          </>
        ) : (
          needsRestaurantPicker && (
            <Button size="xs" variant="light" color="darkred" onClick={onOpenRestaurantPicker}>
              Select a restaurant
            </Button>
          )
        )}
      </Group>

      <Group gap="sm" wrap="wrap">
        <TextInput
          placeholder="Search food & menus..."
          leftSection={<IconSearch size={18} />}
          value={searchQuery}
          onChange={(e) => onSearchQueryChange(e.currentTarget.value)}
          size="md"
          style={{ width: 260 }}
        />

        {[
          {
            keyAsc: 'name-asc' as SortKey,
            keyDesc: 'name-desc' as SortKey,
            iconAsc: IconSortAscendingLetters,
            iconDesc: IconSortDescendingLetters,
            label: 'Name',
          },
          {
            keyAsc: 'price-asc' as SortKey,
            keyDesc: 'price-desc' as SortKey,
            iconAsc: IconArrowUp,
            iconDesc: IconArrowDown,
            label: 'Price',
          },
        ].map((s) => {
          const isAsc = sortKey === s.keyAsc;
          const isDesc = sortKey === s.keyDesc;
          const isActive = isAsc || isDesc;
          const Icon = isActive ? (isDesc ? s.iconDesc : s.iconAsc) : IconArrowsSort;
          return (
            <Button
              key={s.label}
              size="sm"
              variant={isActive ? 'filled' : 'light'}
              color="darkred"
              leftSection={<Icon size={15} />}
              onClick={() => onSortKeyChange(isAsc ? s.keyDesc : s.keyAsc)}
            >
              {s.label}
            </Button>
          );
        })}
      </Group>
    </Group>
  );
};
