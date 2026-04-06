import { Box, Button, Group, Stack, Text, Title } from '@mantine/core';
import { useEffect, useRef, useState } from 'react';
import { useSuspenseQuery } from '@tanstack/react-query';
import type { components, paths } from '../../types/api-schema.generated.ts';
import { useApi } from '../../lib/api.ts';
import { Header } from '../Header/Header.tsx';
import { Footer } from '../Footer/Footer.tsx';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { RestaurantPickerModal } from '../common/SearchablePickerModals/RestaurantPickerModal.tsx';

type Order = components['schemas']['OrderResponseDto'];
type OrderStatus = Extract<components['schemas']['OrderStatus'], 'InProgress' | 'Ready'>;
type OrderQuery = NonNullable<paths['/api/Order']['get']['parameters']['query']>;

const statusColor: Record<string, string> = {
  InProgress: '#5f5f5f',
  Ready: '#1b5e20',
};

const statusLabel: Record<string, string> = {
  InProgress: 'In Progress',
  Ready: 'Ready',
};

const OrderNumbers = ({ orders, status }: { orders: Order[]; status: 'InProgress' | 'Ready' }) => {
  return (
    <Group gap="xl" wrap="wrap">
      {orders.map((order) => (
        <Text key={order.id} fz={56} fw={900} c={statusColor[status]} style={{ lineHeight: 1.1 }}>
          {order.orderNumber}
        </Text>
      ))}
    </Group>
  );
};

export const OrderStatusDisplay = () => {
  const { noPerm, necessaryPerm, recommendedPerm } = useApi('OrderStatusDisplay');
  const { data: currentUser } = useSuspenseQuery(noPerm().queryOptions('get', '/api/User/me'));
  const currentUserType = currentUser.userType.toLowerCase();
  const isEmployeeUser = currentUserType === 'employee';
  const isMachineUser = currentUserType === 'machine';
  const isCustomer = currentUserType === 'customer';
  const isAdmin = currentUserType === 'admin';
  const needsRestaurantPicker = isCustomer || isAdmin;

  const [{ data: employeeProfile }, { data: machineProfile }] = useConditionalSuspenseQueries([
    isEmployeeUser ? noPerm().queryOptions('get', '/api/Employee/me') : undefined,
    isMachineUser ? noPerm().queryOptions('get', '/api/Machine/me') : undefined,
  ]);

  const lockedRestaurantId = isEmployeeUser
    ? (employeeProfile?.worksAtRestaurantId ?? null)
    : isMachineUser
      ? (machineProfile?.locatedAtRestaurantId ?? null)
      : null;

  const orderReadApi = necessaryPerm('Permission:Order:Read');
  const restaurantReadApi = recommendedPerm('Permission:Restaurant:Read');
  const [restaurantId, setRestaurantId] = useState<number | null>(null);
  const [isFullscreen, setIsFullscreen] = useState(Boolean(document.fullscreenElement));
  const [showExitButton, setShowExitButton] = useState(false);
  const [restaurantPickerOpen, setRestaurantPickerOpen] = useState(false);
  const [restaurantSearch, setRestaurantSearch] = useState('');
  const hideExitButtonTimeoutRef = useRef<number | null>(null);

  const buildOrderQuery = (selectedRestaurantId: number, status: OrderStatus): OrderQuery => ({
    restaurantId: selectedRestaurantId,
    status,
  });

  const [
    { data: restaurant },
    { data: inProgressOrders = [] },
    { data: readyOrders = [] },
    { data: restaurants = [] },
  ] = useConditionalSuspenseQueries([
    restaurantReadApi && restaurantId !== null
      ? restaurantReadApi.queryOptions('get', '/api/Restaurant/{id}', {
          params: { path: { id: restaurantId } },
        })
      : undefined,
    restaurantId !== null
      ? orderReadApi.queryOptions(
          'get',
          '/api/Order',
          {
            params: { query: buildOrderQuery(restaurantId, 'InProgress') },
          },
          {
            refetchInterval: 2500,
          },
        )
      : undefined,
    restaurantId !== null
      ? orderReadApi.queryOptions(
          'get',
          '/api/Order',
          {
            params: { query: buildOrderQuery(restaurantId, 'Ready') },
          },
          {
            refetchInterval: 2500,
          },
        )
      : undefined,
    restaurantReadApi?.queryOptions('get', '/api/Restaurant'),
  ]);

  const filteredRestaurants = restaurants.filter((r) =>
    r.name.toLowerCase().includes(restaurantSearch.toLowerCase()),
  );

  const canChangeRestaurant = Boolean(
    restaurantReadApi && restaurants.length > 0 && lockedRestaurantId === null,
  );

  const openRestaurantPicker = () => {
    setRestaurantSearch('');
    setRestaurantPickerOpen(true);
  };

  const handleSelectRestaurant = (newRestaurantId: number) => {
    setRestaurantId(newRestaurantId);
    setRestaurantPickerOpen(false);
    setRestaurantSearch('');
  };

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

  const toggleFullscreen = async () => {
    if (document.fullscreenElement) {
      await document.exitFullscreen();
      return;
    }

    await document.documentElement.requestFullscreen();
  };

  useEffect(() => {
    const handleFullscreenChange = () => {
      setIsFullscreen(Boolean(document.fullscreenElement));
    };

    document.addEventListener('fullscreenchange', handleFullscreenChange);
    return () => document.removeEventListener('fullscreenchange', handleFullscreenChange);
  }, []);

  useEffect(() => {
    if (!isFullscreen) {
      setShowExitButton(false);
      if (hideExitButtonTimeoutRef.current !== null) {
        window.clearTimeout(hideExitButtonTimeoutRef.current);
        hideExitButtonTimeoutRef.current = null;
      }
      return;
    }

    const revealExitButton = () => {
      setShowExitButton(true);

      if (hideExitButtonTimeoutRef.current !== null) {
        window.clearTimeout(hideExitButtonTimeoutRef.current);
      }

      hideExitButtonTimeoutRef.current = window.setTimeout(() => {
        setShowExitButton(false);
      }, 2500);
    };

    // Show once on fullscreen entry, then show again whenever the mouse moves.
    revealExitButton();
    window.addEventListener('mousemove', revealExitButton);

    return () => {
      window.removeEventListener('mousemove', revealExitButton);
      if (hideExitButtonTimeoutRef.current !== null) {
        window.clearTimeout(hideExitButtonTimeoutRef.current);
        hideExitButtonTimeoutRef.current = null;
      }
    };
  }, [isFullscreen]);

  useEffect(() => {
    const shouldHideCursor = isFullscreen && !showExitButton;
    document.documentElement.style.cursor = shouldHideCursor ? 'none' : '';

    return () => {
      document.documentElement.style.cursor = '';
    };
  }, [isFullscreen, showExitButton]);

  const headerTitle = restaurantReadApi && restaurant ? `Orders - ${restaurant.name}` : 'Orders';

  return (
    <>
      {!isFullscreen && <Header title={headerTitle} />}

      <Box
        p="xl"
        pb={isFullscreen ? 'xl' : 80}
        style={{ backgroundColor: '#ffffff', minHeight: '100vh' }}
      >
        {(!isFullscreen || showExitButton) && (
          <Group
            justify="flex-end"
            mb="md"
            style={isFullscreen ? { position: 'fixed', top: 16, right: 16, zIndex: 20 } : undefined}
          >
            {canChangeRestaurant && (
              <Button variant="light" onClick={openRestaurantPicker}>
                Change Restaurant
              </Button>
            )}
            <Button variant="light" onClick={() => void toggleFullscreen()}>
              {isFullscreen ? 'Exit Full Screen' : 'Full Screen'}
            </Button>
          </Group>
        )}

        <Stack gap="xl">
          {inProgressOrders.length > 0 && (
            <Box>
              <Title order={2} mb="md" c="#1f1f1f">
                {statusLabel.InProgress}
              </Title>
              <OrderNumbers orders={inProgressOrders} status="InProgress" />
            </Box>
          )}

          {readyOrders.length > 0 && (
            <Box>
              <Title order={2} mb="md" c="#0f3d16">
                {statusLabel.Ready}
              </Title>
              <OrderNumbers orders={readyOrders} status="Ready" />
            </Box>
          )}
        </Stack>
      </Box>
      <RestaurantPickerModal
        opened={restaurantPickerOpen}
        restaurantId={restaurantId}
        searchValue={restaurantSearch}
        restaurants={filteredRestaurants}
        onSearchChange={setRestaurantSearch}
        onClose={() => {
          if (restaurantId !== null || !needsRestaurantPicker) {
            setRestaurantPickerOpen(false);
          }
        }}
        onSelectRestaurant={handleSelectRestaurant}
        title="Switch restaurant"
        emptyMessage="No restaurants found"
      />
      {!isFullscreen && <Footer />}
    </>
  );
};
