import { useState } from 'react';
import {
  Badge,
  Box,
  Collapse,
  Divider,
  Group,
  Paper,
  Stack,
  Text,
  UnstyledButton,
} from '@mantine/core';
import { IconChevronDown, IconChevronUp, IconClock, IconReceipt2 } from '@tabler/icons-react';
import type { OrderSummary } from './types.ts';
import { STATUS_META, formatDate } from './helpers.ts';

interface OrderRowCollapseProps {
  order: OrderSummary;
}

export const OrderRowCollapse = ({ order }: OrderRowCollapseProps) => {
  const [open, setOpen] = useState(false);
  const meta = STATUS_META[order.status] ?? STATUS_META.Pending;

  return (
    <Paper
      withBorder
      radius="md"
      style={{
        borderColor: open ? 'var(--mantine-color-orange-5)' : 'var(--mantine-color-orange-9)',
        overflow: 'hidden',
        transition: 'border-color 0.15s',
      }}
    >
      <UnstyledButton onClick={() => setOpen((o) => !o)} style={{ width: '100%' }}>
        <Group
          px="md"
          py="sm"
          justify="space-between"
          wrap="nowrap"
          style={{
            background: open ? 'var(--mantine-color-orange-9)' : 'var(--mantine-color-dark-8)',
            transition: 'background 0.15s',
          }}
        >
          <Group gap="sm" wrap="nowrap" style={{ minWidth: 0 }}>
            <Box
              style={{
                width: 36,
                height: 36,
                borderRadius: 8,
                background: 'var(--mantine-color-orange-6)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                flexShrink: 0,
              }}
            >
              <IconReceipt2 size={18} color="white" />
            </Box>
            <Stack gap={2} style={{ minWidth: 0 }}>
              <Text fw={700} size="sm" c="orange.4" style={{ lineHeight: 1 }}>
                #{order.orderNumber}
              </Text>
              <Text size="xs" c="dimmed" lineClamp={1}>
                {order.restaurantName}
              </Text>
            </Stack>
          </Group>

          <Group gap={4} visibleFrom="sm">
            <IconClock size={13} color="var(--mantine-color-orange-4)" />
            <Text size="xs" c="orange.4">
              {formatDate(order.placedAt)}
            </Text>
          </Group>

          <Group gap="sm" wrap="nowrap" style={{ flexShrink: 0 }}>
            <Badge color={meta.color} variant="light" size="sm">
              {meta.label}
            </Badge>
            <Text fw={800} size="sm" c="orange.5" style={{ minWidth: 60, textAlign: 'right' }}>
              ${order.totalPrice.toFixed(2)}
            </Text>
            {open ? (
              <IconChevronUp size={16} color="var(--mantine-color-orange-4)" />
            ) : (
              <IconChevronDown size={16} color="var(--mantine-color-orange-4)" />
            )}
          </Group>
        </Group>
      </UnstyledButton>

      <Collapse in={open}>
        <Divider color="orange.8" />
        <Stack gap={0} px="md" py="sm" style={{ background: 'var(--mantine-color-dark-7)' }}>
          <Group gap={4} hiddenFrom="sm" mb="xs">
            <IconClock size={13} color="var(--mantine-color-orange-4)" />
            <Text size="xs" c="orange.4">
              {formatDate(order.placedAt)}
            </Text>
          </Group>

          {order.items.length === 0 ? (
            <Text size="sm" c="dimmed" py="xs">
              No items found
            </Text>
          ) : (
            order.items.map((item, i) => (
              <Box key={i}>
                {i > 0 && <Divider color="orange.9" my={6} />}
                <Group justify="space-between" wrap="nowrap" py={4}>
                  <Stack gap={0} style={{ flex: 1, minWidth: 0 }}>
                    <Group gap="xs">
                      <Box
                        style={{
                          width: 6,
                          height: 6,
                          borderRadius: '50%',
                          background: 'var(--mantine-color-orange-5)',
                          flexShrink: 0,
                        }}
                      />
                      <Text size="sm" c="gray.2" lineClamp={1}>
                        {item.name}
                      </Text>
                      <Badge size="xs" color="orange" variant="outline">
                        ×{item.quantity}
                      </Badge>
                    </Group>
                    {item.note && (
                      <Text size="xs" c="dimmed" ml={14} fs="italic">
                        {item.note}
                      </Text>
                    )}
                  </Stack>
                  <Text size="sm" fw={600} c="orange.4" style={{ flexShrink: 0, marginLeft: 12 }}>
                    ${(item.price * item.quantity).toFixed(2)}
                  </Text>
                </Group>
              </Box>
            ))
          )}

          <Divider color="orange.8" my="sm" />
          <Group justify="space-between">
            <Text size="sm" fw={700} c="orange.3">
              Total
            </Text>
            <Text size="md" fw={800} c="orange.5">
              ${order.totalPrice.toFixed(2)}
            </Text>
          </Group>
        </Stack>
      </Collapse>
    </Paper>
  );
};
