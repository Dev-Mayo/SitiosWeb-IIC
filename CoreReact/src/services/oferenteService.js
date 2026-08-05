import { postJson } from './apiClient';
import { SERVICES } from '../config';

// Equivalente a oferentes.php (Core 2). POST a OferenteService.
export async function obtenerOferentesPorPuesto(codigoPuesto, usuario) {
  const resultado = await postJson(SERVICES.oferentesPorPuesto, {
    CodigoPuesto: Number(codigoPuesto) || 0,
    Usuario: usuario
  });

  if (!resultado.Success) {
    throw new Error(
      resultado.Mensaje ?? 'No se pudo obtener el listado de oferentes.'
    );
  }

  const oferentes = Array.isArray(resultado.Oferentes) ? resultado.Oferentes : [];

  return oferentes.map((item) => ({
    identificacion: item.Identificacion,
    nombreCompleto: item.NombreCompleto
  }));
}
