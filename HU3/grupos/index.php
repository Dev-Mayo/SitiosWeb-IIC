<?php
session_start();
require_once __DIR__ . '/../clases/Grupo.php';

$mensaje = $_SESSION['mensaje'] ?? '';
unset($_SESSION['mensaje']);

$grupos = Grupo::listar();
?>
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <title>Grupos – Control de Tareas</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4">

  <div class="d-flex justify-content-between align-items-center mb-3">
    <h2>Grupos</h2>
    <a href="crear.php" class="btn btn-primary">+ Nuevo grupo</a>
  </div>

  <?php if ($mensaje): ?>
    <div class="alert alert-success alert-dismissible fade show">
      <?= htmlspecialchars($mensaje) ?>
      <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
  <?php endif; ?>

  <?php if (empty($grupos)): ?>
    <div class="alert alert-info">No hay grupos registrados.</div>
  <?php else: ?>
    <table class="table table-bordered table-hover bg-white">
      <thead class="table-dark">
        <tr>
          <th>#</th>
          <th>Nombre</th>
          <th>Creado en</th>
          <th class="text-center">Acciones</th>
        </tr>
      </thead>
      <tbody>
      <?php foreach ($grupos as $g): ?>
        <tr>
          <td><?= $g['id'] ?></td>
          <td><?= htmlspecialchars($g['nombre']) ?></td>
          <td><?= $g['creado_en'] ?></td>
          <td class="text-center">
            <a href="editar.php?id=<?= $g['id'] ?>" class="btn btn-sm btn-warning">Editar</a>
            <a href="eliminar.php?id=<?= $g['id'] ?>"
               class="btn btn-sm btn-danger"
               onclick="return confirm('¿Eliminar este grupo? Las tareas quedarán sin grupo.')">
              Eliminar
            </a>
          </td>
        </tr>
      <?php endforeach; ?>
      </tbody>
    </table>
  <?php endif; ?>

</div>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>