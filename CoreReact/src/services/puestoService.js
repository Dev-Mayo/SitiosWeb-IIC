import { getJson } from './apiClient';
import { SERVICES } from '../config';

// Puestos disponibles para oferentes. GET /api/puestos devuelve la lista
// directamente (PuestoDto[]).
export async function listarPuestos() {
  const puestos = await getJson(SERVICES.puestos);
  const lista = Array.isArray(puestos) ? puestos : [];

  return lista
    .map((item) => ({
      puestoId: Number(item.puestoId) || 0,
      nombre: String(item.nombre ?? '').trim()
    }))
    .filter((p) => p.puestoId > 0 && p.nombre !== '');
}
