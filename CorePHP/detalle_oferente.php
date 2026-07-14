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

/*
|--------------------------------------------------------------------------
| MUCHACHONES AQUI HAY DATOS SIMULADOS, por fa al que le toque core 8 lea los comentarios de abajo
|--------------------------------------------------------------------------
| Este bloque se reemplazará por el servicio Core 8.
| Core 8 deberá recibir la identificación y devolver todos los datos
| registrados para el oferente.
|--------------------------------------------------------------------------
*/

$oferente = null;

$oferentesSimulados = [
    '305550555' => [
        'Identificacion'     => '305550555',
        'TipoIdentificacion' => 'Cedula',
        'NombreCompleto'     => 'María Fernanda Pérez',
        'FechaNacimiento'    => '1995-04-20',
        'Correos'            => [
            'maria.perez@email.com',
            'maria.trabajo@email.com'
        ],
        'Telefonos'          => [
            '8888-8888',
            '2222-2222'
        ],
        'Curriculum'         => 'curriculum_maria_perez.pdf'
    ],
    '208880888' => [
        'Identificacion'     => '208880888',
        'TipoIdentificacion' => 'Cedula',
        'NombreCompleto'     => 'Carlos Andrés Rodríguez',
        'FechaNacimiento'    => '1990-08-15',
        'Correos'            => [
            'carlos.rodriguez@email.com'
        ],
        'Telefonos'          => [
            '8777-7777'
        ],
        'Curriculum'         => 'curriculum_carlos_rodriguez.pdf'
    ],
    '109990999' => [
        'Identificacion'     => '109990999',
        'TipoIdentificacion' => 'Cedula',
        'NombreCompleto'     => 'Daniela Vargas Gómez',
        'FechaNacimiento'    => '1998-11-10',
        'Correos'            => [
            'daniela.vargas@email.com'
        ],
        'Telefonos'          => [
            '8666-6666'
        ],
        'Curriculum'         => 'curriculum_daniela_vargas.pdf'
    ]
];

if ($identificacion !== '' && isset($oferentesSimulados[$identificacion])) {
    $oferente = $oferentesSimulados[$identificacion];
} elseif ($identificacion !== '' && $error === '') {
    $error = 'No se encontró información para el oferente seleccionado.';
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

        /*
        $payload = json_encode([
            'Identificacion'     => $oferente['Identificacion'],
            'TipoIdentificacion' => $oferente['TipoIdentificacion'],
            'NombreCompleto'     => $oferente['NombreCompleto'],
            'FechaNacimiento'    => $oferente['FechaNacimiento'],
            'CodigoPuesto'       => $codigo_puesto,
            'Correos'            => $oferente['Correos'],
            'Telefonos'          => $oferente['Telefonos']
        ]);

        Aquí se realizará el cURL hacia Core 3.
        */

        $mensaje = 'Empleado creado con éxito.';
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

        <!-- <div class="puesto-info">
            <strong>Código del puesto seleccionado:</strong>

            <?php if ($codigo_puesto > 0): ?>

                <?php echo htmlspecialchars((string)$codigo_puesto); ?>

            <?php else: ?>

                No disponible

            <?php endif; ?>
        </div> -->

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
                                $oferente['FechaNacimiento']
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
                                📧 <?php echo htmlspecialchars($correo); ?>
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
                                📞 <?php echo htmlspecialchars($telefono); ?>
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
                Curriculum
            </div>

            <div class="detail-value mb-4">
                📄
                <?php echo htmlspecialchars($oferente['Curriculum']); ?>
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