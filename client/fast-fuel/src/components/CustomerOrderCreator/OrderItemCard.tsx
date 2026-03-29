import { Badge, Box, Card, Group, Image, Stack, Text } from '@mantine/core';
import type { Food, UnifiedItem } from './types.ts';

type OrderItemCardProps = {
  item: UnifiedItem;
  allFoods: Food[];
  onAdd: () => void;
  onRemove: () => void;
  onOpen: () => void;
};

export const OrderItemCard = ({ item, allFoods, onOpen }: OrderItemCardProps) => {
  const { name, description, price, imageUrl, type } = item;
  const foods = item.type === 'menu' ? item.foods : [];

  return (
    <Card
      shadow="sm"
      padding={0}
      withBorder
      onClick={onOpen}
      style={{ cursor: 'pointer', overflow: 'hidden', width: '100%', borderRadius: 0 }}
    >
      <Card.Section>
        <Image
          src={imageUrl ?? undefined}
          height={160}
          fit="cover"
          fallbackSrc="https://placehold.co/180x160?text=No+Image"
          alt={name}
        />
      </Card.Section>

      <Stack gap={2} px={6} pt={4} pb={6}>
        <Group justify="space-between" wrap="nowrap">
          <Text fw={700} size="sm" lineClamp={1} style={{ flex: 1 }}>
            {name}
          </Text>
          {/*{q > 1 && (*/}
          {/*  <Badge color="darkred" size="sm" circle style={{ flexShrink: 0 }}>*/}
          {/*    {quantity}*/}
          {/*  </Badge>*/}
          {/*)}*/}
        </Group>

        {description && (
          <Text size="xs" c="dimmed" lineClamp={1}>
            {description}
          </Text>
        )}

        <Box visibleFrom="xs">
          {foods.length > 0 && (
            <Group gap={3} wrap="wrap">
              {foods.slice(0, 2).map((f, i) => {
                const food = allFoods.find((food) => food.id === f.foodId);
                return (
                  <Badge key={i} size="xs" variant="light" color="gray" style={{ fontSize: 9 }}>
                    {food ? food.name : `#${f.foodId}`} ×{f.quantity}
                  </Badge>
                );
              })}
              {foods.length > 2 && (
                <Badge size="xs" variant="light" color="gray" style={{ fontSize: 9 }}>
                  +{foods.length - 2}
                </Badge>
              )}
            </Group>
          )}
        </Box>

        <Group justify="space-between" wrap="nowrap">
          <Text fw={700} c="darkred" size="sm">
            ${price.toFixed(2)}
          </Text>
          <Badge
            size="xs"
            variant="light"
            color={type === 'menu' ? 'orange' : 'blue'}
            style={{ flexShrink: 0 }}
          >
            {type}
          </Badge>
        </Group>
      </Stack>
    </Card>
  );
};
