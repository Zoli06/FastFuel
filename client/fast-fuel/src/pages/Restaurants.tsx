import { Restaurant } from '../components/Restaurant/Restaurants';
import { Footer } from '../components/Footer/Footer';
import { HeaderGeneral } from '../components/Headers/HeaderGeneral';

export const Home = () => {
  return (
    <>
      <HeaderGeneral title="Restaurants" />

      <Restaurant />

      <Footer />
    </>
  );
};
