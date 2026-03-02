import { Container, Paper } from '@mantine/core';
import type { ReactNode } from 'react';

// TODO: Move this to an outlet
export const Main = ({ children }: { children: ReactNode }) => {
  return (
    <Container>
      <Paper p="xl" bg="beige" withBorder>
        {children}
      </Paper>
    </Container>
  );
};
