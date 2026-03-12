import { OrderManager } from '../../components/EntityManagers/OrderManager/OrderManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../apiClient.ts';
import { LoadingPage } from '../LoadingPage.tsx';
import { ErrorPage } from '../ErrorPage.tsx';

export const OrderManagerPage = () => {
  const {
    data: orders,
    isLoading: isLoadingOrders,
    error: errorOrders,
    refetch: refetchOrders,
  } = apiClient.useQuery('get', '/api/Order');
  const {
    data: menus,
    isLoading: isLoadingMenus,
    error: errorMenus,
  } = apiClient.useQuery('get', '/api/Menu');
  const {
    data: foods,
    isLoading: isLoadingFoods,
    error: errorFoods,
  } = apiClient.useQuery('get', '/api/Food');
  const {
    data: restaurants,
    isLoading: isLoadingRestaurants,
    error: errorRestaurants,
  } = apiClient.useQuery('get', '/api/Restaurant');

  if (isLoadingOrders || isLoadingMenus || isLoadingFoods || isLoadingRestaurants) {
    return <LoadingPage />;
  }

  if (errorOrders || errorMenus || errorFoods || errorRestaurants) {
    return <ErrorPage title="Failed to Load Orders" />;
  }

  return (
    <>
      <Header title="Orders" />

      <OrderManager
        orders={orders || []}
        refetchOrders={refetchOrders}
        menus={menus || []}
        foods={foods || []}
        restaurants={restaurants || []}
      />

      <Footer />
    </>
  );
};
