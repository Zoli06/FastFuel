import { RestaurantManager } from '../../components/EntityManagers/RestaurantManager/RestaurantManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const RestaurantManagerPage = () => {
  return (
    <>
      <Header title="Restaurants" />

      <PagePermissionGuard page="RestaurantManager">
        <RestaurantManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
