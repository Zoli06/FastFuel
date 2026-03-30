import { FoodManager } from '../../components/EntityManagers/FoodManager/FoodManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const FoodManagerPage = () => {
  return (
    <PagePermissionGuard page="FoodManager">
      <FoodManager />
    </PagePermissionGuard>
  );
};
