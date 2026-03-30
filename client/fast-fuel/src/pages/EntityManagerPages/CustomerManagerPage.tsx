import { CustomerManager } from '../../components/EntityManagers/CustomerManager/CustomerManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const CustomerManagerPage = () => {
  return (
    <PagePermissionGuard page="CustomerManager">
      <CustomerManager />
    </PagePermissionGuard>
  );
};
