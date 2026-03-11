import { FoodManager } from '../../components/EntityManagers/FoodManager/FoodManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';

export const FoodManagerPage = () => {
  const {
    data: foods,
    isLoading: isLoadingFoods,
    error: errorFoods,
    refetch: refetchFoods,
  } = apiClient.useQuery('get', '/api/Food');
  const {
    data: ingredients,
    isLoading: isLoadingIngredients,
    error: errorIngredients,
  } = apiClient.useQuery('get', '/api/Ingredient');

  if (isLoadingFoods || isLoadingIngredients) {
    return <LoadingPage />;
  }

  if (errorFoods || errorIngredients) {
    return <ErrorPage title="Failed to Load Foods" />;
  }

  return (
    <>
      <Header title="Food Items" />

      <FoodManager
        foods={foods || []}
        refetchFoods={refetchFoods}
        ingredients={ingredients || []}
      />

      <Footer />
    </>
  );
};
