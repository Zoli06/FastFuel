import { Button, Stack, TextInput } from '@mantine/core';
import { Main } from '../Main/Main.tsx';

export const LoginForm = () => {
  return (
    <Main>
      <Stack gap="md">
        <TextInput label="Username" placeholder="Enter username" />
        <TextInput label="Password" type="password" placeholder="Enter password" />
        <Button fullWidth>Login</Button>
      </Stack>
    </Main>
  );
};
