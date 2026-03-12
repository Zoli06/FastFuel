import { StationManager } from '../../components/EntityManagers/StationManager/StationManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';

export const StationManagerPage = () => {
  const {
    data: stations,
    isLoading: isLoadingStations,
    error: errorStations,
    refetch: refetchStations,
  } = apiClient.useQuery('get', '/api/Station');
  const {
    data: restaurants,
    isLoading: isLoadingRestaurants,
    error: errorRestaurants,
  } = apiClient.useQuery('get', '/api/Restaurant');
  const {
    data: stationCategories,
    isLoading: isLoadingCategories,
    error: errorCategories,
  } = apiClient.useQuery('get', '/api/StationCategory');

  if (isLoadingStations || isLoadingRestaurants || isLoadingCategories) {
    return <LoadingPage />;
  }

  if (errorStations || errorRestaurants || errorCategories) {
    return <ErrorPage title="Failed to Load Stations" />;
  }

  return (
    <>
      <Header title="Stations" />

      <StationManager
        stations={stations || []}
        refetchStations={refetchStations}
        restaurants={restaurants || []}
        stationCategories={stationCategories || []}
      />

      <Footer />
    </>
  );
};
