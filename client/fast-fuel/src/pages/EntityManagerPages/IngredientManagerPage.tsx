import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';
import { IngredientManager } from '../../components/EntityManagers/IngredientManager/IngredientManager.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';

export const IngredientManagerPage = () => {
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
    return <LoadingPage />;
  }

  if (errorIngredients || errorAllergies || errorStationCategories) {
    return <ErrorPage title="Failed to Load Ingredients" />;
  }

  return (
    <>
      <Header title="Ingredients" />

      <IngredientManager
        ingredients={ingredients || []}
        refetchIngredients={refetchIngredients}
        allergies={allergies || []}
        stationCategories={stationCategories || []}
      />

      <Footer />
    </>
  );
};
