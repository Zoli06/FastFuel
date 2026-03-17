import { OrderManager } from '../../components/EntityManagers/OrderManager/OrderManager.tsx';
import { Footer } from '../../components/Footer/Footer.tsx';
import { Header } from '../../components/Header/Header.tsx';
import { apiClient } from '../../lib/api-client.ts';

export const OrderManagerPage = () => {
  const { data: orders, refetch: refetchOrders } = apiClient.useSuspenseQuery('get', '/api/Order');
  const { data: menus } = apiClient.useSuspenseQuery('get', '/api/Menu');
  const { data: foods } = apiClient.useSuspenseQuery('get', '/api/Food');
  const { data: restaurants } = apiClient.useSuspenseQuery('get', '/api/Restaurant');

  return (
    <>
      <Header title="Orders" />

      <OrderManager
        orders={orders}
        refetchOrders={refetchOrders}
        menus={menus}
        foods={foods}
        restaurants={restaurants}
      />

      <Footer />
    </>
  );
};
