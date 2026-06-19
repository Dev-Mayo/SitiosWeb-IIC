<?php
session_start();
require_once __DIR__ . '/../clases/Tarea.php';

$mensaje = $_SESSION['mensaje'] ?? '';
unset($_SESSION['mensaje']);

$tareas = Tarea::listar();
$estados = [
    'pendiente' => 'Pendiente',
    'en_progreso' => 'En progreso',
    'bloqueada' => 'Bloqueada',
    'finalizada' => 'Finalizada'
];
?>
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Tareas – Control de Tareas</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4">

  <div class="d-flex justify-content-between align-items-center mb-3">
    <h2>Tareas</h2>
    <a href="crear.php" class="btn btn-primary">+ Nueva tarea</a>
  </div>

  <?php if ($mensaje): ?>
    <div class="alert alert-success alert-dismissible fade show">
      <?= htmlspecialchars($mensaje) ?>
      <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
  <?php endif; ?>

  <?php if (empty($tareas)): ?>
    <div class="alert alert-info">No hay tareas registradas.</div>
  <?php else: ?>
    <div class="table-responsive">
      <table class="table table-bordered table-hover bg-white align-middle">
        <thead class="table-dark">
          <tr>
            <th>#</th>
            <th>Detalle</th>
            <th>Prioridad</th>
            <th>Estado</th>
            <th>Fecha límite</th>
            <th>Responsable</th>
            <th>Fecha finalización</th>
            <th class="text-center">Acciones</th>
          </tr>
        </thead>
        <tbody>
          <?php foreach ($tareas as $t): ?>
          <tr>
            <td><?= $t['id'] ?></td>
            <td class="<?= $t['estado'] === 'finalizada' ? 'text-decoration-line-through text-muted' : '' ?>">
              <?= htmlspecialchars($t['detalle']) ?>
            </td>
            <td><?= htmlspecialchars($t['prioridad']) ?></td>
            <td><?= htmlspecialchars($estados[$t['estado']] ?? $t['estado']) ?></td>
            <td><?= htmlspecialchars($t['fecha_limite'] ?? '-') ?></td>
            <td><?= htmlspecialchars($t['responsable'] ?? 'Sin responsable asignado') ?></td>
            <td><?= htmlspecialchars($t['fecha_finalizacion'] ?? '-') ?></td>
            <td class="text-center">
              <div class="btn-group btn-group-sm" role="group">
                <?php if ($t['estado'] === 'pendiente'): ?>
                  <a href="cambiar_estado.php?id=<?= $t['id'] ?>&estado=en_progreso" class="btn btn-outline-primary">En progreso</a>
                <?php elseif ($t['estado'] === 'en_progreso'): ?>
                  <a href="cambiar_estado.php?id=<?= $t['id'] ?>&estado=pendiente" class="btn btn-outline-secondary">Pendiente</a>
                  <a href="cambiar_estado.php?id=<?= $t['id'] ?>&estado=bloqueada" class="btn btn-outline-warning">Bloquear</a>
                  <a href="cambiar_estado.php?id=<?= $t['id'] ?>&estado=finalizada" class="btn btn-outline-success">Finalizar</a>
                <?php elseif ($t['estado'] === 'bloqueada'): ?>
                  <a href="cambiar_estado.php?id=<?= $t['id'] ?>&estado=en_progreso" class="btn btn-outline-primary">Reactivar</a>
                <?php elseif ($t['estado'] === 'finalizada'): ?>
                  <a href="cambiar_estado.php?id=<?= $t['id'] ?>&estado=en_progreso" class="btn btn-outline-primary">Reactivar</a>
                <?php endif; ?>
              </div>
              <div class="btn-group btn-group-sm mt-1" role="group">
                <a href="editar.php?id=<?= $t['id'] ?>" class="btn btn-warning">Editar</a>
                <a href="eliminar.php?id=<?= $t['id'] ?>"
                   class="btn btn-danger"
                   onclick="return confirm('¿Eliminar esta tarea?')">
                  Eliminar
                </a>
              </div>
            </td>
          </tr>
          <?php endforeach; ?>
        </tbody>
      </table>
    </div>
  <?php endif; ?>

</div>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
