import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { HomePage } from './pages/HomePage.tsx';
import { LoginPage } from './pages/LoginPage.tsx';
import { RestaurantManagerPage } from './pages/EntityManagerPages/RestaurantManagerPage.tsx';
import { AllergyManagerPage } from './pages/EntityManagerPages/AllergyManagerPage.tsx';
import { FoodManagerPage } from './pages/EntityManagerPages/FoodManagerPage.tsx';
import { IngredientManagerPage } from './pages/EntityManagerPages/IngredientManagerPage.tsx';
import { MenuManagerPage } from './pages/EntityManagerPages/MenuManagerPage.tsx';
import { StationCategoryManagerPage } from './pages/EntityManagerPages/StationCategoryManagerPage.tsx';
import { StationManagerPage } from './pages/EntityManagerPages/StationManagerPage.tsx';
import { OrderManagerPage } from './pages/EntityManagerPages/OrderManagerPage.tsx';

export function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<HomePage />} />

        <Route path="manage">
          <Route path="allergy" element={<AllergyManagerPage />} />
          <Route path="ingredient" element={<IngredientManagerPage />} />
          <Route path="food" element={<FoodManagerPage />} />
          <Route path="menu" element={<MenuManagerPage />} />
          <Route path="order" element={<OrderManagerPage />} />
          <Route path="station-category" element={<StationCategoryManagerPage />} />
          <Route path="station" element={<StationManagerPage />} />
          <Route path="restaurant" element={<RestaurantManagerPage />} />
        </Route>

        <Route path="login" element={<LoginPage />} />
      </Routes>
    </BrowserRouter>
  );
}
