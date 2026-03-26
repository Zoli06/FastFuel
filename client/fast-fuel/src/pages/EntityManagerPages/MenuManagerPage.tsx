import { MenuManager } from '../../components/EntityManagers/MenuManager/MenuManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const MenuManagerPage = () => {
  return (
    <>
      <Header title="Menus" />

      <PagePermissionGuard page="MenuManager">
        <MenuManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
