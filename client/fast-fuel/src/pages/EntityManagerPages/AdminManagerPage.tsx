import { AdminManager } from '../../components/EntityManagers/AdminManager/AdminManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const AdminManagerPage = () => {
  return (
    <PagePermissionGuard page="AdminManager">
      <AdminManager />
    </PagePermissionGuard>
  );
};
