import { Box, Button, Group, Stack, Text, Title } from '@mantine/core';
import { useEffect, useRef, useState } from 'react';
import type { components, paths } from '../../types/api';
import { apiClient } from '../../lib/api-client.ts';
import { Header } from '../Header/Header.tsx';
import { Footer } from '../Footer/Footer.tsx';
import { usePagePermissions } from '../../hooks/usePagePermissions.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';

type Order = components['schemas']['OrderResponseDto'];
type OrderStatus = Extract<components['schemas']['OrderStatus'], 'InProgress' | 'Ready'>;
type OrderQuery = NonNullable<paths['/api/Order']['get']['parameters']['query']>;

type OrderStatusDisplayProps = {
  restaurantId: number;
};

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

export const OrderStatusDisplay = ({ restaurantId }: OrderStatusDisplayProps) => {
  const { necessary, recommended } = usePagePermissions('OrderStatusDisplay', { split: true });
  const [isFullscreen, setIsFullscreen] = useState(Boolean(document.fullscreenElement));
  const [showExitButton, setShowExitButton] = useState(false);
  const hideExitButtonTimeoutRef = useRef<number | null>(null);

  const buildOrderQuery = (status: OrderStatus): OrderQuery => ({
    restaurantId,
    status,
  });

  const [{ data: restaurant }, { data: inProgressOrders = [] }, { data: readyOrders = [] }] =
    useConditionalSuspenseQueries([
      recommended.Restaurant.Read &&
        apiClient.queryOptions('get', '/api/Restaurant/{id}', {
          params: { path: { id: restaurantId } },
        }),
      necessary.Order.Read &&
        apiClient.queryOptions(
          'get',
          '/api/Order',
          {
            params: { query: buildOrderQuery('InProgress') },
          },
          {
            refetchInterval: 2500,
          },
        ),
      necessary.Order.Read &&
        apiClient.queryOptions(
          'get',
          '/api/Order',
          {
            params: { query: buildOrderQuery('Ready') },
          },
          {
            refetchInterval: 2500,
          },
        ),
    ]);

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

  const headerTitle =
    recommended.Restaurant.Read && restaurant ? `Orders - ${restaurant.name}` : 'Orders';

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
      {!isFullscreen && <Footer />}
    </>
  );
};
