import { Box, Center, SimpleGrid, Text, UnstyledButton } from '@mantine/core';
import { Paper } from '../common/Paper/Paper.tsx';
import { Link } from 'react-router-dom';
import { myPermissionsQueryOptions } from '../../lib/api-client.ts';
import { useSuspenseQuery } from '@tanstack/react-query';

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
  const { data: permissions } = useSuspenseQuery(myPermissionsQueryOptions());
  const menuLinks: {
    text: string;
    to: string;
    requiredPermission: (typeof permissions)[number];
  }[] = [
    {
      text: 'Allergies',
      to: '/manage/allergy',
      requiredPermission: 'Permission:Allergy:Read',
    },
    {
      text: 'Employees',
      to: '/manage/employee',
      requiredPermission: 'Permission:Employee:Read',
    },
    { text: 'Foods', to: '/manage/food', requiredPermission: 'Permission:Food:Read' },
    {
      text: 'Ingredients',
      to: '/manage/ingredient',
      requiredPermission: 'Permission:Ingredient:Read',
    },
    { text: 'Menus', to: '/manage/menu', requiredPermission: 'Permission:Menu:Read' },
    { text: 'Orders', to: '/manage/order', requiredPermission: 'Permission:Order:Read' },
    { text: 'Roles', to: '/manage/role', requiredPermission: 'Permission:Role:Read' },
    {
      text: 'Restaurants',
      to: '/manage/restaurant',
      requiredPermission: 'Permission:Restaurant:Read',
    },
    { text: 'Shifts', to: '/manage/shift', requiredPermission: 'Permission:Shift:Read' },
    {
      text: 'Station Categories',
      to: '/manage/station-category',
      requiredPermission: 'Permission:StationCategory:Read',
    },
    {
      text: 'Stations',
      to: '/manage/station',
      requiredPermission: 'Permission:Station:Read',
    },
  ];
  const visibleLinks = menuLinks.filter((link) => permissions?.includes(link.requiredPermission));

  return (
    <Paper>
      <Box pb={40}>
        {visibleLinks.length > 0 ? (
          <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="md" verticalSpacing="sm">
            {visibleLinks.map((link, index) => (
              <Center key={index}>{link.text && <MenuLink text={link.text} to={link.to} />}</Center>
            ))}
          </SimpleGrid>
        ) : (
          <Center>
            <Text c="dimmed" fw={500}>
              No accessible modules.
            </Text>
          </Center>
        )}
      </Box>
    </Paper>
  );
};
