import { Box, Center, SimpleGrid, UnstyledButton } from '@mantine/core';
import { Paper } from '../common/Paper/Paper.tsx';
import { Link } from 'react-router-dom';

const MenuLink = ({ text, to }: { text: string; to: string }) => {
  return (
    <UnstyledButton
      component={Link}
      to={to}
      p="xs"
      px="sm"
      fw="bold"
      fz={{ base: '1.5rem', sm: '2rem', md: '3rem' }}
      c="darkred"
    >
      {text}
    </UnstyledButton>
  );
};

export const HomeMenu = () => {
  return (
    <Paper>
      <Box pb={40}>
        <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="md" verticalSpacing="sm">
          {[
            { text: 'Allergies', to: '/manage/allergy' },
            { text: 'Employees', to: '/manage/employee' },
            { text: 'Foods', to: '/manage/food' },
            { text: 'Ingredients', to: '/manage/ingredient' },
            { text: 'Menus', to: '/manage/menu' },
            { text: 'Orders', to: '/manage/order' },
            { text: 'Restaurants', to: '/manage/restaurant' },
            { text: 'Shifts', to: '/manage/shift' },
            { text: 'Station Categories', to: '/manage/station-category' },
            { text: 'Stations', to: '/manage/station' },
          ].map((link, index) => (
            <Center key={index}>{link.text && <MenuLink text={link.text} to={link.to} />}</Center>
          ))}
        </SimpleGrid>
      </Box>
    </Paper>
  );
};
