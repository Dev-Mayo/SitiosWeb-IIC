<?php

define('OFE_DB_HOST', 'mysql-admin-personal-iic-2026-admin-personal-iic-2026.k.aivencloud.com');
define('OFE_DB_PORT', 16341);
define('OFE_DB_NAME', 'EMP');
define('OFE_DB_USER', 'avnadmin');
define('OFE_DB_PASS', 'AVNS_D9NXIT8nECYcHW1YV31');

function ofe_conectar_bd() {
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

    return $conectado ? $mysqli : null;
}

$conexion = ofe_conectar_bd();

if (!$conexion) {
    wp_die('No fue posible conectar con la base de datos.');
}

mysqli_set_charset($conexion, 'utf8mb4');


$mensajeExito = '';
$mensajeError = '';

$puestoId = isset($_GET['id'])
    ? absint($_GET['id'])
    : 0;

$codigoConcurso = 0;

$identificacion = '';
$tipoIdentificacion = '';
$nombreCompleto = '';
$fechaNacimiento = '';
$correo = '';
$telefono = '';


/*
|--------------------------------------------------------------------------
| CONSULTAR EL PUESTO Y EL CONCURSO VIGENTE
|--------------------------------------------------------------------------
*/

if ($puestoId > 0) {

    $sqlPuesto = "
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

    $stmtPuesto = mysqli_prepare($conexion, $sqlPuesto);

    if ($stmtPuesto) {

        mysqli_stmt_bind_param(
            $stmtPuesto,
            'i',
            $puestoId
        );

        mysqli_stmt_execute($stmtPuesto);

        $resultadoPuesto = mysqli_stmt_get_result($stmtPuesto);
        $filaPuesto = mysqli_fetch_assoc($resultadoPuesto);

        mysqli_stmt_close($stmtPuesto);

        if ($filaPuesto) {
            $puestoNombre = $filaPuesto['puesto_nombre'];
            $codigoConcurso = (int) $filaPuesto['codigo_concurso'];
        } else {
            $puestoId = 0;
            $puestoNombre = 'Puesto no disponible';
            $mensajeError =
                'El puesto no existe, no está disponible o no tiene un concurso vigente.';
        }
    } else {
        $mensajeError =
            'No fue posible consultar la información del puesto.';
    }
}


/*
|--------------------------------------------------------------------------
| PROCESAR FORMULARIO
|--------------------------------------------------------------------------
*/

if (
    $_SERVER['REQUEST_METHOD'] === 'POST'
    && isset($_POST['guardar_oferente'])
) {

    /*
    |--------------------------------------------------------------------------
    | VALIDAR NONCE
    |--------------------------------------------------------------------------
    */

    $nonceValido = isset($_POST['aut3_nonce'])
        && wp_verify_nonce(
            sanitize_text_field(
                wp_unslash($_POST['aut3_nonce'])
            ),
            'guardar_oferente_aut3'
        );

    if (!$nonceValido) {

        $mensajeError =
            'La solicitud no es válida. Actualice la página e inténtelo nuevamente.';

    } else {

        /*
        |--------------------------------------------------------------------------
        | RECUPERAR DATOS
        |--------------------------------------------------------------------------
        */

        $puestoId = isset($_POST['puesto_id'])
            ? absint($_POST['puesto_id'])
            : 0;

        $identificacion = isset($_POST['identificacion'])
            ? sanitize_text_field(
                wp_unslash($_POST['identificacion'])
            )
            : '';

        $tipoIdentificacion = isset($_POST['tipo_identificacion'])
            ? sanitize_text_field(
                wp_unslash($_POST['tipo_identificacion'])
            )
            : '';

        $nombreCompleto = isset($_POST['nombre_completo'])
            ? sanitize_text_field(
                wp_unslash($_POST['nombre_completo'])
            )
            : '';

        $fechaNacimiento = isset($_POST['fecha_nacimiento'])
            ? sanitize_text_field(
                wp_unslash($_POST['fecha_nacimiento'])
            )
            : '';

        $correo = isset($_POST['correo'])
            ? sanitize_email(
                wp_unslash($_POST['correo'])
            )
            : '';

        $telefono = isset($_POST['telefono'])
            ? sanitize_text_field(
                wp_unslash($_POST['telefono'])
            )
            : '';

        $codigoConcurso = 0;

        $sqlConcurso = "
            SELECT
                p.nombre AS puesto_nombre,
                c.codigo_concurso
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

        $stmtConcurso = mysqli_prepare(
            $conexion,
            $sqlConcurso
        );

        if ($stmtConcurso) {

            mysqli_stmt_bind_param(
                $stmtConcurso,
                'i',
                $puestoId
            );

            mysqli_stmt_execute($stmtConcurso);

            $resultadoConcurso =
                mysqli_stmt_get_result($stmtConcurso);

            $filaConcurso =
                mysqli_fetch_assoc($resultadoConcurso);

            mysqli_stmt_close($stmtConcurso);

            if ($filaConcurso) {
                $codigoConcurso =
                    (int) $filaConcurso['codigo_concurso'];

                $puestoNombre =
                    $filaConcurso['puesto_nombre'];
            }
        }


        /*
        |--------------------------------------------------------------------------
        | VALIDACIONES
        |--------------------------------------------------------------------------
        */

        $tiposPermitidos = array(
            'Cedula',
            'DIMEX',
            'Pasaporte'
        );

        if (
            empty($identificacion)
            || empty($tipoIdentificacion)
            || empty($nombreCompleto)
            || empty($fechaNacimiento)
            || empty($correo)
            || empty($telefono)
        ) {

            $mensajeError =
                'Debe completar todos los campos requeridos.';

        } elseif ($puestoId <= 0 || $codigoConcurso <= 0) {

            $mensajeError =
                'El puesto seleccionado no tiene un concurso vigente.';

        } elseif (
            !in_array(
                $tipoIdentificacion,
                $tiposPermitidos,
                true
            )
        ) {

            $mensajeError =
                'El tipo de identificación seleccionado no es válido.';

        } elseif (!is_email($correo)) {

            $mensajeError =
                'El correo electrónico no tiene un formato válido.';

        } elseif (
            !preg_match(
                '/^[0-9+\-\s]{8,20}$/',
                $telefono
            )
        ) {

            $mensajeError =
                'El teléfono debe contener entre 8 y 20 caracteres válidos.';

        } elseif (
            !DateTime::createFromFormat(
                'Y-m-d',
                $fechaNacimiento
            )
        ) {

            $mensajeError =
                'La fecha de nacimiento no es válida.';

        } elseif (
            !isset($_FILES['curriculum'])
            || $_FILES['curriculum']['error']
                !== UPLOAD_ERR_OK
        ) {

            $mensajeError =
                'Debe seleccionar un currículum válido.';

        } else {

            /*
            |--------------------------------------------------------------------------
            | VALIDAR QUE NO EXISTA LA MISMA POSTULACIÓN
            |--------------------------------------------------------------------------
            */

            $sqlExistePostulacion = "
                SELECT COUNT(*) AS cantidad
                FROM OFE.oferente_concursos
                WHERE identificacion = ?
                  AND codigo_concurso = ?
            ";

            $stmtExistePostulacion = mysqli_prepare(
                $conexion,
                $sqlExistePostulacion
            );

            mysqli_stmt_bind_param(
                $stmtExistePostulacion,
                'si',
                $identificacion,
                $codigoConcurso
            );

            mysqli_stmt_execute($stmtExistePostulacion);

            $resultadoExiste =
                mysqli_stmt_get_result(
                    $stmtExistePostulacion
                );

            $filaExiste =
                mysqli_fetch_assoc($resultadoExiste);

            mysqli_stmt_close(
                $stmtExistePostulacion
            );

            if ((int) $filaExiste['cantidad'] > 0) {

                $mensajeError =
                    'El oferente ya está registrado en este concurso.';

            } else {

                /*
                |--------------------------------------------------------------------------
                | VALIDAR ARCHIVO
                |--------------------------------------------------------------------------
                */

                $archivo = $_FILES['curriculum'];

                $nombreArchivo = sanitize_file_name(
                    $archivo['name']
                );

                $extension = strtolower(
                    pathinfo(
                        $nombreArchivo,
                        PATHINFO_EXTENSION
                    )
                );

                $extensionesPermitidas = array(
                    'pdf',
                    'doc',
                    'docx'
                );

                $tamanoMaximo = 5 * 1024 * 1024;

                if (
                    !in_array(
                        $extension,
                        $extensionesPermitidas,
                        true
                    )
                ) {

                    $mensajeError =
                        'El currículum debe ser PDF, DOC o DOCX.';

                } elseif (
                    (int) $archivo['size'] > $tamanoMaximo
                ) {

                    $mensajeError =
                        'El currículum no puede superar los 5 MB.';

                } else {

                    /*
                    |--------------------------------------------------------------------------
                    | SUBIR CURRÍCULUM A WORDPRESS
                    |--------------------------------------------------------------------------
                    */

                    require_once ABSPATH
                        . 'wp-admin/includes/file.php';

                    $subida = wp_handle_upload(
                        $archivo,
                        array(
                            'test_form' => false,
                            'mimes' => array(
                                'pdf' =>
                                    'application/pdf',

                                'doc' =>
                                    'application/msword',

                                'docx' =>
                                    'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
                            )
                        )
                    );

                    if (
                        isset($subida['error'])
                        || empty($subida['url'])
                    ) {

                        $mensajeError = isset($subida['error'])
                            ? $subida['error']
                            : 'No fue posible subir el currículum.';

                    } else {

                        $curriculumRuta = esc_url_raw(
                            $subida['url']
                        );

                        /*
                        |--------------------------------------------------------------------------
                        | INICIAR TRANSACCIÓN
                        |--------------------------------------------------------------------------
                        */

                        mysqli_begin_transaction($conexion);

                        try {

                            /*
                            |--------------------------------------------------------------------------
                            | INSERTAR O ACTUALIZAR OFERENTE
                            |--------------------------------------------------------------------------
                            */

                            $sqlOferente = "
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
                                    tipo_identificacion =
                                        VALUES(tipo_identificacion),

                                    nombre_completo =
                                        VALUES(nombre_completo),

                                    fecha_nacimiento =
                                        VALUES(fecha_nacimiento)
                            ";

                            $stmtOferente = mysqli_prepare(
                                $conexion,
                                $sqlOferente
                            );

                            if (!$stmtOferente) {
                                throw new Exception(
                                    mysqli_error($conexion)
                                );
                            }

                            mysqli_stmt_bind_param(
                                $stmtOferente,
                                'ssss',
                                $identificacion,
                                $tipoIdentificacion,
                                $nombreCompleto,
                                $fechaNacimiento
                            );

                            if (
                                !mysqli_stmt_execute(
                                    $stmtOferente
                                )
                            ) {
                                throw new Exception(
                                    mysqli_stmt_error(
                                        $stmtOferente
                                    )
                                );
                            }

                            mysqli_stmt_close($stmtOferente);


                            /*
                            |--------------------------------------------------------------------------
                            | INSERTAR CORREO SI NO EXISTE
                            |--------------------------------------------------------------------------
                            */

                            $sqlCorreo = "
                                INSERT INTO OFE.oferente_emails
                                (
                                    identificacion,
                                    email
                                )
                                SELECT ?, ?
                                WHERE NOT EXISTS
                                (
                                    SELECT 1
                                    FROM OFE.oferente_emails
                                    WHERE identificacion = ?
                                      AND email = ?
                                )
                            ";

                            $stmtCorreo = mysqli_prepare(
                                $conexion,
                                $sqlCorreo
                            );

                            if (!$stmtCorreo) {
                                throw new Exception(
                                    mysqli_error($conexion)
                                );
                            }

                            mysqli_stmt_bind_param(
                                $stmtCorreo,
                                'ssss',
                                $identificacion,
                                $correo,
                                $identificacion,
                                $correo
                            );

                            if (
                                !mysqli_stmt_execute(
                                    $stmtCorreo
                                )
                            ) {
                                throw new Exception(
                                    mysqli_stmt_error(
                                        $stmtCorreo
                                    )
                                );
                            }

                            mysqli_stmt_close($stmtCorreo);


                            /*
                            |--------------------------------------------------------------------------
                            | INSERTAR TELÉFONO SI NO EXISTE
                            |--------------------------------------------------------------------------
                            */

                            $sqlTelefono = "
                                INSERT INTO OFE.oferente_telefonos
                                (
                                    identificacion,
                                    telefono
                                )
                                SELECT ?, ?
                                WHERE NOT EXISTS
                                (
                                    SELECT 1
                                    FROM OFE.oferente_telefonos
                                    WHERE identificacion = ?
                                      AND telefono = ?
                                )
                            ";

                            $stmtTelefono = mysqli_prepare(
                                $conexion,
                                $sqlTelefono
                            );

                            if (!$stmtTelefono) {
                                throw new Exception(
                                    mysqli_error($conexion)
                                );
                            }

                            mysqli_stmt_bind_param(
                                $stmtTelefono,
                                'ssss',
                                $identificacion,
                                $telefono,
                                $identificacion,
                                $telefono
                            );

                            if (
                                !mysqli_stmt_execute(
                                    $stmtTelefono
                                )
                            ) {
                                throw new Exception(
                                    mysqli_stmt_error(
                                        $stmtTelefono
                                    )
                                );
                            }

                            mysqli_stmt_close($stmtTelefono);


                            /*
                            |--------------------------------------------------------------------------
                            | INSERTAR POSTULACIÓN
                            |--------------------------------------------------------------------------
                            */

                            $sqlPostulacion = "
                                INSERT INTO OFE.oferente_concursos
                                (
                                    identificacion,
                                    codigo_concurso,
                                    curriculum_ruta
                                )
                                VALUES (?, ?, ?)
                            ";

                            $stmtPostulacion = mysqli_prepare(
                                $conexion,
                                $sqlPostulacion
                            );

                            if (!$stmtPostulacion) {
                                throw new Exception(
                                    mysqli_error($conexion)
                                );
                            }

                            mysqli_stmt_bind_param(
                                $stmtPostulacion,
                                'sis',
                                $identificacion,
                                $codigoConcurso,
                                $curriculumRuta
                            );

                            if (
                                !mysqli_stmt_execute(
                                    $stmtPostulacion
                                )
                            ) {
                                throw new Exception(
                                    mysqli_stmt_error(
                                        $stmtPostulacion
                                    )
                                );
                            }

                            mysqli_stmt_close(
                                $stmtPostulacion
                            );


                            /*
                            |--------------------------------------------------------------------------
                            | CONFIRMAR TRANSACCIÓN
                            |--------------------------------------------------------------------------
                            */

                            mysqli_commit($conexion);

                            $mensajeExito =
                                'Datos guardados de manera satisfactoria.';

                            $identificacion = '';
                            $tipoIdentificacion = '';
                            $nombreCompleto = '';
                            $fechaNacimiento = '';
                            $correo = '';
                            $telefono = '';

                        } catch (Throwable $error) {

                            mysqli_rollback($conexion);

                            /*
                            | Si falló la BD, se elimina el archivo subido.
                            */
                            if (
                                !empty($subida['file'])
                                && file_exists($subida['file'])
                            ) {
                                unlink($subida['file']);
                            }

                            $mensajeError =
                                'No fue posible guardar los datos: '
                                . $error->getMessage();
                        }
                    }
                }
            }
        }
    }
}

get_header();
?>

<!-- HERO -->
<section class="hero">
    <div class="container">
        <h1>Registro de Oferente</h1>

        <p>
            Complete sus datos para participar por el puesto seleccionado.
        </p>
    </div>
</section>

<!-- FORMULARIO AUT3 -->
<section class="section">
    <div class="container">

        <div class="row justify-content-center">
            <div class="col-lg-9">

                <div class="card-shirofy">
                    <div class="card-body p-4 p-md-5">

                        <div class="mb-4">

                            <p class="text-uppercase fw-semibold mb-1"
                               style="color:var(--primary);
                                      font-size:0.85rem;
                                      letter-spacing:1px">

                                Puesto seleccionado
                            </p>

                            <h2 class="section-title mb-2">
                                <?php
                                echo esc_html($puestoNombre);
                                ?>
                            </h2>

                            <div class="divider"></div>
                        </div>


                        <?php if (!empty($mensajeError)): ?>

                            <div class="alert alert-danger">
                                <?php
                                echo esc_html($mensajeError);
                                ?>
                            </div>

                        <?php endif; ?>


                        <?php if (!empty($mensajeExito)): ?>

                            <div class="alert alert-success">

                                <p class="mb-3">
                                    <?php
                                    echo esc_html($mensajeExito);
                                    ?>
                                </p>

                                <a href="<?php
                                    echo esc_url(
                                        home_url('/puestos/')
                                    );
                                ?>"
                                   class="btn btn-success">

                                    Aceptar
                                </a>

                            </div>

                        <?php else: ?>

                            <?php if ($puestoId <= 0): ?>

                                <div class="alert alert-warning">
                                    No se recibió un puesto válido.
                                    Regrese al listado y seleccione uno.
                                </div>

                            <?php endif; ?>


                            <form method="post"
                                  enctype="multipart/form-data"
                                  id="formRegistroOferente">

                                <?php
                                wp_nonce_field(
                                    'guardar_oferente_aut3',
                                    'aut3_nonce'
                                );
                                ?>

                                <input type="hidden"
                                       name="puesto_id"
                                       value="<?php
                                            echo esc_attr(
                                                $puestoId
                                            );
                                       ?>">


                                <div class="row g-3">

                                    <div class="col-md-6">

                                        <label for="identificacion"
                                               class="form-label">

                                            Identificación
                                        </label>

                                        <input type="text"
                                               id="identificacion"
                                               name="identificacion"
                                               class="form-control"
                                               maxlength="20"
                                               value="<?php
                                                    echo esc_attr(
                                                        $identificacion
                                                    );
                                               ?>"
                                               required>
                                    </div>


                                    <div class="col-md-6">

                                        <label for="tipo_identificacion"
                                               class="form-label">

                                            Tipo de identificación
                                        </label>

                                        <select id="tipo_identificacion"
                                                name="tipo_identificacion"
                                                class="form-select"
                                                required>

                                            <option value="">
                                                Seleccione
                                            </option>

                                            <option value="Cedula"
                                                <?php
                                                selected(
                                                    $tipoIdentificacion,
                                                    'Cedula'
                                                );
                                                ?>>

                                                Cédula de identidad
                                            </option>

                                            <option value="DIMEX"
                                                <?php
                                                selected(
                                                    $tipoIdentificacion,
                                                    'DIMEX'
                                                );
                                                ?>>

                                                DIMEX
                                            </option>

                                            <option value="Pasaporte"
                                                <?php
                                                selected(
                                                    $tipoIdentificacion,
                                                    'Pasaporte'
                                                );
                                                ?>>

                                                Pasaporte
                                            </option>
                                        </select>
                                    </div>


                                    <div class="col-12">

                                        <label for="nombre_completo"
                                               class="form-label">

                                            Nombre completo
                                        </label>

                                        <input type="text"
                                               id="nombre_completo"
                                               name="nombre_completo"
                                               class="form-control"
                                               maxlength="100"
                                               value="<?php
                                                    echo esc_attr(
                                                        $nombreCompleto
                                                    );
                                               ?>"
                                               required>
                                    </div>


                                    <div class="col-md-6">

                                        <label for="fecha_nacimiento"
                                               class="form-label">

                                            Fecha de nacimiento
                                        </label>

                                        <input type="date"
                                               id="fecha_nacimiento"
                                               name="fecha_nacimiento"
                                               class="form-control"
                                               value="<?php
                                                    echo esc_attr(
                                                        $fechaNacimiento
                                                    );
                                               ?>"
                                               required>
                                    </div>


                                    <div class="col-md-6">

                                        <label for="correo"
                                               class="form-label">

                                            Correo electrónico
                                        </label>

                                        <input type="email"
                                               id="correo"
                                               name="correo"
                                               class="form-control"
                                               maxlength="100"
                                               value="<?php
                                                    echo esc_attr(
                                                        $correo
                                                    );
                                               ?>"
                                               required>
                                    </div>


                                    <div class="col-md-6">

                                        <label for="telefono"
                                               class="form-label">

                                            Teléfono
                                        </label>

                                        <input type="tel"
                                               id="telefono"
                                               name="telefono"
                                               class="form-control"
                                               maxlength="20"
                                               pattern="[0-9+\-\s]{8,20}"
                                               value="<?php
                                                    echo esc_attr(
                                                        $telefono
                                                    );
                                               ?>"
                                               required>

                                        <small class="text-muted">
                                            Debe indicar al menos un teléfono.
                                        </small>
                                    </div>


                                    <div class="col-md-6">

                                        <label for="curriculum"
                                               class="form-label">

                                            Currículum
                                        </label>

                                        <input type="file"
                                               id="curriculum"
                                               name="curriculum"
                                               class="form-control"
                                               accept=".pdf,.doc,.docx"
                                               required>

                                        <small class="text-muted">
                                            Formatos permitidos:
                                            PDF, DOC y DOCX.
                                            Máximo 5 MB.
                                        </small>
                                    </div>

                                </div>


                                <div class="d-flex flex-column
                                            flex-sm-row
                                            justify-content-end
                                            gap-2 mt-4">

                                    <a href="<?php
                                        echo esc_url(
                                            home_url('/puestos/')
                                        );
                                    ?>"
                                       class="btn btn-secondary">

                                        Cancelar
                                    </a>

                                    <button type="submit"
                                            name="guardar_oferente"
                                            class="btn text-white"
                                            style="background:var(--primary)"
                                            <?php
                                            echo $puestoId <= 0
                                                ? 'disabled'
                                                : '';
                                            ?>>

                                        Aceptar
                                    </button>

                                </div>

                            </form>

                        <?php endif; ?>

                    </div>
                </div>

            </div>
        </div>

    </div>
</section>

<?php
mysqli_close($conexion);
get_footer();
?>