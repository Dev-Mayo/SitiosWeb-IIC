import { API_BASE_URL } from '../config';

/**
 * Cliente HTTP genérico que centraliza el manejo de errores.
 * Sustituye al bloque cURL que usaba PHP.
 */
async function request(path, { method = 'GET', body = null, timeoutMs = 30000 } = {}) {
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), timeoutMs);

  const opciones = {
    method,
    signal: controller.signal
  };

  if (body !== null) {
    opciones.headers = { 'Content-Type': 'application/json' };
    opciones.body = JSON.stringify(body);
  }

  try {
    const response = await fetch(`${API_BASE_URL}${path}`, opciones);

    if (!response.ok) {
      throw new Error(`El servicio respondió con el código HTTP ${response.status}.`);
    }

    return await response.json();
  } catch (err) {
    if (err.name === 'AbortError') {
      throw new Error('El servicio no respondió a tiempo. Verifique que esté en ejecución.');
    }
    throw err;
  } finally {
    clearTimeout(timer);
  }
}

export function getJson(path) {
  return request(path, { method: 'GET' });
}

export function postJson(path, body) {
  return request(path, { method: 'POST', body });
}
