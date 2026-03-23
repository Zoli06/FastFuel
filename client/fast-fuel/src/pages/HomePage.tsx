import { Footer } from '../components/Footer/Footer';
import { HomeMenu } from '../components/HomeMenu/HomeMenu.tsx';
import { Header } from '../components/Header/Header.tsx';

export const HomePage = () => {
  return (
    <>
      <Header title={'Admin Panel'} />

      <HomeMenu />

      <Footer />
    </>
  );
};
