import { ShiftManager } from '../../components/EntityManagers/ShiftManager/ShiftManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const ShiftManagerPage = () => {
  return (
    <>
      <Header title="Shifts" />

      <PagePermissionGuard page="ShiftManager">
        <ShiftManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
