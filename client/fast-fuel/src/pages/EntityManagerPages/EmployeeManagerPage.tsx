import { EmployeeManager } from '../../components/EntityManagers/EmployeeManager/EmployeeManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';

export const EmployeeManagerPage = () => {
  const {
    data: employees,
    isLoading: isLoadingEmployees,
    error: errorEmployees,
    refetch: refetchEmployees,
  } = apiClient.useQuery('get', '/api/Employee');
  const {
    data: stationCategories,
    isLoading: isLoadingStationCategories,
    error: errorStationCategories,
  } = apiClient.useQuery('get', '/api/StationCategory');

  if (isLoadingEmployees || isLoadingStationCategories) {
    return <LoadingPage />;
  }

  if (errorEmployees || errorStationCategories) {
    return <ErrorPage title="Failed to Load Employees" />;
  }

  return (
    <>
      <Header title="Employees" />

      <EmployeeManager
        employees={employees || []}
        refetchEmployees={refetchEmployees}
        stationCategories={stationCategories || []}
      />

      <Footer />
    </>
  );
};
