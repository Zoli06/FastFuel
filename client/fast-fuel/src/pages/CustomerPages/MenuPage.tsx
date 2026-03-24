import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { MenuList } from '../../components/CustomerOrderCreator/MenuList.tsx';

export const MenuPage = () => {
  return (
    <>
      <Header title="Menu" />
      <MenuList />
      <Footer />
    </>
  );
};
