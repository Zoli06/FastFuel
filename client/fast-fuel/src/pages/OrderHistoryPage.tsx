import { OrderHistory } from '../components/OrderHistory/OrderHistory.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const OrderHistoryPage = () => (
  <PagePermissionGuard page="OrderHistory">
    <OrderHistory />
  </PagePermissionGuard>
);
