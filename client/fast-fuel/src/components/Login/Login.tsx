import { Anchor, Button, PasswordInput, Stack, TextInput } from '@mantine/core';
import { Form, useForm } from '@mantine/form';
import { Link, useNavigate } from 'react-router-dom';
import { Paper } from '../common/Paper/Paper';
import { useApi } from '../../lib/api.ts';
import type { components } from '../../types/api-schema.generated.ts';

type LoginValues = components['schemas']['LoginRequestDto'];

export const Login = () => {
  const navigate = useNavigate();
  const { useNoPerm, invalidateApiCache } = useApi(null);

  const form = useForm<LoginValues>({
    initialValues: { userName: '', password: '' },
  });

  const { mutateAsync: login, isPending } = useNoPerm().useMutation('post', '/api/Auth/login');

  const handleSubmit = async (values: LoginValues) => {
    try {
      await login({
        body: values,
        params: { query: { useCookies: true, useSessionCookies: true } },
      });
      invalidateApiCache();
      navigate('/', { replace: true });
    } catch {
      form.setFieldError('password', 'Incorrect username or password');
    }
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
