import { Foods } from '../components/Foods/Foods.tsx';
import { Footer } from '../components/Footer/Footer';
import { HeaderGeneral } from '../components/Headers/HeaderGeneral';
import { apiClient } from '../apiClient.ts';
import { LoadingScreen } from './LoadingPage.tsx';
import { ErrorPage } from './ErrorPage.tsx';

export const FoodsPage = () => {
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
    return <LoadingScreen />;
  }

  if (errorFoods || errorIngredients) {
    return <ErrorPage title="Failed to Load Foods" />;
  }

  return (
    <>
      <HeaderGeneral title="Food Items" />

      <Foods foods={foods || []} refetchFoods={refetchFoods} ingredients={ingredients || []} />

      <Footer />
    </>
  );
};
