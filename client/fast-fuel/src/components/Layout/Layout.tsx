import { Header } from '../Header/Header.tsx';
import { Footer } from '../Footer/Footer.tsx';
import { Outlet, useMatches } from 'react-router-dom';
import type { HeaderAuthButton } from '../Header/Header.tsx';
import { useEffect, useState } from 'react';

type LayoutHandle = {
  header?: {
    title?: string;
    authButton?: HeaderAuthButton;
    showHomeButton?: boolean;
    showAuthButton?: boolean;
  };
  layout?: {
    hideChromeInFullscreen?: boolean;
  };
};

export const Layout = () => {
  const matches = useMatches();
  const handles = matches.map((match) => match.handle as LayoutHandle | undefined);
  const [isFullscreen, setIsFullscreen] = useState(Boolean(document.fullscreenElement));

  const headerConfig = [...handles].reverse().find((handle) => handle?.header)?.header;
  const hideChromeInFullscreen =
    [...handles].reverse().find((handle) => handle?.layout)?.layout?.hideChromeInFullscreen ??
    false;
  const showChrome = !(hideChromeInFullscreen && isFullscreen);

  const title = headerConfig?.title ?? 'Fast Fuel';

  useEffect(() => {
    const handleFullscreenChange = () => {
      setIsFullscreen(Boolean(document.fullscreenElement));
    };

    document.addEventListener('fullscreenchange', handleFullscreenChange);
    return () => document.removeEventListener('fullscreenchange', handleFullscreenChange);
  }, []);

  return (
    <>
      {showChrome && (
        <Header
          title={title}
          authButton={headerConfig?.authButton}
          showHomeButton={headerConfig?.showHomeButton}
          showAuthButton={headerConfig?.showAuthButton}
        />
      )}
      <Outlet />

      {showChrome && <Footer />}
    </>
  );
};
