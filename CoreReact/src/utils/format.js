// Convierte una fecha a "dd/mm/yyyy". Acepta tanto el formato WCF
// "/Date(milisegundos)/" como fechas ISO (las que devuelven los microservicios,
// ej. "1990-05-10T00:00:00").
export function formatearFecha(fecha) {
  if (!fecha) {
    return 'No disponible';
  }

  const texto = String(fecha);

  // Formato WCF: /Date(milisegundos)/
  const coincidencia = texto.match(/\/Date\((-?\d+)/);
  let date;
  if (coincidencia) {
    date = new Date(Number(coincidencia[1]));
  } else {
    const milisegundos = Date.parse(texto);
    if (!Number.isNaN(milisegundos)) {
      date = new Date(milisegundos);
    }
  }

  if (date && !Number.isNaN(date.getTime())) {
    const dia = String(date.getDate()).padStart(2, '0');
    const mes = String(date.getMonth() + 1).padStart(2, '0');
    return `${dia}/${mes}/${date.getFullYear()}`;
  }

  return fecha;
}

// Obtiene las iniciales del nombre completo (primeras dos palabras).
// Igual que el cálculo del avatar en los .php.
export function obtenerIniciales(nombreCompleto) {
  const palabras = String(nombreCompleto ?? '')
    .trim()
    .split(/\s+/)
    .filter(Boolean);

  return palabras
    .slice(0, 2)
    .map((palabra) => palabra.charAt(0).toUpperCase())
    .join('');
}
