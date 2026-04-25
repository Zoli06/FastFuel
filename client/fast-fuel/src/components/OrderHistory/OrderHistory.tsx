import { useState } from 'react';
import { Box, Group, Stack, Text } from '@mantine/core';
import { IconShoppingBag } from '@tabler/icons-react';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useApi } from '../../lib/api.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { OrderHistoryFilters } from './OrderHistoryFilter.tsx';
import { OrderHistoryList } from './OrderHistoryList.tsx';
import { resolveItems, sortOrders } from './helpers.ts';
import type { OrderStatus, OrderSummary, SortKey } from './types.ts';

export const OrderHistory = () => {
  const { useNoPerm } = useApi('OrderHistory');

  const [statusFilter, setStatusFilter] = useState<OrderStatus | undefined>(undefined);
  const [search, setSearch] = useState('');
  const [sortKey, setSortKey] = useState<SortKey>('date-desc');
  const { data: rawOrders = [] } = useSuspenseQuery(
    useNoPerm().queryOptions('get', '/api/Order/my'),
  );

  const [{ data: restaurants = [] }] = useConditionalSuspenseQueries([
    useNoPerm().queryOptions('get', '/api/Restaurant'),
  ]);

  const restaurantMap = new Map(restaurants.map((r) => [r.id, r]));

  const mapped: OrderSummary[] = rawOrders.map((o) => ({
    id: o.id,
    orderNumber: o.orderNumber,
    restaurantId: o.restaurantId,
    restaurantName: restaurantMap.get(o.restaurantId)?.name ?? `Restaurant #${o.restaurantId}`,
    status: o.status as OrderStatus,
    totalPrice:
      o.foods.reduce((sum, f) => sum + f.originalFoodPrice * f.quantity, 0) +
      o.menus.reduce((sum, m) => sum + m.originalMenuPrice * m.quantity, 0),
    placedAt: o.createdAt,
    items: resolveItems(o.foods ?? [], o.menus ?? []),
  }));

  const filtered = mapped
    .filter((o) => !statusFilter || o.status === statusFilter)
    .filter(
      (o) =>
        String(o.orderNumber).includes(search) ||
        o.restaurantName.toLowerCase().includes(search.toLowerCase()),
    );

  const sorted = sortOrders(filtered, sortKey);

  return (
    <Stack gap="md" p="md" maw={780} mx="auto">
      <Group align="center" gap="sm" mb={4}>
        <Box
          style={{
            width: 42,
            height: 42,
            borderRadius: 10,
            background: '#c92a2a',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}
        >
          <IconShoppingBag size={22} color="white" />
        </Box>
        <Stack gap={2}>
          <Text
            fw={800}
            size="xl"
            style={{
              color: '#c92a2a',
              textTransform: 'uppercase',
              letterSpacing: 1,
              lineHeight: 1,
            }}
          >
            Order History
          </Text>
          <Text size="xs" c="dimmed">
            {sorted.length} orders placed
          </Text>
        </Stack>
      </Group>

      <OrderHistoryFilters
        search={search}
        onSearchChange={setSearch}
        sortKey={sortKey}
        onSortChange={setSortKey}
        statusFilter={statusFilter}
        onStatusChange={setStatusFilter}
      />

      <OrderHistoryList orders={sorted} search={search} statusFilter={statusFilter} />
    </Stack>
  );
};
