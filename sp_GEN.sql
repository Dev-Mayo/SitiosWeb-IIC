
-- =========================================
-- Instituciones
-- =========================================
USE GEN;

DELIMITER $$
CREATE PROCEDURE ObtenerInstituciones()
BEGIN
    SELECT * FROM inst_educativas;
END$$
DELIMITER $$
