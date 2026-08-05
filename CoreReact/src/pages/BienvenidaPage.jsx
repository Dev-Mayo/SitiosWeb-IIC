import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function BienvenidaPage() {
  const { user } = useAuth();

  return (
    <div className="page-body">
      <div className="welcome-card">
        <div className="welcome-icon">🏢</div>

        <h3 className="fw-bold">Bienvenido al Sistema</h3>

        <div className="welcome-msg">
          Bienvenido, {user?.nombreCompleto}
        </div>

        <p className="text-muted">
          Ha iniciado sesión correctamente en el sistema de Recursos Humanos.
        </p>

        <div className="d-flex justify-content-center gap-3 mt-4">
          <Link to="/puestos" className="btn btn-success action-btn">
            Ver puestos
          </Link>

          <Link to="/logout" className="btn btn-danger action-btn">
            Cerrar sesión
          </Link>
        </div>
      </div>
    </div>
  );
}
