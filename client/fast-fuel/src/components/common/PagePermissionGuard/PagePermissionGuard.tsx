import type { ReactNode } from 'react';
import { Alert, Text } from '@mantine/core';
import type { Page } from '../../../lib/page-definitions.ts';
import { usePagePermissions } from '../../../hooks/usePagePermissions.ts';
import { Paper } from '../Paper/Paper.tsx';

export const PagePermissionGuard = ({ page, children }: { page: Page; children: ReactNode }) => {
  const perms = usePagePermissions(page, { split: true });

  if (!perms.hasNecessary) {
    return (
      <Paper>
        <Alert icon="⚠️" color="red" title="Access Denied">
          <Text>You do not have the necessary permissions to access this page.</Text>
        </Alert>
      </Paper>
    );
  }

  return <>{children}</>;
};
