import { ScrollArea, Stack, Text } from '@mantine/core';
import { IconReceipt2 } from '@tabler/icons-react';
import { OrderRowCollapse } from './OrderRowCollapse.tsx';
import type { OrderSummary, OrderStatus } from './types.ts';

interface OrderHistoryListProps {
  orders: OrderSummary[];
  search: string;
  statusFilter: OrderStatus | undefined;
}

export const OrderHistoryList = ({ orders, search, statusFilter }: OrderHistoryListProps) => (
  <ScrollArea>
    <Stack gap="sm">
      {orders.length === 0 ? (
        <Stack align="center" py={60} gap="xs">
          <IconReceipt2 size={48} color="var(--mantine-color-orange-9)" />
          <Text c="dimmed" size="sm">
            {search || statusFilter
              ? 'No orders match your filters'
              : "You haven't placed any orders yet"}
          </Text>
        </Stack>
      ) : (
        orders.map((order) => <OrderRowCollapse key={order.id} order={order} />)
      )}
    </Stack>
  </ScrollArea>
);
