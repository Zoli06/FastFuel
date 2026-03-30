import { RoleManager } from '../../components/EntityManagers/RoleManager/RoleManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const RoleManagerPage = () => {
  return (
    <PagePermissionGuard page="RoleManager">
      <RoleManager />
    </PagePermissionGuard>
  );
};
