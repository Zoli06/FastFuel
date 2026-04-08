import { Avatar, Badge, Card, Group, Stack, Text } from '@mantine/core';

interface ProfileHeaderProps {
  name: string;
  userName: string;
  userType: string;
}

export const ProfileHeader = ({ name, userName, userType }: ProfileHeaderProps) => {
  const initials = name
    .split(' ')
    .map((n) => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2);

  return (
    <Card withBorder radius="md" p="lg">
      <Group>
        <Avatar size={64} radius="xl" color="#c92a2a">
          {initials}
        </Avatar>
        <Stack gap={4}>
          <Text fw={500} size="xl">
            {name}
          </Text>
          <Text c="dimmed" size="sm">
            @{userName}
          </Text>
          <Badge color="#BFB48F" variant="light">
            {userType}
          </Badge>
        </Stack>
      </Group>
    </Card>
  );
};
