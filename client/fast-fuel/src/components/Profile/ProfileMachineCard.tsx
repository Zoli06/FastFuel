import { Card, Group, Stack, Text } from '@mantine/core';
import type { components } from '../../types/api.ts';

type MachineData = components['schemas']['MachineResponseDto'];

export const ProfileMachineCard = ({ data }: { data: MachineData }) => (
  <Card withBorder radius="md" p="lg">
    <Text fw={500} mb="md">
      Machine details
    </Text>
    <Stack gap="xs">
      <Group>
        <Text size="sm" c="dimmed" w={160}>
          Located at restaurant
        </Text>
        <Text size="sm">{data.locatedAtRestaurantId}</Text>
      </Group>
    </Stack>
  </Card>
);
