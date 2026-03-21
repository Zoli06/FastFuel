import { useParams } from 'react-router-dom';
import { StationTasks } from '../components/StationTasks/StationTasks.tsx';

export const StationTasksPage = () => {
  const { id } = useParams<{ id: string }>();
  const stationId = parseInt(id!);

  return <StationTasks stationId={stationId} />;
};
