import { Container, Title, Text, Button, Stack, Center, Box } from '@mantine/core';

export type ErrorPageProps = {
  title?: string;
  message?: string;
  onRetry?: () => void;
};

export const ErrorPage = ({
  title = 'An Error Occurred',
  message = 'Please try refreshing the page or head back to the home screen.',
  onRetry,
}: ErrorPageProps) => {
  return (
    <Box h="100vh" w="100vw" bg="gray.0">
      <Center h="100%">
        <Container size="sm">
          <Stack align="center" gap="lg" style={{ textAlign: 'center' }}>
            <Text size="120px" fw={900} c="gray.2" style={{ lineHeight: 1, marginBottom: -40 }}>
              !
            </Text>

            <Title order={1} size="h2" fw={800} c="dark">
              {title}
            </Title>

            <Text c="dimmed" size="lg" maw={480}>
              {message}
            </Text>

            <Stack gap="sm" mt="md" w="100%" maw={200}>
              <Button
                variant="filled"
                color="#904E55"
                size="md"
                onClick={() => window.location.reload()}
              >
                Refresh Page
              </Button>

              {onRetry && (
                <Button variant="light" color="gray" size="md" onClick={onRetry}>
                  Go Back
                </Button>
              )}
            </Stack>
          </Stack>
        </Container>
      </Center>
    </Box>
  );
};
