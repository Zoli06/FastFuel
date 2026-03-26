import { StationCategoryManager } from '../../components/EntityManagers/StationCategoryManager/StationCategoryManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const StationCategoryManagerPage = () => {
  return (
    <>
      <Header title="Station Categories" />

      <PagePermissionGuard page="StationCategoryManager">
        <StationCategoryManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
