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
import type { components } from '../../types/api-schema.generated.ts';

interface ProfileEditFormProps {
  name: string;
  userName: string;
  email: string;
  onSaved: () => void;
}

export const ProfileEditForm = ({ name, userName, email, onSaved }: ProfileEditFormProps) => {
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

  const handleSubmit = form.onSubmit(async (values) => {
    const payload = {
      name: values.name,
      userName: values.userName,
      email: values.email,
      themeId: null,
      password: values.password || null,
    } as components['schemas']['CustomerRequestDto'];

    const response = await fetch('http://localhost:5249/api/Customer/me', {
      method: 'PUT',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload),
    });

    if (response.ok) {
      onSaved();
      notifications.show({
        title: 'Profile updated',
        message: 'Your changes have been saved.',
        color: 'green',
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
