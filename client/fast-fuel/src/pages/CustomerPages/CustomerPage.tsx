import { Footer } from '../../components/Footer/Footer';
import { CustomerMenu } from '../../components/CustomerMenu/Customer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const CustomerPage = () => {
  return (
    <>
      <Header title={'Customer Panel'} />

      <CustomerMenu />

      <Footer />
    </>
  );
};
