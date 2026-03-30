import { useEffect, useState } from 'react';
import {
  Box,
  Button,
  Divider,
  Group,
  Modal,
  NumberInput,
  Paper,
  ScrollArea,
  SimpleGrid,
  Stack,
  Text,
  Textarea,
  TextInput,
  UnstyledButton,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { notifications } from '@mantine/notifications';
import { OrderItemCard } from './OrderItemCard.tsx';
import { OrderDetailModal } from './OrderDetailModal.tsx';
import { apiClient } from '../../lib/api-client.ts';
import {
  IconArrowsSort,
  IconArrowDown,
  IconArrowUp,
  IconBottleFilled,
  IconBurger,
  IconLayoutGrid,
  IconSearch,
  IconSortAscendingLetters,
  IconSortDescendingLetters,
  IconToolsKitchen3,
} from '@tabler/icons-react';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import type { CartItem, SortKey, UnifiedItem } from './types.ts';
import { myCurrentUserQueryOptions } from '../../lib/api-client.ts';

const CATEGORIES = [
  { label: 'All', value: 'all', icons: [IconLayoutGrid] },
  { label: 'Foods', value: 'food', icons: [IconToolsKitchen3] },
  { label: 'Menus', value: 'menu', icons: [IconBurger, IconBottleFilled] },
];

type CheckoutStep = 'idle' | 'confirm' | 'payment' | 'thankyou';
type CartEntry = CartItem & { cartKey: string };

export const OrderCreator = () => {
  const { data: currentUser } = useSuspenseQuery(myCurrentUserQueryOptions());
  const currentUserType = currentUser.userType.toLowerCase();
  const isEmployeeUser = currentUserType === 'employee';
  const isMachineUser = currentUserType === 'machine';
  const isCustomer = currentUserType === 'customer';
  const isAdmin = currentUserType === 'admin';
  const needsRestaurantPicker = isCustomer || isAdmin;

  const [
    { data: employeeProfile },
    { data: machineProfile },
    { data: menus = [] },
    { data: foods = [] },
    { data: restaurants = [] },
  ] = useConditionalSuspenseQueries([
    isEmployeeUser ? apiClient.queryOptions('get', '/api/Employee/me') : false,
    isMachineUser ? apiClient.queryOptions('get', '/api/Machine/me') : false,
    apiClient.queryOptions('get', '/api/Menu'),
    apiClient.queryOptions('get', '/api/Food'),
    apiClient.queryOptions('get', '/api/Restaurant'),
  ]);

  const lockedRestaurantId = isEmployeeUser
    ? (employeeProfile?.worksAtRestaurantId ?? null)
    : isMachineUser
      ? (machineProfile?.locatedAtRestaurantId ?? null)
      : null;

  const [restaurantId, setRestaurantId] = useState<number | null>(null);
  const [restaurantPickerOpen, setRestaurantPickerOpen] = useState(false);
  const [restaurantSearch, setRestaurantSearch] = useState('');

  const [cart, setCart] = useState<CartEntry[]>([]);
  const [selectedItem, setSelectedItem] = useState<UnifiedItem | null>(null);
  const [opened, { open, close }] = useDisclosure(false);
  const [activeCategory, setActiveCategory] = useState('all');
  const [sortKey, setSortKey] = useState<SortKey>('name-asc');
  const [searchQuery, setSearchQuery] = useState('');
  const [checkoutStep, setCheckoutStep] = useState<CheckoutStep>('idle');
  const [placedOrderNumber, setPlacedOrderNumber] = useState<number | null>(null);
  const [editingEntry, setEditingEntry] = useState<CartEntry | null>(null);

  useEffect(() => {
    if (needsRestaurantPicker && restaurantId === null) {
      setRestaurantPickerOpen(true);
    }
  }, [needsRestaurantPicker, restaurantId]);

  useEffect(() => {
    if (lockedRestaurantId !== null) {
      setRestaurantId((prev) => (prev === lockedRestaurantId ? prev : lockedRestaurantId));
    }
  }, [lockedRestaurantId]);

  const { mutateAsync: createOrder, isPending } = apiClient.useMutation('post', '/api/Order');

  const selectedRestaurant = restaurants.find((r) => r.id === restaurantId);
  const filteredRestaurants = restaurants.filter((r) =>
    r.name.toLowerCase().includes(restaurantSearch.toLowerCase()),
  );

  const unifiedMenus: UnifiedItem[] = menus.map((m) => ({ ...m, type: 'menu' as const }));
  const unifiedFoods: UnifiedItem[] = foods.map((f) => ({ ...f, type: 'food' as const }));

  const allItems = [...unifiedMenus, ...unifiedFoods].filter((item) => {
    if (activeCategory !== 'all' && item.type !== activeCategory) return false;
    if (searchQuery.trim()) return item.name.toLowerCase().includes(searchQuery.toLowerCase());
    return true;
  });

  const filteredAndSorted = [...allItems].sort((a, b) => {
    if (sortKey === 'name-asc') return a.name.localeCompare(b.name);
    if (sortKey === 'name-desc') return b.name.localeCompare(a.name);
    if (sortKey === 'price-asc') return a.price - b.price;
    if (sortKey === 'price-desc') return b.price - a.price;
    return 0;
  });

  const addItemToCart = (item: CartItem) => {
    const cartKey = `${item.item.type}-${item.item.id}-${Date.now()}-${Math.random()}`;
    setCart((prev) => [...prev, { ...item, cartKey }]);
  };

  const removeOneFromCart = (cartKey: string) => {
    setCart((prev) => {
      const entry = prev.find((c) => c.cartKey === cartKey);
      if (!entry) return prev;
      if (entry.quantity === 1) return prev.filter((c) => c.cartKey !== cartKey);
      return prev.map((c) => (c.cartKey === cartKey ? { ...c, quantity: c.quantity - 1 } : c));
    });
  };

  const addOneToCart = (cartKey: string) => {
    setCart((prev) =>
      prev.map((c) => (c.cartKey === cartKey ? { ...c, quantity: c.quantity + 1 } : c)),
    );
  };

  const removeEntireEntry = (cartKey: string) => {
    setCart((prev) => prev.filter((c) => c.cartKey !== cartKey));
  };

  const saveEditedEntry = (
    cartKey: string,
    quantity: number,
    specialInstructions: string | null,
  ) => {
    setCart((prev) =>
      prev.map((c) => (c.cartKey === cartKey ? { ...c, quantity, specialInstructions } : c)),
    );
    setEditingEntry(null);
  };

  const totalPrice = cart.reduce((sum, c) => sum + c.item.price * c.quantity, 0);
  const totalItems = cart.reduce((sum, c) => sum + c.quantity, 0);

  const CART_HEADER = 44;
  const CART_ITEM_HEIGHT = 64;
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
    setCheckoutStep('confirm');
  };

  const handleConfirmOrder = () => setCheckoutStep('payment');

  const handleFakePayment = async () => {
    const created = await createOrder({
      body: {
        restaurantId: restaurantId!,
        menus: cart
          .filter((c) => c.item.type === 'menu')
          .map((c) => ({
            menuId: c.item.id,
            quantity: c.quantity,
            specialInstructions: c.specialInstructions,
          })),
        foods: cart
          .filter((c) => c.item.type === 'food')
          .map((c) => ({
            foodId: c.item.id,
            quantity: c.quantity,
            specialInstructions: c.specialInstructions,
          })),
      },
    });
    setCart([]);
    setPlacedOrderNumber(created.orderNumber);
    setCheckoutStep('thankyou');
  };

  return (
    <>
      <Group align="flex-start" gap={0} style={{ minHeight: '100vh' }}>
        {/* Sidebar */}
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

          <Group justify="space-between" align="flex-end" wrap="wrap" gap="sm">
            {/* Restaurant indicator */}
            <Group gap="xs" align="center">
              {selectedRestaurant ? (
                <>
                  <Text size="sm" c="dimmed">
                    Ordering from:
                  </Text>
                  <Text fw={700} size="sm" c="orange">
                    {selectedRestaurant.name}
                  </Text>
                  {needsRestaurantPicker && (
                    <Button
                      size="compact-xs"
                      variant="subtle"
                      color="orange"
                      onClick={() => {
                        setRestaurantSearch('');
                        setRestaurantPickerOpen(true);
                      }}
                    >
                      Change
                    </Button>
                  )}
                </>
              ) : (
                needsRestaurantPicker && (
                  <Button
                    size="xs"
                    variant="light"
                    color="orange"
                    onClick={() => {
                      setRestaurantSearch('');
                      setRestaurantPickerOpen(true);
                    }}
                  >
                    Select a restaurant
                  </Button>
                )
              )}
            </Group>

            <Group gap="sm" wrap="wrap">
              {/* Search bar */}
              <TextInput
                placeholder="Search food & menus..."
                leftSection={<IconSearch size={18} />}
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.currentTarget.value)}
                size="md"
                style={{ width: 260 }}
              />
              {/* Sort buttons — size="sm" */}
              {[
                {
                  keyAsc: 'name-asc' as SortKey,
                  keyDesc: 'name-desc' as SortKey,
                  iconAsc: IconSortAscendingLetters,
                  iconDesc: IconSortDescendingLetters,
                  label: 'Name',
                },
                {
                  keyAsc: 'price-asc' as SortKey,
                  keyDesc: 'price-desc' as SortKey,
                  iconAsc: IconArrowUp,
                  iconDesc: IconArrowDown,
                  label: 'Price',
                },
              ].map((s) => {
                const isAsc = sortKey === s.keyAsc;
                const isDesc = sortKey === s.keyDesc;
                const isActive = isAsc || isDesc;
                const Icon = isActive ? (isDesc ? s.iconDesc : s.iconAsc) : IconArrowsSort;
                return (
                  <Button
                    key={s.label}
                    size="sm"
                    variant={isActive ? 'filled' : 'light'}
                    color="orange"
                    leftSection={<Icon size={15} />}
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
              <OrderItemCard
                key={`${item.type}-${item.id}`}
                item={item}
                allFoods={foods}
                onAdd={() => addItemToCart({ item, quantity: 1, specialInstructions: null })}
                onRemove={() => {}}
                onOpen={() => {
                  setSelectedItem(item);
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
                    <Group gap={12} wrap="nowrap" style={{ flexShrink: 0 }} align="center">
                      <Button
                        size="sm"
                        variant="outline"
                        color="gray"
                        w={38}
                        h={38}
                        p={0}
                        style={{ fontSize: 20, fontWeight: 700, borderRadius: 8, flexShrink: 0 }}
                        onClick={() => removeOneFromCart(c.cartKey)}
                      >
                        −
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
                        onClick={() => addOneToCart(c.cartKey)}
                      >
                        +
                      </Button>
                      <Text
                        size="sm"
                        fw={700}
                        c="darkred"
                        style={{ minWidth: 60, textAlign: 'right' }}
                      >
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
          <Button fullWidth color="darkred" loading={isPending} onClick={handlePlaceOrder}>
            Place order
          </Button>
        </Paper>
      )}

      {/* Item detail modal */}
      {selectedItem && (
        <OrderDetailModal
          item={selectedItem}
          allFoods={foods}
          opened={opened}
          onClose={() => {
            setSelectedItem(null);
            close();
          }}
          onAddToCart={addItemToCart}
        />
      )}

      {/* Restaurant picker modal */}
      <Modal
        opened={restaurantPickerOpen}
        onClose={() => {
          if (restaurantId !== null) setRestaurantPickerOpen(false);
        }}
        title="Where are you ordering from?"
        centered
        size="md"
        closeOnClickOutside={restaurantId !== null}
        closeOnEscape={restaurantId !== null}
        withCloseButton={restaurantId !== null}
      >
        <Stack gap="sm">
          <TextInput
            placeholder="Search restaurants..."
            leftSection={<IconSearch size={15} />}
            value={restaurantSearch}
            onChange={(e) => setRestaurantSearch(e.currentTarget.value)}
            autoFocus
          />
          <ScrollArea h={320}>
            <Stack gap={4}>
              {filteredRestaurants.length === 0 && (
                <Text c="dimmed" size="sm" ta="center" py="md">
                  No restaurants found
                </Text>
              )}
              {filteredRestaurants.map((r) => (
                <UnstyledButton
                  key={r.id}
                  onClick={() => {
                    setRestaurantId(r.id);
                    setRestaurantPickerOpen(false);
                  }}
                  style={{
                    padding: '10px 14px',
                    borderRadius: 8,
                    background:
                      restaurantId === r.id
                        ? 'var(--mantine-color-orange-5)'
                        : 'var(--mantine-color-blue-9)',
                    border:
                      restaurantId === r.id
                        ? '2px solid var(--mantine-color-blue-4)'
                        : '2px solid var(--mantine-color-blue-8)',
                    transition: 'all 0.12s',
                  }}
                >
                  <Text
                    fw={restaurantId === r.id ? 700 : 500}
                    size="sm"
                    c={restaurantId === r.id ? 'blue.2' : 'blue.1'}
                  >
                    {r.name}
                  </Text>
                </UnstyledButton>
              ))}
            </Stack>
          </ScrollArea>
        </Stack>
      </Modal>

      {/* Confirmation modal */}
      <Modal
        opened={checkoutStep === 'confirm'}
        onClose={() => setCheckoutStep('idle')}
        title="Confirm your order"
        centered
        size="sm"
      >
        <Stack gap="sm">
          <Text size="sm" c="dimmed">
            Ordering from:{' '}
            <Text span fw={700} c="orange">
              {selectedRestaurant?.name}
            </Text>
          </Text>
          <Divider />
          <Stack gap={6}>
            {cart.map((c) => (
              <Group key={c.cartKey} justify="space-between" align="flex-start" wrap="nowrap">
                <Stack gap={2} style={{ flex: 1, minWidth: 0 }}>
                  <Text size="sm">
                    {c.item.name}{' '}
                    <Text span c="dimmed">
                      ×{c.quantity}
                    </Text>
                  </Text>
                  {c.specialInstructions && (
                    <Text size="xs" c="dimmed" fs="italic">
                      {c.specialInstructions}
                    </Text>
                  )}
                </Stack>
                <Group gap={6} align="center" style={{ flexShrink: 0 }}>
                  <Text size="sm" fw={600} c="darkred">
                    ${(c.item.price * c.quantity).toFixed(2)}
                  </Text>
                  <Button
                    size="compact-xs"
                    variant="subtle"
                    color="orange"
                    onClick={() => {
                      setEditingEntry({ ...c });
                      setCheckoutStep('idle');
                    }}
                  >
                    Edit
                  </Button>
                  <Button
                    size="compact-xs"
                    variant="subtle"
                    color="red"
                    onClick={() => removeEntireEntry(c.cartKey)}
                  >
                    Remove
                  </Button>
                </Group>
              </Group>
            ))}
          </Stack>
          <Divider />
          <Group justify="space-between">
            <Text fw={700}>Total</Text>
            <Text fw={800} c="darkred" size="lg">
              ${totalPrice.toFixed(2)}
            </Text>
          </Group>
          <Group justify="flex-end" gap="sm" mt="xs">
            <Button variant="subtle" color="gray" onClick={() => setCheckoutStep('idle')}>
              Back
            </Button>
            <Button color="darkred" onClick={handleConfirmOrder} disabled={cart.length === 0}>
              Confirm & Pay
            </Button>
          </Group>
        </Stack>
      </Modal>

      {/* Edit entry modal — opened from confirm */}
      {editingEntry && (
        <EditCartEntryModal
          entry={editingEntry}
          onSave={(qty, instr) => {
            saveEditedEntry(editingEntry.cartKey, qty, instr);
            setCheckoutStep('confirm');
          }}
          onCancel={() => {
            setEditingEntry(null);
            setCheckoutStep('confirm');
          }}
        />
      )}

      {/* Fake payment modal */}
      <Modal
        opened={checkoutStep === 'payment'}
        onClose={() => setCheckoutStep('idle')}
        title="Payment"
        centered
        size="sm"
        closeOnClickOutside={false}
        closeOnEscape={false}
        withCloseButton={false}
      >
        <Stack gap="md">
          <Text size="sm" c="dimmed">
            Total to pay:
          </Text>
          <Text fw={800} size="xl" c="darkred" ta="center">
            ${totalPrice.toFixed(2)}
          </Text>
          <Divider label="Mock card details" labelPosition="center" />
          <TextInput label="Card number" disabled value="4242 4242 4242 4242" />
          <Group grow>
            <TextInput label="Expiry" disabled value="12/28" />
            <TextInput label="CVV" disabled value="123" />
          </Group>
          <Button fullWidth color="darkred" loading={isPending} onClick={handleFakePayment}>
            Pay ${totalPrice.toFixed(2)}
          </Button>
          <Button variant="subtle" color="gray" onClick={() => setCheckoutStep('confirm')}>
            Back
          </Button>
        </Stack>
      </Modal>

      {/* Thank you modal */}
      <Modal
        opened={checkoutStep === 'thankyou'}
        onClose={() => setCheckoutStep('idle')}
        title="Order placed!"
        centered
        size="sm"
        withCloseButton={false}
        closeOnClickOutside={false}
        closeOnEscape={false}
      >
        <Stack gap="sm" align="center">
          <Text size="xl">🎉</Text>
          <Text size="sm" c="dimmed" ta="center">
            Your order has been submitted successfully.
          </Text>
          <Text fw={700} size="xl" c="orange">
            Order #{placedOrderNumber}
          </Text>
          <Text size="xs" c="dimmed" ta="center">
            Sit back and relax — your food is on its way!
          </Text>
          <Button fullWidth color="darkred" mt="sm" onClick={() => setCheckoutStep('idle')}>
            OK
          </Button>
        </Stack>
      </Modal>
    </>
  );
};

// ─── Edit cart entry modal ────────────────────────────────────────────────────

type EditCartEntryModalProps = {
  entry: CartEntry;
  onSave: (quantity: number, specialInstructions: string | null) => void;
  onCancel: () => void;
};

const EditCartEntryModal = ({ entry, onSave, onCancel }: EditCartEntryModalProps) => {
  const [quantity, setQuantity] = useState(entry.quantity);
  const [instructions, setInstructions] = useState(entry.specialInstructions ?? '');

  return (
    <Modal opened onClose={onCancel} title={`Edit — ${entry.item.name}`} centered size="sm">
      <Stack gap="md">
        <Stack gap={4}>
          <Text size="sm" fw={500}>
            Quantity
          </Text>
          <Group justify="space-between" align="center" px="md">
            <Button
              variant="outline"
              color="darkred"
              size="lg"
              w={56}
              h={56}
              p={0}
              style={{ fontSize: 28, fontWeight: 700, borderRadius: 12 }}
              onClick={() => setQuantity((q) => Math.max(1, q - 1))}
              disabled={quantity <= 1}
            >
              −
            </Button>
            <NumberInput
              value={quantity}
              onChange={(v) => setQuantity(Math.max(1, Number(v) || 1))}
              min={1}
              style={{ width: 80 }}
              styles={{ input: { textAlign: 'center', fontWeight: 800, fontSize: 20 } }}
              hideControls
            />
            <Button
              variant="filled"
              color="darkred"
              size="lg"
              w={56}
              h={56}
              p={0}
              style={{ fontSize: 28, fontWeight: 700, borderRadius: 12 }}
              onClick={() => setQuantity((q) => q + 1)}
            >
              +
            </Button>
          </Group>
        </Stack>

        <Textarea
          label="Special instructions"
          placeholder="e.g. no onions, extra sauce..."
          value={instructions}
          onChange={(e) => setInstructions(e.currentTarget.value)}
          minRows={2}
          autosize
        />

        <Divider />

        <Group justify="space-between" align="center">
          <Text fw={700} c="darkred" size="lg">
            ${(entry.item.price * quantity).toFixed(2)}
          </Text>
          <Group gap="sm">
            <Button variant="subtle" color="gray" onClick={onCancel}>
              Cancel
            </Button>
            <Button color="darkred" onClick={() => onSave(quantity, instructions || null)}>
              Save
            </Button>
          </Group>
        </Group>
      </Stack>
    </Modal>
  );
};
