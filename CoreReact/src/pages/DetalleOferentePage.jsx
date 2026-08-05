import { useState, useEffect } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { servicios } from '../services';
import { formatearFecha } from '../utils/format';
import Topbar from '../components/Topbar';

export default function DetalleOferentePage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { user } = useAuth();

  const identificacion = searchParams.get('identificacion') ?? '';
  const codigoPuesto = Number(searchParams.get('codigo_puesto')) || 0;

  const [oferente, setOferente] = useState(null);
  const [error, setError] = useState('');
  const [cargando, setCargando] = useState(true);
  const [creando, setCreando] = useState(false);

  useEffect(() => {
    let activo = true;

    if (identificacion === '') {
      setError('No se recibió una identificación válida del oferente.');
      setCargando(false);
      return () => {
        activo = false;
      };
    }

    servicios
      .obtenerDetalleOferente(identificacion, user?.usuario ?? '')
      .then((resultado) => {
        if (activo) setOferente(resultado);
      })
      .catch((err) => {
        if (activo) setError(err.message ?? 'Error al obtener el detalle.');
      })
      .finally(() => {
        if (activo) setCargando(false);
      });

    return () => {
      activo = false;
    };
  }, [identificacion, user?.usuario]);

  const crearEmpleado = async () => {
    const confirmar = window.confirm(
      '¿Desea crear un empleado con los datos del oferente seleccionado?'
    );
    if (!confirmar) return;

    setCreando(true);
    setError('');

    try {
      await servicios.registrarEmpleado({
        identificacion: oferente.identificacion,
        tipoIdentificacion: oferente.tipoIdentificacion,
        nombreCompleto: oferente.nombreCompleto,
        fechaNacimiento: oferente.fechaNacimiento,
        puestoId: codigoPuesto,
        correos: oferente.correos,
        telefonos: oferente.telefonos,
        usuario: user?.usuario ?? ''
      });

      navigate(
        `/oferentes?codigo_puesto=${encodeURIComponent(codigoPuesto)}&msg=empleado_creado`
      );
    } catch (err) {
      setError(err.message ?? 'No fue posible registrar el empleado.');
      setCreando(false);
    }
  };

  const urlOferentes = `/oferentes?codigo_puesto=${encodeURIComponent(codigoPuesto)}`;

  return (
    <>
      <Topbar />

      <div className="page-body">
        <div className="content-card">
          <div className="d-flex justify-content-between align-items-center mb-3">
            <div>
              <h3 className="page-title mb-1">Detalle del oferente</h3>
            </div>
          </div>

          {error !== '' && <div className="alert alert-danger">{error}</div>}

          {cargando && (
            <div className="alert alert-info">
              Cargando detalle del oferente...
            </div>
          )}

          {!cargando && oferente !== null && (
            <>
              <div className="section-title">Información personal</div>

              <div className="row g-3 mb-4">
                <div className="col-md-6">
                  <div className="detail-label">Identificación</div>
                  <div className="detail-value">{oferente.identificacion}</div>
                </div>

                <div className="col-md-6">
                  <div className="detail-label">Tipo de identificación</div>
                  <div className="detail-value">
                    {oferente.tipoIdentificacion}
                  </div>
                </div>

                <div className="col-md-8">
                  <div className="detail-label">Nombre completo</div>
                  <div className="detail-value">{oferente.nombreCompleto}</div>
                </div>

                <div className="col-md-4">
                  <div className="detail-label">Fecha de nacimiento</div>
                  <div className="detail-value">
                    {formatearFecha(oferente.fechaNacimiento)}
                  </div>
                </div>
              </div>

              <div className="section-title">Información de contacto</div>

              <div className="row g-4 mb-4">
                <div className="col-md-6">
                  <div className="detail-label mb-2">Correos electrónicos</div>
                  {oferente.correos.length > 0 ? (
                    oferente.correos.map((correo, i) => (
                      <div key={i} className="list-item-custom">
                        {correo}
                      </div>
                    ))
                  ) : (
                    <div className="text-muted">
                      No hay correos registrados.
                    </div>
                  )}
                </div>

                <div className="col-md-6">
                  <div className="detail-label mb-2">Teléfonos</div>
                  {oferente.telefonos.length > 0 ? (
                    oferente.telefonos.map((telefono, i) => (
                      <div key={i} className="list-item-custom">
                        {telefono}
                      </div>
                    ))
                  ) : (
                    <div className="text-muted">
                      No hay teléfonos registrados.
                    </div>
                  )}
                </div>
              </div>

              <div className="section-title">Preparación académica</div>

              <div className="mb-4">
                {oferente.preparacionAcademica.length > 0 ? (
                  oferente.preparacionAcademica.map((preparacion, i) => (
                    <div key={i} className="list-item-custom">
                      <div className="fw-semibold mb-1">
                        {preparacion.Titulo ?? 'Título no disponible'}
                      </div>
                      <div className="text-muted small">
                        Institución:{' '}
                        {preparacion.CodigoInstitucion ?? 'No disponible'}
                      </div>
                      <div className="text-muted small">
                        Periodo: {formatearFecha(preparacion.FechaInicio)} -{' '}
                        {formatearFecha(preparacion.FechaFin)}
                      </div>
                    </div>
                  ))
                ) : (
                  <div className="text-muted">
                    No hay preparación académica registrada.
                  </div>
                )}
              </div>

              <div className="section-title">Experiencia laboral</div>

              <div className="mb-4">
                {oferente.experienciaLaboral.length > 0 ? (
                  oferente.experienciaLaboral.map((experiencia, i) => (
                    <div key={i} className="list-item-custom">
                      <div className="fw-semibold mb-1">
                        {experiencia.Puesto ?? 'Puesto no disponible'}
                      </div>
                      <div className="text-muted small">
                        Empresa: {experiencia.Empresa ?? 'No disponible'}
                      </div>
                      <div className="text-muted small">
                        Periodo: {formatearFecha(experiencia.FechaInicio)} -{' '}
                        {formatearFecha(experiencia.FechaFin)}
                      </div>
                    </div>
                  ))
                ) : (
                  <div className="text-muted">
                    No hay experiencia laboral registrada.
                  </div>
                )}
              </div>

              <div className="section-title">Concursos</div>

              <div className="mb-4">
                {oferente.concursos.length > 0 ? (
                  oferente.concursos.map((concurso, i) => (
                    <div key={i} className="list-item-custom">
                      <div className="fw-semibold mb-1">
                        {concurso.Nombre ?? 'Concurso no disponible'}
                      </div>
                      <div className="text-muted small">
                        Código: {concurso.CodigoConcurso ?? 'No disponible'}
                      </div>
                      <div className="text-muted small">
                        Estado: {concurso.Estado ?? 'No disponible'}
                      </div>
                      <div className="text-muted small">
                        Periodo: {formatearFecha(concurso.FechaInicio)} -{' '}
                        {formatearFecha(concurso.FechaFin)}
                      </div>
                    </div>
                  ))
                ) : (
                  <div className="text-muted">
                    No hay concursos registrados.
                  </div>
                )}
              </div>

              <div className="section-title">Curriculum</div>

              <div className="detail-value mb-4">
                {oferente.curriculum ?? 'No disponible la información'}
              </div>

              <div className="d-flex flex-wrap gap-2">
                <button
                  type="button"
                  className="btn-crear"
                  onClick={crearEmpleado}
                  disabled={creando}
                >
                  {creando ? 'Creando...' : 'Crear empleado'}
                </button>

                <Link to={urlOferentes} className="btn-cancelar">
                  Cancelar
                </Link>
              </div>
            </>
          )}

          {!cargando && oferente === null && (
            <Link to={urlOferentes} className="btn-cancelar">
              Regresar
            </Link>
          )}
        </div>
      </div>
    </>
  );
}
