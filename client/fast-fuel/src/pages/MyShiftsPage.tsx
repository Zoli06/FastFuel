import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';
import { MyShifts } from '../components/MyShifts/MyShifts.tsx';

export const MyShiftsPage = () => {
  return (
    <PagePermissionGuard page={'MyShifts'}>
      <MyShifts />
    </PagePermissionGuard>
  );
};
