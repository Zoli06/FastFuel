import { RestaurantManager } from '../../components/EntityManagers/RestaurantManager/RestaurantManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const RestaurantManagerPage = () => {
  return (
    <PagePermissionGuard page="RestaurantManager">
      <RestaurantManager />
    </PagePermissionGuard>
  );
};
