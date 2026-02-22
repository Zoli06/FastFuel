import { Footer } from '../components/Footer/Footer';
import { Menu } from '../components/Menu/Menu';
import { HeaderGeneral } from '../components/Headers/HeaderGeneral.tsx';

export const HomePage = () => {
  return (
    <>
      <HeaderGeneral
        title={'Fast Fuel'}
        hideLeftButton={true}
        rightButtonText={'Login'}
        rightButtonNavigateTo={'/login'}
      />

      <Menu />

      <Footer />
    </>
  );
};
