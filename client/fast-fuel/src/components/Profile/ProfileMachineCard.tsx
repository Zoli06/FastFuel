import { Card, Group, Stack, Text } from '@mantine/core';
import type { components } from '../../types/api-schema.generated.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { $api } from '../../lib/api.ts';

type MachineData = components['schemas']['MachineResponseDto'];

export const ProfileMachineCard = ({ data }: { data: MachineData }) => {
  const { recommendedPerm } = $api('Profile');
  const restaurantReadApi = recommendedPerm('Permission:Restaurant:Read');

  const [{ data: restaurants }] = useConditionalSuspenseQueries([
    restaurantReadApi?.queryOptions('get', '/api/Restaurant'),
  ]);

  if (!restaurantReadApi) return null;

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
