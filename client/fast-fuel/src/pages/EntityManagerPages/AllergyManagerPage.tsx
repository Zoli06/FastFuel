import { AllergyManager } from '../../components/EntityManagers/AllergyManager/AllergyManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const AllergyManagerPage = () => {
  return (
    <>
      <Header title="Allergies" />

      <PagePermissionGuard page="AllergyManager">
        <AllergyManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
