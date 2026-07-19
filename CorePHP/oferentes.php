<?php
session_start();

// Proteger la página
if (!isset($_SESSION['id_usuario'])) {
    header("Location: login.php?msg=login");
    exit;
}

$nombre_completo = $_SESSION['nombre_completo'] ?? 'Usuario';
$usuario         = $_SESSION['usuario'] ?? '';

// Recibir el código del puesto desde Core 6
$codigo_puesto = filter_input(INPUT_GET, 'codigo_puesto', FILTER_VALIDATE_INT);

if (!$codigo_puesto) {
    $codigo_puesto = 0;
}

// Iniciales para el avatar
$palabras  = explode(' ', $nombre_completo);
$iniciales = '';

foreach (array_slice($palabras, 0, 2) as $p) {
    $iniciales .= strtoupper(mb_substr($p, 0, 1));
}

/*
|--------------------------------------------------------------------------
| MUCHACHONES AQUI HAY DATOS SIMULADOS, por fa al que le toque core 2 lea los comentarios de abajo
|--------------------------------------------------------------------------
| Este bloque se reemplazará luego por el consumo del servicio Core 2.
|
| Core 2 deberá recibir el código del puesto y devolver:
| - Identificación del oferente
| - Nombre completo del oferente
|--------------------------------------------------------------------------
*/
/*
==========================================================
PENDIENTE INTEGRACIÓN CORE 2

Este bloque de datos simulados será reemplazado por el
consumo del servicio WCF Core 2.

Entrada:
    CodigoPuesto

Salida esperada:
    Identificacion
    NombreCompleto
==========================================================


$oferentes = [
    [
        'Identificacion' => '305550555',
        'NombreCompleto' => 'María Fernanda Pérez'
    ],
    [
        'Identificacion' => '208880888',
        'NombreCompleto' => 'Carlos Andrés Rodríguez'
    ],
    [
        'Identificacion' => '109990999',
        'NombreCompleto' => 'Daniela Vargas Gómez'
    ]
];*/

// Consumo del servicio Core 2 (OferenteService)
$payload = json_encode(["CodigoPuesto" => $codigo_puesto]);

$wcf_url = "http://localhost:63602/OferenteService.svc/obtenerPorPuesto";

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
curl_close($ch);

$oferentes = [];
$errorCore2 = '';

if ($response === false || $httpCode !== 200) {
    $errorCore2 = 'Error al conectar con el servicio de oferentes.';
} else {
    $resultado = json_decode($response, true);
    if (!empty($resultado['Success'])) {
        foreach ($resultado['Oferentes'] as $item) {
            $oferentes[] = [
                'Identificacion' => $item['Identificacion'],
                'NombreCompleto' => $item['NombreCompleto']
            ];
        }
    } else {
        $errorCore2 = $resultado['Mensaje'] ?? 'No se pudo obtener el listado de oferentes.';
    }
}

?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />

    <meta name="viewport"
          content="width=device-width, initial-scale=1" />

    <title>Oferentes — Administración de Personal</title>

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

        .puesto-info {
            background: #e8f8f5;
            border-left: 4px solid #1aad94;
            border-radius: 8px;
            padding: 14px 18px;
            margin-bottom: 24px;
        }

        .table thead th {
            background: #1aad94;
            color: white;
            border-color: #1aad94;
        }

        .oferente-link {
            color: #1aad94;
            font-weight: 600;
            text-decoration: none;
        }

        .oferente-link:hover {
            color: #14836f;
            text-decoration: underline;
        }

        .btn-regresar {
            background: #6c757d;
            color: white;
            border: none;
            padding: 10px 22px;
            border-radius: 8px;
            font-weight: 600;
            text-decoration: none;
            display: inline-block;
        }

        .btn-regresar:hover {
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
                    Oferentes disponibles
                </h3>

                <p class="text-muted mb-0">
                    Seleccione el oferente que será convertido en empleado.
                </p>
            </div>
            

            <!-- <div style="font-size: 44px;">
                👥
            </div> -->

        </div>

        <?php if (!empty($errorCore2)): ?>
            <div class="alert alert-danger">
                <?php echo htmlspecialchars($errorCore2); ?>
            </div>
        <?php endif; ?>

        <?php if (empty($oferentes)): ?>

            <div class="alert alert-warning">
                No se encontraron oferentes que cumplan los requisitos
                para el puesto seleccionado.
            </div>

        <?php else: ?>

            <div class="table-responsive">

                <table class="table table-bordered table-hover align-middle">

                    <thead>
                        <tr>
                            <th>Nombre completo</th>
                            <th>Identificación</th>
                        </tr>
                    </thead>

                    <tbody>

                        <?php foreach ($oferentes as $oferente): ?>

                            <tr>

                                <td>

                                    <a
                                        href="detalle_oferente.php?identificacion=<?php
                                            echo urlencode($oferente['Identificacion']);
                                        ?>&codigo_puesto=<?php
                                            echo urlencode((string)$codigo_puesto);
                                        ?>"
                                        class="oferente-link"
                                    >

                                        <?php
                                            echo htmlspecialchars(
                                                $oferente['NombreCompleto']
                                            );
                                        ?>

                                    </a>

                                </td>

                                <td>
                                    <?php
                                        echo htmlspecialchars(
                                            $oferente['Identificacion']
                                        );
                                    ?>
                                </td>

                            </tr>

                        <?php endforeach; ?>

                    </tbody>

                </table>

            </div>

        <?php endif; ?>

        <div class="mt-4">

            <a href="puestos.php"
               class="btn-regresar">

                ← Regresar

            </a>

        </div>

    </div>
</div>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js">
</script>

</body>
</html>