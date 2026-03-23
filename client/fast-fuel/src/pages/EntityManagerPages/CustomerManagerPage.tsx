import { CustomerManager } from '../../components/EntityManagers/CustomerManager/CustomerManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const CustomerManagerPage = () => {
  return (
    <>
      <Header title="Customers" />

      <CustomerManager />

      <Footer />
    </>
  );
};
