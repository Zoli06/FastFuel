import { StationManager } from '../../components/EntityManagers/StationManager/StationManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const StationManagerPage = () => {
  return (
    <PagePermissionGuard page="StationManager">
      <StationManager />
    </PagePermissionGuard>
  );
};
