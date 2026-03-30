import { useEffect } from 'react';
import { useQuery, useSuspenseQuery, useQueryClient } from '@tanstack/react-query';
import {
  Avatar,
  Badge,
  Button,
  Card,
  Group,
  PasswordInput,
  SimpleGrid,
  Stack,
  Text,
  TextInput,
} from '@mantine/core';
import { useForm } from '@mantine/form';
import { notifications } from '@mantine/notifications';
import { apiClient, myCurrentUserQueryOptions } from '../../lib/api-client.ts';
import type { components } from '../../types/api.ts';

export const Profile = () => {
  const queryClient = useQueryClient();
  const { data: currentUser } = useSuspenseQuery(myCurrentUserQueryOptions());

  const isCustomer = currentUser.userType === 'Customer';
  const isAdmin = currentUser.userType === 'Admin';
  const isEmployee = currentUser.userType === 'Employee';
  const isMachine = currentUser.userType === 'Machine';
  const canEdit = isCustomer || isAdmin;

  const { data: customerData } = useQuery({
    ...apiClient.queryOptions('get', '/api/Customer/me'),
    enabled: isCustomer,
  });

  const { data: adminData } = useQuery({
    ...apiClient.queryOptions('get', '/api/Admin/me'),
    enabled: isAdmin,
  });

  const { data: employeeData } = useQuery({
    ...apiClient.queryOptions('get', '/api/Employee/me'),
    enabled: isEmployee,
  });

  const { data: machineData } = useQuery({
    ...apiClient.queryOptions('get', '/api/Machine/me'),
    enabled: isMachine,
  });

  const specificUser = customerData ?? adminData ?? employeeData ?? machineData;
  const email = specificUser && 'email' in specificUser ? specificUser.email : '';

  const form = useForm({
    initialValues: {
      name: currentUser.name,
      userName: currentUser.userName,
      email: '',
      password: '',
    },
  });

  const { setFieldValue } = form;

  useEffect(() => {
    if (email) {
      setFieldValue('email', email);
    }
  }, [email, setFieldValue]);

  const invalidateProfile = () => {
    void queryClient.invalidateQueries({ queryKey: ['get', '/api/Customer/me'] });
    void queryClient.invalidateQueries({ queryKey: ['get', '/api/Admin/me'] });
    void queryClient.invalidateQueries({ queryKey: myCurrentUserQueryOptions().queryKey });
  };

  const { mutate: updateCustomer } = apiClient.useMutation('put', '/api/Customer/me', {
    onSuccess: () => {
      invalidateProfile();
      notifications.show({
        title: 'Profile updated',
        message: 'Your changes have been saved.',
        color: 'green',
      });
    },
  });

  const { mutate: updateAdmin } = apiClient.useMutation('put', '/api/Admin/{id}', {
    onSuccess: () => {
      invalidateProfile();
      notifications.show({
        title: 'Profile updated',
        message: 'Your changes have been saved.',
        color: 'green',
      });
    },
  });

  const handleSubmit = form.onSubmit((values) => {
    if (isCustomer) {
      updateCustomer({
        body: {
          name: values.name,
          userName: values.userName,
          email: values.email,
          themeId: null,
          password: values.password || null,
        } as components['schemas']['CustomerRequestDto'],
      });
    } else if (isAdmin) {
      updateAdmin({
        params: { path: { id: currentUser.id } },
        body: {
          name: values.name,
          userName: values.userName,
          email: values.email,
          themeId: null,
          password: values.password || null,
        } as components['schemas']['AdminRequestDto'],
      });
    }
  });

  const initials = currentUser.name
    .split(' ')
    .map((n) => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2);

  return (
    <Stack p="md" maw={800} mx="auto">
      <Card withBorder radius="md" p="lg">
        <Group>
          <Avatar size={64} radius="xl" color="orange">
            {initials}
          </Avatar>
          <Stack gap={4}>
            <Text fw={500} size="xl">
              {currentUser.name}
            </Text>
            <Text c="dimmed" size="sm">
              @{currentUser.userName}
            </Text>
            <Badge color="orange" variant="light">
              {currentUser.userType}
            </Badge>
          </Stack>
        </Group>
      </Card>

      {canEdit && (
        <Card withBorder radius="md" p="lg">
          <Text fw={500} mb="md">
            Edit profile
          </Text>
          <form onSubmit={handleSubmit}>
            <Stack>
              <SimpleGrid cols={2}>
                <TextInput label="Name" {...form.getInputProps('name')} />
                <TextInput label="Username" {...form.getInputProps('userName')} />
              </SimpleGrid>
              <TextInput label="Email" type="email" {...form.getInputProps('email')} />
              <PasswordInput
                label="New password"
                placeholder="Leave blank to keep current"
                {...form.getInputProps('password')}
              />
              <Group justify="flex-end">
                <Button type="submit" color="orange">
                  Save changes
                </Button>
              </Group>
            </Stack>
          </form>
        </Card>
      )}

      {isEmployee && employeeData && (
        <Card withBorder radius="md" p="lg">
          <Text fw={500} mb="md">
            Employment details
          </Text>
          <Stack gap="xs">
            <Group>
              <Text size="sm" c="dimmed" w={160}>
                Restaurant ID
              </Text>
              <Text size="sm">{employeeData.worksAtRestaurantId}</Text>
            </Group>
            <Group>
              <Text size="sm" c="dimmed" w={160}>
                Station categories
              </Text>
              <Text size="sm">
                {employeeData.stationCategoryIds.length > 0
                  ? employeeData.stationCategoryIds.join(', ')
                  : '—'}
              </Text>
            </Group>
            <Group>
              <Text size="sm" c="dimmed" w={160}>
                Shifts
              </Text>
              <Text size="sm">
                {employeeData.shiftIds.length > 0
                  ? `${employeeData.shiftIds.length} shift(s)`
                  : '—'}
              </Text>
            </Group>
          </Stack>
        </Card>
      )}

      {isMachine && machineData && (
        <Card withBorder radius="md" p="lg">
          <Text fw={500} mb="md">
            Machine details
          </Text>
          <Stack gap="xs">
            <Group>
              <Text size="sm" c="dimmed" w={160}>
                Located at restaurant
              </Text>
              <Text size="sm">{machineData.locatedAtRestaurantId}</Text>
            </Group>
          </Stack>
        </Card>
      )}
    </Stack>
  );
};
