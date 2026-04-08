import { SimpleGrid, Text } from '@mantine/core';
import { OrderItemCard } from '../OrderItemCard.tsx';
import { CATEGORIES, type CategoryValue } from '../constants.ts';
import type { CartItem, Food, UnifiedItem } from '../types.ts';

type ItemGridProps = {
  activeCategory: CategoryValue;
  items: UnifiedItem[];
  foods: Food[];
  onAddItem: (item: CartItem) => void;
  onOpenItem: (item: UnifiedItem) => void;
};

export const ItemGrid = ({
  activeCategory,
  items,
  foods,
  onAddItem,
  onOpenItem,
}: ItemGridProps) => {
  return (
    <>
      <Text
        fw={700}
        size="xl"
        style={{
          color: 'darkred',
          textTransform: 'uppercase',
          letterSpacing: 1,
        }}
      >
        {CATEGORIES.find((c) => c.value === activeCategory)?.label}
        <Text span size="sm" c="dimmed" fw={400} ml={8}>
          ({items.length} items)
        </Text>
      </Text>

      <SimpleGrid cols={{ base: 1, sm: 3, md: 4, lg: 6 }} spacing={4}>
        {items.map((item) => (
          <OrderItemCard
            key={`${item.type}-${item.id}`}
            item={item}
            allFoods={foods}
            onAdd={() => onAddItem({ item, quantity: 1, specialInstructions: null })}
            onOpen={() => onOpenItem(item)}
          />
        ))}
      </SimpleGrid>
    </>
  );
};
