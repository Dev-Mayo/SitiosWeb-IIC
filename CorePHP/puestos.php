<?php
session_start();


if (!isset($_SESSION['id_usuario'])) {
    header("Location: login.php?msg=login");
    exit;
}

$nombre_completo = $_SESSION['nombre_completo'] ?? 'Usuario';
$usuario         = $_SESSION['usuario'] ?? '';


$palabras  = explode(' ', trim($nombre_completo));
$iniciales = '';

foreach (array_slice($palabras, 0, 2) as $palabra) {
    if ($palabra !== '') {
        $iniciales .= strtoupper(
            mb_substr($palabra, 0, 1)
        );
    }
}

$wcf_url = "http://localhost:63602/PuestoService.svc/listarDisponibles";

$puestos    = [];
$errorCore1 = '';

$ch = curl_init($wcf_url);

curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_HTTPGET, true);
curl_setopt($ch, CURLOPT_HTTPHEADER, [
    'Accept: application/json'
]);

curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, 10);
curl_setopt($ch, CURLOPT_TIMEOUT, 20);

curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false);

$response = curl_exec($ch);

$errorCurl = curl_error($ch);

$httpCode = curl_getinfo(
    $ch,
    CURLINFO_HTTP_CODE
);

curl_close($ch);



if ($response === false) {

    $errorCore1 =
        'Error al conectar con el servicio de puestos: '
        . $errorCurl;

} elseif ($httpCode !== 200) {

    $errorCore1 =
        'El servicio de puestos respondió con el código HTTP '
        . $httpCode . '.';

} else {

    $resultado = json_decode(
        $response,
        true
    );

    if (
        json_last_error() !== JSON_ERROR_NONE
    ) {

        $errorCore1 =
            'El servicio de puestos devolvió una respuesta inválida.';

    } elseif (
        isset($resultado['Success'])
        && $resultado['Success'] === true
    ) {

        if (
            isset($resultado['Puestos'])
            && is_array($resultado['Puestos'])
        ) {

            foreach ($resultado['Puestos'] as $item) {

                $puestoId = isset($item['PuestoId'])
                    ? (int) $item['PuestoId']
                    : 0;

                $puestoNombre = isset($item['Nombre'])
                    ? trim((string) $item['Nombre'])
                    : '';

                /*
                | Core6 solo debe mostrar puestos válidos.
                */
                if (
                    $puestoId > 0
                    && $puestoNombre !== ''
                ) {
                    $puestos[] = [
                        'PuestoId' => $puestoId,
                        'Nombre'   => $puestoNombre
                    ];
                }
            }
        }

    } else {

        $errorCore1 =
            $resultado['Mensaje']
            ?? 'No se pudo obtener el listado de puestos.';
    }
}
?>

<!DOCTYPE html>
<html lang="es">

<head>

    <meta charset="utf-8">

    <meta name="viewport"
          content="width=device-width, initial-scale=1">

    <title>
        Puestos activos — Administración de Personal
    </title>

    <link
        href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet">
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
            <?php
            echo htmlspecialchars(
                $nombre_completo,
                ENT_QUOTES,
                'UTF-8'
            );
            ?>
        </span>

        <div class="avatar">
            <?php
            echo htmlspecialchars(
                $iniciales,
                ENT_QUOTES,
                'UTF-8'
            );
            ?>
        </div>

        <a href="logout.php"
           class="btn-logout">
            Cerrar sesión
        </a>

    </div>

</div>


<div class="page-body">

    <div class="content-card">

        <div class="d-flex
                    justify-content-between
                    align-items-center
                    mb-4">

            <div>

                <h3 class="page-title mb-1">
                    Puestos activos
                </h3>

                <p class="text-muted mb-0">
                    Seleccione el puesto para el cual desea crear
                    un nuevo empleado.
                </p>

            </div>

        </div>

        <?php if (!empty($errorCore1)): ?>

            <div class="alert alert-danger">

                <?php
                echo htmlspecialchars(
                    $errorCore1,
                    ENT_QUOTES,
                    'UTF-8'
                );
                ?>

            </div>

        <?php endif; ?>

        <?php if (empty($puestos)): ?>

            <?php if (empty($errorCore1)): ?>

                <div class="alert alert-warning">
                    No se encontraron puestos activos disponibles.
                </div>

            <?php endif; ?>

        <?php else: ?>

            <div class="table-responsive">

                <table class="table
                              table-bordered
                              table-hover
                              align-middle">

                    <thead>

                        <tr>
                            <th>Nombre del puesto</th>
                        </tr>

                    </thead>

                    <tbody>

                        <?php foreach ($puestos as $puesto): ?>

                            <tr>

                                <td>

                                    <a
                                        href="oferentes.php?codigo_puesto=<?php
                                            echo urlencode(
                                                (string) $puesto['PuestoId']
                                            );
                                        ?>"
                                        class="puesto-link"
                                    >

                                        <?php
                                        echo htmlspecialchars(
                                            $puesto['Nombre'],
                                            ENT_QUOTES,
                                            'UTF-8'
                                        );
                                        ?>

                                    </a>

                                </td>

                            </tr>

                        <?php endforeach; ?>

                    </tbody>

                </table>

            </div>

        <?php endif; ?>

        <div class="mt-4">

            <a href="bienvenida.php"
               class="btn-regresar">

                ← Regresar

            </a>

        </div>

    </div>

</div>

<script
    src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js">
</script>

</body>

</html>