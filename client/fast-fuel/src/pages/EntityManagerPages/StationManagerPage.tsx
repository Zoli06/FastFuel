import { StationManager } from '../../components/EntityManagers/StationManager/StationManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../lib/api-client.ts';

export const StationManagerPage = () => {
  const { data: stations, refetch: refetchStations } = apiClient.useSuspenseQuery(
    'get',
    '/api/Station',
  );
  const { data: restaurants } = apiClient.useSuspenseQuery('get', '/api/Restaurant');
  const { data: stationCategories } = apiClient.useSuspenseQuery('get', '/api/StationCategory');

  return (
    <>
      <Header title="Stations" />

      <StationManager
        stations={stations}
        refetchStations={refetchStations}
        restaurants={restaurants}
        stationCategories={stationCategories}
      />

      <Footer />
    </>
  );
};
