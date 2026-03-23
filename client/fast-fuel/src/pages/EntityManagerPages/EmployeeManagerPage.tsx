import { EmployeeManager } from '../../components/EntityManagers/EmployeeManager/EmployeeManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';

export const EmployeeManagerPage = () => {
  return (
    <>
      <Header title="Employees" />

      <EmployeeManager />

      <Footer />
    </>
  );
};
