import { StrictMode, Suspense } from 'react';
import { createRoot } from 'react-dom/client';
import { MantineProvider } from '@mantine/core';
import { QueryClientProvider } from '@tanstack/react-query';
import '@mantine/core/styles.css';
import '@mantine/dates/styles.css';
import '@mantine/notifications/styles.css';
import './app.css';
import { RouterProvider } from 'react-router-dom';
import { router } from './lib/router.tsx';
import { Notifications } from '@mantine/notifications';
import { queryClient } from './lib/api.ts';
import { LoadingPage } from './pages/LoadingPage.tsx';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <MantineProvider>
      <QueryClientProvider client={queryClient}>
        <Suspense fallback={<LoadingPage />}>
          <RouterProvider router={router} />
          <Notifications />
        </Suspense>
      </QueryClientProvider>
    </MantineProvider>
  </StrictMode>,
);
