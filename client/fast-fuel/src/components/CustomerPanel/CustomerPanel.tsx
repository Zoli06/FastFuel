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

export const CustomerPanel = () => {
  return (
    <Paper>
      <Box pb={40}>
        <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="md" verticalSpacing="sm">
          {[
            { text: 'My Orders', to: '/customer/order' },
            { text: 'My Profile', to: '/customer/profile' },
            { text: 'Menu', to: '/customer/menu' },
            { text: 'Restaurants', to: '/customer/restaurant' },
          ].map((link, index) => (
            <Center key={index}>{link.text && <MenuLink text={link.text} to={link.to} />}</Center>
          ))}
        </SimpleGrid>
      </Box>
    </Paper>
  );
};
