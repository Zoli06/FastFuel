import { useState } from 'react';
import {
  Badge,
  Box,
  Button,
  Center,
  Divider,
  Group,
  Loader,
  Select,
  SimpleGrid,
  Stack,
  Text,
  Paper,
  UnstyledButton,
  ScrollArea,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { notifications } from '@mantine/notifications';
import { Menu } from './Menu.tsx';
import { MenuDetailModal } from './MenuDetailModal.tsx';
import { apiClient } from '../../lib/api-client.ts';
import {
  IconBottleFilled,
  IconBurger,
  IconToolsKitchen3,
  IconLayoutGrid,
  IconSortAscendingLetters,
  IconSortDescendingLetters,
  IconArrowUp,
  IconArrowDown,
} from '@tabler/icons-react';

const useMenus = () => apiClient.useQuery('get', '/api/Menu');
const useFoods = () => apiClient.useQuery('get', '/api/Food');
const useRestaurants = () => apiClient.useQuery('get', '/api/Restaurant');

type ApiMenu = NonNullable<ReturnType<typeof useMenus>['data']>[number];
type ApiFood = NonNullable<ReturnType<typeof useFoods>['data']>[number];
type ApiRestaurant = NonNullable<ReturnType<typeof useRestaurants>['data']>[number];

type CartItem = {
  menuId: number;
  name: string;
  price: number;
  quantity: number;
  specialInstructions: string | null;
  type: 'food' | 'menu';
};

type UnifiedItem =
  | (ApiMenu & { type: 'menu' })
  | (ApiFood & { foods: { foodId: number; quantity: number }[]; type: 'food' });

const CATEGORIES = [
  { label: 'All', value: 'all', icons: [IconLayoutGrid] },
  { label: 'Foods', value: 'food', icons: [IconToolsKitchen3] },
  { label: 'Menus', value: 'menu', icons: [IconBurger, IconBottleFilled] },
];

type SortKey = 'name-asc' | 'name-desc' | 'price-asc' | 'price-desc';

export const MenuList = () => {
  const { data: menus, isLoading: menusLoading, isError: menusError } = useMenus();
  const { data: foods, isLoading: foodsLoading, isError: foodsError } = useFoods();
  const { data: restaurants } = useRestaurants();

  const [restaurantId, setRestaurantId] = useState<number | null>(null);
  const [cart, setCart] = useState<CartItem[]>([]);
  const [selectedMenu, setSelectedMenu] = useState<UnifiedItem | null>(null);
  const [opened, { open, close }] = useDisclosure(false);
  const [activeCategory, setActiveCategory] = useState('all');
  const [sortKey, setSortKey] = useState<SortKey>('name-asc');

  const { mutate: createOrder, isPending } = apiClient.useMutation('post', '/api/Order', {
    onSuccess: () => {
      setCart([]);
      notifications.show({
        title: 'Order placed!',
        message: 'Your order has been submitted.',
        color: 'green',
      });
    },
  });

  const restaurantOptions = (restaurants ?? []).map((r: ApiRestaurant) => ({
    value: String(r.id),
    label: r.name,
  }));

  const unifiedMenus: UnifiedItem[] = (menus ?? []).map((m: ApiMenu) => ({
    ...m,
    type: 'menu' as const,
  }));

  const unifiedFoods: UnifiedItem[] = (foods ?? []).map((f: ApiFood) => ({
    ...f,
    foods: [],
    type: 'food' as const,
  }));

  const allItems: UnifiedItem[] =
    activeCategory === 'all'
      ? [...unifiedMenus, ...unifiedFoods]
      : activeCategory === 'menu'
        ? unifiedMenus
        : unifiedFoods;

  const filteredAndSorted = [...allItems].sort((a, b) => {
    if (sortKey === 'name-asc') return a.name.localeCompare(b.name);
    if (sortKey === 'name-desc') return b.name.localeCompare(a.name);
    if (sortKey === 'price-asc') return a.price - b.price;
    if (sortKey === 'price-desc') return b.price - a.price;
    return 0;
  });

  const getQuantity = (id: number, type: 'food' | 'menu') =>
    cart.find((c) => c.menuId === id && c.type === type)?.quantity ?? 0;

  const addToCart = (item: UnifiedItem) => {
    setCart((prev) => {
      const existing = prev.find((c) => c.menuId === item.id && c.type === item.type);
      if (existing)
        return prev.map((c) =>
          c.menuId === item.id && c.type === item.type ? { ...c, quantity: c.quantity + 1 } : c,
        );
      return [
        ...prev,
        {
          menuId: item.id,
          name: item.name,
          price: item.price,
          quantity: 1,
          specialInstructions: null,
          type: item.type,
        },
      ];
    });
  };

  const addToCartFromModal = (
    menuId: number,
    quantity: number,
    specialInstructions: string | null,
    type: 'food' | 'menu',
  ) => {
    const source = type === 'menu' ? unifiedMenus : unifiedFoods;
    const item = source.find((m) => m.id === menuId);
    if (!item) return;
    setCart((prev) => {
      const existing = prev.find((c) => c.menuId === menuId && c.type === type);
      if (existing)
        return prev.map((c) =>
          c.menuId === menuId && c.type === type
            ? { ...c, quantity: c.quantity + quantity, specialInstructions }
            : c,
        );
      return [
        ...prev,
        { menuId, name: item.name, price: item.price, quantity, specialInstructions, type },
      ];
    });
  };

  const removeFromCart = (id: number, type: 'food' | 'menu') => {
    setCart((prev) => {
      const existing = prev.find((c) => c.menuId === id && c.type === type);
      if (!existing || existing.quantity <= 1)
        return prev.filter((c) => !(c.menuId === id && c.type === type));
      return prev.map((c) =>
        c.menuId === id && c.type === type ? { ...c, quantity: c.quantity - 1 } : c,
      );
    });
  };

  const totalPrice = cart.reduce((sum, c) => sum + c.price * c.quantity, 0);
  const totalItems = cart.reduce((sum, c) => sum + c.quantity, 0);

  const CART_HEADER = 44;
  const CART_ITEM_HEIGHT = 52;
  const CART_FOOTER = 90;
  const CART_PADDING = 32;
  const MAX_VISIBLE_ITEMS = 3;

  const cartScrollHeight = Math.min(cart.length, MAX_VISIBLE_ITEMS) * CART_ITEM_HEIGHT;
  const cartPanelHeight =
    cart.length > 0 ? CART_HEADER + cartScrollHeight + CART_FOOTER + CART_PADDING : 0;

  const handlePlaceOrder = () => {
    if (!restaurantId) {
      notifications.show({ title: 'Select a restaurant', message: '', color: 'orange' });
      return;
    }
    if (cart.length === 0) {
      notifications.show({ title: 'Your cart is empty', message: '', color: 'orange' });
      return;
    }
    createOrder({
      body: {
        restaurantId,
        menus: cart
          .filter((c) => c.type === 'menu')
          .map((c) => ({
            menuId: c.menuId,
            quantity: c.quantity,
            specialInstructions: c.specialInstructions,
          })),
        foods: cart
          .filter((c) => c.type === 'food')
          .map((c) => ({
            foodId: c.menuId,
            quantity: c.quantity,
            specialInstructions: c.specialInstructions,
          })),
      },
    });
  };

  const isLoading = menusLoading || foodsLoading;
  const isError = menusError || foodsError;

  if (isLoading)
    return (
      <Center h={200}>
        <Loader color="red" />
      </Center>
    );
  if (isError)
    return (
      <Center h={200}>
        <Text c="red">Failed to load items</Text>
      </Center>
    );

  return (
    <>
      <Group align="flex-start" gap={0} style={{ minHeight: '100vh' }}>
        {/* Sidebar — tablet and desktop only */}
        <Stack
          visibleFrom="sm"
          gap={4}
          p="sm"
          style={{
            width: 100,
            minHeight: '100vh',
            background: 'var(--mantine-color-dark-8, #1a1a1a)',
            borderRight: '2px solid var(--mantine-color-orange-6)',
            position: 'sticky',
            top: 0,
            flexShrink: 0,
          }}
        >
          {CATEGORIES.map((cat) => {
            const isActive = activeCategory === cat.value;
            return (
              <UnstyledButton
                key={cat.value}
                onClick={() => setActiveCategory(cat.value)}
                style={{
                  display: 'flex',
                  flexDirection: 'column',
                  alignItems: 'center',
                  gap: 4,
                  padding: '10px 6px',
                  borderRadius: 8,
                  background: isActive ? 'var(--mantine-color-orange-6)' : 'transparent',
                  color: isActive ? '#fff' : 'var(--mantine-color-gray-4)',
                  fontWeight: isActive ? 700 : 400,
                  fontSize: 11,
                  transition: 'all 0.15s',
                  textAlign: 'center',
                }}
              >
                <Group gap={2} justify="center">
                  {cat.icons.map((Icon, i) => (
                    <Icon key={i} size={18} />
                  ))}
                </Group>
                {cat.label}
              </UnstyledButton>
            );
          })}
        </Stack>

        {/* Main content */}
        <Stack
          gap="md"
          p="md"
          style={{ flex: 1 }}
          pb={cart.length > 0 ? cartPanelHeight + 24 : 'md'}
        >
          {/* Mobile category pill bar */}
          <Box hiddenFrom="sm">
            <Group
              gap={8}
              style={{
                overflowX: 'auto',
                flexWrap: 'nowrap',
                paddingBottom: 4,
                scrollbarWidth: 'none',
              }}
            >
              {CATEGORIES.map((cat) => {
                const isActive = activeCategory === cat.value;
                return (
                  <UnstyledButton
                    key={cat.value}
                    onClick={() => setActiveCategory(cat.value)}
                    style={{
                      display: 'flex',
                      alignItems: 'center',
                      gap: 6,
                      padding: '6px 14px',
                      borderRadius: 20,
                      background: isActive
                        ? 'var(--mantine-color-orange-6)'
                        : 'var(--mantine-color-dark-6)',
                      color: isActive ? '#fff' : 'var(--mantine-color-gray-4)',
                      fontWeight: isActive ? 700 : 400,
                      fontSize: 13,
                      whiteSpace: 'nowrap',
                      flexShrink: 0,
                      transition: 'all 0.15s',
                    }}
                  >
                    <Group gap={4}>
                      {cat.icons.map((Icon, i) => (
                        <Icon key={i} size={15} />
                      ))}
                    </Group>
                    {cat.label}
                  </UnstyledButton>
                );
              })}
            </Group>
          </Box>

          <Group justify="space-between" align="flex-end">
            <Select
              label="Restaurant"
              placeholder="Where are you ordering from?"
              data={restaurantOptions}
              value={restaurantId ? String(restaurantId) : null}
              onChange={(v) => setRestaurantId(v ? Number(v) : null)}
              searchable
              style={{ flex: 1, maxWidth: 320 }}
            />
            <Group gap={6}>
              {(
                [
                  {
                    keyAsc: 'name-asc',
                    keyDesc: 'name-desc',
                    iconAsc: IconSortAscendingLetters,
                    iconDesc: IconSortDescendingLetters,
                    label: 'Name',
                  },
                  {
                    keyAsc: 'price-asc',
                    keyDesc: 'price-desc',
                    iconAsc: IconArrowUp,
                    iconDesc: IconArrowDown,
                    label: 'Price',
                  },
                ] as {
                  keyAsc: SortKey;
                  keyDesc: SortKey;
                  iconAsc: typeof IconArrowUp;
                  iconDesc: typeof IconArrowUp;
                  label: string;
                }[]
              ).map((s) => {
                const isAsc = sortKey === s.keyAsc;
                const isDesc = sortKey === s.keyDesc;
                const isActive = isAsc || isDesc;
                const Icon = isDesc ? s.iconDesc : s.iconAsc;
                return (
                  <Button
                    key={s.label}
                    size="xs"
                    variant={isActive ? 'filled' : 'light'}
                    color="orange"
                    leftSection={<Icon size={14} />}
                    onClick={() => setSortKey(isAsc ? s.keyDesc : s.keyAsc)}
                  >
                    {s.label}
                  </Button>
                );
              })}
            </Group>
          </Group>

          <Text
            fw={700}
            size="xl"
            style={{
              color: 'var(--mantine-color-orange-6)',
              textTransform: 'uppercase',
              letterSpacing: 1,
            }}
          >
            {CATEGORIES.find((c) => c.value === activeCategory)?.label}
            <Text span size="sm" c="dimmed" fw={400} ml={8}>
              ({filteredAndSorted.length} items)
            </Text>
          </Text>

          <SimpleGrid cols={{ base: 1, sm: 3, md: 4, lg: 6 }} spacing={4}>
            {filteredAndSorted.map((item) => (
              <Menu
                key={`${item.type}-${item.id}`}
                {...item}
                quantity={getQuantity(item.id, item.type)}
                onAdd={() => addToCart(item)}
                onRemove={() => removeFromCart(item.id, item.type)}
                onOpen={() => {
                  setSelectedMenu(item);
                  open();
                }}
              />
            ))}
          </SimpleGrid>
        </Stack>
      </Group>

      {/* Cart panel */}
      {cart.length > 0 && (
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
            borderTop: '2px solid var(--mantine-color-orange-6)',
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
                <Box key={`${c.type}-${c.menuId}`}>
                  {idx > 0 && <Divider my={6} />}
                  <Group justify="space-between" wrap="nowrap" gap="xs">
                    <Stack gap={2} style={{ flex: 1, minWidth: 0 }}>
                      <Group gap={6} wrap="nowrap">
                        <Text size="sm" fw={600} lineClamp={1} style={{ flex: 1 }}>
                          {c.name}
                        </Text>
                        <Badge
                          size="xs"
                          color={c.type === 'menu' ? 'orange' : 'blue'}
                          variant="light"
                          style={{ flexShrink: 0 }}
                        >
                          {c.type}
                        </Badge>
                      </Group>
                      {c.specialInstructions && (
                        <Text size="xs" c="dimmed" fs="italic" lineClamp={1}>
                          {c.specialInstructions}
                        </Text>
                      )}
                    </Stack>
                    <Group gap={4} wrap="nowrap" style={{ flexShrink: 0 }}>
                      <Button
                        size="compact-xs"
                        variant="subtle"
                        color="gray"
                        onClick={() => removeFromCart(c.menuId, c.type)}
                      >
                        −
                      </Button>
                      <Badge color="orange" variant="light" size="sm">
                        ×{c.quantity}
                      </Badge>
                      <Button
                        size="compact-xs"
                        variant="subtle"
                        color="gray"
                        onClick={() => {
                          const source = c.type === 'menu' ? unifiedMenus : unifiedFoods;
                          const item = source.find((m) => m.id === c.menuId);
                          if (item) addToCart(item);
                        }}
                      >
                        +
                      </Button>
                      <Text
                        size="sm"
                        fw={700}
                        c="darkred"
                        style={{ minWidth: 60, textAlign: 'right' }}
                      >
                        ${(c.price * c.quantity).toFixed(2)}
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

          <Button fullWidth color="darkred" loading={isPending} onClick={handlePlaceOrder}>
            Place order
          </Button>
        </Paper>
      )}

      <MenuDetailModal
        opened={opened}
        onClose={close}
        menu={selectedMenu}
        onAddToCart={addToCartFromModal}
      />
    </>
  );
};
