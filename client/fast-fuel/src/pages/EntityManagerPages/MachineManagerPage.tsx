import { MachineManager } from '../../components/EntityManagers/MachineManager/MachineManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const MachineManagerPage = () => {
  return (
    <>
      <Header title="Machines" />

      <MachineManager />

      <Footer />
    </>
  );
};
