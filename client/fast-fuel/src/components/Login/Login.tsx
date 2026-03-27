import { Anchor, Button, PasswordInput, Stack, TextInput } from '@mantine/core';
import { Form, useForm } from '@mantine/form';
import { Link, useNavigate } from 'react-router-dom';
import { Paper } from '../common/Paper/Paper';
import { apiClient, triggerPermissionsRefresh } from '../../lib/api-client';
import { fetchClient } from '../../lib/api-client';
import type { components } from '../../types/api';

type LoginValues = components['schemas']['LoginRequestDto'];

export const Login = () => {
  const navigate = useNavigate();

  const form = useForm<LoginValues>({
    initialValues: { userName: '', password: '' },
  });

  const { mutate: login, isPending } = apiClient.useMutation('post', '/api/Auth/login', {
    onSuccess: async () => {
      triggerPermissionsRefresh();
      const { data: user } = await fetchClient.GET('/api/User/me');
      navigate(user?.userType === 'Customer' ? '/Customer' : '/', { replace: true });
    },
    onError: () => {
      form.setFieldError('password', 'Incorrect username or password');
    },
  });

  const handleSubmit = (values: LoginValues) => {
    login({
      body: values,
      params: { query: { useCookies: true, useSessionCookies: true } },
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
