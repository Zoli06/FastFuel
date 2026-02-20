import './HeaderGeneral.css';
import { Button, Flex } from '@mantine/core';
import { useNavigate } from 'react-router-dom';

interface HeaderProps {
  title: string;
  hideLeftButton?: boolean;
  rightButtonText?: string;
  rightButtonNavigateTo?: string | number;
}

export const HeaderGeneral = ({
  title,
  hideLeftButton = false,
  rightButtonText = 'Go back',
  rightButtonNavigateTo = -1,
}: HeaderProps) => {
  const navigate = useNavigate();

  return (
    <>
      <Flex className="header-flex" align="center" justify="space-between" px="md" py="xs">
        {/* Left */}
        <div
          style={{
            flex: 1,
            display: 'flex',
            justifyContent: 'flex-start',
            visibility: hideLeftButton ? 'hidden' : 'visible',
          }}
        >
          <Button
            variant="filled"
            className="home-button"
            onClick={() => navigate('/home')}
            color={'gray'}
          >
            Home
          </Button>
        </div>

        {/* Center Section */}
        <h1 className="general-header" style={{ margin: 0, whiteSpace: 'nowrap' }}>
          {title}
        </h1>

        {/* Right Section */}
        <div style={{ flex: 1, display: 'flex', justifyContent: 'flex-end' }}>
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
        </div>
      </Flex>
    </>
  );
};
