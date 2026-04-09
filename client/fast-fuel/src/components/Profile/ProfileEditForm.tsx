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
import { Form, useForm } from '@mantine/form';
import { notifications } from '@mantine/notifications';
import { useApi } from '../../lib/api.ts';
import { validatePasswordComplexityIfProvided } from '../../lib/password-validation.ts';

interface ProfileEditFormProps {
  name: string;
  userName: string;
  email: string;
  onSaved: () => void;
}

export const ProfileEditForm = ({ name, userName, email, onSaved }: ProfileEditFormProps) => {
  const form = useForm({
    initialValues: { name, userName, email, password: '' },
    validate: {
      password: (value) => validatePasswordComplexityIfProvided(value),
    },
  });

  const { setFieldValue } = form;

  useEffect(() => {
    setFieldValue('name', name);
    setFieldValue('userName', userName);
  }, [name, userName, setFieldValue]);

  useEffect(() => {
    if (email) setFieldValue('email', email);
  }, [email, setFieldValue]);

  const updateCustomer = useApi('Profile')
    .useRecommendedPerm('Permission:Customer:UpdateSelf')
    ?.useMutation('put', '/api/Customer/me', {
      onSuccess: () => {
        onSaved();
        notifications.show({
          title: 'Profile updated',
          message: 'Your changes have been saved.',
          color: 'green',
        });
      },
    }).mutate;

  const handleSubmit = (values: typeof form.values) => {
    if (updateCustomer) {
      updateCustomer({
        body: {
          name: values.name,
          userName: values.userName,
          email: values.email,
          password: values.password || null,
        },
      });
    }
  };

  return (
    <Card withBorder radius="md" p="lg">
      <Text fw={500} mb="md">
        Edit profile
      </Text>
      <Form form={form} onSubmit={handleSubmit}>
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
            <Button type="submit" color="#c92a2a">
              Save changes
            </Button>
          </Group>
        </Stack>
      </Form>
    </Card>
  );
};
