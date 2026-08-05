import { getJson } from './apiClient';
import { SERVICES } from '../config';

// Oferentes por puesto. GET /api/oferentes?codigoPuesto={id} devuelve la
// lista directamente (OferenteDto[]).
export async function obtenerOferentesPorPuesto(codigoPuesto, _usuario) {
  const ruta = `${SERVICES.oferentesPorPuesto}?codigoPuesto=${encodeURIComponent(
    Number(codigoPuesto) || 0
  )}`;
  const oferentes = await getJson(ruta);
  const lista = Array.isArray(oferentes) ? oferentes : [];

  return lista.map((item) => ({
    identificacion: item.identificacion,
    nombreCompleto: item.nombreCompleto
  }));
}
