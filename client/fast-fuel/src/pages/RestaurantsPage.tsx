import { Restaurants } from '../components/Restaurant/Restaurants';
import { Footer } from '../components/Footer/Footer';
import { HeaderGeneral } from '../components/Headers/HeaderGeneral';
import { apiClient } from '../apiClient.ts';
import { LoadingScreen } from './LoadingPage.tsx';
import { ErrorPage } from './ErrorPage.tsx';

export const RestaurantsPage = () => {
  const { data, isLoading, error, refetch } = apiClient.useQuery('get', '/api/Restaurant');

  if (isLoading) {
    return <LoadingScreen />;
  }

  if (error) {
    return <ErrorPage title="Failed to Load Restaurants" />;
  }

  return (
    <>
      <HeaderGeneral title="Restaurants" />

      <Restaurants restaurants={data || []} refetchRestaurants={refetch} />

      <Footer />
    </>
  );
};
