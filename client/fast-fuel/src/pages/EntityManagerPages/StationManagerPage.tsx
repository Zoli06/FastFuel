import { StationManager } from '../../components/EntityManagers/StationManager/StationManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const StationManagerPage = () => {
  return (
    <>
      <Header title="Stations" />

      <PagePermissionGuard page="StationManager">
        <StationManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
