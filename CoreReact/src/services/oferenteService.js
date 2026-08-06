import { getJson } from './apiClient';
import { SERVICES } from '../config';

// Oferentes por puesto (paginados).
// GET /api/oferentes?codigoPuesto={id}&pagina={n}&tamanoPagina={n} devuelve
// { datos: OferenteDto[], pagina, tamanoPagina, totalRegistros, totalPaginas }.
export async function obtenerOferentesPorPuesto(
  codigoPuesto,
  _usuario,
  pagina = 1,
  tamanoPagina = 10
) {
  const params = new URLSearchParams({
    codigoPuesto: String(Number(codigoPuesto) || 0),
    pagina: String(pagina),
    tamanoPagina: String(tamanoPagina)
  });

  const ruta = `${SERVICES.oferentesPorPuesto}?${params.toString()}`;
  const respuesta = await getJson(ruta);

  const datos = Array.isArray(respuesta?.datos) ? respuesta.datos : [];

  return {
    oferentes: datos.map((item) => ({
      identificacion: item.identificacion,
      nombreCompleto: item.nombreCompleto
    })),
    pagina: respuesta?.pagina ?? pagina,
    tamanoPagina: respuesta?.tamanoPagina ?? tamanoPagina,
    totalRegistros: respuesta?.totalRegistros ?? datos.length,
    totalPaginas: respuesta?.totalPaginas ?? (datos.length > 0 ? 1 : 0)
  };
}
