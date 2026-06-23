<?php
session_start();
require_once __DIR__ . '/../clases/Grupo.php';

$id = (int)($_GET['id'] ?? 0);
$grupo = Grupo::obtener($id);

if (!$grupo) {
    $_SESSION['mensaje'] = 'Grupo no encontrado.';
    header('Location: index.php');
    exit;
}

$tareas = Grupo::tareasDelGrupo($id);
?>
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <title>Tareas del grupo</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4">

  <div class="d-flex justify-content-between mb-3">
    <h2>Grupo: <?= htmlspecialchars($grupo['nombre']) ?></h2>
    <a href="index.php" class="btn btn-secondary">Volver</a>
  </div>

  <?php if (empty($tareas)): ?>
    <div class="alert alert-info">Este grupo no tiene tareas.</div>
  <?php else: ?>
    <table class="table table-bordered bg-white">
      <thead class="table-dark">
        <tr>
          <th>ID</th>
          <th>Detalle</th>
          <th>Prioridad</th>
          <th>Estado</th>
          <th>Responsable</th>
          <th>Fecha límite</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
      <?php foreach ($tareas as $t): ?>
        <tr>
          <td><?= $t['id'] ?></td>
          <td><?= htmlspecialchars($t['detalle']) ?></td>
          <td><?= htmlspecialchars($t['prioridad']) ?></td>
          <td><?= htmlspecialchars($t['estado']) ?></td>
          <td><?= htmlspecialchars($t['responsable']) ?></td>
          <td><?= $t['fecha_limite'] ?: 'Sin fecha límite' ?></td>
          <td>
            <a href="../../HU2/tareas/editar.php?id=<?= $t['id'] ?>" class="btn btn-sm btn-warning">
              Editar tarea
            </a>

            <a href="quitar_tarea.php?id=<?= $t['id'] ?>&grupo=<?= $grupo['id'] ?>"
               onclick="return confirm('¿Quitar esta tarea del grupo?')"
               class="btn btn-sm btn-danger">
              Quitar
            </a>
          </td>
        </tr>
      <?php endforeach; ?>
      </tbody>
    </table>
  <?php endif; ?>

</div>
</body>
</html>