
-- =========================================
-- EMP
-- =========================================
use EMP;

DELIMITER $$
CREATE PROCEDURE sp_obtener_nombre_empleados()
BEGIN
        -- Todos los oferentes
        SELECT empleado_id, nombre_completo
        FROM empleados;
END$$
DELIMITER $$