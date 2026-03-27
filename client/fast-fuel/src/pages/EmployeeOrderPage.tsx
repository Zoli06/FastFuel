import { EmployeeOrderCreator } from '../components/EmployeeOrderCreator/EmployeeOrderCreator.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const EmployeeOrderPage = () => {
  return (
    <PagePermissionGuard page="EmployeeOrder">
      <EmployeeOrderCreator />
    </PagePermissionGuard>
  );
};
