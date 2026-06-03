
-- =========================================
-- SEG
-- =========================================
use SEG;

DELIMITER $$
CREATE PROCEDURE sp_rol_en_uso_bit(IN p_id_rol INT) -- Procedimiento de consulta para validar info despues
BEGIN
    IF EXISTS (SELECT 1 FROM usuarios_roles WHERE id_rol = p_id_rol)
       OR EXISTS (SELECT 1 FROM roles_modulos WHERE id_rol = p_id_rol) THEN
        SELECT 1 AS EnUso;
    ELSE
        SELECT 0 AS EnUso;
    END IF;
END $$
DELIMITER ;