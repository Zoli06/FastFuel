import { useParams } from 'react-router-dom';
import { EmployeeOrderCreator } from '../components/EmployeeOrder/EmployeeOrderCreator.tsx';
import { ErrorPage } from './ErrorPage.tsx';
import { Footer } from '../components/Footer/Footer.tsx';
import { Header } from '../components/Header/Header.tsx';

export const EmployeeOrderPage = () => {
  const { id } = useParams<{ id: string }>();
  const restaurantId = Number(id);

  if (!Number.isFinite(restaurantId)) {
    return <ErrorPage />;
  }

  return (
    <>
      <Header title="Create Order" />

      <EmployeeOrderCreator restaurantId={restaurantId} />

      <Footer />
    </>
  );
};
