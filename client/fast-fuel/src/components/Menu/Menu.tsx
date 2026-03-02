import { Center, SimpleGrid } from '@mantine/core';
import { MenuLink } from './MenuLink';
import { Main } from '../Main/Main.tsx';

export const Menu = () => {
  return (
    <Main>
      <SimpleGrid cols={3} spacing="md" verticalSpacing="sm">
        {[
          { text: 'Allergies', to: '/allergies' },
          { text: 'Foods', to: '/foods' },
          { text: 'Ingredients', to: '/ingredients' },
          { text: 'Menus', to: '/menus' },
          { text: 'Orders', to: '/orders' },
          { text: 'Restaurants', to: '/restaurants' },
          {},
          { text: 'Stations', to: '/stations' },
        ].map((link, index) => (
          <Center key={index}>{link.text && <MenuLink text={link.text} to={link.to} />}</Center>
        ))}
      </SimpleGrid>
    </Main>
  );
};
