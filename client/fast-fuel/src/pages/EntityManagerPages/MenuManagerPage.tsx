import { MenuManager } from '../../components/EntityManagers/MenuManager/MenuManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const MenuManagerPage = () => {
  return (
    <>
      <Header title="Menus" />

      <MenuManager />

      <Footer />
    </>
  );
};
