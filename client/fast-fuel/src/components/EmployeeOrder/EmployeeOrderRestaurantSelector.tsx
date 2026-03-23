import { Button, Stack, Text, Select } from '@mantine/core';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../../lib/api-client.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { useSuspensePermissions } from '../../hooks/useSuspensePermissions.ts';
import { Paper } from '../common/Paper/Paper.tsx';

const requiredPermissions = [
  'Permission:Food:Read',
  'Permission:Menu:Read',
  'Permission:Order:ReadOwn',
  'Permission:Order:Create',
] as const;

export const EmployeeOrderRestaurantSelector = () => {
  const can = useSuspensePermissions();
  const navigate = useNavigate();
  const [selectedRestaurantId, setSelectedRestaurantId] = useState<string | null>(null);

  const missingPermissions = requiredPermissions.filter((permission) => {
    switch (permission) {
      case 'Permission:Food:Read':
        return !can.Food.Read;
      case 'Permission:Menu:Read':
        return !can.Menu.Read;
      case 'Permission:Order:ReadOwn':
        return !can.Order.ReadOwn;
      case 'Permission:Order:Create':
        return !can.Order.Create;
    }
  });

  const [{ data: restaurants = [] }] = useConditionalSuspenseQueries([
    can.Restaurant.Read && apiClient.queryOptions('get', '/api/Restaurant'),
  ]);

  if (missingPermissions.length > 0) {
    return (
      <Paper>
        <Stack gap="xs">
          <Text fw={700}>You do not have the required permissions to create orders.</Text>
          <Text size="sm" c="dimmed">
            Missing permissions: {missingPermissions.join(', ')}.
          </Text>
        </Stack>
      </Paper>
    );
  }

  if (!can.Restaurant.Read) {
    return (
      <Paper>
        <Text>You need Permission:Restaurant:Read to select a restaurant.</Text>
      </Paper>
    );
  }

  return (
    <Paper>
      <Stack gap="md">
        <Text fw={700} fz="xl">
          Select a restaurant
        </Text>

        <Select
          label="Restaurant"
          placeholder="Choose restaurant"
          data={restaurants.map((restaurant) => ({
            value: restaurant.id.toString(),
            label: restaurant.name,
          }))}
          searchable
          value={selectedRestaurantId}
          onChange={setSelectedRestaurantId}
        />

        <Button
          onClick={() => navigate(`/employee/order/${selectedRestaurantId}`)}
          disabled={!selectedRestaurantId}
        >
          Start order
        </Button>
      </Stack>
    </Paper>
  );
};
