import { AdminManager } from '../../components/EntityManagers/AdminManager/AdminManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const AdminManagerPage = () => {
  return (
    <>
      <Header title="Admins" />

      <AdminManager />

      <Footer />
    </>
  );
};
