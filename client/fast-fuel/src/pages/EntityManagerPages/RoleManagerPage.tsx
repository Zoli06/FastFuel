import { RoleManager } from '../../components/EntityManagers/RoleManager/RoleManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const RoleManagerPage = () => {
  return (
    <>
      <Header title="Roles" />

      <PagePermissionGuard page="RoleManager">
        <RoleManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
