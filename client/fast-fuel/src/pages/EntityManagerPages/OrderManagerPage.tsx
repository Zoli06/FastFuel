import { OrderManager } from '../../components/EntityManagers/OrderManager/OrderManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const OrderManagerPage = () => {
  return (
    <PagePermissionGuard page="OrderManager">
      <OrderManager />
    </PagePermissionGuard>
  );
};
