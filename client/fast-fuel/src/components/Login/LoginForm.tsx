import { Button, Stack, TextInput } from '@mantine/core';
import { useForm } from '@mantine/form';
import { Main } from '../Main/Main.tsx';
import { apiClient } from '../../apiClient.ts';

type LoginValues = {
  username: string;
  password: string;
};

export const LoginForm = () => {
  const form = useForm<LoginValues>({
    initialValues: {
      username: '',
      password: '',
    },
  });

  const { mutate: login, isPending } = apiClient.useMutation('post', '/api/Auth/login', {
    onSuccess: () => {
      // cookie is now stored by the browser
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
    });
  };

  return (
    <Main>
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Stack gap="md">
          <TextInput
            label="Username"
            placeholder="Enter username"
            {...form.getInputProps('username')}
          />

          <TextInput
            label="Password"
            type="password"
            placeholder="Enter password"
            {...form.getInputProps('password')}
          />

          <Button type="submit" fullWidth loading={isPending}>
            Login
          </Button>
        </Stack>
      </form>
    </Main>
  );
};
