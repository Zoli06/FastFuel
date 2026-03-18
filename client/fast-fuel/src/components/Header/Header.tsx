import { Button, Center, Flex, Text } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../../lib/api-client.ts';

interface HeaderProps {
  title: string;
  authButton?: 'Login' | 'Logout' | 'Register';
}

export const Header = ({ title, authButton }: HeaderProps) => {
  authButton = authButton || 'Logout';

  const navigate = useNavigate();
  const { mutate: logout } = apiClient.useMutation('post', '/api/Auth/logout', {
    onSuccess: () => {
      navigate('/login');
    },
  });

  const authButtonStates = {
    Login: { text: 'Login', color: 'green', action: () => navigate('/login') },
    Logout: { text: 'Logout', color: 'red', action: () => logout({}) },
    Register: { text: 'Register', color: 'blue', action: () => navigate('/register') },
  };

  return (
    <>
      <Flex className="header-flex" align="center" justify="space-between" px="md" py="xs">
        <Flex flex={1} justify="flex-start">
          <Button variant="filled" onClick={() => navigate('/')} color={'gray'}>
            Home
          </Button>
        </Flex>
        <Center>
          <Text fz="2rem">{title}</Text>
        </Center>

        <Flex flex={1} justify="flex-end">
          <Button
            variant="filled"
            onClick={authButtonStates[authButton].action}
            color={authButtonStates[authButton].color}
          >
            {authButtonStates[authButton].text}
          </Button>
        </Flex>
      </Flex>
    </>
  );
};
