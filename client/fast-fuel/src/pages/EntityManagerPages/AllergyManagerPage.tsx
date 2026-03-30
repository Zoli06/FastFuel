import { AllergyManager } from '../../components/EntityManagers/AllergyManager/AllergyManager.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const AllergyManagerPage = () => {
  return (
    <PagePermissionGuard page="AllergyManager">
      <AllergyManager />
    </PagePermissionGuard>
  );
};
