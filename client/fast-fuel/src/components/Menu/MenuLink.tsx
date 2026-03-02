import { UnstyledButton } from '@mantine/core';
import { Link } from 'react-router-dom';

interface MenuLinkProps {
  text: string;
  to: string;
}

export const MenuLink = ({ text, to }: MenuLinkProps) => {
  return (
    <UnstyledButton component={Link} to={to} p="xs" px="sm" fw="bold" fz="3rem" c="darkred">
      {text}
    </UnstyledButton>
  );
};
