import Food from '../components/Food.tsx';
import { apiClient } from '../apiClient.ts';

export const BrowseFoods = () => {
  const { data, error, isLoading } = apiClient.useQuery('get', '/api/Food', {});

  // TODO: make a nice loading and error component
  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div>
      <h1>Browse Foods</h1>
      {data?.map((food) => (
        <Food key={food.id} food={food} />
      ))}
    </div>
  );
};
