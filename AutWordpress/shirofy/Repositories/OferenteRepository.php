<?php

require_once __DIR__ . '/../ET/OferenteET.php';

class OferenteRepository
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

    public function obtenerPuestoConcursoVigente(int $puestoId): ?array
    {
        $sql = "
            SELECT
                p.puesto_id,
                p.nombre AS puesto_nombre,
                c.codigo_concurso,
                c.nombre AS concurso_nombre
            FROM EMP.puestos p
            INNER JOIN OFE.concursos c
                ON c.puesto_id = p.puesto_id
            WHERE p.puesto_id = ?
              AND p.disponible = 1
              AND c.estado = 'Vigente'
              AND CURDATE() BETWEEN c.fecha_inicio AND c.fecha_fin
            ORDER BY c.fecha_inicio DESC
            LIMIT 1
        ";

        $stmt = mysqli_prepare($this->conexion, $sql);
        if (!$stmt) {
            error_log('Error preparando obtenerPuestoConcursoVigente: ' . mysqli_error($this->conexion));
            return null;
        }

        mysqli_stmt_bind_param($stmt, 'i', $puestoId);
        mysqli_stmt_execute($stmt);
        $resultado = mysqli_stmt_get_result($stmt);
        $fila = mysqli_fetch_assoc($resultado);
        mysqli_stmt_close($stmt);

        return $fila ?: null;
    }

    public function existePostulacion(string $identificacion, int $codigoConcurso): bool
    {
        $sql = "
            SELECT COUNT(*) AS cantidad
            FROM OFE.oferente_concursos
            WHERE identificacion = ?
              AND codigo_concurso = ?
        ";

        $stmt = mysqli_prepare($this->conexion, $sql);
        if (!$stmt) {
            throw new RuntimeException(mysqli_error($this->conexion));
        }

        mysqli_stmt_bind_param($stmt, 'si', $identificacion, $codigoConcurso);
        mysqli_stmt_execute($stmt);
        $resultado = mysqli_stmt_get_result($stmt);
        $fila = mysqli_fetch_assoc($resultado);
        mysqli_stmt_close($stmt);

        return (int) $fila['cantidad'] > 0;
    }

    public function guardarPostulacion(OferenteET $oferente, string $curriculumRuta): void
    {
        mysqli_begin_transaction($this->conexion);

        try {
            $this->guardarOferente($oferente);
            $this->guardarCorreo($oferente->identificacion, $oferente->correo);
            $this->guardarTelefono($oferente->identificacion, $oferente->telefono);
            $this->guardarConcurso(
                $oferente->identificacion,
                $oferente->codigoConcurso,
                $curriculumRuta
            );
            mysqli_commit($this->conexion);
        } catch (Throwable $error) {
            mysqli_rollback($this->conexion);
            throw $error;
        }
    }

    private function guardarOferente(OferenteET $oferente): void
    {
        $sql = "
            INSERT INTO OFE.oferentes
            (
                identificacion,
                tipo_identificacion,
                nombre_completo,
                fecha_nacimiento,
                contratado
            )
            VALUES (?, ?, ?, ?, 0)
            ON DUPLICATE KEY UPDATE
                tipo_identificacion = VALUES(tipo_identificacion),
                nombre_completo = VALUES(nombre_completo),
                fecha_nacimiento = VALUES(fecha_nacimiento)
        ";

        $stmt = mysqli_prepare($this->conexion, $sql);
        if (!$stmt) {
            throw new RuntimeException(mysqli_error($this->conexion));
        }

        mysqli_stmt_bind_param(
            $stmt,
            'ssss',
            $oferente->identificacion,
            $oferente->tipoIdentificacion,
            $oferente->nombreCompleto,
            $oferente->fechaNacimiento
        );

        if (!mysqli_stmt_execute($stmt)) {
            $error = mysqli_stmt_error($stmt);
            mysqli_stmt_close($stmt);
            throw new RuntimeException($error);
        }

        mysqli_stmt_close($stmt);
    }

    private function guardarCorreo(string $identificacion, string $correo): void
    {
        $sql = "
            INSERT INTO OFE.oferente_emails (identificacion, email)
            SELECT ?, ?
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM OFE.oferente_emails
                WHERE identificacion = ?
                  AND email = ?
            )
        ";

        $stmt = mysqli_prepare($this->conexion, $sql);
        if (!$stmt) {
            throw new RuntimeException(mysqli_error($this->conexion));
        }

        mysqli_stmt_bind_param($stmt, 'ssss', $identificacion, $correo, $identificacion, $correo);

        if (!mysqli_stmt_execute($stmt)) {
            $error = mysqli_stmt_error($stmt);
            mysqli_stmt_close($stmt);
            throw new RuntimeException($error);
        }

        mysqli_stmt_close($stmt);
    }

    private function guardarTelefono(string $identificacion, string $telefono): void
    {
        $sql = "
            INSERT INTO OFE.oferente_telefonos (identificacion, telefono)
            SELECT ?, ?
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM OFE.oferente_telefonos
                WHERE identificacion = ?
                  AND telefono = ?
            )
        ";

        $stmt = mysqli_prepare($this->conexion, $sql);
        if (!$stmt) {
            throw new RuntimeException(mysqli_error($this->conexion));
        }

        mysqli_stmt_bind_param($stmt, 'ssss', $identificacion, $telefono, $identificacion, $telefono);

        if (!mysqli_stmt_execute($stmt)) {
            $error = mysqli_stmt_error($stmt);
            mysqli_stmt_close($stmt);
            throw new RuntimeException($error);
        }

        mysqli_stmt_close($stmt);
    }

    private function guardarConcurso(string $identificacion, int $codigoConcurso, string $curriculumRuta): void
    {
        $sql = "
            INSERT INTO OFE.oferente_concursos
            (
                identificacion,
                codigo_concurso,
                curriculum_ruta
            )
            VALUES (?, ?, ?)
        ";

        $stmt = mysqli_prepare($this->conexion, $sql);
        if (!$stmt) {
            throw new RuntimeException(mysqli_error($this->conexion));
        }

        mysqli_stmt_bind_param($stmt, 'sis', $identificacion, $codigoConcurso, $curriculumRuta);

        if (!mysqli_stmt_execute($stmt)) {
            $error = mysqli_stmt_error($stmt);
            mysqli_stmt_close($stmt);
            throw new RuntimeException($error);
        }

        mysqli_stmt_close($stmt);
    }

    public function cerrar(): void
    {
        mysqli_close($this->conexion);
    }
}
