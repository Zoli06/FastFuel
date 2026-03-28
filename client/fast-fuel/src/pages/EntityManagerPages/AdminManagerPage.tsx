import { AdminManager } from '../../components/EntityManagers/AdminManager/AdminManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const AdminManagerPage = () => {
  return (
    <>
      <Header title="Admins" />

      <PagePermissionGuard page="AdminManager">
        <AdminManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
