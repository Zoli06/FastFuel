import { Restaurants } from '../components/Restaurant/Restaurants';
import { Footer } from '../components/Footer/Footer';
import { HeaderGeneral } from '../components/Headers/HeaderGeneral';

export const RestaurantsPage = () => {
  return (
    <>
      <HeaderGeneral title="RestaurantsPage" />

      <Restaurants />

      <Footer />
    </>
  );
};
