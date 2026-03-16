import { Button, Stack, TextInput, PasswordInput } from '@mantine/core';
import { Form, useForm } from '@mantine/form';
import { Paper } from '../common/Paper/Paper';
import { apiClient } from '../../apiClient';
import type { components } from '../../types/api';

type RegisterFormValues = {
  name: string;
  email: string;
  userName: string;
  themeId: number | null;
  password: string;
  confirmPassword: string;
};

export const Register = () => {
  const form = useForm<RegisterFormValues>({
    initialValues: {
      name: '',
      email: '',
      userName: '',
      themeId: null,
      password: '',
      confirmPassword: '',
    },
    validate: {
      confirmPassword: (value, values) =>
        value !== values.password ? 'Passwords do not match' : null,
    },
  });

  const { mutateAsync: register, isPending: isRegistering } = apiClient.useMutation(
    'post',
    '/api/Customer',
  );

  const { mutateAsync: login, isPending: isLoggingIn } = apiClient.useMutation(
    'post',
    '/api/Auth/login',
  );

  const handleSubmit = async (values: RegisterFormValues) => {
    try {
      await register({
        body: {
          name: values.name,
          email: values.email,
          userName: values.userName,
          themeId: values.themeId,
          password: values.password,
        } as components['schemas']['CustomerRequestDto'],
      });

      await login({
        body: {
          userName: values.userName,
          password: values.password,
        },
        params: {
          query: {
            useCookies: true,
            useSessionCookies: true,
          },
        },
      });

      window.location.href = '/';
    } catch {
      form.setErrors({
        userName: 'Registration or login failed',
      });
    }
  };

  return (
    <Paper>
      <Form form={form} onSubmit={handleSubmit}>
        <Stack gap="xs">
          <TextInput label="Name" placeholder="Enter your name" {...form.getInputProps('name')} />

          <TextInput
            label="Email"
            placeholder="Enter your email"
            {...form.getInputProps('email')}
          />

          <TextInput
            label="Username"
            placeholder="Enter username"
            {...form.getInputProps('userName')}
          />

          <PasswordInput
            label="Password"
            placeholder="Enter password"
            {...form.getInputProps('password')}
          />

          <PasswordInput
            label="Confirm Password"
            placeholder="Confirm password"
            {...form.getInputProps('confirmPassword')}
          />

          <Button type="submit" fullWidth loading={isRegistering || isLoggingIn}>
            Register
          </Button>
        </Stack>
      </Form>
    </Paper>
  );
};
