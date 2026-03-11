import { Button, Stack, TextInput } from '@mantine/core';
import { Paper } from '../common/Paper/Paper.tsx';

export const Login = () => {
  return (
    <Paper>
      <Stack gap="md">
        <TextInput label="Username" placeholder="Enter username" />
        <TextInput label="Password" type="password" placeholder="Enter password" />
        <Button fullWidth>Login</Button>
      </Stack>
    </Paper>
  );
};
