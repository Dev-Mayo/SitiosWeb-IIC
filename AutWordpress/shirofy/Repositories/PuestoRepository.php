<?php

require_once __DIR__ . '/../ET/PuestoET.php';

class PuestoRepository
{
    private mysqli $conexion;

    public function __construct()
    {
        if (!defined('OFE_DB_HOST')) {
            define('OFE_DB_HOST', 'mysql-admin-personal-iic-2026-admin-personal-iic-2026.k.aivencloud.com');
            define('OFE_DB_PORT', 16341);
            define('OFE_DB_NAME', 'EMP');
            define('OFE_DB_USER', 'avnadmin');
            define('OFE_DB_PASS', 'AVNS_D9NXIT8nECYcHW1YV31');
        }

        $mysqli = mysqli_init();
        $mysqli->ssl_set(null, null, null, null, null);
        $conectado = @$mysqli->real_connect(
            OFE_DB_HOST,
            OFE_DB_USER,
            OFE_DB_PASS,
            OFE_DB_NAME,
            OFE_DB_PORT,
            null,
            MYSQLI_CLIENT_SSL
        );

        if (!$conectado) {
            throw new RuntimeException('No fue posible conectar con la base de datos.');
        }

        mysqli_set_charset($mysqli, 'utf8mb4');
        $this->conexion = $mysqli;
    }

    /**
     * @return PuestoET[]
     */
    public function listarDisponibles(): array
    {
        $puestos = array();

        if ($this->conexion->multi_query("CALL sp_listar_puestos_disponibles()")) {
            do {
                if ($resultado = $this->conexion->store_result()) {
                    while ($fila = $resultado->fetch_assoc()) {
                        $puesto = new PuestoET();
                        $puesto->cargarDesdeFila($fila);
                        $puestos[] = $puesto;
                    }
                    $resultado->free();
                }
            } while ($this->conexion->more_results() && $this->conexion->next_result());
        } else {
            throw new RuntimeException('No se pudieron cargar los puestos disponibles en este momento.');
        }

        return $puestos;
    }

    public function cerrar(): void
    {
        mysqli_close($this->conexion);
    }
}