import { useEffect, useState } from 'react';
import { Group, Stack } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { notifications } from '@mantine/notifications';
import { OrderDetailModal } from './modals/OrderDetailModal.tsx';
import { CategoryNav } from './sections/CategoryNav.tsx';
import { CartPanel } from './sections/CartPanel.tsx';
import { ItemGrid } from './sections/ItemGrid.tsx';
import { OrderCatalogHeader } from './sections/OrderCatalogHeader.tsx';
import { RestaurantPickerModal } from '../common/SearchablePickerModals/RestaurantPickerModal.tsx';
import { CheckoutModal } from './modals/CheckoutModal.tsx';
import { EditCartEntryModal } from './modals/EditCartEntryModal.tsx';
import type { CategoryValue } from './constants.ts';
import { useApi } from '../../lib/api.ts';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import type { CartEntry, CartItem, CheckoutStep, SortKey, UnifiedItem } from './types.ts';

export const OrderCreator = () => {
  const { useNoPerm, useNecessaryPerm } = useApi('OrderCreator');
  const noPermApi = useNoPerm();

  const { data: currentUser } = useSuspenseQuery(noPermApi.queryOptions('get', '/api/User/me'));
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
    isEmployeeUser ? noPermApi.queryOptions('get', '/api/Employee/me') : undefined,
    isMachineUser ? noPermApi.queryOptions('get', '/api/Machine/me') : undefined,
    useNecessaryPerm('Permission:Menu:Read').queryOptions('get', '/api/Menu'),
    useNecessaryPerm('Permission:Food:Read').queryOptions('get', '/api/Food'),
    useNecessaryPerm('Permission:Restaurant:Read').queryOptions('get', '/api/Restaurant'),
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
  const [activeCategory, setActiveCategory] = useState<CategoryValue>('all');
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

  const { mutateAsync: createOrder, isPending } = useNecessaryPerm(
    'Permission:Order:Create',
  ).useMutation('post', '/api/Order');

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

  const saveEditedEntry = (cartKey: string, specialInstructions: string | null) => {
    setCart((prev) => prev.map((c) => (c.cartKey === cartKey ? { ...c, specialInstructions } : c)));
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
        <CategoryNav activeCategory={activeCategory} onCategoryChange={setActiveCategory} />

        {/* Main content */}
        <Stack
          gap="md"
          p="md"
          style={{ flex: 1 }}
          pb={cart.length > 0 ? cartPanelHeight + 24 : 'md'}
        >
          <OrderCatalogHeader
            selectedRestaurant={selectedRestaurant}
            needsRestaurantPicker={needsRestaurantPicker}
            searchQuery={searchQuery}
            onSearchQueryChange={setSearchQuery}
            sortKey={sortKey}
            onSortKeyChange={setSortKey}
            onOpenRestaurantPicker={() => {
              setRestaurantSearch('');
              setRestaurantPickerOpen(true);
            }}
          />

          <ItemGrid
            activeCategory={activeCategory}
            items={filteredAndSorted}
            foods={foods}
            onAddItem={addItemToCart}
            onOpenItem={(item) => {
              setSelectedItem(item);
              open();
            }}
          />
        </Stack>
      </Group>

      <CartPanel
        cart={cart}
        totalItems={totalItems}
        totalPrice={totalPrice}
        cartScrollHeight={cartScrollHeight}
        isPending={isPending}
        onEditEntry={(entry) => setEditingEntry({ ...entry })}
        onRemoveOne={removeOneFromCart}
        onAddOne={addOneToCart}
        onPlaceOrder={handlePlaceOrder}
        onRemoveEntry={removeEntireEntry}
      />

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

      <RestaurantPickerModal
        opened={restaurantPickerOpen}
        restaurantId={restaurantId}
        searchValue={restaurantSearch}
        restaurants={filteredRestaurants}
        onSearchChange={setRestaurantSearch}
        onClose={() => {
          if (restaurantId !== null) setRestaurantPickerOpen(false);
        }}
        onSelectRestaurant={(newRestaurantId) => {
          setRestaurantId(newRestaurantId);
          setRestaurantPickerOpen(false);
        }}
      />

      <CheckoutModal
        checkoutStep={checkoutStep}
        selectedRestaurantName={selectedRestaurant?.name}
        cart={cart}
        totalPrice={totalPrice}
        placedOrderNumber={placedOrderNumber}
        isPending={isPending}
        onCloseCheckout={() => setCheckoutStep('idle')}
        onConfirmOrder={handleConfirmOrder}
        onBackToConfirm={() => setCheckoutStep('confirm')}
        onPay={handleFakePayment}
      />

      {/* Edit entry modal — opened from cart panel */}
      {editingEntry && (
        <EditCartEntryModal
          entry={editingEntry}
          onSave={(specialInstructions) =>
            saveEditedEntry(editingEntry.cartKey, specialInstructions)
          }
          onCancel={() => setEditingEntry(null)}
        />
      )}
    </>
  );
};
