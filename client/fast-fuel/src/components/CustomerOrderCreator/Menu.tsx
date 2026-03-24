import { Badge, Box, Card, Group, Image, Stack, Text } from '@mantine/core';
import { apiClient } from '../../lib/api-client.ts';

const useFoods = () => apiClient.useQuery('get', '/api/Food');

type Food = NonNullable<ReturnType<typeof useFoods>['data']>[number];
type MenuFood = { foodId: number; quantity: number };

type MenuProps = {
  id: number;
  name: string;
  price: number;
  description: string | null;
  imageUrl: string | null;
  foods: MenuFood[];
  quantity: number;
  type: 'food' | 'menu';
  onAdd: () => void;
  onRemove: () => void;
  onOpen: () => void;
};

export const Menu = ({
  name,
  price,
  description,
  imageUrl,
  foods,
  quantity,
  type,
  onOpen,
}: MenuProps) => {
  const { data: allFoods } = useFoods();

  const getFoodName = (foodId: number): string =>
    allFoods?.find((f: Food) => f.id === foodId)?.name ?? `#${foodId}`;

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
          {quantity > 0 && (
            <Badge color="darkred" size="sm" circle style={{ flexShrink: 0 }}>
              {quantity}
            </Badge>
          )}
        </Group>

        {description && (
          <Text size="xs" c="dimmed" lineClamp={1}>
            {description}
          </Text>
        )}

        <Box visibleFrom="xs">
          <Group gap={3} wrap="wrap">
            {foods.slice(0, 2).map((f) => (
              <Badge
                key={f.foodId}
                size="xs"
                variant="light"
                color="orange"
                style={{ fontSize: 9 }}
              >
                {getFoodName(f.foodId)}
              </Badge>
            ))}
            {foods.length > 2 && (
              <Badge size="xs" variant="light" color="gray" style={{ fontSize: 9 }}>
                +{foods.length - 2}
              </Badge>
            )}
          </Group>
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
