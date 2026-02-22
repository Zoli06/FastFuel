import { Container, Button, Stack, Paper, TextInput } from '@mantine/core';

export const Loginform = () => {
  return (
    <Container size="xs">
      <Paper p="xl" withBorder bg="var(--mantine-color-gray-0)">
        <Stack gap="md">
          <TextInput label="Username" placeholder="Enter username" />
          <TextInput label="Password" type="password" placeholder="Enter password" />
          <Button fullWidth>Login</Button>
        </Stack>
      </Paper>
    </Container>
  );
};
