import { Card, Group, Stack, Text } from '@mantine/core';
import type { components } from '../../types/api-schema.generated.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { useApi } from '../../lib/api.ts';

type MachineData = components['schemas']['MachineResponseDto'];

export const ProfileMachineCard = ({ data }: { data: MachineData }) => {
  const { useNoPerm } = useApi('Profile');

  const [{ data: restaurants }] = useConditionalSuspenseQueries([
    useNoPerm().queryOptions('get', '/api/Restaurant'),
  ]);

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
