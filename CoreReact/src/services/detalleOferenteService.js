import { postJson } from './apiClient';
import { SERVICES } from '../config';

// Equivalente a detalle_oferente.php (Core 8). POST a DetalleOferenteService.
// Normaliza la respuesta para que la vista no dependa del WCF.
export async function obtenerDetalleOferente(identificacion, usuario) {
  const resultado = await postJson(SERVICES.detalleOferente, {
    Identificacion: identificacion,
    Usuario: usuario
  });

  if (!resultado.Exito) {
    throw new Error(
      resultado.Mensaje ?? 'No se encontró información para el oferente seleccionado.'
    );
  }

  return {
    identificacion: resultado.Identificacion,
    tipoIdentificacion: resultado.TipoIdentificacion,
    nombreCompleto: resultado.NombreCompleto,
    fechaNacimiento: resultado.FechaNacimiento,
    contratado: Boolean(resultado.Contratado),
    correos: Array.isArray(resultado.Correos) ? resultado.Correos : [],
    telefonos: Array.isArray(resultado.Telefonos) ? resultado.Telefonos : [],
    preparacionAcademica: Array.isArray(resultado.PreparacionAcademica)
      ? resultado.PreparacionAcademica
      : [],
    experienciaLaboral: Array.isArray(resultado.ExperienciaLaboral)
      ? resultado.ExperienciaLaboral
      : [],
    concursos: Array.isArray(resultado.Concursos) ? resultado.Concursos : [],
    curriculum: resultado.Curriculum
  };
}
