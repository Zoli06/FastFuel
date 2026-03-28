import { useSuspenseQuery } from '@tanstack/react-query';
import { Footer } from '../components/Footer/Footer';
import { HomeMenu } from '../components/HomeMenu/HomeMenu.tsx';
import { Header } from '../components/Header/Header.tsx';
import { myCurrentUserQueryOptions } from '../lib/api-client.ts';

export const HomePage = () => {
  const { data: currentUser } = useSuspenseQuery(myCurrentUserQueryOptions());
  const title = currentUser.userType + ' Panel';

  return (
    <>
      <Header title={title} />

      <HomeMenu />

      <Footer />
    </>
  );
};
