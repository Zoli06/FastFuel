import { apiClient } from '../../lib/api-client.ts';
import { IngredientManager } from '../../components/EntityManagers/IngredientManager/IngredientManager.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';

export const IngredientManagerPage = () => {
  const { data: ingredients, refetch: refetchIngredients } = apiClient.useSuspenseQuery(
    'get',
    '/api/Ingredient',
  );
  const { data: allergies } = apiClient.useSuspenseQuery('get', '/api/Allergy');
  const { data: stationCategories } = apiClient.useSuspenseQuery('get', '/api/StationCategory');

  return (
    <>
      <Header title="Ingredients" />

      <IngredientManager
        ingredients={ingredients}
        refetchIngredients={refetchIngredients}
        allergies={allergies}
        stationCategories={stationCategories}
      />

      <Footer />
    </>
  );
};
