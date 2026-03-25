import { Footer } from '../../components/Footer/Footer';
import { CustomerPanel } from '../../components/CustomerPanel/CustomerPanel.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const CustomerPage = () => {
  return (
    <>
      <Header title={'Customer Panel'} />

      <CustomerPanel />

      <Footer />
    </>
  );
};
