import { MachineManager } from '../../components/EntityManagers/MachineManager/MachineManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const MachineManagerPage = () => {
  return (
    <PagePermissionGuard page="MachineManager">
      <MachineManager />
    </PagePermissionGuard>
  );
};
