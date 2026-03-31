import { Card, Group, Stack, Text } from '@mantine/core';
import type { components } from '../../types/api.ts';
import { apiClient } from '../../lib/api-client.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { usePagePermissions } from '../../hooks/usePagePermissions.ts';

type EmployeeData = components['schemas']['EmployeeResponseDto'];

export const ProfileEmployeeCard = ({ data }: { data: EmployeeData }) => {
  const { recommended } = usePagePermissions('Profile');

  const [{ data: restaurants }, { data: stationCategories }] = useConditionalSuspenseQueries([
    recommended.Restaurant.Read && apiClient.queryOptions('get', '/api/Restaurant'),
    recommended.StationCategory.Read && apiClient.queryOptions('get', '/api/StationCategory'),
  ]);

  return (
    <Card withBorder radius="md" p="lg">
      <Text fw={500} mb="md">
        Employment details
      </Text>
      <Stack gap="xs">
        {recommended.Restaurant.Read && (
          <Group>
            <Text size="sm" c="dimmed" w={160}>
              Working at
            </Text>
            <Text size="sm">
              {restaurants?.find((r) => r.id === data.worksAtRestaurantId)?.name ??
                `#${data.worksAtRestaurantId}`}
            </Text>
          </Group>
        )}
        {recommended.StationCategory.Read && (
          <Group>
            <Text size="sm" c="dimmed" w={160}>
              Station categories
            </Text>
            <Text size="sm">
              {stationCategories
                ?.filter((c) => data.stationCategoryIds?.includes(c.id))
                .map((c) => c.name)
                .join(', ') ?? 'None'}
            </Text>
          </Group>
        )}
      </Stack>
    </Card>
  );
};
