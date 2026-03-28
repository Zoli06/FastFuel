import { Header } from '../Header/Header.tsx';
import { Footer } from '../Footer/Footer.tsx';
import { Outlet, useMatches } from 'react-router-dom';
import type { HeaderAuthButton } from '../Header/Header.tsx';

type LayoutHandle = {
  header?: {
    title?: string;
    authButton?: HeaderAuthButton;
  };
};

export const Layout = () => {
  const matches = useMatches();
  const handles = matches.map((match) => match.handle as LayoutHandle | undefined);

  const headerConfig = [...handles].reverse().find((handle) => handle?.header)?.header;

  const title = headerConfig?.title ?? 'Fast Fuel';

  return (
    <>
      <Header title={title} authButton={headerConfig?.authButton} />

      <Outlet />

      <Footer />
    </>
  );
};
