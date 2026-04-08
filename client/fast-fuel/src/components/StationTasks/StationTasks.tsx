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
import { useSuspenseQuery } from '@tanstack/react-query';
import { useEffect, useState } from 'react';
import { useMediaQuery } from '@mantine/hooks';
import { useNavigate, useParams } from 'react-router-dom';
import type { components } from '../../types/api-schema.generated.ts';
import { useApi } from '../../lib/api.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { StationPickerModal } from '../common/SearchablePickerModals/StationPickerModal.tsx';

type StationTask = components['schemas']['StationTasksResponseDto'];
type StationTaskOrder = components['schemas']['StationTaskOrder'];
type OrderStatus = components['schemas']['OrderStatus'];

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

export const StationTasks = () => {
  const { id } = useParams<{ id?: string }>();
  const navigate = useNavigate();
  const parsedRouteStationId = Number(id);
  const activeStationId =
    Number.isInteger(parsedRouteStationId) && parsedRouteStationId > 0
      ? parsedRouteStationId
      : null;
  const { useNoPerm, useNecessaryPerm, useRecommendedPerm } = useApi('StationTasks');
  const noPermApi = useNoPerm();
  const { data: currentUser } = useSuspenseQuery(noPermApi.queryOptions('get', '/api/User/me'));
  const currentUserType = currentUser.userType.toLowerCase();
  const isEmployeeUser = currentUserType === 'employee';
  const isMachineUser = currentUserType === 'machine';
  const stationTasksApi = useNecessaryPerm('Permission:Station:ViewTasks');
  const stationReadApi = useRecommendedPerm('Permission:Station:Read');
  const restaurantReadApi = useRecommendedPerm('Permission:Restaurant:Read');
  const updateStatusApi = useRecommendedPerm('Permission:Order:UpdateStatus');
  const isWideScreen = useMediaQuery('(min-width: 1400px)');
  const [stationPickerOpen, setStationPickerOpen] = useState(activeStationId === null);
  const [stationSearch, setStationSearch] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<Category>('Pending');

  const [{ data: employeeProfile }, { data: machineProfile }] = useConditionalSuspenseQueries([
    isEmployeeUser ? noPermApi.queryOptions('get', '/api/Employee/me') : undefined,
    isMachineUser ? noPermApi.queryOptions('get', '/api/Machine/me') : undefined,
  ]);

  const lockedRestaurantId = isEmployeeUser
    ? (employeeProfile?.worksAtRestaurantId ?? null)
    : isMachineUser
      ? (machineProfile?.locatedAtRestaurantId ?? null)
      : null;

  const [
    { data: tasks, refetch: refetchTasks },
    { data: stations = [] },
    { data: lockedRestaurant },
  ] = useConditionalSuspenseQueries([
    activeStationId !== null
      ? stationTasksApi.queryOptions(
          'get',
          '/api/Station/{id}/tasks',
          {
            params: { path: { id: activeStationId } },
          },
          {
            refetchInterval: 2500,
          },
        )
      : undefined,
    stationReadApi
      ? stationReadApi.queryOptions('get', '/api/Station', {
          params: {
            query: lockedRestaurantId !== null ? { restaurantId: lockedRestaurantId } : undefined,
          },
        })
      : undefined,
    restaurantReadApi && lockedRestaurantId !== null
      ? restaurantReadApi.queryOptions('get', '/api/Restaurant/{id}', {
          params: { path: { id: lockedRestaurantId } },
        })
      : undefined,
  ]);

  useEffect(() => {
    if (activeStationId === null) {
      setStationSearch('');
      setStationPickerOpen(true);
    }
  }, [activeStationId]);

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
  const filteredStations = stations.filter((s) =>
    s.name.toLowerCase().includes(stationSearch.toLowerCase()),
  );
  const canChangeStation = Boolean(
    stationReadApi && stations.length > 1 && lockedRestaurantId === null,
  );
  const stationPickerTitle =
    (isEmployeeUser || isMachineUser) && restaurantReadApi && lockedRestaurant?.name
      ? `Switch station (${lockedRestaurant.name} resturant)`
      : 'Switch station';

  const openStationPicker = () => {
    setStationSearch('');
    setStationPickerOpen(true);
  };

  const handleSelectStation = (newStationId: number) => {
    setStationPickerOpen(false);
    setStationSearch('');
    void navigate(`/station-tasks/${newStationId}`);
  };

  const handleCloseStationPicker = () => {
    if (activeStationId !== null) {
      setStationPickerOpen(false);
    }
  };

  if (allOrders.length === 0) {
    return (
      <>
        <Box pt="xl" px="xl" pb="xl">
          {canChangeStation && (
            <Group justify="flex-end" mb="md">
              <Button variant="light" onClick={openStationPicker}>
                Change Station
              </Button>
            </Group>
          )}
          <Text ta="center" c="dimmed" fz="xl">
            No pending tasks
          </Text>
        </Box>
        <StationPickerModal
          opened={stationPickerOpen}
          stationId={activeStationId}
          searchValue={stationSearch}
          stations={filteredStations}
          onSearchChange={setStationSearch}
          onClose={handleCloseStationPicker}
          onSelectStation={handleSelectStation}
          title={stationPickerTitle}
        />
      </>
    );
  }

  return (
    <>
      <Box pt="md" px="md" pb="xl">
        {canChangeStation && (
          <Group justify="flex-end" mb="md">
            <Button variant="light" onClick={openStationPicker}>
              Change Station
            </Button>
          </Group>
        )}

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
      <StationPickerModal
        opened={stationPickerOpen}
        stationId={activeStationId}
        searchValue={stationSearch}
        stations={filteredStations}
        onSearchChange={setStationSearch}
        onClose={handleCloseStationPicker}
        onSelectStation={handleSelectStation}
        title={stationPickerTitle}
      />
    </>
  );
};
