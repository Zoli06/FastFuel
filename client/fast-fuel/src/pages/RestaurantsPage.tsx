import { Restaurants } from '../components/Restaurant/Restaurants';
import { Footer } from '../components/Footer/Footer';
import { HeaderGeneral } from '../components/Headers/HeaderGeneral';
import { apiClient } from '../apiClient.ts';

export const RestaurantsPage = () => {
  const { data, isLoading, error, refetch } = apiClient.useQuery('get', '/api/Restaurant');

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <>
      <HeaderGeneral title="Restaurants" />

      <Restaurants restaurants={data || []} refetchRestaurants={refetch} />

      <Footer />
    </>
  );
};
