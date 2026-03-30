import { StationCategoryManager } from '../../components/EntityManagers/StationCategoryManager/StationCategoryManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const StationCategoryManagerPage = () => {
  return (
    <PagePermissionGuard page="StationCategoryManager">
      <StationCategoryManager />
    </PagePermissionGuard>
  );
};
