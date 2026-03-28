import { IngredientManager } from '../../components/EntityManagers/IngredientManager/IngredientManager.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { PagePermissionGuard } from '../../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const IngredientManagerPage = () => {
  return (
    <>
      <Header title="Ingredients" />

      <PagePermissionGuard page="IngredientManager">
        <IngredientManager />
      </PagePermissionGuard>

      <Footer />
    </>
  );
};
