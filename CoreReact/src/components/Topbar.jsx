import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { obtenerIniciales } from '../utils/format';

export default function Topbar({ showLogout = true }) {
  const { user } = useAuth();

  return (
    <div className="topbar">
      <span className="topbar-brand">Sistema de Recursos Humanos</span>

      <div className="user-info">
        <span className="fw-semibold">{user?.nombreCompleto}</span>
        <div className="avatar">{obtenerIniciales(user?.nombreCompleto)}</div>

        {showLogout && (
          <Link to="/logout" className="btn-logout">
            Cerrar sesión
          </Link>
        )}
      </div>
    </div>
  );
}
