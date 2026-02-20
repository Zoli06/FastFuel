import '@mantine/core/styles.css';
import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Home } from './pages/Home.tsx';
import { Restaurant } from './components/Restaurant/Restaurants.tsx';
import { Login } from './components/Login/Login.tsx';

export function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <Routes>
          {/* First page */}
          <Route path="/" element={<Restaurant />} />

          {/* Home */}
          <Route path="/home" element={<Home />} />

          {/* Login */}
          <Route path="/login" element={<Login />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}
