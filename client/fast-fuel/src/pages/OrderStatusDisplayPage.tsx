import { OrderStatusDisplay } from '../components/OrderStatusDisplay/OrderStatusDisplay.tsx';
import { useParams } from 'react-router-dom';
import { ErrorPage } from './ErrorPage.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const OrderStatusDisplayPage = () => {
  const { id } = useParams<{ id: string }>();
  const restaurantId = Number(id);

  if (!Number.isFinite(restaurantId)) {
    return <ErrorPage />;
  }

  return (
    <PagePermissionGuard page="OrderStatusDisplay">
      <OrderStatusDisplay restaurantId={restaurantId} />
    </PagePermissionGuard>
  );
};
