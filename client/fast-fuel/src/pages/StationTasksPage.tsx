import { apiClient } from '../lib/api-client.ts';
import { useParams } from 'react-router-dom';
import { Header } from '../components/Header/Header.tsx';
import { StationTasks } from '../components/StationTasks/StationTasks.tsx';
import { Footer } from '../components/Footer/Footer.tsx';

export const StationTasksPage = () => {
  const { id } = useParams<{ id: string }>();
  const { data: tasks, refetch: refetchTasks } = apiClient.useSuspenseQuery(
    'get',
    '/api/Station/{id}/tasks',
    {
      params: { path: { id: parseInt(id!) } },
    },
    {
      refetchInterval: 2500,
    },
  );
  const { data: station } = apiClient.useSuspenseQuery('get', '/api/Station/{id}', {
    params: { path: { id: parseInt(id!) } },
  });

  return (
    <>
      <Header title={`Tasks: ${station.name}`} />

      <StationTasks tasks={tasks} refetchTasks={refetchTasks} station={station} />

      <Footer />
    </>
  );
};
