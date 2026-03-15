import { MenuManager } from '../../components/EntityManagers/MenuManager/MenuManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../lib/api-client.ts';

export const MenuManagerPage = () => {
  const { data: menus, refetch: refetchMenus } = apiClient.useSuspenseQuery('get', '/api/Menu');
  const { data: foods } = apiClient.useSuspenseQuery('get', '/api/Food');

  return (
    <>
      <Header title="Menus" />

      <MenuManager menus={menus} refetchMenus={refetchMenus} foods={foods} />

      <Footer />
    </>
  );
};
