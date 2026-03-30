import { Container, Paper as MantinePaper } from '@mantine/core';
import type { ReactNode } from 'react';

// TODO: Move this to an outlet
export const Paper = ({ children }: { children: ReactNode }) => {
  return (
    <Container>
      <MantinePaper p="xl" bg="beige" withBorder>
        {children}
      </MantinePaper>
    </Container>
  );
};
