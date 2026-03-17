import { Button, PasswordInput, Stack, TextInput } from '@mantine/core';
import { Form, useForm } from '@mantine/form';
import { Paper } from '../common/Paper/Paper';
import { apiClient } from '../../lib/api-client';
import type { components } from '../../types/api';
import { useNavigate } from 'react-router-dom';

type RegisterFormValues = components['schemas']['CustomerRequestDto'] & {
  password: string;
  confirmPassword: string;
};

export const Register = () => {
  const redirect = useNavigate();

  const form = useForm<RegisterFormValues>({
    mode: 'uncontrolled',
    initialValues: {
      name: '',
      email: '',
      userName: '',
      themeId: null,
      password: '',
      confirmPassword: '',
    } as RegisterFormValues,
    validate: {
      confirmPassword: (value, values) =>
        value !== values.password ? 'Passwords do not match' : null,
    },
  });

  const { mutateAsync: register, isPending: isRegistering } = apiClient.useMutation(
    'post',
    '/api/Customer',
    {
      onSuccess: () => {
        form.reset();
      },
      onError: () => {
        // TODO: On the dev branch there is already a system for (error) notifications, use that
        form.setErrors({
          userName: 'Unknown error occurred. Please try again later.',
        });
      },
    },
  );

  const { mutateAsync: login, isPending: isLoggingIn } = apiClient.useMutation(
    'post',
    '/api/Auth/login',
    {
      onSuccess: () => {
        redirect('/');
      },
      onError: () => {
        form.setErrors({
          userName: 'Unknown error occurred during login. Please try logging in manually.',
        });
      },
    },
  );

  const handleSubmit = async (values: RegisterFormValues) => {
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

    redirect('/');
  };

  return (
    <Paper>
      <Form form={form} onSubmit={handleSubmit}>
        <Stack gap="xs">
          <TextInput
            key={form.key('name')}
            label="Name"
            placeholder="Enter your name"
            {...form.getInputProps('name')}
          />

          <TextInput
            key={form.key('email')}
            label="Email"
            placeholder="Enter your email"
            {...form.getInputProps('email')}
          />

          <TextInput
            key={form.key('userName')}
            label="Username"
            placeholder="Enter username"
            {...form.getInputProps('userName')}
          />

          <PasswordInput
            key={form.key('password')}
            label="Password"
            placeholder="Enter password"
            {...form.getInputProps('password')}
          />

          <PasswordInput
            key={form.key('confirmPassword')}
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
