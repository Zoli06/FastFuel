import { AllergyManager } from '../../components/EntityManagers/AllergyManager/AllergyManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const AllergyManagerPage = () => {
  return (
    <>
      <Header title="Allergies" />

      <AllergyManager />

      <Footer />
    </>
  );
};
