import { useEffect } from 'react';
import {
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
import { apiClient } from '../../lib/api-client.ts';
import type { components } from '../../types/api.ts';

interface ProfileEditFormProps {
  userId: number;
  name: string;
  userName: string;
  email: string;
  userType: 'Customer' | 'Admin';
  onSaved: () => void;
}

export const ProfileEditForm = ({
  userId,
  name,
  userName,
  email,
  userType,
  onSaved,
}: ProfileEditFormProps) => {
  const form = useForm({
    initialValues: { name, userName, email, password: '' },
  });

  const { setFieldValue } = form;

  useEffect(() => {
    setFieldValue('name', name);
    setFieldValue('userName', userName);
  }, [name, userName, setFieldValue]);

  useEffect(() => {
    if (email) setFieldValue('email', email);
  }, [email, setFieldValue]);

  const { mutate: updateCustomer } = apiClient.useMutation('put', '/api/Customer/me', {
    onSuccess: () => {
      onSaved();
      notifications.show({
        title: 'Profile updated',
        message: 'Your changes have been saved.',
        color: 'green',
      });
    },
  });

  const { mutate: updateAdmin } = apiClient.useMutation('put', '/api/Admin/{id}', {
    onSuccess: () => {
      onSaved();
      notifications.show({
        title: 'Profile updated',
        message: 'Your changes have been saved.',
        color: 'green',
      });
    },
  });

  const handleSubmit = form.onSubmit((values) => {
    const body = {
      name: values.name,
      userName: values.userName,
      email: values.email,
      themeId: null,
      password: values.password || null,
    };

    if (userType === 'Customer') {
      updateCustomer({ body: body as components['schemas']['CustomerRequestDto'] });
    } else {
      updateAdmin({
        params: { path: { id: userId } },
        body: body as components['schemas']['AdminRequestDto'],
      });
    }
  });

  return (
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
  );
};
