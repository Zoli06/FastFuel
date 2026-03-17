import { StationCategoryManager } from '../../components/EntityManagers/StationCategoryManager/StationCategoryManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../lib/api-client.ts';

export const StationCategoryManagerPage = () => {
  const { data: stationCategories, refetch: refetchStationCategories } = apiClient.useSuspenseQuery(
    'get',
    '/api/StationCategory',
  );
  const { data: ingredients } = apiClient.useSuspenseQuery('get', '/api/Ingredient');

  return (
    <>
      <Header title="Station Categories" />

      <StationCategoryManager
        stationCategories={stationCategories}
        refetchStationCategories={refetchStationCategories}
        ingredients={ingredients}
      />

      <Footer />
    </>
  );
};
