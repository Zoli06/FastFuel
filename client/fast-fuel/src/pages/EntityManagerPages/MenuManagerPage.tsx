import { MenuManager } from '../../components/EntityManagers/MenuManager/MenuManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';

export const MenuManagerPage = () => {
  const {
    data: menus,
    isLoading: isLoadingMenus,
    error: errorMenus,
    refetch: refetchMenus,
  } = apiClient.useQuery('get', '/api/Menu');
  const {
    data: foods,
    isLoading: isLoadingFoods,
    error: errorFoods,
  } = apiClient.useQuery('get', '/api/Food');

  if (isLoadingMenus || isLoadingFoods) {
    return <LoadingPage />;
  }

  if (errorMenus || errorFoods) {
    return <ErrorPage title="Failed to Load Menus" />;
  }

  return (
    <>
      <Header title="Menus" />

      <MenuManager menus={menus || []} refetchMenus={refetchMenus} foods={foods || []} />

      <Footer />
    </>
  );
};
