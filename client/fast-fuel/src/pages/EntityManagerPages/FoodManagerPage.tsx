import { FoodManager } from '../../components/EntityManagers/FoodManager/FoodManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const FoodManagerPage = () => {
  return (
    <>
      <Header title="Food Items" />

      <FoodManager />

      <Footer />
    </>
  );
};
