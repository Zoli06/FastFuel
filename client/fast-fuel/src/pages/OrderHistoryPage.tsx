import { Suspense } from 'react';
import { Loader, Stack } from '@mantine/core';
import { OrderHistory } from '../components/OrderHistory/OrderHistory.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const OrderHistoryPage = () => (
  <PagePermissionGuard page="OrderHistory">
    <Suspense
      fallback={
        <Stack align="center" justify="center" h="60vh">
          <Loader color="orange" />
        </Stack>
      }
    >
      <OrderHistory />
    </Suspense>
  </PagePermissionGuard>
);
