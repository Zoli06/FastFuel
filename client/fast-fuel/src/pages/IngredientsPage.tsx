import { apiClient } from '../apiClient.ts';
import { LoadingScreen } from './LoadingPage.tsx';
import { ErrorPage } from './ErrorPage.tsx';
import { Ingredients } from '../components/Ingredients/Ingredients.tsx';

export const IngredientsPage = () => {
  const {
    data: ingredients,
    isLoading: isLoadingIngredients,
    error: errorIngredients,
    refetch: refetchIngredients,
  } = apiClient.useQuery('get', '/api/Ingredient');
  const {
    data: allergies,
    isLoading: isLoadingAllergies,
    error: errorAllergies,
  } = apiClient.useQuery('get', '/api/Allergy');
  const {
    data: stationCategories,
    isLoading: isLoadingStationCategories,
    error: errorStationCategories,
  } = apiClient.useQuery('get', '/api/StationCategory');

  if (isLoadingIngredients || isLoadingAllergies || isLoadingStationCategories) {
    return <LoadingScreen />;
  }

  if (errorIngredients || errorAllergies || errorStationCategories) {
    return <ErrorPage title="Failed to Load Ingredients" />;
  }

  return (
    <Ingredients
      ingredients={ingredients || []}
      refetchIngredients={refetchIngredients}
      allergies={allergies || []}
      stationCategories={stationCategories || []}
    />
  );
};
