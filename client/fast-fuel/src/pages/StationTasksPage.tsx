import { StationTasks } from '../components/StationTasks/StationTasks.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const StationTasksPage = () => {
  return (
    <PagePermissionGuard page="StationTasks">
      <StationTasks />
    </PagePermissionGuard>
  );
};
