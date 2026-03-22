import { EmployeeOrderRestaurantSelector } from '../components/EmployeeOrder/EmployeeOrderRestaurantSelector.tsx';
import { Footer } from '../components/Footer/Footer.tsx';
import { Header } from '../components/Header/Header.tsx';

export const EmployeeOrderRestaurantSelectorPage = () => {
  return (
    <>
      <Header title="Create Order" />

      <EmployeeOrderRestaurantSelector />

      <Footer />
    </>
  );
};
