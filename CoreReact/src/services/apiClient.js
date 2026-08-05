import { API_BASE_URL, TOKEN_KEY } from '../config';

/**
 * Cliente HTTP genérico que centraliza el manejo de errores.
 * Adjunta automáticamente el token JWT (Authorization: Bearer) y, ante una
 * respuesta con error HTTP, lanza un Error con el mensaje del servidor.
 */
async function request(path, { method = 'GET', body = null, timeoutMs = 30000 } = {}) {
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), timeoutMs);

  const opciones = {
    method,
    signal: controller.signal,
    headers: {}
  };

  const token = obtenerToken();
  if (token) {
    opciones.headers['Authorization'] = `Bearer ${token}`;
  }

  if (body !== null) {
    opciones.headers['Content-Type'] = 'application/json';
    opciones.body = JSON.stringify(body);
  }

  try {
    const response = await fetch(`${API_BASE_URL}${path}`, opciones);
    const datos = await leerJson(response);

    if (!response.ok) {
      const mensaje =
        datos?.mensaje ??
        datos?.detail ??
        `El servicio respondió con el código HTTP ${response.status}.`;

      const error = new Error(mensaje);
      error.status = response.status;
      throw error;
    }

    return datos;
  } catch (err) {
    if (err.name === 'AbortError') {
      throw new Error('El servicio no respondió a tiempo. Verifique que esté en ejecución.');
    }
    throw err;
  } finally {
    clearTimeout(timer);
  }
}

function obtenerToken() {
  try {
    return localStorage.getItem(TOKEN_KEY) ?? '';
  } catch {
    return '';
  }
}

async function leerJson(response) {
  try {
    return await response.json();
  } catch {
    return null;
  }
}

export function getJson(path) {
  return request(path, { method: 'GET' });
}

export function postJson(path, body) {
  return request(path, { method: 'POST', body });
}
