import {
  Badge,
  Box,
  Button,
  Card,
  Divider,
  Group,
  ScrollArea,
  SegmentedControl,
  SimpleGrid,
  Stack,
  Text,
  Title,
} from '@mantine/core';
import { useSuspenseQueries } from '@tanstack/react-query';
import { useState } from 'react';
import { useMediaQuery } from '@mantine/hooks';
import type { components } from '../../types/api';
import { apiClient } from '../../lib/api-client.ts';
import { Header } from '../Header/Header.tsx';
import { Footer } from '../Footer/Footer.tsx';
import { usePagePermissions } from '../../hooks/usePagePermissions.ts';

type StationTask = components['schemas']['StationTasksResponseDto'];
type StationTaskOrder = components['schemas']['StationTaskOrder'];
type OrderStatus = components['schemas']['OrderStatus'];

export type StationTasksProps = {
  stationId: number;
};

const nextStatus: Partial<Record<OrderStatus, OrderStatus>> = {
  Pending: 'InProgress',
  InProgress: 'Ready',
  Ready: 'Completed',
};

const statusColor: Record<string, string> = {
  Pending: 'yellow',
  InProgress: 'blue',
  Ready: 'cyan',
};

const statusLabel: Record<string, string> = {
  Pending: 'Pending',
  InProgress: 'In Progress',
  Ready: 'Ready',
};

const nextStatusLabel: Record<string, string> = {
  Pending: 'Start',
  InProgress: 'Ready',
  Ready: 'Complete',
};

const OrderCard = ({
  order,
  onAdvance,
  canAdvanceStatus,
}: {
  order: StationTaskOrder;
  onAdvance: (id: number, status: OrderStatus) => void;
  canAdvanceStatus: boolean;
}) => {
  const next = nextStatus[order.status];

  return (
    <Card
      withBorder
      shadow="sm"
      radius="md"
      padding="md"
      bg={order.status === 'Pending' ? 'white' : '#fffbe6'}
    >
      <Group justify="space-between" mb="xs">
        <Group gap="xs">
          <Title order={4}>#{order.orderNumber}</Title>
          <Badge color={statusColor[order.status]} variant="filled">
            {statusLabel[order.status]}
          </Badge>
        </Group>
        <Text size="xs" c="dimmed">
          {new Date(order.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
        </Text>
      </Group>

      <Divider mb="xs" />

      <Stack gap="xs">
        {order.menus.length > 0 && (
          <Box>
            <Text size="xs" fw={700} tt="uppercase" c="dimmed" mb={4}>
              Menus
            </Text>
            <Stack gap={4}>
              {order.menus.map((menu) => (
                <Box key={menu.id}>
                  <Group justify="space-between">
                    <Text size="sm" fw={500}>
                      {menu.quantity}× {menu.name}
                    </Text>
                  </Group>
                  {menu.specialInstructions && (
                    <Text size="xs" c="orange.7" fs="italic" ml="sm">
                      {menu.specialInstructions}
                    </Text>
                  )}
                  {menu.foods.length > 0 && (
                    <Stack gap={2} ml="sm" mt={2}>
                      {menu.foods.map((food) => (
                        <Box key={food.id}>
                          <Text size="xs" c="dimmed">
                            {food.quantity}× {food.name}
                          </Text>
                          {food.specialInstructions && (
                            <Text size="xs" c="orange.7" fs="italic" ml="sm">
                              {food.specialInstructions}
                            </Text>
                          )}
                          {food.ingredients.filter((i) => i.isRelevant).length > 0 && (
                            <Text size="xs" c="grape.6" ml="sm">
                              {food.ingredients
                                .filter((i) => i.isRelevant)
                                .map((i) => `${i.quantity}${i.unit} ${i.name}`)
                                .join(', ')}
                            </Text>
                          )}
                        </Box>
                      ))}
                    </Stack>
                  )}
                </Box>
              ))}
            </Stack>
          </Box>
        )}

        {order.foods.length > 0 && (
          <Box>
            <Text size="xs" fw={700} tt="uppercase" c="dimmed" mb={4}>
              Foods
            </Text>
            <Stack gap={4}>
              {order.foods.map((food) => (
                <Box key={food.id}>
                  <Group justify="space-between">
                    <Text size="sm" fw={500}>
                      {food.quantity}× {food.name}
                    </Text>
                  </Group>
                  {food.specialInstructions && (
                    <Text size="xs" c="orange.7" fs="italic" ml="sm">
                      {food.specialInstructions}
                    </Text>
                  )}
                  {food.ingredients.filter((i) => i.isRelevant).length > 0 && (
                    <Text size="xs" c="grape.6" ml="sm">
                      {food.ingredients
                        .filter((i) => i.isRelevant)
                        .map((i) => `${i.quantity}${i.unit} ${i.name}`)
                        .join(', ')}
                    </Text>
                  )}
                </Box>
              ))}
            </Stack>
          </Box>
        )}
      </Stack>

      {next && canAdvanceStatus && (
        <>
          <Divider mt="sm" mb="xs" />
          <Button
            fullWidth
            size="sm"
            color={next === 'Completed' ? 'green' : 'blue'}
            variant={next === 'Completed' ? 'filled' : 'light'}
            onClick={() => onAdvance(order.id, next)}
          >
            {nextStatusLabel[order.status]}
          </Button>
        </>
      )}
    </Card>
  );
};

type Category = 'Pending' | 'InProgress' | 'Ready';

export const StationTasks = ({ stationId }: StationTasksProps) => {
  const { recommended } = usePagePermissions('StationTasks', { split: true });
  const isWideScreen = useMediaQuery('(min-width: 1400px)');
  const [selectedCategory, setSelectedCategory] = useState<Category>('Pending');

  const [{ data: tasks, refetch: refetchTasks }, { data: station }] = useSuspenseQueries({
    queries: [
      apiClient.queryOptions(
        'get',
        '/api/Station/{id}/tasks',
        {
          params: { path: { id: stationId } },
        },
        {
          refetchInterval: 2500,
        },
      ),
      apiClient.queryOptions('get', '/api/Station/{id}', {
        params: { path: { id: stationId } },
      }),
    ] as const,
  });

  const { mutate: updateStatus } = apiClient.useMutation('put', '/api/Order/{id}/status', {
    onSuccess: refetchTasks,
  });

  const handleAdvance = (id: number, status: OrderStatus) => {
    updateStatus({ params: { path: { id } }, body: status });
  };

  const tasksArray: StationTask[] = Array.isArray(tasks) ? tasks : [tasks];
  const allOrders = tasksArray.flatMap((t) => t.orders);
  const pendingOrders = allOrders.filter((o) => o.status === 'Pending');
  const inProgressOrders = allOrders.filter((o) => o.status === 'InProgress');
  const readyOrders = allOrders.filter((o) => o.status === 'Ready');

  if (allOrders.length === 0) {
    return (
      <>
        <Header title={`Tasks: ${station.name}`} />
        <Box p="xl" pb={80}>
          <Text ta="center" c="dimmed" fz="xl">
            No pending tasks
          </Text>
        </Box>
        <Footer />
      </>
    );
  }

  return (
    <>
      <Header title={`Tasks: ${station.name}`} />
      <Box p="md" pb={80}>
        {!isWideScreen && (
          <Box mb="md">
            <SegmentedControl
              fullWidth
              value={selectedCategory}
              onChange={(value) => setSelectedCategory(value as Category)}
              data={[
                { label: `Pending (${pendingOrders.length})`, value: 'Pending' },
                { label: `In Progress (${inProgressOrders.length})`, value: 'InProgress' },
                { label: `Ready (${readyOrders.length})`, value: 'Ready' },
              ]}
            />
          </Box>
        )}

        {isWideScreen ? (
          <SimpleGrid cols={3} spacing="lg">
            <Box>
              <Group mb="sm" gap="xs">
                <Title order={3}>Pending</Title>
                <Badge color="yellow" variant="filled" size="lg">
                  {pendingOrders.length}
                </Badge>
              </Group>
              <ScrollArea>
                <Stack gap="sm">
                  {pendingOrders.length === 0 ? (
                    <Text c="dimmed" ta="center" py="md">
                      No pending orders
                    </Text>
                  ) : (
                    pendingOrders.map((order) => (
                      <OrderCard
                        key={order.id}
                        order={order}
                        onAdvance={handleAdvance}
                        canAdvanceStatus={recommended.Order.UpdateStatus}
                      />
                    ))
                  )}
                </Stack>
              </ScrollArea>
            </Box>

            <Box>
              <Group mb="sm" gap="xs">
                <Title order={3}>In Progress</Title>
                <Badge color="blue" variant="filled" size="lg">
                  {inProgressOrders.length}
                </Badge>
              </Group>
              <ScrollArea>
                <Stack gap="sm">
                  {inProgressOrders.length === 0 ? (
                    <Text c="dimmed" ta="center" py="md">
                      No orders in progress
                    </Text>
                  ) : (
                    inProgressOrders.map((order) => (
                      <OrderCard
                        key={order.id}
                        order={order}
                        onAdvance={handleAdvance}
                        canAdvanceStatus={recommended.Order.UpdateStatus}
                      />
                    ))
                  )}
                </Stack>
              </ScrollArea>
            </Box>

            <Box>
              <Group mb="sm" gap="xs">
                <Title order={3}>Ready</Title>
                <Badge color="cyan" variant="filled" size="lg">
                  {readyOrders.length}
                </Badge>
              </Group>
              <ScrollArea>
                <Stack gap="sm">
                  {readyOrders.length === 0 ? (
                    <Text c="dimmed" ta="center" py="md">
                      No ready orders
                    </Text>
                  ) : (
                    readyOrders.map((order) => (
                      <OrderCard
                        key={order.id}
                        order={order}
                        onAdvance={handleAdvance}
                        canAdvanceStatus={recommended.Order.UpdateStatus}
                      />
                    ))
                  )}
                </Stack>
              </ScrollArea>
            </Box>
          </SimpleGrid>
        ) : (
          <Box>
            {selectedCategory === 'Pending' && (
              <>
                <Group mb="sm" gap="xs">
                  <Title order={3}>Pending</Title>
                  <Badge color="yellow" variant="filled" size="lg">
                    {pendingOrders.length}
                  </Badge>
                </Group>
                <ScrollArea>
                  <Stack gap="sm">
                    {pendingOrders.length === 0 ? (
                      <Text c="dimmed" ta="center" py="md">
                        No pending orders
                      </Text>
                    ) : (
                      pendingOrders.map((order) => (
                        <OrderCard
                          key={order.id}
                          order={order}
                          onAdvance={handleAdvance}
                          canAdvanceStatus={recommended.Order.UpdateStatus}
                        />
                      ))
                    )}
                  </Stack>
                </ScrollArea>
              </>
            )}

            {selectedCategory === 'InProgress' && (
              <>
                <Group mb="sm" gap="xs">
                  <Title order={3}>In Progress</Title>
                  <Badge color="blue" variant="filled" size="lg">
                    {inProgressOrders.length}
                  </Badge>
                </Group>
                <ScrollArea>
                  <Stack gap="sm">
                    {inProgressOrders.length === 0 ? (
                      <Text c="dimmed" ta="center" py="md">
                        No orders in progress
                      </Text>
                    ) : (
                      inProgressOrders.map((order) => (
                        <OrderCard
                          key={order.id}
                          order={order}
                          onAdvance={handleAdvance}
                          canAdvanceStatus={recommended.Order.UpdateStatus}
                        />
                      ))
                    )}
                  </Stack>
                </ScrollArea>
              </>
            )}

            {selectedCategory === 'Ready' && (
              <>
                <Group mb="sm" gap="xs">
                  <Title order={3}>Ready</Title>
                  <Badge color="cyan" variant="filled" size="lg">
                    {readyOrders.length}
                  </Badge>
                </Group>
                <ScrollArea>
                  <Stack gap="sm">
                    {readyOrders.length === 0 ? (
                      <Text c="dimmed" ta="center" py="md">
                        No ready orders
                      </Text>
                    ) : (
                      readyOrders.map((order) => (
                        <OrderCard
                          key={order.id}
                          order={order}
                          onAdvance={handleAdvance}
                          canAdvanceStatus={recommended.Order.UpdateStatus}
                        />
                      ))
                    )}
                  </Stack>
                </ScrollArea>
              </>
            )}
          </Box>
        )}
      </Box>
      <Footer />
    </>
  );
};
