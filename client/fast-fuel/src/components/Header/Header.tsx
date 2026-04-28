import { Button, Center, Flex, Stack, Text } from '@mantine/core';
import { useQuery } from '@tanstack/react-query';
import { Link, useNavigate } from 'react-router-dom';
import { useApi } from '../../lib/api.ts';

export type HeaderAuthButton = 'Login' | 'Logout' | 'Register';

interface HeaderProps {
  title: string;
  authButton?: HeaderAuthButton;
  showHomeButton?: boolean;
  showAuthButton?: boolean;
}

const authButtonConfig = {
  Login: { text: 'Login', color: 'green', path: '/login' },
  Logout: { text: 'Logout', color: 'red', path: null },
  Register: { text: 'Register', color: 'blue', path: '/register' },
};

export const Header = ({
  title,
  authButton = 'Logout',
  showHomeButton = true,
  showAuthButton = true,
}: HeaderProps) => {
  const navigate = useNavigate();
  const shouldLoadCurrentUser = authButton === 'Logout';
  const { useNoPerm, invalidateApiCache } = useApi(null);

  const { data: currentUser } = useQuery({
    ...useNoPerm().queryOptions('get', '/api/User/me'),
    enabled: shouldLoadCurrentUser,
  });

  const { mutate: logout } = useNoPerm().useMutation('post', '/api/Auth/logout', {
    onSuccess: () => {
      invalidateApiCache();
      navigate('/login');
    },
  });

  const config = authButtonConfig[authButton];

  return (
    <Flex className="header-flex" align="center" justify="space-between" px="md" py="xs">
      <Flex flex={1} justify="flex-start">
        {showHomeButton && (
          <Link to={currentUser ? '/home' : '/'}>
            <Button variant="filled" color="gray">
              {currentUser ? 'Home' : 'Welcome'}
            </Button>
          </Link>
        )}
      </Flex>
      <Center flex={1}>
        <Stack align="center" gap={0}>
          <Text fz={{ base: '1em', xs: '1.2rem', sm: '1.5em' }} ta={'center'}>
            {title}
          </Text>
          {currentUser && (
            <Text size="sm" c="dimmed" ta={'center'}>
              {currentUser.name} ({currentUser.userType.toLowerCase()})
            </Text>
          )}
        </Stack>
      </Center>
      <Flex flex={1} justify="flex-end">
        {showAuthButton && (
          <Button
            variant="filled"
            color={config.color}
            onClick={() => (config.path ? navigate(config.path) : logout({}))}
          >
            {config.text}
          </Button>
        )}
      </Flex>
    </Flex>
  );
};
