import { EmployeeManager } from '../../components/EntityManagers/EmployeeManager/EmployeeManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const EmployeeManagerPage = () => {
  return (
    <PagePermissionGuard page="EmployeeManager">
      <EmployeeManager />
    </PagePermissionGuard>
  );
};
