import {
  Badge,
  Box,
  Button,
  Card,
  Divider,
  Group,
  ScrollArea,
  SimpleGrid,
  Stack,
  Text,
  Title,
} from '@mantine/core';
import type { components } from '../../types/api';
import { apiClient } from '../../lib/api-client.ts';

type StationTask = components['schemas']['StationTasksResponseDto'];
type StationTaskOrder = components['schemas']['StationTaskOrder'];
type Station = components['schemas']['StationResponseDto'];
type OrderStatus = components['schemas']['OrderStatus'];

export type StationTasksProps = {
  tasks: StationTask | StationTask[];
  refetchTasks: () => void;
  station: Station;
};

const nextStatus: Partial<Record<OrderStatus, OrderStatus>> = {
  Pending: 'InProgress',
  InProgress: 'Completed',
};

const statusColor: Record<string, string> = {
  Pending: 'yellow',
  InProgress: 'blue',
};

const statusLabel: Record<string, string> = {
  Pending: 'Pending',
  InProgress: 'In Progress',
};

const nextStatusLabel: Record<string, string> = {
  Pending: 'Start',
  InProgress: 'Complete',
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

export const StationTasks = ({ tasks, refetchTasks }: StationTasksProps) => {
  const { data: permissions } = apiClient.useSuspenseQuery('get', '/api/Permission/my');
  const canUpdateOrderStatus = permissions?.includes('Permission:Order:UpdateStatus') ?? false;

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

  if (allOrders.length === 0) {
    return (
      <Box p="xl">
        <Text ta="center" c="dimmed" fz="xl">
          No pending tasks
        </Text>
      </Box>
    );
  }

  return (
    <Box p="md" pb={80}>
      <SimpleGrid cols={2} spacing="lg">
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
                    canAdvanceStatus={canUpdateOrderStatus}
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
                    canAdvanceStatus={canUpdateOrderStatus}
                  />
                ))
              )}
            </Stack>
          </ScrollArea>
        </Box>
      </SimpleGrid>
    </Box>
  );
};
