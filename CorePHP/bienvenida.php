<?php
session_start();

// Proteger la página — si no hay sesión redirigir al login
if (!isset($_SESSION['id_usuario'])) {
    header("Location: login.php?msg=login");
    exit;
}

$nombre_completo = $_SESSION['nombre_completo'] ?? 'Usuario';
$usuario         = $_SESSION['usuario'] ?? '';

// Iniciales para el avatar
$palabras  = explode(' ', $nombre_completo);
$iniciales = '';
foreach (array_slice($palabras, 0, 2) as $p) {
    $iniciales .= strtoupper(mb_substr($p, 0, 1));
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Bienvenida — Administración de Personal</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
          rel="stylesheet" />
        <link rel="stylesheet" href="css/estilos.css">
    
</head>
<body class="welcome-page">

<!-- TOPBAR -->
<div class="topbar">
    <span class="topbar-brand">
        🏢 Sistema de Recursos Humanos
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
    <div class="welcome-card">

        <div class="welcome-icon">🏢</div>
        <!-- <img src="imagen/EDIFICIO.jpg" alt="Recursos Humanos" class="logo-sistema"> -->

        <h3 class="fw-bold">Bienvenido al Sistema</h3>

        <div class="welcome-msg">
            Bienvenido, <?php echo htmlspecialchars($nombre_completo); ?>
        </div>

        <p class="text-muted">
            Ha iniciado sesión correctamente en el sistema de 
            Recursos Humanos.
        </p>

        <div class="d-flex justify-content-center gap-3 mt-4">
    <a href="puestos.php" class="btn btn-success action-btn">
        Ver puestos
    </a>

    <a href="logout.php" class="btn btn-danger action-btn">
        Cerrar sesión
    </a>
</div>

    </div>
</div>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>