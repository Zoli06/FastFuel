import { createBrowserRouter, Outlet } from 'react-router-dom';
import { AllergyManagerPage } from '../pages/EntityManagerPages/AllergyManagerPage.tsx';
import { IngredientManagerPage } from '../pages/EntityManagerPages/IngredientManagerPage.tsx';
import { FoodManagerPage } from '../pages/EntityManagerPages/FoodManagerPage.tsx';
import { MenuManagerPage } from '../pages/EntityManagerPages/MenuManagerPage.tsx';
import { OrderManagerPage } from '../pages/EntityManagerPages/OrderManagerPage.tsx';
import { StationCategoryManagerPage } from '../pages/EntityManagerPages/StationCategoryManagerPage.tsx';
import { StationManagerPage } from '../pages/EntityManagerPages/StationManagerPage.tsx';
import { RestaurantManagerPage } from '../pages/EntityManagerPages/RestaurantManagerPage.tsx';
import { LoginPage } from '../pages/LoginPage.tsx';
import { HomePage } from '../pages/HomePage.tsx';
import { ErrorPage } from '../pages/ErrorPage.tsx';
import { EmployeeManagerPage } from '../pages/EntityManagerPages/EmployeeManagerPage.tsx';
import { ShiftManagerPage } from '../pages/EntityManagerPages/ShiftManagerPage.tsx';
import { RoleManagerPage } from '../pages/EntityManagerPages/RoleManagerPage.tsx';
import { StationTasksPage } from '../pages/StationTasksPage.tsx';
import { RegistrationPage } from '../pages/RegistrationPage.tsx';
import { OrderStatusDisplayPage } from '../pages/OrderStatusDisplayPage.tsx';
import { MachineManagerPage } from '../pages/EntityManagerPages/MachineManagerPage.tsx';
import { AdminManagerPage } from '../pages/EntityManagerPages/AdminManagerPage.tsx';
import { CustomerManagerPage } from '../pages/EntityManagerPages/CustomerManagerPage.tsx';
import { Layout } from '../components/Layout/Layout.tsx';
import type { HeaderAuthButton } from '../components/Header/Header.tsx';
import { OrderCreator } from '../components/OrderCreator/OrderCreator.tsx';
import { OrderHistoryPage } from '../pages/OrderHistoryPage.tsx';
import { ProfilePage } from '../pages/ProfilePage.tsx';
import { MyShiftsPage } from '../pages/MyShiftsPage.tsx';
import { AboutPage } from '../pages/AboutPage.tsx';

type RouteHandle = {
  header?: {
    title?: string;
    authButton?: HeaderAuthButton;
  };
};

const withHeader = (title: string, authButton?: HeaderAuthButton): RouteHandle => ({
  header: {
    title,
    authButton,
  },
});

export const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    errorElement: <ErrorPage />,
    children: [
      {
        index: true,
        element: <HomePage />,
        handle: withHeader('Home'),
      },
      {
        path: 'login',
        element: <LoginPage />,
        handle: withHeader('Login', 'Register'),
      },
      {
        path: 'register',
        element: <RegistrationPage />,
        handle: withHeader('Registration', 'Login'),
      },
      {
        path: 'order',
        element: <OrderCreator />,
        handle: withHeader('Place Order'),
      },
      {
        path: 'employee/my-shifts',
        element: <MyShiftsPage />,
        handle: withHeader('My Shifts'),
      },
      {
        path: 'order-history',
        element: <OrderHistoryPage />,
        handle: withHeader('Order History'),
      },
      {
        path: 'profile',
        element: <ProfilePage />,
        handle: withHeader('Profile'),
      },
      {
        path: 'about',
        element: <AboutPage />,
        handle: withHeader('About'),
      },
      {
        path: 'manage',
        element: <Outlet />,
        children: [
          {
            path: 'allergy',
            element: <AllergyManagerPage />,
            handle: withHeader('Allergy Manager'),
          },
          {
            path: 'customer',
            element: <CustomerManagerPage />,
            handle: withHeader('Customer Manager'),
          },
          {
            path: 'ingredient',
            element: <IngredientManagerPage />,
            handle: withHeader('Ingredient Manager'),
          },
          {
            path: 'employee',
            element: <EmployeeManagerPage />,
            handle: withHeader('Employee Manager'),
          },
          {
            path: 'machine',
            element: <MachineManagerPage />,
            handle: withHeader('Machine Manager'),
          },
          {
            path: 'food',
            element: <FoodManagerPage />,
            handle: withHeader('Food Manager'),
          },
          {
            path: 'menu',
            element: <MenuManagerPage />,
            handle: withHeader('Menu Manager'),
          },
          {
            path: 'order',
            element: <OrderManagerPage />,
            handle: withHeader('Order Manager'),
          },
          {
            path: 'role',
            element: <RoleManagerPage />,
            handle: withHeader('Role Manager'),
          },
          {
            path: 'shift',
            element: <ShiftManagerPage />,
            handle: withHeader('Shift Manager'),
          },
          {
            path: 'station-category',
            element: <StationCategoryManagerPage />,
            handle: withHeader('Station Category Manager'),
          },
          {
            path: 'station',
            element: <StationManagerPage />,
            handle: withHeader('Station Manager'),
          },
          {
            path: 'restaurant',
            element: <RestaurantManagerPage />,
            handle: withHeader('Restaurant Manager'),
          },
          {
            path: 'admin',
            element: <AdminManagerPage />,
            handle: withHeader('Admin Manager'),
          },
        ],
      },
    ],
  },
  {
    path: '/stations/:id/tasks',
    element: <StationTasksPage />,
    errorElement: <ErrorPage />,
  },
  {
    path: '/restaurants/order-status-display',
    element: <OrderStatusDisplayPage />,
    errorElement: <ErrorPage />,
  },
]);
