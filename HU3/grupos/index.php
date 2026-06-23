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
  <title>Grupos</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4">

  <div class="d-flex justify-content-between mb-3">
    <h2>Grupos</h2>
    <a href="crear.php" class="btn btn-primary">+ Nuevo grupo</a>
  </div>

  <?php if ($mensaje): ?>
    <div class="alert alert-success"><?= htmlspecialchars($mensaje) ?></div>
  <?php endif; ?>

  <?php if (empty($grupos)): ?>
    <div class="alert alert-info">No hay grupos registrados.</div>
  <?php else: ?>
    <table class="table table-bordered bg-white">
      <thead class="table-dark">
        <tr>
          <th>ID</th>
          <th>Nombre</th>
          <th>Creado en</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
      <?php foreach ($grupos as $g): ?>
        <tr>
          <td><?= $g['id'] ?></td>
          <td>
            <a href="ver.php?id=<?= $g['id'] ?>">
              <?= htmlspecialchars($g['nombre']) ?>
            </a>
          </td>
          <td><?= htmlspecialchars($g['creado_en']) ?></td>
          <td>
            <a href="ver.php?id=<?= $g['id'] ?>" class="btn btn-sm btn-info">Ver tareas</a>
            <a href="editar.php?id=<?= $g['id'] ?>" class="btn btn-sm btn-warning">Editar</a>
            <a href="eliminar.php?id=<?= $g['id'] ?>"
               onclick="return confirm('¿Eliminar grupo? Las tareas quedarán sin grupo.')"
               class="btn btn-sm btn-danger">Eliminar</a>
          </td>
        </tr>
      <?php endforeach; ?>
      </tbody>
    </table>
  <?php endif; ?>

</div>
</body>
</html>