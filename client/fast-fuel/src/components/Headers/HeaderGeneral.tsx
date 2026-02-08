import './HeaderGeneral.css';
import { Button, Flex } from '@mantine/core';

interface HeaderProps {
  title: string;
}

export const HeaderGeneral = ({ title }: HeaderProps) => {
  return (
    <>
      <Flex className="header-flex">
        <Button variant="filled" className="home-button">
          Home
        </Button>
        <h1 className="general-header">{title}</h1>
        <Button variant="filled" className="login-button">
          Login
        </Button>
      </Flex>
    </>
  );
};
