import { getJson } from './apiClient';
import { SERVICES } from '../config';

// GET /api/puestos?pagina=1&tamanoPagina=10
export async function listarPuestos(pagina = 1, tamanoPagina = 10) {

  const url =
    `${SERVICES.puestos}?pagina=${pagina}&tamanoPagina=${tamanoPagina}`;

  const respuesta = await getJson(url);

  const puestos = Array.isArray(respuesta?.puestos)
    ? respuesta.puestos
    : [];

  const lista = puestos
    .map((item) => ({
      puestoId: Number(item.puestoId) || 0,
      nombre: String(item.nombre ?? '').trim()
    }))
    .filter(
      (p) =>
        p.puestoId > 0 &&
        p.nombre !== ''
    );

  return {
    puestos: lista,
    pagina: Number(respuesta?.pagina) || pagina,
    tamanoPagina:
      Number(respuesta?.tamanoPagina) || tamanoPagina,
    totalRegistros:
      Number(respuesta?.totalRegistros) || 0,
    totalPaginas:
      Number(respuesta?.totalPaginas) || 0
  };
}