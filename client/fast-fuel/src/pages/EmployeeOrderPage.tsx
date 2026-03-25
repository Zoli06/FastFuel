import { EmployeeOrderCreator } from '../components/EmployeeOrderCreator/EmployeeOrderCreator.tsx';
import { Footer } from '../components/Footer/Footer.tsx';
import { Header } from '../components/Header/Header.tsx';

export const EmployeeOrderPage = () => {
  return (
    <>
      <Header title="Create Order" />

      <EmployeeOrderCreator />

      <Footer />
    </>
  );
};
