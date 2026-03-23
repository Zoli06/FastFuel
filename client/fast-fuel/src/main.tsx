import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { MantineProvider } from '@mantine/core';
import { QueryClientProvider } from '@tanstack/react-query';
import '@mantine/core/styles.css';
import '@mantine/dates/styles.css';
import '@mantine/notifications/styles.css';
import { RouterProvider } from 'react-router-dom';
import { router } from './lib/router.tsx';
import { Notifications } from '@mantine/notifications';
import { queryClient } from './lib/query-client.ts';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <MantineProvider>
      <QueryClientProvider client={queryClient}>
        <RouterProvider router={router} />
        <Notifications />
      </QueryClientProvider>
    </MantineProvider>
  </StrictMode>,
);
