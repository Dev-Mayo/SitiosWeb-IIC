import { useState, useEffect, useRef } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { servicios } from '../services';
import Topbar from '../components/Topbar';

const TAMANO_PAGINA = 10;

export default function OferentesPage() {
  const [searchParams] = useSearchParams();
  const { user } = useAuth();

  const codigoPuesto = Number(searchParams.get('codigo_puesto')) || 0;
  const mensaje = searchParams.get('msg') === 'empleado_creado'
    ? 'Empleado creado con éxito.'
    : '';

  const [oferentes, setOferentes] = useState([]);
  const [pagina, setPagina] = useState(1);
  const [totalPaginas, setTotalPaginas] = useState(0);
  const [totalRegistros, setTotalRegistros] = useState(0);
  const [error, setError] = useState('');
  const [cargando, setCargando] = useState(true);

  const puestoCargado = useRef(0);

  useEffect(() => {
    if (puestoCargado.current !== codigoPuesto) {
      puestoCargado.current = codigoPuesto;
      if (pagina !== 1) {
        setPagina(1);
        return;
      }
    }

    let activo = true;
    setCargando(true);
    setError('');

    servicios
      .obtenerOferentesPorPuesto(
        codigoPuesto,
        user?.usuario ?? '',
        pagina,
        TAMANO_PAGINA
      )
      .then((resultado) => {
        if (!activo) return;
        setOferentes(resultado.oferentes);
        setTotalPaginas(resultado.totalPaginas);
        setTotalRegistros(resultado.totalRegistros);
      })
      .catch((err) => {
        if (activo) setError(err.message ?? 'Error al obtener los oferentes.');
      })
      .finally(() => {
        if (activo) setCargando(false);
      });

    return () => {
      activo = false;
    };
  }, [codigoPuesto, user?.usuario, pagina]);

  return (
    <>
      <Topbar />

      <div className="page-body">
        <div className="content-card">
          <div className="d-flex justify-content-between align-items-center mb-3">
            <div>
              <h3 className="page-title mb-1">Oferentes disponibles</h3>
              <p className="text-muted mb-0">
                Seleccione el oferente que será convertido en empleado.
              </p>
            </div>
          </div>

          {mensaje !== '' && (
            <div className="alert alert-success">{mensaje}</div>
          )}

          {error !== '' && <div className="alert alert-danger">{error}</div>}

          {cargando && (
            <div className="alert alert-info">
              Cargando oferentes para el puesto seleccionado...
            </div>
          )}

          {!cargando && error === '' && oferentes.length === 0 && (
            <div className="alert alert-warning">
              No se encontraron oferentes que cumplan los requisitos para el
              puesto seleccionado.
            </div>
          )}

          {!cargando && oferentes.length > 0 && (
            <>
              <div className="table-responsive">
                <table className="table table-bordered table-hover align-middle">
                  <thead>
                    <tr>
                      <th>Nombre completo</th>
                      <th>Identificación</th>
                    </tr>
                  </thead>
                  <tbody>
                    {oferentes.map((oferente) => (
                      <tr key={oferente.identificacion}>
                        <td>
                          <Link
                            to={`/detalle-oferente?identificacion=${encodeURIComponent(oferente.identificacion)}&codigo_puesto=${encodeURIComponent(codigoPuesto)}`}
                            className="oferente-link"
                          >
                            {oferente.nombreCompleto}
                          </Link>
                        </td>
                        <td>{oferente.identificacion}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              <div className="d-flex justify-content-between align-items-center mt-3">
                <span className="text-muted">
                  Página {pagina} de {totalPaginas} · {totalRegistros}{' '}
                  oferente(s)
                </span>
                <nav aria-label="Paginación de oferentes">
                  <ul className="pagination pagination-sm mb-0">
                    <li className={`page-item ${pagina <= 1 ? 'disabled' : ''}`}>
                      <button
                        type="button"
                        className="page-link"
                        onClick={() => setPagina((p) => p - 1)}
                        disabled={pagina <= 1}
                      >
                        Anterior
                      </button>
                    </li>
                    <li
                      className={`page-item ${pagina >= totalPaginas ? 'disabled' : ''}`}
                    >
                      <button
                        type="button"
                        className="page-link"
                        onClick={() => setPagina((p) => p + 1)}
                        disabled={pagina >= totalPaginas}
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
            <Link to="/puestos" className="btn-regresar">
              ← Regresar
            </Link>
          </div>
        </div>
      </div>
    </>
  );
}
