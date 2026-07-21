<?php
session_start();

// Proteger la página
if (!isset($_SESSION['id_usuario'])) {
    header("Location: login.php?msg=login");
    exit;
}

$nombre_completo = $_SESSION['nombre_completo'] ?? 'Usuario';
$usuario         = $_SESSION['usuario'] ?? '';
$mensaje = '';

if (isset($_GET['msg']) && $_GET['msg'] === 'empleado_creado') {
    $mensaje = 'Empleado creado con éxito.';
}

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


// Consumo del servicio Core 2 (OferenteService)
$payload = json_encode(["CodigoPuesto" => $codigo_puesto,"Usuario" => $usuario]);

$wcf_url = "http://localhost:63602/OferenteService.svc/obtenerPorPuesto";

$ch = curl_init($wcf_url);
// echo '<pre>';

// echo "USUARIO EN SESIÓN:\n";
// var_dump($_SESSION['usuario'] ?? null);

// echo "\nVARIABLE USUARIO:\n";
// var_dump($usuario);

// echo "\nPAYLOAD:\n";
// var_dump($payload);

// echo '</pre>';
// exit;
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
// echo '<pre>';

// echo "HTTP CODE:\n";
// var_dump($httpCode);

// echo "\nRESPUESTA CRUDA:\n";
// var_dump($response);

// echo "\nJSON DECODIFICADO:\n";
// var_dump($resultado);

// echo '</pre>';

// exit;

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
<link rel="stylesheet" href="css/estilos.css">
   
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

        <?php if ($mensaje !== ''): ?>
            <div class="alert alert-success">
                <?php echo htmlspecialchars($mensaje); ?>
            </div>

        <?php endif; ?>
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