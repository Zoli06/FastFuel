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
import type { CartItem, Food, UnifiedItem } from './types.ts';

type MenuDetailModalProps = {
  opened: boolean;
  onClose: () => void;
  item: UnifiedItem;
  allFoods: Food[];
  onAddToCart: (cartItem: CartItem) => void;
};

export const CustomerOrderDetailModal = ({
  opened,
  onClose,
  item,
  allFoods,
  onAddToCart,
}: MenuDetailModalProps) => {
  const [quantity, setQuantity] = useState(1);
  const [specialInstructions, setSpecialInstructions] = useState('');

  if (!opened) return null;

  const foods = item.type === 'menu' ? item.foods : [];

  const handleAdd = () => {
    onAddToCart({
      item: item,
      quantity: quantity,
      specialInstructions: specialInstructions || null,
    });
    setQuantity(1);
    setSpecialInstructions('');
    onClose();
  };

  return (
    <Modal
      opened={opened}
      onClose={onClose}
      title={item.name}
      size="lg"
      radius="md"
      styles={{
        body: { padding: 0 },
        header: { padding: '16px 20px 8px' },
      }}
    >
      <Stack gap={0}>
        <Image
          src={item.imageUrl ?? undefined}
          height={260}
          fallbackSrc="https://placehold.co/600x260?text=No+Image"
          alt={item.name}
        />

        <Stack gap="sm" p="lg">
          {item.description && (
            <Text size="sm" c="dimmed">
              {item.description}
            </Text>
          )}

          {foods && foods.length > 0 && (
            <>
              <Divider label="Includes" labelPosition="left" />
              <Group gap={6} wrap="wrap">
                {foods.map((food, index) => (
                  <Badge key={index} variant="light" color="orange">
                    {allFoods.find((f) => f.id === food.foodId)?.name ?? `#${food.foodId}`} x
                    {food.quantity}
                  </Badge>
                ))}
              </Group>
            </>
          )}

          <Divider label="Customise" labelPosition="left" />

          <NumberInput
            label="Quantity"
            min={1}
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
              ${(item.price * quantity).toFixed(2)}
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
