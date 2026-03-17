import { RestaurantManager } from '../../components/EntityManagers/RestaurantManager/RestaurantManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const RestaurantManagerPage = () => {
  return (
    <>
      <Header title="Restaurants" />

      <RestaurantManager />

      <Footer />
    </>
  );
};
