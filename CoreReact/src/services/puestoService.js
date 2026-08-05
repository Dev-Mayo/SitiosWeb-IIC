import { getJson } from './apiClient';
import { SERVICES } from '../config';

// Equivalente a puestos.php (Core 6). GET a PuestoService.
export async function listarPuestos() {
  const resultado = await getJson(SERVICES.puestos);

  if (!resultado.Success) {
    throw new Error(
      resultado.Mensaje ?? 'No se pudo obtener el listado de puestos.'
    );
  }

  const puestos = Array.isArray(resultado.Puestos) ? resultado.Puestos : [];

  return puestos
    .map((item) => ({
      puestoId: Number(item.PuestoId) || 0,
      nombre: String(item.Nombre ?? '').trim()
    }))
    .filter((p) => p.puestoId > 0 && p.nombre !== '');
}
