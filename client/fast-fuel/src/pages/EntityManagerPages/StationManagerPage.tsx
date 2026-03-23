import { StationManager } from '../../components/EntityManagers/StationManager/StationManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const StationManagerPage = () => {
  return (
    <>
      <Header title="Stations" />

      <StationManager />

      <Footer />
    </>
  );
};
