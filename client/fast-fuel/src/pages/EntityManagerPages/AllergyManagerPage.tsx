import { AllergyManager } from '../../components/EntityManagers/AllergyManager/AllergyManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../lib/api-client.ts';

export const AllergyManagerPage = () => {
  const { data, refetch } = apiClient.useSuspenseQuery('get', '/api/Allergy');
  const { data: ingredients } = apiClient.useSuspenseQuery('get', '/api/Ingredient');

  return (
    <>
      <Header title="Allergies" />

      <AllergyManager allergies={data} refetchAllergies={refetch} ingredients={ingredients} />

      <Footer />
    </>
  );
};
