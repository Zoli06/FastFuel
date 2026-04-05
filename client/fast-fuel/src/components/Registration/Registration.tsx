import { Button, PasswordInput, Stack, TextInput } from '@mantine/core';
import { Form, useForm } from '@mantine/form';
import { Paper } from '../common/Paper/Paper';
import { useApi } from '../../lib/api.ts';
import type { components } from '../../types/api-schema.generated.ts';
import { useNavigate } from 'react-router-dom';

type RegisterFormValues = components['schemas']['CustomerRequestDto'] & {
  password: string;
  confirmPassword: string;
};

export const Register = () => {
  const navigate = useNavigate();
  const { useNoPerm, invalidateApiCache } = useApi(null);

  const form = useForm<RegisterFormValues>({
    mode: 'uncontrolled',
    initialValues: {
      name: '',
      email: '',
      userName: '',
      password: '',
      confirmPassword: '',
    } as RegisterFormValues,
    validate: {
      confirmPassword: (value, values) =>
        value !== values.password ? 'Passwords do not match' : null,
    },
  });

  const { mutateAsync: register, isPending: isRegistering } = useNoPerm().useMutation(
    'post',
    '/api/Customer',
  );

  const { mutateAsync: login, isPending: isLoggingIn } = useNoPerm().useMutation(
    'post',
    '/api/Auth/login',
  );

  const handleSubmit = async (values: RegisterFormValues) => {
    await register({
      body: {
        name: values.name,
        email: values.email,
        userName: values.userName,
        password: values.password,
      },
    });

    await login({
      body: {
        userName: values.userName,
        password: values.password,
      },
      params: { query: { useCookies: true, useSessionCookies: true } },
    });
    invalidateApiCache();
    navigate('/', { replace: true });
    form.reset();
  };

  return (
    <Paper>
      <Form form={form} onSubmit={handleSubmit}>
        <Stack gap="xs">
          <TextInput
            key={form.key('name')}
            label="Name"
            placeholder="Enter your name"
            required
            {...form.getInputProps('name')}
          />
          <TextInput
            type="email"
            key={form.key('email')}
            label="Email"
            placeholder="Enter your email"
            required
            {...form.getInputProps('email')}
          />
          <TextInput
            key={form.key('userName')}
            label="Username"
            placeholder="Enter username"
            required
            {...form.getInputProps('userName')}
          />
          <PasswordInput
            key={form.key('password')}
            label="Password"
            placeholder="Enter password"
            required
            {...form.getInputProps('password')}
          />
          <PasswordInput
            key={form.key('confirmPassword')}
            label="Confirm Password"
            placeholder="Confirm password"
            required
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
