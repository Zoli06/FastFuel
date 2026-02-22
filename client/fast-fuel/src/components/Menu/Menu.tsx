import { MenuLink } from './MenuLink';
import './Menu.css';

export const Menu = () => {
  return (
    <div className="menu">
      <MenuLink text="Allergies" to="/allergies" />
      <MenuLink text="Foods" to="/foods" />
      <MenuLink text="Ingredients" to="/ingredients" />
      <MenuLink text="Menus" to="/menus" />
      <MenuLink text="Orders" to="/orders" />
      <MenuLink text="Restaurants" to="/restaurants" />
      <MenuLink text="Stations" to="/stations" />
    </div>
  );
};
