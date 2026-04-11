import { Box, Button, Container, Image, Paper, Stack, Text, Title } from '@mantine/core';
import { Link } from 'react-router-dom';
import welcomeBg from './WelcomeBg_transitioned.webp';

export const Welcome = () => {
  return (
    <Container size="lg" py={80}>
      <Paper
        pos="relative"
        radius="xl"
        p={{ base: 'xl', sm: '3rem' }}
        mih={420}
        style={{
          overflow: 'hidden',
          background:
            'linear-gradient(145deg, rgba(255, 245, 230, 0.98) 0%, rgba(255, 225, 180, 0.96) 52%, rgba(255, 204, 153, 0.94) 100%)',
          border: '1px solid rgba(140, 72, 22, 0.16)',
          boxShadow: '0 24px 60px rgba(140, 72, 22, 0.12)',
        }}
      >
        <Box
          style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))',
            gap: '2rem',
            alignItems: 'center',
          }}
        >
          <Stack gap="lg" justify="center" maw={520}>
            <Text fw={700} tt="uppercase" c="orange.8" size="sm">
              Welcome to Fast Fuel
            </Text>
            <Title order={1} fz={{ base: '2.5rem', sm: '3.5rem' }} lh={1.05}>
              Fast orders, clear workflows, and more all in one.
            </Title>
            <Button
              component={Link}
              to="/login"
              size="lg"
              radius="xl"
              w={{ base: '100%', sm: 'fit-content' }}
              color="dark"
            >
              Login
            </Button>
          </Stack>

          <Image
            src={welcomeBg}
            alt="Fast Fuel welcome background"
            radius="lg"
            fit="cover"
            h={{ base: 220, sm: 320 }}
            style={{
              pointerEvents: 'none',
              userSelect: 'none',
              boxShadow: '0 18px 40px rgba(120, 68, 24, 0.18)',
            }}
          />
        </Box>
      </Paper>
    </Container>
  );
};
