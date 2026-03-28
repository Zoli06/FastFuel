import { OrderManager } from '../../components/EntityManagers/OrderManager/OrderManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const OrderManagerPage = () => {
  return (
    <>
      <Header title="Orders" />

      <PagePermissionGuard page="OrderManager">
        <OrderManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
