import { Card, Group, Stack, Text } from '@mantine/core';
import type { components } from '../../types/api-schema.generated.ts';
import { $api } from '../../lib/api.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';

type EmployeeData = components['schemas']['EmployeeResponseDto'];

export const ProfileEmployeeCard = ({ data }: { data: EmployeeData }) => {
  const { recommendedPerm } = $api('Profile');
  const restaurantReadApi = recommendedPerm('Permission:Restaurant:Read');
  const stationCategoryReadApi = recommendedPerm('Permission:StationCategory:Read');

  const [{ data: restaurants }, { data: stationCategories }] = useConditionalSuspenseQueries([
    restaurantReadApi?.queryOptions('get', '/api/Restaurant'),
    stationCategoryReadApi?.queryOptions('get', '/api/StationCategory'),
  ]);

  return (
    <Card withBorder radius="md" p="lg">
      <Text fw={500} mb="md">
        Employment details
      </Text>
      <Stack gap="xs">
        {restaurantReadApi && (
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
        {stationCategoryReadApi && (
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
