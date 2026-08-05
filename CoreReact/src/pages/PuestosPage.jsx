import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { servicios } from '../services';
import Topbar from '../components/Topbar';

export default function PuestosPage() {
  const [puestos, setPuestos] = useState([]);
  const [error, setError] = useState('');
  const [cargando, setCargando] = useState(true);

  useEffect(() => {
    let activo = true;

    servicios
      .listarPuestos()
      .then((resultado) => {
        if (activo) setPuestos(resultado);
      })
      .catch((err) => {
        if (activo) setError(err.message ?? 'Error al obtener los puestos.');
      })
      .finally(() => {
        if (activo) setCargando(false);
      });

    return () => {
      activo = false;
    };
  }, []);

  return (
    <>
      <Topbar />

      <div className="page-body">
        <div className="content-card">
          <div className="d-flex justify-content-between align-items-center mb-4">
            <div>
              <h3 className="page-title mb-1">Puestos activos</h3>
              <p className="text-muted mb-0">
                Seleccione el puesto para el cual desea crear un nuevo empleado.
              </p>
            </div>
          </div>

          {error !== '' && <div className="alert alert-danger">{error}</div>}

          {cargando && (
            <div className="alert alert-info">
              Cargando puestos disponibles...
            </div>
          )}

          {!cargando && error === '' && puestos.length === 0 && (
            <div className="alert alert-warning">
              No se encontraron puestos activos disponibles.
            </div>
          )}

          {!cargando && puestos.length > 0 && (
            <div className="table-responsive">
              <table className="table table-bordered table-hover align-middle">
                <thead>
                  <tr>
                    <th>Nombre del puesto</th>
                  </tr>
                </thead>
                <tbody>
                  {puestos.map((puesto) => (
                    <tr key={puesto.puestoId}>
                      <td>
                        <Link
                          to={`/oferentes?codigo_puesto=${encodeURIComponent(puesto.puestoId)}`}
                          className="puesto-link"
                        >
                          {puesto.nombre}
                        </Link>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          <div className="mt-4">
            <Link to="/" className="btn-regresar">
              ← Regresar
            </Link>
          </div>
        </div>
      </div>
    </>
  );
}
