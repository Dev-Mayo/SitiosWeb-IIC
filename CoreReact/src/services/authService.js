import { postJson } from './apiClient';
import { SERVICES } from '../config';

/**
 * Autentica contra el microservicio Auth vía el gateway.
 * En caso de éxito devuelve el perfil + token JWT; en caso de error devuelve
 * un objeto con exito=false y el mensaje del servidor.
 */
export async function autenticar({ usuario, password }) {
  try {
    const resultado = await postJson(SERVICES.autenticacion, {
      Usuario: usuario,
      Password: password
    });

    return {
      exito: true,
      idUsuario: resultado.idUsuario,
      nombreCompleto: resultado.nombreCompleto,
      usuario: resultado.usuario ?? usuario,
      token: resultado.token,
      expiraEn: resultado.expiraEn
    };
  } catch (err) {
    return {
      exito: false,
      mensaje: err.message ?? 'Usuario y/o contraseña incorrectos.',
      bloqueado: err.status === 423
    };
  }
}
