import { CustomerManager } from '../../components/EntityManagers/CustomerManager/CustomerManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const CustomerManagerPage = () => {
  return (
    <>
      <Header title="Customers" />

      <PagePermissionGuard page="CustomerManager">
        <CustomerManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
