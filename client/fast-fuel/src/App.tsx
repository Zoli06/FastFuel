import '@mantine/core/styles.css';
import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Home } from './pages/Home.tsx';
import { Login } from './components/Login/Login.tsx';
import { Restaurant } from './components/Restaurant/Restaurants.tsx';

export function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <Routes>
          {/* Default */}
          <Route path="/" element={<Home />} />

          <Route path="/restaurants" element={<Restaurant />} />

          <Route path="/login" element={<Login />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}
