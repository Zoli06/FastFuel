import { IngredientManager } from '../../components/EntityManagers/IngredientManager/IngredientManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const IngredientManagerPage = () => {
  return (
    <PagePermissionGuard page="IngredientManager">
      <IngredientManager />
    </PagePermissionGuard>
  );
};
