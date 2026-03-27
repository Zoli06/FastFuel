import { ShiftManager } from '../../components/EntityManagers/ShiftManager/ShiftManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const ShiftManagerPage = () => {
  return (
    <PagePermissionGuard page="ShiftManager">
      <ShiftManager />
    </PagePermissionGuard>
  );
};
