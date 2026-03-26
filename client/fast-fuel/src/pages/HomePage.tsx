import { useSuspenseQuery } from '@tanstack/react-query';
import { Footer } from '../components/Footer/Footer';
import { HomeMenu } from '../components/HomeMenu/HomeMenu.tsx';
import { Header } from '../components/Header/Header.tsx';
import { myCurrentUserQueryOptions } from '../lib/api-client.ts';

const panelTitles: Record<string, string> = {
  customer: 'Customer Panel',
  employee: 'Employee Panel',
  admin: 'Admin Panel',
};

export const HomePage = () => {
  const { data: currentUser } = useSuspenseQuery(myCurrentUserQueryOptions());
  const title = panelTitles[currentUser.userType.toLowerCase()] ?? 'Home';

  return (
    <>
      <Header title={title} />

      <HomeMenu />

      <Footer />
    </>
  );
};
