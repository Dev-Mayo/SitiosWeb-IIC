<?php
// responsables/index.php  – Listado de responsables
require_once __DIR__ . '/../clases/Responsable.php';

$mensaje = $_SESSION['mensaje'] ?? '';
session_start();
$mensaje = $_SESSION['mensaje'] ?? '';
unset($_SESSION['mensaje']);

$responsables = Responsable::listar();
?>
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Responsables – Control de Tareas</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4">

  <div class="d-flex justify-content-between align-items-center mb-3">
    <h2>Responsables</h2>
    <a href="crear.php" class="btn btn-primary">+ Nuevo responsable</a>
  </div>

  <?php if ($mensaje): ?>
    <div class="alert alert-success alert-dismissible fade show">
      <?= htmlspecialchars($mensaje) ?>
      <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
  <?php endif; ?>

  <?php if (empty($responsables)): ?>
    <div class="alert alert-info">No hay responsables registrados.</div>
  <?php else: ?>
    <table class="table table-bordered table-hover bg-white">
      <thead class="table-dark">
        <tr>
          <th>#</th>
          <th>Nombre</th>
          <th>Apellidos</th>
          <th>Identificación</th>
          <th class="text-center">Acciones</th>
        </tr>
      </thead>
      <tbody>
        <?php foreach ($responsables as $r): ?>
        <tr>
          <td><?= $r['id'] ?></td>
          <td><?= htmlspecialchars($r['nombre']) ?></td>
          <td><?= htmlspecialchars($r['apellidos']) ?></td>
          <td><?= htmlspecialchars($r['identificacion']) ?></td>
          <td class="text-center">
            <a href="editar.php?id=<?= $r['id'] ?>" class="btn btn-sm btn-warning">Editar</a>
            <a href="eliminar.php?id=<?= $r['id'] ?>"
               class="btn btn-sm btn-danger"
               onclick="return confirm('¿Eliminar este responsable? Las tareas asignadas quedarán sin responsable.')">
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
