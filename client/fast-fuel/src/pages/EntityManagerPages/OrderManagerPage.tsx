import { OrderManager } from '../../components/EntityManagers/OrderManager/OrderManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const OrderManagerPage = () => {
  return (
    <>
      <Header title="Orders" />

      <OrderManager />

      <Footer />
    </>
  );
};
