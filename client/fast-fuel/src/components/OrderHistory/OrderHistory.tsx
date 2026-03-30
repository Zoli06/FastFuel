import {
  Badge,
  Box,
  Button,
  Collapse,
  Divider,
  Group,
  Paper,
  ScrollArea,
  Select,
  Stack,
  Text,
  TextInput,
  UnstyledButton,
} from '@mantine/core';
import {
  IconArrowDown,
  IconArrowUp,
  IconChevronDown,
  IconChevronUp,
  IconClock,
  IconReceipt2,
  IconRefresh,
  IconSearch,
  IconShoppingBag,
  IconSortAscendingLetters,
  IconSortDescendingLetters,
} from '@tabler/icons-react';
import { useSuspenseQuery, useQueries } from '@tanstack/react-query';
import { useState } from 'react';
import { apiClient } from '../../lib/api-client.ts';
import { ReorderModal, type ReorderItem } from './ReOrderModal.tsx';

// ── Types ─────────────────────────────────────────────────────────────────────

type OrderStatus = 'Pending' | 'InProgress' | 'Ready' | 'Completed' | 'Cancelled';
type SortKey = 'date-desc' | 'date-asc' | 'price-desc' | 'price-asc' | 'name-asc' | 'name-desc';

interface OrderSummary {
  id: number;
  orderNumber: number;
  restaurantId: number;
  restaurantName: string;
  status: OrderStatus;
  totalPrice: number;
  placedAt: string;
  items: {
    name: string;
    quantity: number;
    price: number;
    note?: string;
    type: 'food' | 'menu';
    itemId: number;
  }[];
}

type RawOrderFood = { foodId: number; quantity: number; specialInstructions?: string };
type RawOrderMenu = { menuId: number; quantity: number; specialInstructions?: string };

type RawOrder = {
  id: number;
  orderNumber: number;
  restaurantId: number;
  status: OrderStatus;
  price: number;
  createdAt: string;
  completedAt?: string | null;
  menus: RawOrderMenu[];
  foods: RawOrderFood[];
};

type RawFood = { id: number; name: string; price: number };
type RawMenu = { id: number; name: string; price: number };
type RawRestaurant = { id: number; name: string };

// ── Helpers ───────────────────────────────────────────────────────────────────

const STATUS_META: Record<OrderStatus, { label: string; color: string }> = {
  Pending: { label: 'Pending', color: 'orange' },
  InProgress: { label: 'In Progress', color: 'orange' },
  Ready: { label: 'Ready', color: 'teal' },
  Completed: { label: 'Completed', color: 'green' },
  Cancelled: { label: 'Cancelled', color: 'red' },
};

const ALL_STATUSES = Object.keys(STATUS_META) as OrderStatus[];

const SORT_OPTIONS: { value: SortKey; label: string }[] = [
  { value: 'date-desc', label: 'Newest first' },
  { value: 'date-asc', label: 'Oldest first' },
  { value: 'price-desc', label: 'Price: high→low' },
  { value: 'price-asc', label: 'Price: low→high' },
  { value: 'name-asc', label: 'Restaurant A→Z' },
  { value: 'name-desc', label: 'Restaurant Z→A' },
];

function formatDate(iso: string) {
  const d = new Date(iso);
  return d.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

function resolveItems(
  order: RawOrder,
  foodMap: Map<number, RawFood>,
  menuMap: Map<number, RawMenu>,
): OrderSummary['items'] {
  const items: OrderSummary['items'] = [];

  for (const f of order.foods ?? []) {
    const found = foodMap.get(f.foodId);
    items.push({
      itemId: f.foodId,
      type: 'food',
      name: found?.name ?? `Food #${f.foodId}`,
      quantity: f.quantity,
      price: found?.price ?? 0,
      note: f.specialInstructions || undefined,
    });
  }

  for (const m of order.menus ?? []) {
    const found = menuMap.get(m.menuId);
    items.push({
      itemId: m.menuId,
      type: 'menu',
      name: found?.name ? `${found.name} (Menu)` : `Menu #${m.menuId}`,
      quantity: m.quantity,
      price: found?.price ?? 0,
      note: m.specialInstructions || undefined,
    });
  }

  return items;
}

function sortOrders(orders: OrderSummary[], key: SortKey): OrderSummary[] {
  return [...orders].sort((a, b) => {
    switch (key) {
      case 'date-desc':
        return new Date(b.placedAt).getTime() - new Date(a.placedAt).getTime();
      case 'date-asc':
        return new Date(a.placedAt).getTime() - new Date(b.placedAt).getTime();
      case 'price-desc':
        return b.totalPrice - a.totalPrice;
      case 'price-asc':
        return a.totalPrice - b.totalPrice;
      case 'name-asc':
        return a.restaurantName.localeCompare(b.restaurantName);
      case 'name-desc':
        return b.restaurantName.localeCompare(a.restaurantName);
      default:
        return 0;
    }
  });
}

// ── Order Row ─────────────────────────────────────────────────────────────────

interface OrderRowProps {
  order: OrderSummary;
  onReorder: (order: OrderSummary) => void;
}

function OrderRow({ order, onReorder }: OrderRowProps) {
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

          <Button
            mt="sm"
            variant="light"
            color="orange"
            leftSection={<IconRefresh size={16} />}
            onClick={(e) => {
              e.stopPropagation();
              onReorder(order);
            }}
            fullWidth
          >
            Reorder
          </Button>
        </Stack>
      </Collapse>
    </Paper>
  );
}

// ── Main component ────────────────────────────────────────────────────────────

export const OrderHistory = () => {
  const [statusFilter, setStatusFilter] = useState<OrderStatus | undefined>(undefined);
  const [search, setSearch] = useState('');
  const [sortKey, setSortKey] = useState<SortKey>('date-desc');
  const [reorderTarget, setReorderTarget] = useState<OrderSummary | null>(null);

  // Fetch ALL orders once — no status param sent to API.
  // Status filtering is done client-side below so the buttons actually work.
  const { data: orders } = useSuspenseQuery(
    apiClient.queryOptions('get', '/api/Order/my' as '/api/Order'),
  );

  const rawOrders = (orders as unknown as RawOrder[]) ?? [];

  // ── Collect unique IDs ─────────────────────────────────────────────────────

  const uniqueRestaurantIds = [...new Set(rawOrders.map((o) => o.restaurantId))];
  const uniqueFoodIds = [
    ...new Set(rawOrders.flatMap((o) => (o.foods ?? []).map((f) => f.foodId))),
  ];
  const uniqueMenuIds = [
    ...new Set(rawOrders.flatMap((o) => (o.menus ?? []).map((m) => m.menuId))),
  ];

  // ── Parallel queries ───────────────────────────────────────────────────────

  const restaurantResults = useQueries({
    queries: uniqueRestaurantIds.map((rid) =>
      apiClient.queryOptions('get', '/api/Restaurant/{id}', { params: { path: { id: rid } } }),
    ),
  });

  const foodResults = useQueries({
    queries: uniqueFoodIds.map((fid) =>
      apiClient.queryOptions('get', '/api/Food/{id}', { params: { path: { id: fid } } }),
    ),
  });

  const menuResults = useQueries({
    queries: uniqueMenuIds.map((mid) =>
      apiClient.queryOptions('get', '/api/Menu/{id}', { params: { path: { id: mid } } }),
    ),
  });

  // ── Build lookup maps ──────────────────────────────────────────────────────

  const restaurantMap = new Map<number, RawRestaurant>();
  uniqueRestaurantIds.forEach((rid, i) => {
    const data = restaurantResults[i]?.data;
    if (data) restaurantMap.set(rid, data as unknown as RawRestaurant);
  });

  const foodMap = new Map<number, RawFood>();
  uniqueFoodIds.forEach((fid, i) => {
    const data = foodResults[i]?.data;
    if (data) foodMap.set(fid, data as unknown as RawFood);
  });

  const menuMap = new Map<number, RawMenu>();
  uniqueMenuIds.forEach((mid, i) => {
    const data = menuResults[i]?.data;
    if (data) menuMap.set(mid, data as unknown as RawMenu);
  });

  // ── Map → status filter → search filter → sort ────────────────────────────

  const mapped = rawOrders.map<OrderSummary>((o) => {
    const restaurant = restaurantMap.get(o.restaurantId);
    return {
      id: o.id,
      orderNumber: o.orderNumber,
      restaurantId: o.restaurantId,
      restaurantName: restaurant?.name ?? `Restaurant #${o.restaurantId}`,
      status: o.status,
      totalPrice: o.price,
      placedAt: o.createdAt,
      items: resolveItems(o, foodMap, menuMap),
    };
  });

  const statusFiltered = statusFilter ? mapped.filter((o) => o.status === statusFilter) : mapped;

  const searchFiltered = statusFiltered.filter(
    (o) =>
      String(o.orderNumber).includes(search) ||
      o.restaurantName.toLowerCase().includes(search.toLowerCase()),
  );

  const sorted = sortOrders(searchFiltered, sortKey);

  // ── Reorder items ──────────────────────────────────────────────────────────

  const reorderItems: ReorderItem[] = (reorderTarget?.items ?? []).map((item) => ({
    id: item.itemId,
    type: item.type,
    name: item.name,
    price: item.price,
    quantity: item.quantity,
    specialInstructions: item.note,
  }));

  return (
    <Stack gap="md" p="md" maw={780} mx="auto">
      <Group align="center" gap="sm" mb={4}>
        <Box
          style={{
            width: 42,
            height: 42,
            borderRadius: 10,
            background: 'var(--mantine-color-orange-6)',
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
              color: 'var(--mantine-color-orange-5)',
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

      {/* Search + Sort */}
      <Group gap="sm" wrap="wrap">
        <TextInput
          placeholder="Search by order # or restaurant..."
          leftSection={<IconSearch size={16} color="var(--mantine-color-orange-5)" />}
          value={search}
          onChange={(e) => setSearch(e.currentTarget.value)}
          size="md"
          style={{ flex: 1, minWidth: 200 }}
          styles={{ input: { borderColor: 'var(--mantine-color-orange-8)' } }}
        />
        <Select
          data={SORT_OPTIONS}
          value={sortKey}
          onChange={(v) => v && setSortKey(v as SortKey)}
          size="md"
          style={{ width: 190 }}
          styles={{ input: { borderColor: 'var(--mantine-color-orange-8)' } }}
          leftSection={
            sortKey.includes('asc') ? (
              sortKey.startsWith('name') ? (
                <IconSortAscendingLetters size={16} color="var(--mantine-color-orange-5)" />
              ) : (
                <IconArrowUp size={16} color="var(--mantine-color-orange-5)" />
              )
            ) : sortKey.startsWith('name') ? (
              <IconSortDescendingLetters size={16} color="var(--mantine-color-orange-5)" />
            ) : (
              <IconArrowDown size={16} color="var(--mantine-color-orange-5)" />
            )
          }
        />
      </Group>

      {/* Status filter pills */}
      <Group gap={6} wrap="wrap">
        <Button
          size="compact-sm"
          variant={statusFilter === undefined ? 'filled' : 'light'}
          color="orange"
          onClick={() => setStatusFilter(undefined)}
        >
          All
        </Button>
        {ALL_STATUSES.map((s) => (
          <Button
            key={s}
            size="compact-sm"
            variant={statusFilter === s ? 'filled' : 'light'}
            color={STATUS_META[s].color}
            onClick={() => setStatusFilter((prev) => (prev === s ? undefined : s))}
          >
            {STATUS_META[s].label}
          </Button>
        ))}
      </Group>

      <ScrollArea>
        <Stack gap="sm">
          {sorted.length === 0 ? (
            <Stack align="center" py={60} gap="xs">
              <IconReceipt2 size={48} color="var(--mantine-color-orange-9)" />
              <Text c="dimmed" size="sm">
                {search || statusFilter
                  ? 'No orders match your filters'
                  : "You haven't placed any orders yet"}
              </Text>
            </Stack>
          ) : (
            sorted.map((order) => (
              <OrderRow key={order.id} order={order} onReorder={(o) => setReorderTarget(o)} />
            ))
          )}
        </Stack>
      </ScrollArea>

      {reorderTarget && (
        <ReorderModal
          opened
          onClose={() => setReorderTarget(null)}
          orderNumber={reorderTarget.orderNumber}
          restaurantId={reorderTarget.restaurantId}
          restaurantName={reorderTarget.restaurantName}
          items={reorderItems}
        />
      )}
    </Stack>
  );
};
