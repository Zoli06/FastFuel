import './HeaderGeneral.css';
import { Button, Flex } from '@mantine/core';
import { useNavigate } from 'react-router-dom';

interface HeaderProps {
  title: string;
}

export const HeaderGeneral = ({ title }: HeaderProps) => {
  const navigate = useNavigate();

  return (
    <>
      <Flex className="header-flex" align="center" justify="space-between" px="md" py="xs">
        {/* Left */}
        <div style={{ flex: 1, display: 'flex', justifyContent: 'flex-start' }}>
          <Button variant="filled" className="home-button" onClick={() => navigate('/home')}>
            Home
          </Button>
        </div>

        {/* Center Section */}
        <h1 className="general-header" style={{ margin: 0, whiteSpace: 'nowrap' }}>
          {title}
        </h1>

        {/* Right Section */}
        <div style={{ flex: 1, display: 'flex', justifyContent: 'flex-end' }}>
          <Button variant="filled" className="home-button" onClick={() => navigate('/login')}>
            Login
          </Button>
        </div>
      </Flex>
    </>
  );
};
