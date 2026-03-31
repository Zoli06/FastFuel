import { Card, Group, Stack, Text } from '@mantine/core';
import type { components } from '../../types/api.ts';
import { usePagePermissions } from '../../hooks/usePagePermissions.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { apiClient } from '../../lib/api-client.ts';

type MachineData = components['schemas']['MachineResponseDto'];

export const ProfileMachineCard = ({ data }: { data: MachineData }) => {
  const { recommended } = usePagePermissions('Profile');

  const [{ data: restaurants }] = useConditionalSuspenseQueries([
    recommended.Restaurant.Read && apiClient.queryOptions('get', '/api/Restaurant'),
  ]);

  if (!recommended.Restaurant.Read) return null;

  return (
    <Card withBorder radius="md" p="lg">
      <Text fw={500} mb="md">
        Machine details
      </Text>
      <Stack gap="xs">
        <Group>
          <Text size="sm" c="dimmed" w={160}>
            Located at restaurant
          </Text>
          <Text size="sm">
            {restaurants?.find((r) => r.id === data.locatedAtRestaurantId)?.name ??
              `#${data.locatedAtRestaurantId}`}
          </Text>
        </Group>
      </Stack>
    </Card>
  );
};
