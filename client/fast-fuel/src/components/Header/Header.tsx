import { Button, Center, Flex, Stack, Text } from '@mantine/core';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { apiClient, clearAuthData, myCurrentUserQueryOptions } from '../../lib/api-client.ts';

export type HeaderAuthButton = 'Login' | 'Logout' | 'Register';

interface HeaderProps {
  title: string;
  authButton?: HeaderAuthButton;
}

export const Header = ({ title, authButton }: HeaderProps) => {
  const currentAuthButton = authButton ?? 'Logout';
  const shouldLoadCurrentUser = currentAuthButton === 'Logout';

  const navigate = useNavigate();
  const { data: currentUser } = useQuery({
    ...myCurrentUserQueryOptions(),
    enabled: shouldLoadCurrentUser,
  });

  const { mutate: logout } = apiClient.useMutation('post', '/api/Auth/logout', {
    onSuccess: () => {
      clearAuthData();
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
            onClick={authButtonStates[currentAuthButton].action}
            color={authButtonStates[currentAuthButton].color}
          >
            {authButtonStates[currentAuthButton].text}
          </Button>
        </Flex>
      </Flex>
    </>
  );
};
