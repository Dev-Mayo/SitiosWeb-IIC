import { postJson } from './apiClient';
import { SERVICES } from '../config';

// Equivalente al botón "Crear empleado" de detalle_oferente.php (Core 3).
// POST a EmpleadoService. Lanza error si el servicio no confirma el registro.
export async function registrarEmpleado({
  identificacion,
  tipoIdentificacion,
  nombreCompleto,
  fechaNacimiento,
  puestoId,
  correos,
  telefonos,
  usuario
}) {
  const resultado = await postJson(SERVICES.registrarEmpleado, {
    Identificacion: identificacion,
    TipoIdentificacion: tipoIdentificacion,
    NombreCompleto: nombreCompleto,
    FechaNacimiento: fechaNacimiento,
    PuestoId: Number(puestoId) || 0,
    Correos: Array.isArray(correos) ? correos : [],
    Telefonos: Array.isArray(telefonos) ? telefonos : [],
    Usuario: usuario
  });

  if (!resultado.Exito) {
    throw new Error(
      resultado.Mensaje ?? 'No fue posible registrar el empleado.'
    );
  }

  return {
    exito: true,
    empleadoId: resultado.EmpleadoId
  };
}
