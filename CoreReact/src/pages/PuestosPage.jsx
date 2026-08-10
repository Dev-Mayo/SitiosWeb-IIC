import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { servicios } from '../services';
import Topbar from '../components/Topbar';

const TAMANO_PAGINA = 10;

export default function PuestosPage() {
  const [puestos, setPuestos] = useState([]);
  const [pagina, setPagina] = useState(1);
  const [totalPaginas, setTotalPaginas] = useState(0);
  const [totalRegistros, setTotalRegistros] = useState(0);

  const [error, setError] = useState('');
  const [cargando, setCargando] = useState(true);

  useEffect(() => {
    let activo = true;

    setCargando(true);
    setError('');

    servicios
      .listarPuestos(
        pagina,
        TAMANO_PAGINA
      )
      .then((resultado) => {
        if (!activo) return;

        setPuestos(resultado.puestos);
        setTotalPaginas(resultado.totalPaginas);
        setTotalRegistros(resultado.totalRegistros);
      })
      .catch((err) => {
        if (activo) {
          setError(
            err.message ??
            'Error al obtener los puestos.'
          );
        }
      })
      .finally(() => {
        if (activo) {
          setCargando(false);
        }
      });

    return () => {
      activo = false;
    };
  }, [pagina]);

  return (
    <>
      <Topbar />

      <div className="page-body">
        <div className="content-card">

          <div className="d-flex justify-content-between align-items-center mb-3">
            <div>
              <h3 className="page-title mb-1">
                Puestos activos
              </h3>

              <p className="text-muted mb-0">
                Seleccione el puesto para el cual desea crear un nuevo empleado.
              </p>
            </div>
          </div>

          {error !== '' && (
            <div className="alert alert-danger">
              {error}
            </div>
          )}

          {cargando && (
            <div className="alert alert-info">
              Cargando puestos disponibles...
            </div>
          )}

          {!cargando &&
            error === '' &&
            puestos.length === 0 && (
              <div className="alert alert-warning">
                No se encontraron puestos activos disponibles.
              </div>
            )}

          {!cargando && puestos.length > 0 && (
            <>
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
                            to={`/oferentes?codigo_puesto=${encodeURIComponent(
                              puesto.puestoId
                            )}`}
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

              <div className="d-flex justify-content-between align-items-center mt-3">

                <span className="text-muted">
                  Página {pagina} de {totalPaginas} · {totalRegistros}{' '}
                  puesto(s)
                </span>

                <nav aria-label="Paginación de puestos">
                  <ul className="pagination pagination-sm mb-0">

                    <li
                      className={`page-item ${
                        pagina <= 1 ? 'disabled' : ''
                      }`}
                    >
                      <button
                        type="button"
                        className="page-link"
                        onClick={() =>
                          setPagina((p) => p - 1)
                        }
                        disabled={pagina <= 1}
                      >
                        Anterior
                      </button>
                    </li>

                    <li
                      className={`page-item ${
                        pagina >= totalPaginas
                          ? 'disabled'
                          : ''
                      }`}
                    >
                      <button
                        type="button"
                        className="page-link"
                        onClick={() =>
                          setPagina((p) => p + 1)
                        }
                        disabled={
                          pagina >= totalPaginas
                        }
                      >
                        Siguiente
                      </button>
                    </li>

                  </ul>
                </nav>

              </div>
            </>
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