import { OrderStatusDisplay } from '../components/OrderStatusDisplay/OrderStatusDisplay.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const OrderStatusDisplayPage = () => {
  return (
    <PagePermissionGuard page="OrderStatusDisplay">
      <OrderStatusDisplay />
    </PagePermissionGuard>
  );
};
