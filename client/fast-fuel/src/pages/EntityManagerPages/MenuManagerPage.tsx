import { MenuManager } from '../../components/EntityManagers/MenuManager/MenuManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const MenuManagerPage = () => {
  return (
    <PagePermissionGuard page="MenuManager">
      <MenuManager />
    </PagePermissionGuard>
  );
};
