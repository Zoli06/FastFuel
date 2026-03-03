import '@mantine/core/styles.css';
import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { HomePage } from './pages/HomePage.tsx';
import { LoginPage } from './pages/LoginPage.tsx';
import { RestaurantsPage } from './pages/RestaurantsPage.tsx';
import { AllergiesPage } from './pages/AllergiesPage.tsx';

export function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <Routes>
          {/* Default */}
          <Route path="/" element={<HomePage />} />

          <Route path="/restaurants" element={<RestaurantsPage />} />

          <Route path="/login" element={<LoginPage />} />

          <Route path="/home" element={<HomePage />} />

          <Route path="/allergies" element={<AllergiesPage />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}
