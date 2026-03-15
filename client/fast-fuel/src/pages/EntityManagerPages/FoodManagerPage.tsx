import { FoodManager } from '../../components/EntityManagers/FoodManager/FoodManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../lib/api-client.ts';

export const FoodManagerPage = () => {
  const { data: foods, refetch: refetchFoods } = apiClient.useSuspenseQuery('get', '/api/Food');
  const { data: ingredients } = apiClient.useSuspenseQuery('get', '/api/Ingredient');

  return (
    <>
      <Header title="Food Items" />

      <FoodManager foods={foods} refetchFoods={refetchFoods} ingredients={ingredients} />

      <Footer />
    </>
  );
};
