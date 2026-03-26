import { MachineManager } from '../../components/EntityManagers/MachineManager/MachineManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const MachineManagerPage = () => {
  return (
    <>
      <Header title="Machines" />

      <PagePermissionGuard page="MachineManager">
        <MachineManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
