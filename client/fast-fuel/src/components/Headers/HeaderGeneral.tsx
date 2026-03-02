import { Button, Center, Flex, Text } from '@mantine/core';
import { useNavigate } from 'react-router-dom';

interface HeaderProps {
  title: string;
  hideLeftButton?: boolean;
  rightButtonText?: string;
  rightButtonNavigateTo?: string | number;
}

export const HeaderGeneral = ({
  title,
  rightButtonText = 'Go Back',
  rightButtonNavigateTo = -1,
}: HeaderProps) => {
  const navigate = useNavigate();

  return (
    <>
      <Flex className="header-flex" align="center" justify="space-between" px="md" py="xs">
        {/* Left */}
        <Flex flex={1} justify="flex-start">
          <Button
            variant="filled"
            className="home-button"
            onClick={() => navigate('/home')}
            color={'gray'}
          >
            Home
          </Button>
        </Flex>
        <Center>
          <Text fz="4rem">
            {title}
          </Text>
        </Center>

        {/* Right Section */}
        <Flex flex={1} justify="flex-end">
          <Button
            variant="filled"
            className="home-button"
            onClick={() => {
              // Thomas never seen such bullshit before
              if (typeof rightButtonNavigateTo === 'number') {
                navigate(rightButtonNavigateTo);
              } else {
                navigate(rightButtonNavigateTo);
              }
            }}
            color={'gray'}
          >
            {rightButtonText}
          </Button>
        </Flex>
      </Flex>
    </>
  );
};
