import { ShiftManager } from '../../components/EntityManagers/ShiftManager/ShiftManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';

export const ShiftManagerPage = () => {
  const {
    data: shifts,
    isLoading: isLoadingShifts,
    error: errorShifts,
    refetch: refetchShifts,
  } = apiClient.useQuery('get', '/api/Shift');
  const {
    data: employees,
    isLoading: isLoadingEmployees,
    error: errorEmployees,
  } = apiClient.useQuery('get', '/api/Employee');

  if (isLoadingShifts || isLoadingEmployees) {
    return <LoadingPage />;
  }

  if (errorShifts || errorEmployees) {
    return <ErrorPage title="Failed to Load Shifts" />;
  }

  return (
    <>
      <Header title="Shifts" />

      <ShiftManager
        shifts={shifts || []}
        refetchShifts={refetchShifts}
        employees={employees || []}
      />

      <Footer />
    </>
  );
};
