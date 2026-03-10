import { RestaurantManager } from '../../components/EntityManagers/RestaurantManager/RestaurantManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';

export const RestaurantManagerPage = () => {
  const { data, isLoading, error, refetch } = apiClient.useQuery('get', '/api/Restaurant');

  if (isLoading) {
    return <LoadingPage />;
  }

  if (error) {
    return <ErrorPage title="Failed to Load Restaurants" />;
  }

  return (
    <>
      <Header title="Restaurants" />

      <RestaurantManager restaurants={data || []} refetchRestaurants={refetch} />

      <Footer />
    </>
  );
};
