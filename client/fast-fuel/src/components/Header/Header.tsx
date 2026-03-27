import { Button, Center, Flex, Text } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { apiClient, clearAuthData, fetchClient } from '../../lib/api-client.ts';
import { notifications } from '@mantine/notifications';

interface HeaderProps {
  title: string;
  authButton?: 'Login' | 'Logout' | 'Register';
}

const authButtonConfig = {
  Login: { text: 'Login', color: 'green', path: '/login' },
  Logout: { text: 'Logout', color: 'red', path: null },
  Register: { text: 'Register', color: 'blue', path: '/register' },
};

export const Header = ({ title, authButton = 'Logout' }: HeaderProps) => {
  const navigate = useNavigate();
  const { mutate: logout } = apiClient.useMutation('post', '/api/Auth/logout', {
    onSuccess: () => {
      clearAuthData();
      navigate('/login');
    },
  });

  const handleHomeClick = async () => {
    const { data: user } = await fetchClient.GET('/api/User/me').catch(() => ({ data: null }));
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
        <Text fz="2rem">{title}</Text>
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
