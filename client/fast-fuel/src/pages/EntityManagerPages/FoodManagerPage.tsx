import { FoodManager } from '../../components/EntityManagers/FoodManager/FoodManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const FoodManagerPage = () => {
  return (
    <>
      <Header title="Food Items" />

      <PagePermissionGuard page="FoodManager">
        <FoodManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
