
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