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
        borderColor: '#c92a2a',
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
            transition: 'background 0.15s',
          }}
        >
          <Group gap="sm" wrap="nowrap" style={{ minWidth: 0 }}>
            <Box
              style={{
                width: 36,
                height: 36,
                borderRadius: 8,
                background: '#c92a2a',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                flexShrink: 0,
              }}
            >
              <IconReceipt2 size={18} color="white" />
            </Box>
            <Stack gap={2} style={{ minWidth: 0 }}>
              <Text fw={700} size="sm" c="#c92a2a" style={{ lineHeight: 1 }}>
                #{order.orderNumber}
              </Text>
              <Text size="xs" c="dimmed" lineClamp={1}>
                {order.restaurantName}
              </Text>
            </Stack>
          </Group>

          <Group gap={4} visibleFrom="sm">
            <IconClock size={13} color="#c92a2a" />
            <Text size="xs" c="#c92a2a">
              {formatDate(order.placedAt)}
            </Text>
          </Group>

          <Group gap="sm" wrap="nowrap" style={{ flexShrink: 0 }}>
            <Badge color={meta.color} variant="light" size="sm">
              {meta.label}
            </Badge>
            <Text fw={800} size="sm" c="#c92a2a" style={{ minWidth: 60, textAlign: 'right' }}>
              ${order.totalPrice.toFixed(2)}
            </Text>
            {open ? (
              <IconChevronUp size={16} color="#c92a2a" />
            ) : (
              <IconChevronDown size={16} color="#c92a2a" />
            )}
          </Group>
        </Group>
      </UnstyledButton>

      <Collapse in={open}>
        <Divider color="#c92a2a" />
        <Stack gap={0} px="md" py="sm">
          <Group gap={4} hiddenFrom="sm" mb="xs">
            <IconClock size={13} color="#c92a2a" />
            <Text size="xs" c="#c92a2a">
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
                {i > 0 && <Divider color="#c92a2a" my={6} />}
                <Group justify="space-between" wrap="nowrap" py={4}>
                  <Stack gap={0} style={{ flex: 1, minWidth: 0 }}>
                    <Group gap="xs">
                      <Box
                        style={{
                          width: 6,
                          height: 6,
                          borderRadius: '50%',
                          background: '#c92a2a',
                          flexShrink: 0,
                        }}
                      />
                      <Text size="sm" lineClamp={1}>
                        {item.name}
                      </Text>
                      <Text size="xs" c="dimmed">
                        ${item.price.toFixed(2)} each
                      </Text>
                      <Badge size="xs" color="#c92a2a" variant="outline">
                        ×{item.quantity}
                      </Badge>
                    </Group>
                    {item.note && (
                      <Text size="xs" c="dimmed" ml={14} fs="italic">
                        {item.note}
                      </Text>
                    )}
                  </Stack>
                  <Text size="sm" fw={600} c="#c92a2a" style={{ flexShrink: 0, marginLeft: 12 }}>
                    ${(item.price * item.quantity).toFixed(2)}
                  </Text>
                </Group>
              </Box>
            ))
          )}

          <Divider color="#c92a2a" my="sm" />
          <Group justify="space-between">
            <Text size="sm" fw={700} c="#c92a2a">
              Total
            </Text>
            <Text size="md" fw={800} c="#c92a2a">
              ${order.totalPrice.toFixed(2)}
            </Text>
          </Group>
        </Stack>
      </Collapse>
    </Paper>
  );
};
