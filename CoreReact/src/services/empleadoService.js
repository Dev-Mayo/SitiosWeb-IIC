import { postJson } from './apiClient';
import { SERVICES } from '../config';

// Registro de empleado a partir de un oferente. POST /api/empleados.
// En caso de error HTTP (400/409/500) apiClient lanza un Error con el mensaje
// del servidor, por lo que aquí solo se devuelve la confirmación.
export async function registrarEmpleado({
  identificacion,
  tipoIdentificacion,
  nombreCompleto,
  fechaNacimiento,
  puestoId,
  correos,
  telefonos,
  _usuario
}) {
  const resultado = await postJson(SERVICES.registrarEmpleado, {
    Identificacion: identificacion,
    TipoIdentificacion: tipoIdentificacion,
    NombreCompleto: nombreCompleto,
    FechaNacimiento: fechaNacimiento,
    PuestoId: Number(puestoId) || 0,
    Correos: Array.isArray(correos) ? correos : [],
    Telefonos: Array.isArray(telefonos) ? telefonos : []
  });

  return {
    exito: true,
    empleadoId: resultado.empleadoId
  };
}
