import { Footer } from '../components/Footer/Footer';
import { HomeMenu } from '../components/HomeMenu/HomeMenu.tsx';
import { Header } from '../components/Header/Header.tsx';

export const HomePage = () => {
  return (
    <>
      <Header
        title={'Fast Fuel'}
        hideLeftButton={true}
        rightButtonText={'Login'}
        rightButtonNavigateTo={'/login'}
      />

      <HomeMenu />

      <Footer />
    </>
  );
};
