import { ShiftManager } from '../../components/EntityManagers/ShiftManager/ShiftManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const ShiftManagerPage = () => {
  return (
    <>
      <Header title="Shifts" />

      <ShiftManager />

      <Footer />
    </>
  );
};
