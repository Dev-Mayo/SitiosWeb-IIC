
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

DROP PROCEDURE IF EXISTS sp_listar_puestos_disponibles;
DELIMITER $$
CREATE PROCEDURE sp_listar_puestos_disponibles()
BEGIN
    SELECT
    p.puesto_id, p.nombre, p.salario,
    jefe_emp.nombre_completo AS nombre_jefe,
    p.disponible
FROM puestos p
LEFT JOIN empleados jefe_emp ON jefe_emp.puesto_id = p.jefe_puesto_id
WHERE p.disponible = 1
ORDER BY p.nombre;
END$$
DELIMITER ;


