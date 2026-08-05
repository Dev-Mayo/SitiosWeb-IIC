import { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { servicios } from '../services';

export default function LoginPage() {
  const [searchParams] = useSearchParams();
  const { login } = useAuth();
  const navigate = useNavigate();

  const [usuario, setUsuario] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [bloqueado, setBloqueado] = useState(false);
  const [enviando, setEnviando] = useState(false);

  const redirigidoPorSesion = searchParams.get('msg') === 'login';

  const manejarEnvio = async (evento) => {
    evento.preventDefault();
    setError('');
    setBloqueado(false);

    if (usuario.trim() === '' || password.trim() === '') {
      setError('Usuario y/o contraseña incorrectos.');
      return;
    }

    setEnviando(true);
    try {
      const resultado = await servicios.autenticar({
        usuario: usuario.trim(),
        password: password.trim()
      });

      if (!resultado.exito) {
        setError(resultado.mensaje);
        setBloqueado(resultado.bloqueado);
        return;
      }

      login({
        idUsuario: resultado.idUsuario,
        nombreCompleto: resultado.nombreCompleto,
        usuario: resultado.usuario,
        token: resultado.token
      });

      navigate('/', { replace: true });
    } catch (err) {
      setError(
        err.message ?? 'Error al conectar con el servicio de autenticación.'
      );
    } finally {
      setEnviando(false);
    }
  };

  return (
    <div className="card-login bg-white">
      <div className="card-header-custom">
        <div className="logo-emoji">🏢</div>
        <h5 className="mt-2 mb-0 fw-bold">Recursos Humano</h5>
        <small className="opacity-75">Portal Administrativo</small>
      </div>

      <div className="p-4">
        {error !== '' && (
          <div className="alert alert-danger">{error}</div>
        )}

        {bloqueado && (
          <div className="alert alert-danger">
            <strong>Usuario bloqueado.</strong> Se superaron 3 intentos fallidos.
          </div>
        )}

        {redirigidoPorSesion && (
          <div className="alert alert-warning">
            Por favor inicie sesión para utilizar el sistema.
          </div>
        )}

        <form onSubmit={manejarEnvio}>
          <div className="mb-3">
            <label className="form-label fw-semibold">ID de usuario</label>
            <input
              type="text"
              className="form-control"
              placeholder="Ingrese su ID de usuario"
              value={usuario}
              onChange={(e) => setUsuario(e.target.value)}
              required
            />
          </div>

          <div className="mb-4">
            <label className="form-label fw-semibold">Contraseña</label>
            <input
              type="password"
              className="form-control"
              placeholder="Ingrese su contraseña"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          <button
            type="submit"
            className="btn btn-primary w-100 py-2 fw-semibold"
            style={{ background: '#1aad94', borderColor: '#1aad94' }}
            disabled={enviando}
          >
            {enviando ? 'Ingresando...' : 'Ingresar'}
          </button>
        </form>
      </div>
    </div>
  );
}
