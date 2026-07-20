
-- =========================================
-- EMP
-- =========================================
use EMP;

DELIMITER $$
CREATE PROCEDURE sp_obtener_nombre_empleados()
BEGIN
        -- Todos los oferentes
        SELECT empleado_id as EmpleadoId, nombre_completo as NombreCompleto
        FROM empleados;
END$$
DELIMITER $$







DROP PROCEDURE IF EXISTS EMP.sp_listar_puestos_disponibles;

DELIMITER $$
CREATE PROCEDURE EMP.sp_listar_puestos_disponibles()
BEGIN
    SELECT DISTINCT
        p.puesto_id,
        p.nombre,
        p.salario,
        COALESCE(j.nombre_completo, 'Sin asignar') AS nombre_jefe,
        p.disponible
    FROM EMP.puestos p
    LEFT JOIN EMP.empleados j
        ON j.empleado_id = p.jefe_puesto_id
    INNER JOIN OFE.concursos c
        ON c.puesto_id = p.puesto_id
    WHERE p.disponible = 1
      AND c.estado = 'Vigente'
      AND CURDATE() BETWEEN c.fecha_inicio AND c.fecha_fin
    ORDER BY p.nombre;
END$$
DELIMITER ;

