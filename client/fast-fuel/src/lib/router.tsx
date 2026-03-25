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
import { Suspense } from 'react';
import { LoadingPage } from '../pages/LoadingPage.tsx';
import { ErrorPage } from '../pages/ErrorPage.tsx';
import { EmployeeManagerPage } from '../pages/EntityManagerPages/EmployeeManagerPage.tsx';
import { ShiftManagerPage } from '../pages/EntityManagerPages/ShiftManagerPage.tsx';
import { RoleManagerPage } from '../pages/EntityManagerPages/RoleManagerPage.tsx';
import { StationTasksPage } from '../pages/StationTasksPage.tsx';
import { RegistrationPage } from '../pages/RegistrationPage.tsx';
import { CustomerPage } from '../pages/CustomerPages/CustomerPage.tsx';
import { MenuPage } from '../pages/CustomerPages/MenuPage.tsx';

export const router = createBrowserRouter([
  {
    path: '/',
    element: (
      <Suspense fallback={<LoadingPage />}>
        <Outlet />
      </Suspense>
    ),
    errorElement: <ErrorPage />,
    children: [
      {
        index: true,
        element: <HomePage />,
      },
      {
        path: 'login',
        element: <LoginPage />,
      },
      {
        path: 'register',
        element: <RegistrationPage />,
      },
      {
        path: 'customer',
        element: <Outlet />,
        children: [
          {
            index: true,
            element: <CustomerPage />,
          },
          {
            path: 'menu',
            element: <MenuPage />,
          },
        ],
      },
      {
        path: 'stations/:id/tasks',
        element: <StationTasksPage />,
      },
      {
        path: 'manage',
        element: <Outlet />,
        children: [
          {
            path: 'allergy',
            element: <AllergyManagerPage />,
          },
          {
            path: 'ingredient',
            element: <IngredientManagerPage />,
          },
          {
            path: 'employee',
            element: <EmployeeManagerPage />,
          },
          {
            path: 'food',
            element: <FoodManagerPage />,
          },
          {
            path: 'menu',
            element: <MenuManagerPage />,
          },
          {
            path: 'order',
            element: <OrderManagerPage />,
          },
          {
            path: 'role',
            element: <RoleManagerPage />,
          },
          {
            path: 'shift',
            element: <ShiftManagerPage />,
          },
          {
            path: 'station-category',
            element: <StationCategoryManagerPage />,
          },
          {
            path: 'station',
            element: <StationManagerPage />,
          },
          {
            path: 'restaurant',
            element: <RestaurantManagerPage />,
          },
        ],
      },
    ],
  },
]);
