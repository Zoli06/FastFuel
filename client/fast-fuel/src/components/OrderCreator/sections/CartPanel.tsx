import { Box, Button, Divider, Group, Paper, ScrollArea, Stack, Text } from '@mantine/core';
import type { CartEntry } from '../types.ts';
import { IconPencil, IconTrash } from '@tabler/icons-react';

type CartPanelProps = {
  cart: CartEntry[];
  totalItems: number;
  totalPrice: number;
  cartScrollHeight: number;
  isPending: boolean;
  onEditEntry: (entry: CartEntry) => void;
  onRemoveOne: (cartKey: string) => void;
  onAddOne: (cartKey: string) => void;
  onPlaceOrder: () => void;
  onRemoveEntry: (cartKey: string) => void;
};

export const CartPanel = ({
  cart,
  totalItems,
  totalPrice,
  cartScrollHeight,
  isPending,
  onEditEntry,
  onRemoveOne,
  onAddOne,
  onPlaceOrder,
  onRemoveEntry,
}: CartPanelProps) => {
  if (cart.length === 0) return null;

  return (
    <Paper
      withBorder
      p="md"
      radius={0}
      style={{
        position: 'fixed',
        bottom: 0,
        left: 0,
        right: 0,
        width: '100%',
        zIndex: 100,
        background: 'var(--mantine-color-body)',
        borderTop: '2px solid beige',
        borderLeft: 'none',
        borderRight: 'none',
        borderBottom: 'none',
      }}
    >
      <Text fw={700} size="lg" mb="xs">
        Your order
        <Text span size="sm" c="dimmed" fw={400} ml={8}>
          ({totalItems} items)
        </Text>
      </Text>

      <ScrollArea h={cartScrollHeight} scrollbarSize={4} mb="xs">
        <Stack gap={0}>
          {cart.map((c, idx) => (
            <Box key={c.cartKey}>
              {idx > 0 && <Divider my={6} />}
              <Group justify="space-between" wrap="nowrap" gap="xs">
                <Stack gap={2} style={{ flex: 1, minWidth: 0 }}>
                  <Text size="sm" fw={600} lineClamp={1}>
                    {c.item.name}
                  </Text>
                  {c.specialInstructions && (
                    <Text size="xs" c="dimmed" fs="italic" lineClamp={1}>
                      {c.specialInstructions}
                    </Text>
                  )}
                </Stack>
                <Button
                  size="sm"
                  variant="light"
                  color="blue"
                  w={38}
                  h={38}
                  p={0}
                  style={{ fontSize: 12, fontWeight: 700, borderRadius: 8, flexShrink: 0 }}
                  onClick={() => onEditEntry(c)}
                >
                  <IconPencil size={16} />
                </Button>
                <Button
                  size="sm"
                  variant="light"
                  color="red"
                  w={38}
                  h={38}
                  p={0}
                  style={{ fontSize: 12, fontWeight: 700, borderRadius: 8, flexShrink: 0 }}
                  onClick={() => onRemoveEntry(c.cartKey)}
                >
                  <IconTrash size={16} />
                </Button>
                <Group gap={12} wrap="nowrap" style={{ flexShrink: 0 }} align="center">
                  <Button
                    size="sm"
                    variant="outline"
                    color="gray"
                    w={38}
                    h={38}
                    p={0}
                    style={{ fontSize: 20, fontWeight: 700, borderRadius: 8, flexShrink: 0 }}
                    onClick={() => onRemoveOne(c.cartKey)}
                  >
                    -
                  </Button>
                  <Text fw={800} size="md" style={{ minWidth: 24, textAlign: 'center' }}>
                    {c.quantity}
                  </Text>
                  <Button
                    size="sm"
                    variant="filled"
                    color="gray"
                    w={38}
                    h={38}
                    p={0}
                    style={{ fontSize: 20, fontWeight: 700, borderRadius: 8, flexShrink: 0 }}
                    onClick={() => onAddOne(c.cartKey)}
                  >
                    +
                  </Button>
                  <Text size="sm" fw={700} c="darkred" style={{ minWidth: 60, textAlign: 'right' }}>
                    ${(c.item.price * c.quantity).toFixed(2)}
                  </Text>
                </Group>
              </Group>
            </Box>
          ))}
        </Stack>
      </ScrollArea>

      <Divider mb="sm" />
      <Group justify="space-between" mb="sm">
        <Text fw={700} size="md">
          Total
        </Text>
        <Text fw={800} size="lg" c="darkred">
          ${totalPrice.toFixed(2)}
        </Text>
      </Group>
      <Button fullWidth color="darkred" loading={isPending} onClick={onPlaceOrder}>
        Place order
      </Button>
    </Paper>
  );
};
