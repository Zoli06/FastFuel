import { Link } from 'react-router-dom';
import './MenuLink.css';

interface MenuLinkProps {
  text: string;
  to: string;
}

export const MenuLink = ({ text, to }: MenuLinkProps) => {
  return (
    <Link typeof="button" to={to} className="menu-link">
      {text}
    </Link>
  );
};
