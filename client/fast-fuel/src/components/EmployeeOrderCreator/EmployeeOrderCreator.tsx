import {
  ActionIcon,
  Badge,
  Button,
  Card,
  Collapse,
  Divider,
  Grid,
  Group,
  ScrollArea,
  Stack,
  Text,
  TextInput,
  ThemeIcon,
  Title,
  Tooltip,
  UnstyledButton,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { useMemo, useRef, useState } from 'react';
import type { components } from '../../types/api';
import { apiClient } from '../../lib/api-client.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { Paper } from '../common/Paper/Paper.tsx';

type OrderMenuLine = components['schemas']['OrderMenuDto'];
type OrderFoodLine = components['schemas']['OrderFoodDto'];
type CatalogMode = 'food' | 'menu';
type CartLineType = 'food' | 'menu';

type CartLine = {
  id: string;
  type: CartLineType;
  itemId: number;
  quantity: number;
  specialInstructions: string;
};

const formatPrice = (value: number) =>
  new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);

const InlineStepper = ({
  value,
  onChange,
  min = 1,
}: {
  value: number;
  onChange: (v: number) => void;
  min?: number;
}) => (
  <Group gap={4} wrap="nowrap">
    <ActionIcon
      size="sm"
      variant="default"
      radius="xl"
      onClick={(e) => {
        e.stopPropagation();
        onChange(Math.max(min, value - 1));
      }}
      style={{ fontWeight: 700 }}
    >
      −
    </ActionIcon>
    <Text size="sm" fw={700} w={20} ta="center" style={{ fontVariantNumeric: 'tabular-nums' }}>
      {value}
    </Text>
    <ActionIcon
      size="sm"
      variant="default"
      radius="xl"
      onClick={(e) => {
        e.stopPropagation();
        onChange(value + 1);
      }}
      style={{ fontWeight: 700 }}
    >
      +
    </ActionIcon>
  </Group>
);

const CartLineCard = ({
  line,
  name,
  unitPrice,
  onRemove,
  onQuantityChange,
  onInstructionsChange,
}: {
  line: CartLine;
  name: string;
  unitPrice: number;
  onRemove: () => void;
  onQuantityChange: (q: number) => void;
  onInstructionsChange: (v: string) => void;
}) => {
  const [open, { toggle }] = useDisclosure(!!line.specialInstructions);
  const lineTotal = unitPrice * line.quantity;

  return (
    <Card
      withBorder
      radius="md"
      p="sm"
      style={{
        borderColor: 'var(--mantine-color-default-border)',
        transition: 'border-color 120ms',
      }}
    >
      <Stack gap={6}>
        {/* Row 1: name + remove */}
        <Group justify="space-between" align="flex-start" gap="xs" wrap="nowrap">
          <Group gap="xs" wrap="nowrap" style={{ flex: 1, minWidth: 0 }}>
            <Badge
              size="xs"
              variant="dot"
              color={line.type === 'food' ? 'orange' : 'violet'}
              style={{ flexShrink: 0 }}
            >
              {line.type === 'food' ? 'Food' : 'Menu'}
            </Badge>
            <Text fw={600} size="sm" truncate>
              {name}
            </Text>
          </Group>
          <Tooltip label="Remove" withArrow position="top">
            <ActionIcon
              size="xs"
              variant="subtle"
              color="red"
              onClick={onRemove}
              style={{ flexShrink: 0 }}
            >
              ✕
            </ActionIcon>
          </Tooltip>
        </Group>

        {/* Row 2: qty stepper + line total */}
        <Group justify="space-between" align="center">
          <Group gap={6} align="center">
            <InlineStepper value={line.quantity} onChange={onQuantityChange} />
            <Text size="xs" c="dimmed" style={{ fontVariantNumeric: 'tabular-nums' }}>
              × {formatPrice(unitPrice)}
            </Text>
          </Group>
          <Text fw={700} size="sm" style={{ fontVariantNumeric: 'tabular-nums' }}>
            {formatPrice(lineTotal)}
          </Text>
        </Group>

        {/* Row 3: collapsible special request */}
        <UnstyledButton onClick={toggle} style={{ display: 'flex', alignItems: 'center', gap: 4 }}>
          <Text size="xs" c="dimmed" style={{ userSelect: 'none' }}>
            {open ? '▾' : '▸'}{' '}
            {line.specialInstructions ? (
              <Text span size="xs" c="orange" fw={500}>
                Special request
              </Text>
            ) : (
              <Text span size="xs" c="dimmed">
                Add special request
              </Text>
            )}
          </Text>
        </UnstyledButton>
        <Collapse in={open}>
          <TextInput
            size="xs"
            placeholder="e.g. no onions, extra sauce…"
            value={line.specialInstructions}
            onChange={(e) => onInstructionsChange(e.currentTarget.value)}
            onClick={(e) => e.stopPropagation()}
          />
        </Collapse>
      </Stack>
    </Card>
  );
};

const CatalogItemCard = ({
  name,
  description,
  price,
  type,
  onAdd,
}: {
  name: string;
  description?: string | null;
  price: number;
  type: CatalogMode;
  onAdd: (qty: number) => void;
}) => {
  const [qty, setQty] = useState(1);

  return (
    <Card
      withBorder
      radius="md"
      p="sm"
      style={{
        transition: 'box-shadow 120ms, border-color 120ms',
        cursor: 'default',
      }}
      onMouseEnter={(e) => {
        (e.currentTarget as HTMLDivElement).style.boxShadow = '0 2px 12px rgba(0,0,0,.08)';
        (e.currentTarget as HTMLDivElement).style.borderColor = 'var(--mantine-color-dimmed)';
      }}
      onMouseLeave={(e) => {
        (e.currentTarget as HTMLDivElement).style.boxShadow = '';
        (e.currentTarget as HTMLDivElement).style.borderColor = '';
      }}
    >
      <Group justify="space-between" align="flex-start" wrap="nowrap">
        {/* Left: name + description */}
        <Stack gap={2} style={{ flex: 1, minWidth: 0 }}>
          <Text fw={600} size="sm" truncate>
            {name}
          </Text>
          {description && (
            <Text size="xs" c="dimmed" lineClamp={1}>
              {description}
            </Text>
          )}
        </Stack>

        {/* Right: price + stepper + add */}
        <Group gap="xs" wrap="nowrap" style={{ flexShrink: 0 }}>
          <Text
            fw={700}
            size="sm"
            c={type === 'food' ? 'orange' : 'violet'}
            style={{ fontVariantNumeric: 'tabular-nums', minWidth: 64, textAlign: 'right' }}
          >
            {formatPrice(price)}
          </Text>
          <InlineStepper value={qty} onChange={setQty} />
          <Button
            size="compact-sm"
            variant="filled"
            color={type === 'food' ? 'orange' : 'violet'}
            onClick={() => {
              onAdd(qty);
              setQty(1);
            }}
            style={{ minWidth: 52 }}
          >
            Add
          </Button>
        </Group>
      </Group>
    </Card>
  );
};

export const EmployeeOrderCreator = () => {
  const [catalogMode, setCatalogMode] = useState<CatalogMode>('food');
  const [search, setSearch] = useState('');
  const [cartLines, setCartLines] = useState<CartLine[]>([]);
  const [lastOrderNumber, setLastOrderNumber] = useState<number | null>(null);
  const nextLineId = useRef(1);

  const [{ data: foods = [] }, { data: menus = [] }] = useConditionalSuspenseQueries([
    apiClient.queryOptions('get', '/api/Food'),
    apiClient.queryOptions('get', '/api/Menu'),
  ]);

  const { mutateAsync: createOrder, isPending: isSubmitting } = apiClient.useMutation(
    'post',
    '/api/Order/at-workplace',
  );

  const foodPriceById = useMemo(() => new Map(foods.map((f) => [f.id, f.price])), [foods]);
  const menuPriceById = useMemo(() => new Map(menus.map((m) => [m.id, m.price])), [menus]);
  const foodNameById = useMemo(() => new Map(foods.map((f) => [f.id, f.name])), [foods]);
  const menuNameById = useMemo(() => new Map(menus.map((m) => [m.id, m.name])), [menus]);

  const addLine = (type: CartLineType, itemId: number, quantity: number) => {
    const id = `line-${nextLineId.current++}`;
    setCartLines((cur) => [
      ...cur,
      { id, type, itemId, quantity: Math.max(1, Math.floor(quantity)), specialInstructions: '' },
    ]);
    setLastOrderNumber(null);
  };

  const updateQty = (lineId: string, quantity: number) =>
    setCartLines((cur) =>
      cur.map((l) => (l.id === lineId ? { ...l, quantity: Math.max(1, quantity) } : l)),
    );

  const updateInstructions = (lineId: string, value: string) =>
    setCartLines((cur) =>
      cur.map((l) => (l.id === lineId ? { ...l, specialInstructions: value } : l)),
    );

  const removeLine = (lineId: string) => setCartLines((cur) => cur.filter((l) => l.id !== lineId));

  const linePrice = (line: CartLine) =>
    ((line.type === 'food' ? foodPriceById.get(line.itemId) : menuPriceById.get(line.itemId)) ??
      0) * line.quantity;

  const total = cartLines.reduce((s, l) => s + linePrice(l), 0);
  const foodTotal = cartLines
    .filter((l) => l.type === 'food')
    .reduce((s, l) => s + linePrice(l), 0);
  const menuTotal = cartLines
    .filter((l) => l.type === 'menu')
    .reduce((s, l) => s + linePrice(l), 0);

  const q = search.toLowerCase();
  const filteredFoods = foods.filter((f) => f.name.toLowerCase().includes(q));
  const filteredMenus = menus.filter((m) => m.name.toLowerCase().includes(q));

  const handleSubmit = async () => {
    if (!cartLines.length) return;

    const foodsPayload: OrderFoodLine[] = cartLines
      .filter((l) => l.type === 'food')
      .map((l) => ({
        foodId: l.itemId,
        quantity: l.quantity,
        specialInstructions: l.specialInstructions.trim() || null,
      }));

    const menusPayload: OrderMenuLine[] = cartLines
      .filter((l) => l.type === 'menu')
      .map((l) => ({
        menuId: l.itemId,
        quantity: l.quantity,
        specialInstructions: l.specialInstructions.trim() || null,
      }));

    try {
      const created = await createOrder({
        body: { foods: foodsPayload, menus: menusPayload },
      });
      setLastOrderNumber(created.orderNumber);
      setCartLines([]);
    } catch {
      /* handled by API middleware */
    }
  };

  const catalogItems = catalogMode === 'food' ? filteredFoods : filteredMenus;

  return (
    <Paper>
      <Stack gap="md">
        <Group justify="space-between" align="center">
          <Stack gap={0}>
            <Title order={3} style={{ letterSpacing: '-0.02em' }}>
              New Order
            </Title>
          </Stack>
          {lastOrderNumber !== null && (
            <Badge color="green" variant="light" size="lg" radius="sm">
              ✓ Order #{lastOrderNumber} placed
            </Badge>
          )}
        </Group>

        <Grid gutter="md" align="stretch">
          <Grid.Col span={{ base: 12, lg: 7 }}>
            <Card withBorder radius="md" p="md" h="100%">
              <Stack gap="md" h="100%">
                {/* Tab switcher */}
                <Group gap="xs">
                  {(['food', 'menu'] as const).map((mode) => {
                    const count = mode === 'food' ? foods.length : menus.length;
                    const active = catalogMode === mode;
                    return (
                      <UnstyledButton
                        key={mode}
                        onClick={() => {
                          setCatalogMode(mode);
                          setSearch('');
                        }}
                        style={{
                          padding: '6px 14px',
                          borderRadius: 8,
                          background: active
                            ? mode === 'food'
                              ? 'var(--mantine-color-orange-1)'
                              : 'var(--mantine-color-violet-1)'
                            : 'transparent',
                          border: `1.5px solid ${
                            active
                              ? mode === 'food'
                                ? 'var(--mantine-color-orange-4)'
                                : 'var(--mantine-color-violet-4)'
                              : 'var(--mantine-color-default-border)'
                          }`,
                          transition: 'all 120ms',
                        }}
                      >
                        <Group gap={6}>
                          <Text
                            size="sm"
                            fw={active ? 700 : 400}
                            c={active ? (mode === 'food' ? 'orange' : 'violet') : 'dimmed'}
                          >
                            {mode === 'food' ? '🍽 Foods' : '📋 Menus'}
                          </Text>
                          <Badge
                            size="xs"
                            variant="filled"
                            color={active ? (mode === 'food' ? 'orange' : 'violet') : 'gray'}
                          >
                            {count}
                          </Badge>
                        </Group>
                      </UnstyledButton>
                    );
                  })}

                  <TextInput
                    placeholder={`Search ${catalogMode}s…`}
                    value={search}
                    onChange={(e) => setSearch(e.currentTarget.value)}
                    size="xs"
                    radius="xl"
                    leftSection={<Text size="xs">🔍</Text>}
                    rightSection={
                      search ? (
                        <ActionIcon size="xs" variant="subtle" onClick={() => setSearch('')}>
                          ✕
                        </ActionIcon>
                      ) : null
                    }
                    style={{ flex: 1 }}
                  />
                </Group>

                {/* Item list */}
                <ScrollArea flex={1} h={430}>
                  <Stack gap="xs" pr={4}>
                    {catalogItems.length === 0 ? (
                      <Stack align="center" py="xl" gap="xs">
                        <ThemeIcon size="xl" variant="light" color="gray" radius="xl">
                          <Text size="lg">{search ? '🔍' : '🫙'}</Text>
                        </ThemeIcon>
                        <Text size="sm" c="dimmed">
                          {search
                            ? `No ${catalogMode}s match "${search}"`
                            : `No ${catalogMode}s available`}
                        </Text>
                      </Stack>
                    ) : (
                      catalogItems.map((item) => (
                        <CatalogItemCard
                          key={`${catalogMode}-${item.id}`}
                          name={item.name}
                          description={item.description}
                          price={item.price}
                          type={catalogMode}
                          onAdd={(qty) => addLine(catalogMode, item.id, qty)}
                        />
                      ))
                    )}
                  </Stack>
                </ScrollArea>
              </Stack>
            </Card>
          </Grid.Col>

          <Grid.Col span={{ base: 12, lg: 5 }}>
            <Card withBorder radius="md" p="md" h="100%">
              <Stack gap="sm" h="100%" style={{ display: 'flex', flexDirection: 'column' }}>
                <Group justify="space-between" align="center">
                  <Title order={5}>Cart</Title>
                  {cartLines.length > 0 && (
                    <Badge variant="filled" color="dark" size="sm" radius="xl">
                      {cartLines.length} {cartLines.length === 1 ? 'item' : 'items'}
                    </Badge>
                  )}
                </Group>

                {/* Cart lines */}
                <div style={{ flex: 1 }}>
                  {cartLines.length === 0 ? (
                    <Stack align="center" py="xl" gap="xs">
                      <Text size="2rem">🛒</Text>
                      <Text size="sm" c="dimmed">
                        Cart is empty
                      </Text>
                      <Text size="xs" c="dimmed">
                        Pick items from the catalog
                      </Text>
                    </Stack>
                  ) : (
                    <ScrollArea h={340}>
                      <Stack gap="xs" pr={4}>
                        {cartLines.map((line) => {
                          const name =
                            line.type === 'food'
                              ? (foodNameById.get(line.itemId) ?? `#${line.itemId}`)
                              : (menuNameById.get(line.itemId) ?? `#${line.itemId}`);
                          const unitPrice =
                            line.type === 'food'
                              ? (foodPriceById.get(line.itemId) ?? 0)
                              : (menuPriceById.get(line.itemId) ?? 0);

                          return (
                            <CartLineCard
                              key={line.id}
                              line={line}
                              name={name}
                              unitPrice={unitPrice}
                              onRemove={() => removeLine(line.id)}
                              onQuantityChange={(q) => updateQty(line.id, q)}
                              onInstructionsChange={(v) => updateInstructions(line.id, v)}
                            />
                          );
                        })}
                      </Stack>
                    </ScrollArea>
                  )}
                </div>

                {/* Summary + submit */}
                <Stack gap="xs" style={{ marginTop: 'auto' }}>
                  <Divider />

                  {/* Price breakdown */}
                  <Stack gap={4}>
                    {foodTotal > 0 && (
                      <Group justify="space-between">
                        <Text size="xs" c="dimmed">
                          🍽 Foods
                        </Text>
                        <Text size="xs" style={{ fontVariantNumeric: 'tabular-nums' }}>
                          {formatPrice(foodTotal)}
                        </Text>
                      </Group>
                    )}
                    {menuTotal > 0 && (
                      <Group justify="space-between">
                        <Text size="xs" c="dimmed">
                          📋 Menus
                        </Text>
                        <Text size="xs" style={{ fontVariantNumeric: 'tabular-nums' }}>
                          {formatPrice(menuTotal)}
                        </Text>
                      </Group>
                    )}
                    <Group justify="space-between" align="baseline">
                      <Text fw={700}>Total</Text>
                      <Text fw={800} size="lg" style={{ fontVariantNumeric: 'tabular-nums' }}>
                        {formatPrice(total)}
                      </Text>
                    </Group>
                  </Stack>

                  <Button
                    size="md"
                    fullWidth
                    radius="md"
                    onClick={() => void handleSubmit()}
                    disabled={cartLines.length === 0}
                    loading={isSubmitting}
                    color="dark"
                  >
                    {isSubmitting ? 'Placing order…' : `Place order · ${formatPrice(total)}`}
                  </Button>
                </Stack>
              </Stack>
            </Card>
          </Grid.Col>
        </Grid>
      </Stack>
    </Paper>
  );
};
