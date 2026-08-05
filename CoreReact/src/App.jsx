import { Routes, Route, Navigate } from 'react-router-dom';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import BienvenidaPage from './pages/BienvenidaPage';
import PuestosPage from './pages/PuestosPage';
import OferentesPage from './pages/OferentesPage';
import DetalleOferentePage from './pages/DetalleOferentePage';
import LogoutPage from './pages/LogoutPage';

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/logout" element={<LogoutPage />} />

      <Route
        path="/"
        element={
          <ProtectedRoute>
            <BienvenidaPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/puestos"
        element={
          <ProtectedRoute>
            <PuestosPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/oferentes"
        element={
          <ProtectedRoute>
            <OferentesPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/detalle-oferente"
        element={
          <ProtectedRoute>
            <DetalleOferentePage />
          </ProtectedRoute>
        }
      />

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
