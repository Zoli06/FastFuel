import { Center, Loader, Text, Stack, Box } from '@mantine/core';

export const LoadingScreen = () => {
  return (
    <Box pos="relative" h="100vh" w="100vw">
      <Center h="100%">
        <Stack align="center" gap="md">
          <Loader color="#904E55" size="xl" type="bars" />

          <Stack align="center" gap={4}>
            <Text size="lg" fw={700}>
              Setting things up
            </Text>
            <Text size="sm" c="dimmed">
              Loading will only take a moment...
            </Text>
          </Stack>
        </Stack>
      </Center>
    </Box>
  );
};
