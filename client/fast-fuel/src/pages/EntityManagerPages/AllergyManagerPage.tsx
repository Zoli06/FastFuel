import { AllergyManager } from '../../components/EntityManagers/AllergyManager/AllergyManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';

export const AllergyManagerPage = () => {
  const { data, isLoading, error, refetch } = apiClient.useQuery('get', '/api/Allergy');
  const { data: ingredients } = apiClient.useQuery('get', '/api/Ingredient');

  if (isLoading) {
    return <LoadingPage />;
  }

  if (error) {
    return <ErrorPage title="Failed to Load Restaurants" />;
  }

  return (
    <>
      <Header title="Allergies" />

      <AllergyManager
        allergies={data || []}
        refetchAllergies={refetch}
        ingredients={ingredients || []}
      />

      <Footer />
    </>
  );
};
