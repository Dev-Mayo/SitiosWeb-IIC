<?php
session_start();

// Proteger la página
if (!isset($_SESSION['id_usuario'])) {
    header("Location: login.php?msg=login");
    exit;
}

$nombre_completo = $_SESSION['nombre_completo'] ?? 'Usuario';
$usuario         = $_SESSION['usuario'] ?? '';

// Recibir datos enviados desde Core 7
$identificacion = trim($_GET['identificacion'] ?? '');
$codigo_puesto  = filter_input(INPUT_GET, 'codigo_puesto', FILTER_VALIDATE_INT);

$error   = '';
$mensaje = '';

// Validar parámetros
if ($identificacion === '') {
    $error = 'No se recibió una identificación válida del oferente.';
}

if (!$codigo_puesto) {
    $codigo_puesto = 0;
}

// Iniciales para avatar
$palabras  = explode(' ', $nombre_completo);
$iniciales = '';

foreach (array_slice($palabras, 0, 2) as $p) {
    $iniciales .= strtoupper(mb_substr($p, 0, 1));
}


function formatearFechaWcf($fechaWcf)
{
    if (empty($fechaWcf)) {
        return 'No disponible';
    }

    if (preg_match('/\/Date\((\-?\d+)/', $fechaWcf, $coincidencias)) {
        $milisegundos = (int)$coincidencias[1];
        $segundos = (int)($milisegundos / 1000);

        return date('d/m/Y', $segundos);
    }

    return $fechaWcf;
}
/*
|--------------------------------------------------------------------------
| CONSUMO DEL SERVICIO CORE 8
|--------------------------------------------------------------------------
*/

$oferente = null;

if ($identificacion !== '' && $error === '') {

    $wcf_url = "http://localhost:63602/DetalleOferenteService.svc/obtener-detalle";

    $payload = json_encode([
        "Identificacion" => $identificacion
    ]);

    $ch = curl_init($wcf_url);

    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_POST, true);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $payload);

    curl_setopt($ch, CURLOPT_HTTPHEADER, [
        'Content-Type: application/json',
        'Content-Length: ' . strlen($payload)
    ]);

    curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
    curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false);

    $response = curl_exec($ch);
    $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    $curlError = curl_error($ch);

    curl_close($ch);
    
    if ($response === false || $httpCode !== 200) {
        $error = "Error al conectar con el servicio de detalle de oferente.";

        if ($curlError !== '') {
            $error .= " Detalle: " . $curlError;
        }
    } else {
        $resultado = json_decode($response, true);

        if (!is_array($resultado)) {
            $error = "El servicio devolvió una respuesta inválida.";
        } elseif (!empty($resultado['Exito'])) {
            $oferente = $resultado;
        } else {
            $error = $resultado['Mensaje']
                ?? "No se encontró información para el oferente seleccionado.";
        }
    }
}

/*
|--------------------------------------------------------------------------
| SIMULACIÓN DEL BOTÓN CREAR EMPLEADO - CORE 3
|--------------------------------------------------------------------------
| Luego este bloque llamará al servicio Core 3 mediante cURL.
|--------------------------------------------------------------------------
*/

if ($_SERVER['REQUEST_METHOD'] === 'POST' && isset($_POST['crear_empleado'])) {

    if ($oferente === null) {
        $error = 'No es posible crear el empleado porque no se encontró el oferente.';
    } elseif ($codigo_puesto <= 0) {
        $error = 'No se recibió un código de puesto válido.';
    } else {

        // // Convertir la fecha WCF al formato DateTime que espera Core 3
        // $fechaNacimiento = null;

        // if (
        //     !empty($oferente['FechaNacimiento']) &&
        //     preg_match('/\/Date\((\-?\d+)/', $oferente['FechaNacimiento'], $coincidencias)
        // ) {
        //     $fechaNacimiento = date(
        //         'Y-m-d\TH:i:s',
        //         ((int)$coincidencias[1]) / 1000
        //     );
        // }

        // Construir el JSON que recibirá Core 3
        $payload = json_encode([
    'Identificacion'     => $oferente['Identificacion'],
    'TipoIdentificacion' => $oferente['TipoIdentificacion'],
    'NombreCompleto'     => $oferente['NombreCompleto'],
    'FechaNacimiento'    => $oferente['FechaNacimiento'],
    'PuestoId'           => $codigo_puesto,
    'Correos'            => $oferente['Correos'] ?? [],
    'Telefonos'          => $oferente['Telefonos'] ?? []
]);

        $wcf_url = "http://localhost:63602/EmpleadoService.svc/registrar-empleado"; // Cambiar a la URL del servicio Core 3, depende de la configuración de su puerto y la ruta del servicio.

        $ch = curl_init($wcf_url);

        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_POST, true);
        curl_setopt($ch, CURLOPT_POSTFIELDS, $payload);

        curl_setopt($ch, CURLOPT_HTTPHEADER, [
            'Content-Type: application/json',
            'Content-Length: ' . strlen($payload)
        ]);

        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false);

        $response = curl_exec($ch);
        $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        $curlError = curl_error($ch);

        curl_close($ch);
        
        //   esto lo usé para ver unos errores que tenía 
//   echo '<pre>';

// echo "HTTP CODE:\n";
// var_dump($httpCode);

// echo "\nRESPUESTA:\n";
// var_dump($response);

// echo "\nERROR CURL:\n";
// var_dump($curlError);

// echo '</pre>';

// exit;

        if ($response === false || $httpCode !== 200) {

            $error = 'Error al conectar con el servicio de registro de empleado.';

            if ($curlError !== '') {
                $error .= ' Detalle: ' . $curlError;
            }

        } else {

            $resultado = json_decode($response, true);

            if (!is_array($resultado)) {

                $error = 'El servicio devolvió una respuesta inválida.';

            } elseif (!empty($resultado['Exito'])) {
header(
    'Location: oferentes.php?codigo_puesto='
    . urlencode((string)$codigo_puesto)
    . '&msg=empleado_creado'
);

exit;

            } else {

                $error = $resultado['Mensaje']
                    ?? 'No fue posible registrar el empleado.';

            }
        }
    }
}   

?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />

    <meta name="viewport"
          content="width=device-width, initial-scale=1" />

    <title>Detalle del oferente — Administración de Personal</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
          rel="stylesheet" />

    <style>
        body {
            background: #f0f2f5;
            margin: 0;
        }

        .topbar {
            background: white;
            padding: 14px 28px;
            border-bottom: 1px solid #dee2e6;
            display: flex;
            justify-content: space-between;
            align-items: center;
            box-shadow: 0 2px 8px rgba(0,0,0,0.06);
        }

        .topbar-brand {
            font-size: 1.2rem;
            font-weight: 700;
            color: #1aad94;
        }

        .user-info {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .avatar {
            width: 38px;
            height: 38px;
            border-radius: 50%;
            background: #1aad94;
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            font-size: 0.95rem;
        }

        .page-body {
            padding: 40px 28px;
        }

        .content-card {
            max-width: 950px;
            margin: 0 auto;
            background: white;
            border-radius: 14px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.08);
            padding: 32px;
        }

        .page-title {
            color: #1aad94;
            font-weight: 700;
        }

        .section-title {
            color: #1aad94;
            font-size: 1.05rem;
            font-weight: 700;
            border-bottom: 2px solid #e8f8f5;
            padding-bottom: 8px;
            margin-bottom: 18px;
        }

        .detail-label {
            color: #6c757d;
            font-size: 0.88rem;
            font-weight: 600;
            margin-bottom: 4px;
        }

        .detail-value {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 10px 12px;
            min-height: 45px;
        }

        .puesto-info {
            background: #e8f8f5;
            border-left: 4px solid #1aad94;
            border-radius: 8px;
            padding: 14px 18px;
            margin-bottom: 24px;
        }

        .list-item-custom {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 10px 12px;
            margin-bottom: 8px;
        }

        .btn-crear {
            background: #1aad94;
            color: white;
            border: none;
            padding: 10px 22px;
            border-radius: 8px;
            font-weight: 600;
        }

        .btn-crear:hover {
            background: #14836f;
            color: white;
        }

        .btn-cancelar {
            background: #6c757d;
            color: white;
            border: none;
            padding: 10px 22px;
            border-radius: 8px;
            font-weight: 600;
            text-decoration: none;
            display: inline-block;
        }

        .btn-cancelar:hover {
            background: #5c636a;
            color: white;
        }
    </style>
</head>

<body>

<!-- TOPBAR -->
<div class="topbar">

    <span class="topbar-brand">
         Sistema de Recursos Humanos
    </span>

    <div class="user-info">

        <span class="fw-semibold">
            <?php echo htmlspecialchars($nombre_completo); ?>
        </span>

        <div class="avatar">
            <?php echo htmlspecialchars($iniciales); ?>
        </div>

    </div>
</div>

<!-- CONTENT -->
<div class="page-body">

    <div class="content-card">

        <div class="d-flex justify-content-between align-items-center mb-3">

            <div>
                <h3 class="page-title mb-1">
                    Detalle del oferente
                </h3>

                <!-- <p class="text-muted mb-0">
                    Revise la información antes de crear el empleado.
                </p> -->
            </div>


        </div>

      
       
        <?php if ($mensaje !== ''): ?>

            <div class="alert alert-success">
                <?php echo htmlspecialchars($mensaje); ?>
            </div>

        <?php endif; ?>

        <?php if ($error !== ''): ?>

            <div class="alert alert-danger">
                <?php echo htmlspecialchars($error); ?>
            </div>

        <?php endif; ?>

        <?php if ($oferente !== null): ?>

            <div class="section-title">
                Información personal
            </div>

            <div class="row g-3 mb-4">

                <div class="col-md-6">
                    <div class="detail-label">Identificación</div>

                    <div class="detail-value">
                        <?php
                            echo htmlspecialchars(
                                $oferente['Identificacion']
                            );
                        ?>
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="detail-label">Tipo de identificación</div>

                    <div class="detail-value">
                        <?php
                            echo htmlspecialchars(
                                $oferente['TipoIdentificacion']
                            );
                        ?>
                    </div>
                </div>

                <div class="col-md-8">
                    <div class="detail-label">Nombre completo</div>

                    <div class="detail-value">
                        <?php
                            echo htmlspecialchars(
                                $oferente['NombreCompleto']
                            );
                        ?>
                    </div>
                </div>

                <div class="col-md-4">
                    <div class="detail-label">Fecha de nacimiento</div>

                    <div class="detail-value">
                        <?php
                           echo htmlspecialchars(
    formatearFechaWcf($oferente['FechaNacimiento'])
);
                        ?>
                    </div>
                </div>

            </div>

            <div class="section-title">
                Información de contacto
            </div>

            <div class="row g-4 mb-4">

                <div class="col-md-6">

                    <div class="detail-label mb-2">
                        Correos electrónicos
                    </div>

                    <?php if (!empty($oferente['Correos'])): ?>

                        <?php foreach ($oferente['Correos'] as $correo): ?>

                            <div class="list-item-custom">
                                 <?php echo htmlspecialchars($correo); ?>
                            </div>

                        <?php endforeach; ?>

                    <?php else: ?>

                        <div class="text-muted">
                            No hay correos registrados.
                        </div>

                    <?php endif; ?>

                </div>

                <div class="col-md-6">

                    <div class="detail-label mb-2">
                        Teléfonos
                    </div>

                    <?php if (!empty($oferente['Telefonos'])): ?>

                        <?php foreach ($oferente['Telefonos'] as $telefono): ?>

                            <div class="list-item-custom">
                                 <?php echo htmlspecialchars($telefono); ?>
                            </div>

                        <?php endforeach; ?>

                    <?php else: ?>

                        <div class="text-muted">
                            No hay teléfonos registrados.
                        </div>

                    <?php endif; ?>

                </div>


            </div>
            <div class="section-title">
    Preparación académica
</div>

<div class="mb-4">

    <?php if (!empty($oferente['PreparacionAcademica'])): ?>

        <?php foreach ($oferente['PreparacionAcademica'] as $preparacion): ?>

            <div class="list-item-custom">

                <div class="fw-semibold mb-1">
                    <?php
                        echo htmlspecialchars(
                            $preparacion['Titulo'] ?? 'Título no disponible'
                        );
                    ?>
                </div>

                <div class="text-muted small">
                    Institución:
                    <?php
                        echo htmlspecialchars(
                            $preparacion['CodigoInstitucion'] ?? 'No disponible'
                        );
                    ?>
                </div>

                <div class="text-muted small">
                    Periodo:
                    <?php
                        echo htmlspecialchars(
                            formatearFechaWcf($preparacion['FechaInicio']) ?? 'No disponible'
                        );
                    ?>

                    -

                    <?php
                        echo htmlspecialchars(
                            formatearFechaWcf($preparacion['FechaFin']) ?? 'Actualidad'
                        );
                    ?>
                </div>

            </div>

        <?php endforeach; ?>

    <?php else: ?>

        <div class="text-muted">
            No hay preparación académica registrada.
        </div>

    <?php endif; ?>

</div>
<div class="section-title">
    Experiencia laboral
</div>

<div class="mb-4">

    <?php if (!empty($oferente['ExperienciaLaboral'])): ?>

        <?php foreach ($oferente['ExperienciaLaboral'] as $experiencia): ?>

            <div class="list-item-custom">

                <div class="fw-semibold mb-1">
                    <?php
                        echo htmlspecialchars(
                            $experiencia['Puesto'] ?? 'Puesto no disponible'
                        );
                    ?>
                </div>

                <div class="text-muted small">
                    Empresa:
                    <?php
                        echo htmlspecialchars(
                            $experiencia['Empresa'] ?? 'No disponible'
                        );
                    ?>
                </div>

                <div class="text-muted small">
                    Periodo:
                    <?php
                        echo htmlspecialchars(
                            formatearFechaWcf($experiencia['FechaInicio']) ?? 'No disponible'
                        );
                    ?>

                    -

                    <?php
                        echo htmlspecialchars(
                            formatearFechaWcf($experiencia['FechaFin']) ?? 'Actualidad'
                        );
                    ?>
                </div>

            </div>

        <?php endforeach; ?>

    <?php else: ?>

        <div class="text-muted">
            No hay experiencia laboral registrada.
        </div>

    <?php endif; ?>

</div>

<div class="section-title">
    Concursos
</div>

<div class="mb-4">

    <?php if (!empty($oferente['Concursos'])): ?>

        <?php foreach ($oferente['Concursos'] as $concurso): ?>

            <div class="list-item-custom">

                <div class="fw-semibold mb-1">
                    <?php
                        echo htmlspecialchars(
                            $concurso['Nombre'] ?? 'Concurso no disponible'
                        );
                    ?>
                </div>

                <div class="text-muted small">
                    Código:
                    <?php
                        echo htmlspecialchars(
                            $concurso['CodigoConcurso'] ?? 'No disponible'
                        );
                    ?>
                </div>

                <div class="text-muted small">
                    Estado:
                    <?php
                        echo htmlspecialchars(
                            $concurso['Estado'] ?? 'No disponible'
                        );
                    ?>
                </div>

                <div class="text-muted small">
                    Periodo:
                    <?php
                        echo htmlspecialchars(
                            formatearFechaWcf($concurso['FechaInicio']) ?? 'No disponible'
                        );
                    ?>

                    -

                    <?php
                        echo htmlspecialchars(
                            formatearFechaWcf($concurso['FechaFin'])  ?? 'No disponible'
                        );
                    ?>
                </div>

            </div>

        <?php endforeach; ?>

    <?php else: ?>

        <div class="text-muted">
            No hay concursos registrados.
        </div>

    <?php endif; ?>

</div>

            <div class="section-title">
                Curriculum
            </div>

            <div class="detail-value mb-4">
                
               <?php
        echo htmlspecialchars( $oferente['Curriculum'] ?? 'No disponible la información' );?>
            </div>

            <div class="d-flex flex-wrap gap-2">

                <form method="POST" action="">

                    <button
                        type="submit"
                        name="crear_empleado"
                        class="btn-crear"
                        onclick="return confirm(
                            '¿Desea crear un empleado con los datos del oferente seleccionado?'
                        );"
                    >
                        Crear empleado
                    </button>

                </form>

                <a
                    href="oferentes.php?codigo_puesto=<?php
                        echo urlencode((string)$codigo_puesto);
                    ?>"
                    class="btn-cancelar"
                >
                    Cancelar
                </a>

            </div>

        <?php else: ?>

            <a
                href="oferentes.php?codigo_puesto=<?php
                    echo urlencode((string)$codigo_puesto);
                ?>"
                class="btn-cancelar"
            >
                Regresar
            </a>

        <?php endif; ?>

    </div>
</div>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js">
</script>

</body>
</html>