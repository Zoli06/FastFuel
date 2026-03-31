import { Card, Group, Stack, Text } from '@mantine/core';
import type { components } from '../../types/api.ts';

type EmployeeData = components['schemas']['EmployeeResponseDto'];

export const ProfileEmployeeCard = ({ data }: { data: EmployeeData }) => (
  <Card withBorder radius="md" p="lg">
    <Text fw={500} mb="md">
      Employment details
    </Text>
    <Stack gap="xs">
      <Group>
        <Text size="sm" c="dimmed" w={160}>
          Restaurant ID
        </Text>
        <Text size="sm">{data.worksAtRestaurantId}</Text>
      </Group>
      <Group>
        <Text size="sm" c="dimmed" w={160}>
          Station categories
        </Text>
        <Text size="sm">
          {data.stationCategoryIds.length > 0 ? data.stationCategoryIds.join(', ') : '—'}
        </Text>
      </Group>
      <Group>
        <Text size="sm" c="dimmed" w={160}>
          Shifts
        </Text>
        <Text size="sm">{data.shiftIds.length > 0 ? `${data.shiftIds.length} shift(s)` : '—'}</Text>
      </Group>
    </Stack>
  </Card>
);
