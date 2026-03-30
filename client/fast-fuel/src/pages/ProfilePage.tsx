import { Suspense } from 'react';
import { Loader, Stack } from '@mantine/core';
import { Profile } from '../components/Profile/Profile.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const ProfilePage = () => (
  <PagePermissionGuard page="Profile">
    <Suspense
      fallback={
        <Stack align="center" justify="center" h="60vh">
          <Loader color="orange" />
        </Stack>
      }
    >
      <Profile />
    </Suspense>
  </PagePermissionGuard>
);
