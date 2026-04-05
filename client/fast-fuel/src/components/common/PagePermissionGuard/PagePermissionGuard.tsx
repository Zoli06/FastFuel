import type { ReactNode } from 'react';
import { Alert, Text } from '@mantine/core';
import type { Page } from '../../../lib/page-definitions.ts';
import { $api } from '../../../lib/api.ts';
import { Paper } from '../Paper/Paper.tsx';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const PagePermissionGuard = ({ page, children }: { page: Page; children: ReactNode }) => {
  const { noPerm } = $api(null);

  // Fetch both user permissions and page definitions using noPerm()
  const [{ data: permissions = [] }, { data: pages = [] }] = useConditionalSuspenseQueries([
    noPerm().queryOptions('get', '/api/Permission/my'),
    noPerm().queryOptions('get', '/api/Page'),
  ]);

  // Find the page definition for this page
  const pageDefinition = pages.find((p) => p.page === page);
  const necessaryPermissions = pageDefinition?.necessaryPermissions ?? [];

  // Check if user has all necessary permissions
  const hasAllNecessary = necessaryPermissions.every((permission: string) =>
    (permissions as string[]).includes(permission),
  );

  if (!hasAllNecessary) {
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
