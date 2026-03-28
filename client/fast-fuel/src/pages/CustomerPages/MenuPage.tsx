import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { CustomerOrderCreator } from '../../components/CustomerOrderCreator/CustomerOrderCreator.tsx';

export const MenuPage = () => {
  return (
    <>
      <Header title="Menu" />
      <CustomerOrderCreator />
      <Footer />
    </>
  );
};
