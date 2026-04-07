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
import { useState } from 'react';
import { useMediaQuery } from '@mantine/hooks';
import type { components } from '../../types/api-schema.generated.ts';
import { useApi } from '../../lib/api.ts';
import { Header } from '../Header/Header.tsx';
import { Footer } from '../Footer/Footer.tsx';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';

type StationTask = components['schemas']['StationTasksResponseDto'];
type StationTaskOrder = components['schemas']['StationTaskOrder'];
type OrderStatus = components['schemas']['OrderStatus'];

export type StationTasksProps = {
  stationId: number;
};

type StatusButton = {
  next: OrderStatus;
  label: string;
  color?: string;
  variant?: 'filled' | 'light' | 'outline';
};

type StationStatus = {
  status: OrderStatus;
  color: string;
  label: string;
  buttons: StatusButton[];
};

const statuses: Partial<Record<OrderStatus, StationStatus>> = {
  Pending: {
    status: 'Pending',
    color: 'yellow',
    label: 'Pending',
    buttons: [
      { next: 'InProgress', label: 'Start', color: 'blue', variant: 'light' },
      { next: 'Cancelled', label: 'Cancel', color: 'red', variant: 'light' },
    ],
  },
  InProgress: {
    status: 'InProgress',
    color: 'blue',
    label: 'In Progress',
    buttons: [{ next: 'Ready', label: 'Ready', color: 'blue', variant: 'light' }],
  },
  Ready: {
    status: 'Ready',
    color: 'cyan',
    label: 'Ready',
    buttons: [{ next: 'Completed', label: 'Complete', color: 'green', variant: 'filled' }],
  },
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
  const status = statuses[order.status];
  const buttons = canAdvanceStatus ? (status?.buttons ?? []) : [];

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
          <Badge color={status?.color ?? 'gray'} variant="filled">
            {status?.label ?? order.status}
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

      {buttons.length > 0 && (
        <>
          <Divider mt="sm" mb="xs" />
          <Group grow gap="xs">
            {buttons.map((button) => (
              <Button
                key={button.next}
                fullWidth
                size="sm"
                color={button.color}
                variant={button.variant}
                onClick={() => onAdvance(order.id, button.next)}
              >
                {button.label}
              </Button>
            ))}
          </Group>
        </>
      )}
    </Card>
  );
};

type Category = 'Pending' | 'InProgress' | 'Ready';

export const StationTasks = ({ stationId }: StationTasksProps) => {
  const { useNecessaryPerm, useRecommendedPerm } = useApi('StationTasks');
  const stationReadApi = useRecommendedPerm('Permission:Station:Read');
  const updateStatusApi = useRecommendedPerm('Permission:Order:UpdateStatus');
  const isWideScreen = useMediaQuery('(min-width: 1400px)');
  const [selectedCategory, setSelectedCategory] = useState<Category>('Pending');

  const [{ data: tasks, refetch: refetchTasks }, { data: station }] = useConditionalSuspenseQueries(
    [
      useNecessaryPerm('Permission:Station:ViewTasks').queryOptions(
        'get',
        '/api/Station/{id}/tasks',
        {
          params: { path: { id: stationId } },
        },
        {
          refetchInterval: 2500,
        },
      ),
      stationReadApi?.queryOptions('get', '/api/Station/{id}', {
        params: { path: { id: stationId } },
      }),
    ],
  );

  const updateStatus = updateStatusApi
    ? updateStatusApi.useMutation('put', '/api/Order/{id}/status', {
        onSuccess: refetchTasks,
      }).mutate
    : undefined;

  const handleAdvance = (id: number, status: OrderStatus) => {
    updateStatus?.({ params: { path: { id } }, body: status });
  };

  const tasksArray: StationTask[] = Array.isArray(tasks) ? tasks : tasks ? [tasks] : [];
  const allOrders = tasksArray.flatMap((t) => t.orders);
  const pendingOrders = allOrders.filter((o) => o.status === 'Pending');
  const inProgressOrders = allOrders.filter((o) => o.status === 'InProgress');
  const readyOrders = allOrders.filter((o) => o.status === 'Ready');

  if (allOrders.length === 0) {
    return (
      <>
        <Header title={`Tasks: ${station?.name ?? 'Station'}`} />
        <Box pt="xl" px="xl" pb={80}>
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
      <Header title={`Tasks: ${station?.name ?? 'Station'}`} />
      <Box pt="md" px="md" pb={80}>
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
                        canAdvanceStatus={!!updateStatus}
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
                        canAdvanceStatus={!!updateStatus}
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
                        canAdvanceStatus={!!updateStatus}
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
                          canAdvanceStatus={!!updateStatus}
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
                          canAdvanceStatus={!!updateStatus}
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
                          canAdvanceStatus={!!updateStatus}
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
