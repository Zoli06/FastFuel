import { StationCategoryManager } from '../../components/EntityManagers/StationCategoryManager/StationCategoryManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const StationCategoryManagerPage = () => {
  return (
    <>
      <Header title="Station Categories" />

      <StationCategoryManager />

      <Footer />
    </>
  );
};
