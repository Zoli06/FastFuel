import { useParams } from 'react-router-dom';
import { StationTasks } from '../components/StationTasks/StationTasks.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const StationTasksPage = () => {
  const { id } = useParams<{ id: string }>();
  const stationId = parseInt(id!);

  return (
    <PagePermissionGuard page="StationTasks">
      <StationTasks stationId={stationId} />
    </PagePermissionGuard>
  );
};
