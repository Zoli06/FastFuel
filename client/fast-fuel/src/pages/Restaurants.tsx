import { Restaurant } from '../components/Restaurant/Restaurants';
import { Footer } from '../components/Footer/Footer';
import { HeaderGeneral } from '../components/Headers/HeaderGeneral';

export const Restaurants = () => {
  return (
    <>
      <HeaderGeneral title="Restaurants" />

      <Restaurant />

      <Footer />
    </>
  );
};
