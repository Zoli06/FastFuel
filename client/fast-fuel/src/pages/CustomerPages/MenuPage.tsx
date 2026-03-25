import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { CustomerOrderList } from '../../components/CustomerOrderCreator/CustomerOrderList.tsx';

export const MenuPage = () => {
  return (
    <>
      <Header title="Menu" />
      <CustomerOrderList />
      <Footer />
    </>
  );
};
