import { CustomerManager } from '../../components/EntityManagers/CustomerManager/CustomerManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';

export const CustomerManagerPage = () => {
  const {
    data: customers,
    isLoading: isLoadingCustomers,
    error: errorCustomers,
    refetch: refetchCustomers,
  } = apiClient.useQuery('get', '/api/Customer');

  if (isLoadingCustomers) {
    return <LoadingPage />;
  }

  if (errorCustomers) {
    return <ErrorPage title="Failed to Load Customers" />;
  }

  return (
    <>
      <Header title="Customers" />

      <CustomerManager customers={customers || []} refetchCustomers={refetchCustomers} />

      <Footer />
    </>
  );
};
