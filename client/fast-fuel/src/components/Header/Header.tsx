import { Button, Center, Flex, Stack, Text } from '@mantine/core';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { notifications } from '@mantine/notifications';
import { $api } from '../../lib/api.ts';

export type HeaderAuthButton = 'Login' | 'Logout' | 'Register';

interface HeaderProps {
  title: string;
  authButton?: HeaderAuthButton;
}

const authButtonConfig = {
  Login: { text: 'Login', color: 'green', path: '/login' },
  Logout: { text: 'Logout', color: 'red', path: null },
  Register: { text: 'Register', color: 'blue', path: '/register' },
};

export const Header = ({ title, authButton = 'Logout' }: HeaderProps) => {
  const navigate = useNavigate();
  const shouldLoadCurrentUser = authButton === 'Logout';
  const { noPerm, invalidateCache } = $api(null);

  const { data: currentUser, refetch: refetchCurrentUser } = useQuery({
    ...noPerm().queryOptions('get', '/api/User/me'),
    enabled: shouldLoadCurrentUser,
  });

  const { mutate: logout } = noPerm().useMutation('post', '/api/Auth/logout', {
    onSuccess: () => {
      invalidateCache();
      navigate('/login');
    },
  });

  const handleHomeClick = async () => {
    const { data: user } = currentUser ? { data: currentUser } : await refetchCurrentUser();
    if (!user) {
      notifications.show({
        title: 'Not logged in',
        message: 'Please login to access the home page',
        color: 'red',
      });
      return;
    }
    navigate('/');
  };

  const config = authButtonConfig[authButton];

  return (
    <Flex className="header-flex" align="center" justify="space-between" px="md" py="xs">
      <Flex flex={1} justify="flex-start">
        <Button variant="filled" onClick={handleHomeClick} color="gray">
          Home
        </Button>
      </Flex>
      <Center>
        <Stack align="center" gap={0}>
          <Text fz="2rem">{title}</Text>
          {currentUser && (
            <Text size="sm" c="dimmed">
              {currentUser.name} ({currentUser.userType.toLowerCase()})
            </Text>
          )}
        </Stack>
      </Center>
      <Flex flex={1} justify="flex-end">
        <Button
          variant="filled"
          color={config.color}
          onClick={() => (config.path ? navigate(config.path) : logout({}))}
        >
          {config.text}
        </Button>
      </Flex>
    </Flex>
  );
};
