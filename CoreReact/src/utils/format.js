// Convierte una fecha en formato WCF "/Date(milisegundos)/" a "dd/mm/yyyy".
// Igual que formatearFechaWcf() de detalle_oferente.php.
export function formatearFechaWcf(fechaWcf) {
  if (!fechaWcf) {
    return 'No disponible';
  }

  const coincidencia = String(fechaWcf).match(/\/Date\((-?\d+)/);
  if (coincidencia) {
    const fecha = new Date(Number(coincidencia[1]));
    if (!Number.isNaN(fecha.getTime())) {
      const dia = String(fecha.getDate()).padStart(2, '0');
      const mes = String(fecha.getMonth() + 1).padStart(2, '0');
      return `${dia}/${mes}/${fecha.getFullYear()}`;
    }
  }

  return fechaWcf;
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
