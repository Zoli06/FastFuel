import {
  Badge,
  Button,
  Divider,
  Group,
  Image,
  Modal,
  NumberInput,
  Stack,
  Text,
  Textarea,
} from '@mantine/core';
import { useState } from 'react';
import { useFoods } from '../../hooks/useFoods.ts';

type MenuFood = { foodId: number; quantity: number };

type MenuDetailModalProps = {
  opened: boolean;
  onClose: () => void;
  menu: {
    id: number;
    name: string;
    price: number;
    description: string | null;
    imageUrl: string | null;
    foods: MenuFood[];
    type: 'food' | 'menu';
  } | null;
  onAddToCart: (
    menuId: number,
    quantity: number,
    specialInstructions: string | null,
    type: 'food' | 'menu',
  ) => void;
};

export const MenuDetailModal = ({ opened, onClose, menu, onAddToCart }: MenuDetailModalProps) => {
  const { data: allFoods } = useFoods();
  const [quantity, setQuantity] = useState(1);
  const [specialInstructions, setSpecialInstructions] = useState('');

  const getFoodName = (foodId: number) =>
    allFoods?.find((f) => f.id === foodId)?.name ?? `#${foodId}`;

  if (!menu) return null;

  const handleAdd = () => {
    onAddToCart(menu.id, quantity, specialInstructions.trim() || null, menu.type);
    setQuantity(1);
    setSpecialInstructions('');
    onClose();
  };

  return (
    <Modal
      opened={opened}
      onClose={onClose}
      title={menu.name}
      size="lg"
      radius="md"
      styles={{
        body: { padding: 0 },
        header: { padding: '16px 20px 8px' },
      }}
    >
      <Stack gap={0}>
        <Image
          src={menu.imageUrl ?? undefined}
          height={260}
          fallbackSrc="https://placehold.co/600x260?text=No+Image"
          alt={menu.name}
        />

        <Stack gap="sm" p="lg">
          {menu.description && (
            <Text size="sm" c="dimmed">
              {menu.description}
            </Text>
          )}

          {menu.foods.length > 0 && (
            <>
              <Divider label="Includes" labelPosition="left" />
              <Group gap={6} wrap="wrap">
                {menu.foods.map((f) => (
                  <Badge key={f.foodId} variant="light" color="orange">
                    {getFoodName(f.foodId)} x{f.quantity}
                  </Badge>
                ))}
              </Group>
            </>
          )}

          <Divider label="Customise" labelPosition="left" />

          <NumberInput
            label="Quantity"
            min={1}
            max={99}
            value={quantity}
            onChange={(v) => setQuantity(Number(v))}
          />

          <Textarea
            label="Special instructions"
            placeholder="e.g. no onions, extra sauce..."
            value={specialInstructions}
            onChange={(e) => setSpecialInstructions(e.currentTarget.value)}
            minRows={2}
            autosize
          />

          <Divider />

          <Group justify="space-between">
            <Text fw={700} c="darkred" size="lg">
              {(menu.price * quantity).toFixed(2)} $
            </Text>
            <Button color="darkred" onClick={handleAdd}>
              Add to order
            </Button>
          </Group>
        </Stack>
      </Stack>
    </Modal>
  );
};
