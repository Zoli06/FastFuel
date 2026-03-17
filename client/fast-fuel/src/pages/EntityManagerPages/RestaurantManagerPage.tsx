import { RestaurantManager } from '../../components/EntityManagers/RestaurantManager/RestaurantManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../lib/api-client.ts';

export const RestaurantManagerPage = () => {
  const { data, refetch } = apiClient.useSuspenseQuery('get', '/api/Restaurant');

  return (
    <>
      <Header title="Restaurants" />

      <RestaurantManager restaurants={data} refetchRestaurants={refetch} />

      <Footer />
    </>
  );
};
