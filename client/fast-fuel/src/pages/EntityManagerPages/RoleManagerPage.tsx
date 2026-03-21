import { RoleManager } from '../../components/EntityManagers/RoleManager/RoleManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const RoleManagerPage = () => {
  return (
    <>
      <Header title="Roles" />

      <RoleManager />

      <Footer />
    </>
  );
};
