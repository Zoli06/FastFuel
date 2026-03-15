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
