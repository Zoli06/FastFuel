import { EmployeeOrderCreator } from '../components/EmployeeOrderCreator/EmployeeOrderCreator.tsx';
import { Footer } from '../components/Footer/Footer.tsx';
import { Header } from '../components/Header/Header.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const EmployeeOrderPage = () => {
  return (
    <>
      <Header title="Create Order" />

      <PagePermissionGuard page="EmployeeOrder">
        <EmployeeOrderCreator />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
