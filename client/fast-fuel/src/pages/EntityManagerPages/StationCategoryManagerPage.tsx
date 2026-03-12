import { StationCategoryManager } from '../../components/EntityManagers/StationCategoryManager/StationCategoryManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';

export const StationCategoryManagerPage = () => {
  const {
    data: stationCategories,
    isLoading: isLoadingCategories,
    error: errorCategories,
    refetch: refetchStationCategories,
  } = apiClient.useQuery('get', '/api/StationCategory');
  const {
    data: ingredients,
    isLoading: isLoadingIngredients,
    error: errorIngredients,
  } = apiClient.useQuery('get', '/api/Ingredient');

  if (isLoadingCategories || isLoadingIngredients) {
    return <LoadingPage />;
  }

  if (errorCategories || errorIngredients) {
    return <ErrorPage title="Failed to Load Station Categories" />;
  }

  return (
    <>
      <Header title="Station Categories" />

      <StationCategoryManager
        stationCategories={stationCategories || []}
        refetchStationCategories={refetchStationCategories}
        ingredients={ingredients || []}
      />

      <Footer />
    </>
  );
};
