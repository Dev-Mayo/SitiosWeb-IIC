import { getJson } from './apiClient';
import { SERVICES } from '../config';

// Detalle del oferente. GET /api/oferentes/{identificacion} devuelve el
// DetalleOferenteResponse directamente (404 -> { mensaje }). Normaliza la
// respuesta (camelCase del backend) al formato que espera la vista.
export async function obtenerDetalleOferente(identificacion, _usuario) {
  const ruta = `${SERVICES.detalleOferente}/${encodeURIComponent(identificacion)}`;
  const resultado = await getJson(ruta);

  return {
    identificacion: resultado.identificacion,
    tipoIdentificacion: resultado.tipoIdentificacion,
    nombreCompleto: resultado.nombreCompleto,
    fechaNacimiento: resultado.fechaNacimiento,
    contratado: Boolean(resultado.contratado),
    correos: Array.isArray(resultado.correos) ? resultado.correos : [],
    telefonos: Array.isArray(resultado.telefonos) ? resultado.telefonos : [],
    preparacionAcademica: Array.isArray(resultado.preparacionAcademica)
      ? resultado.preparacionAcademica.map((item) => ({
          CodigoInstitucion: item.codigoInstitucion,
          Titulo: item.titulo,
          FechaInicio: item.fechaInicio,
          FechaFin: item.fechaFin
        }))
      : [],
    experienciaLaboral: Array.isArray(resultado.experienciaLaboral)
      ? resultado.experienciaLaboral.map((item) => ({
          Empresa: item.empresa,
          Puesto: item.puesto,
          FechaInicio: item.fechaInicio,
          FechaFin: item.fechaFin
        }))
      : [],
    concursos: Array.isArray(resultado.concursos)
      ? resultado.concursos.map((item) => ({
          CodigoConcurso: item.codigoConcurso,
          Nombre: item.nombre,
          FechaInicio: item.fechaInicio,
          FechaFin: item.fechaFin,
          Estado: item.estado
        }))
      : [],
    curriculum: resultado.curriculum
  };
}
