import { IngredientManager } from '../../components/EntityManagers/IngredientManager/IngredientManager.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';

export const IngredientManagerPage = () => {
  return (
    <>
      <Header title="Ingredients" />

      <IngredientManager />

      <Footer />
    </>
  );
};
