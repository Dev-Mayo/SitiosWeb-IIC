import { postJson } from './apiClient';
import { SERVICES } from '../config';

// Equivalente a la autenticación de login.php (Core 4).
// Retorna un objeto normalizado: { exito, idUsuario, nombreCompleto, usuario }
// o { exito: false, mensaje, bloqueado }.
export async function autenticar({ usuario, password }) {
  const resultado = await postJson(SERVICES.autenticacion, {
    Usuario: usuario,
    Password: password
  });

  if (!resultado.Success) {
    const mensaje = resultado.Mensaje ?? 'Usuario y/o contraseña incorrectos.';
    return {
      exito: false,
      mensaje,
      bloqueado: mensaje.toLowerCase().includes('bloqueado')
    };
  }

  return {
    exito: true,
    idUsuario: resultado.IdUsuario,
    nombreCompleto: resultado.NombreCompleto,
    usuario
  };
}
