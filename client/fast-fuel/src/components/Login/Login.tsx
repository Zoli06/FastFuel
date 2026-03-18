import { Anchor, Button, PasswordInput, Stack, TextInput } from '@mantine/core';
import { Form, useForm } from '@mantine/form';
import { Link } from 'react-router-dom';
import { Paper } from '../common/Paper/Paper.tsx';
import { apiClient } from '../../lib/api-client.ts';
import type { components } from '../../types/api';

type LoginValues = components['schemas']['LoginRequestDto'];

export const Login = () => {
  const form = useForm<LoginValues>({
    initialValues: {
      userName: '',
      password: '',
    },
  });

  const { mutate: login, isPending } = apiClient.useMutation('post', '/api/Auth/login', {
    onSuccess: () => {
      window.location.href = '/';
    },
    onError: () => {
      form.setErrors({
        password: 'Invalid username or password',
      });
    },
  });

  const handleSubmit = (values: LoginValues) => {
    login({
      body: values,
      params: {
        query: {
          useCookies: true,
          useSessionCookies: true,
        },
      },
    });
  };

  return (
    <Paper>
      <Form form={form} onSubmit={handleSubmit}>
        <Stack gap="xs">
          <TextInput
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

          <Button type="submit" fullWidth loading={isPending}>
            Login
          </Button>

          <Anchor component={Link} to="/register" size="sm" ta="center">
            Don't have an account? Register
          </Anchor>
        </Stack>
      </Form>
    </Paper>
  );
};
