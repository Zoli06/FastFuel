import { apiClient } from '../lib/api-client.ts';
import { useParams } from 'react-router-dom';
import { Header } from '../components/Header/Header.tsx';
import { StationTasks } from '../components/StationTasks/StationTasks.tsx';
import { Footer } from '../components/Footer/Footer.tsx';
import { useSuspenseQueries } from '@tanstack/react-query';

export const StationTasksPage = () => {
  const { id } = useParams<{ id: string }>();
  const stationId = parseInt(id!);
  const [{ data: tasks, refetch: refetchTasks }, { data: station }] = useSuspenseQueries({
    queries: [
      apiClient.queryOptions(
        'get',
        '/api/Station/{id}/tasks',
        {
          params: { path: { id: stationId } },
        },
        {
          refetchInterval: 2500,
        },
      ),
      apiClient.queryOptions('get', '/api/Station/{id}', {
        params: { path: { id: stationId } },
      }),
    ] as const,
  });

  return (
    <>
      <Header title={`Tasks: ${station.name}`} />

      <StationTasks tasks={tasks} refetchTasks={refetchTasks} station={station} />

      <Footer />
    </>
  );
};
