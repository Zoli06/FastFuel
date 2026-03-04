import { Allergies } from '../components/Allergies/Allergies';
import { Footer } from '../components/Footer/Footer';
import { HeaderGeneral } from '../components/Headers/HeaderGeneral';
import { apiClient } from '../apiClient.ts';
import { LoadingScreen } from './LoadingPage.tsx';
import { ErrorPage } from './ErrorPage.tsx';

export const AllergiesPage = () => {
  const { data, isLoading, error, refetch } = apiClient.useQuery('get', '/api/Allergy');
  const { data: ingredients } = apiClient.useQuery('get', '/api/Ingredient');

  if (isLoading) {
    return <LoadingScreen />;
  }

  if (error) {
    return <ErrorPage title="Failed to Load Restaurants" />;
  }

  return (
    <>
      <HeaderGeneral title="Allergies" />

      <Allergies
        allergies={data || []}
        refetchAllergies={refetch}
        ingredients={ingredients || []}
      />

      <Footer />
    </>
  );
};
