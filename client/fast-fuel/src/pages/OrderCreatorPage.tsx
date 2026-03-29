import { OrderCreator } from '../components/OrderCreator/OrderCreator.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const OrderCreatorPage = () => {
  return (
    <PagePermissionGuard page={'OrderCreator'}>
      <OrderCreator />
    </PagePermissionGuard>
  );
};
