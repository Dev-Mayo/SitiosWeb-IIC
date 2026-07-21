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
    <style>
        body { background: #f0f2f5; margin: 0; }

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

        .page-body { padding: 48px 28px; }

        .welcome-card {
            max-width: 600px;
            margin: 0 auto;
            background: white;
            border-radius: 14px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.08);
            padding: 48px;
            text-align: center;
        }

        .welcome-icon { font-size: 72px; margin-bottom: 16px; }

        .welcome-msg {
            background: #e8f8f5;
            border-left: 4px solid #1aad94;
            border-radius: 8px;
            padding: 16px 24px;
            font-size: 1.15rem;
            font-weight: 600;
            color: #1aad94;
            margin: 24px 0;
        }

      .btn-success {
    background-color: #16a085;
    border-color: #16a085;
}

.btn-success:hover {
    background-color: #138d75;
    border-color: #138d75;
}

        .btn-logout:hover { background: #bb2d3b; color: white; }
    </style>
</head>
<body>

<!-- TOPBAR -->
<div class="topbar">
    <span class="topbar-brand">🏢 Sistema de Recursos Humanos</span>
    <div class="user-info">
        <span class="fw-semibold"><?php echo htmlspecialchars($nombre_completo); ?></span>
        <div class="avatar"><?php echo htmlspecialchars($iniciales); ?></div>
    </div>
</div>

<!-- CONTENT -->
<div class="page-body">
    <div class="welcome-card">

        <div class="welcome-icon">🏢</div>

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