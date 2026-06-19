<?php
require_once __DIR__ . '/../HU1/config/db.php';

$pdo = getConexion();

$stmt = $pdo->query("SELECT * FROM vista_tareas");
$tareas = $stmt->fetchAll();

$columnas = [
    'pendiente' => 'Pendiente',
    'en_progreso' => 'En progreso',
    'bloqueada' => 'Bloqueada',
    'finalizada' => 'Finalizada'
];

function tareasPorEstado($tareas, $estado) {
    return array_filter($tareas, fn($tarea) => $tarea['estado'] === $estado);
}
?>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Tablero de Tareas</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
</head>

<body class="bg-light">
<div class="container-fluid mt-4">

    <h2 class="mb-4 text-center">Tablero de Tareas</h2>

    <?php if (isset($_GET['error'])): ?>
        <div class="alert alert-danger">
            <?= htmlspecialchars($_GET['error']) ?>
        </div>
    <?php endif; ?>

    <div class="row">
        <?php foreach ($columnas as $estado => $titulo): ?>
            <div class="col-md-3">
                <div class="card shadow-sm">
                    <div class="card-header bg-dark text-white text-center">
                        <strong><?= $titulo ?></strong>
                    </div>

                    <div class="card-body">
                        <?php foreach (tareasPorEstado($tareas, $estado) as $tarea): ?>
                            <div class="card mb-3">
                                <div class="card-body">

                                    <h6 class="<?= $estado === 'finalizada' ? 'text-decoration-line-through text-muted' : '' ?>">
                                        <?= htmlspecialchars($tarea['detalle']) ?>
                                    </h6>

                                    <p class="mb-1">
                                        <strong>Prioridad:</strong> <?= htmlspecialchars($tarea['prioridad']) ?>
                                    </p>

                                    <p class="mb-1">
                                        <strong>Responsable:</strong> <?= htmlspecialchars($tarea['responsable']) ?>
                                    </p>

                                    <p class="mb-1">
                                        <strong>Grupo:</strong> <?= htmlspecialchars($tarea['grupo'] ?? 'Sin grupo') ?>
                                    </p>

                                    <p class="mb-2">
                                        <strong>Fecha límite:</strong> <?= htmlspecialchars($tarea['fecha_limite'] ?? 'Sin fecha') ?>
                                    </p>

                                    <div class="d-grid gap-1">

                                        <?php if ($estado === 'pendiente'): ?>
                                            <a href="cambiar_estado.php?id=<?= $tarea['id'] ?>&estado=en_progreso" class="btn btn-sm btn-primary">
                                                Iniciar
                                            </a>
                                        <?php endif; ?>

                                        <?php if ($estado === 'en_progreso'): ?>
                                            <a href="cambiar_estado.php?id=<?= $tarea['id'] ?>&estado=pendiente" class="btn btn-sm btn-secondary">
                                                Volver a pendiente
                                            </a>

                                            <a href="cambiar_estado.php?id=<?= $tarea['id'] ?>&estado=bloqueada" class="btn btn-sm btn-warning">
                                                Bloquear
                                            </a>

                                            <a href="cambiar_estado.php?id=<?= $tarea['id'] ?>&estado=finalizada" class="btn btn-sm btn-success">
                                                Finalizar
                                            </a>
                                        <?php endif; ?>

                                        <?php if ($estado === 'bloqueada'): ?>
                                            <a href="cambiar_estado.php?id=<?= $tarea['id'] ?>&estado=en_progreso" class="btn btn-sm btn-primary">
                                                Reactivar
                                            </a>
                                        <?php endif; ?>

                                        <?php if ($estado === 'finalizada'): ?>
                                            <a href="cambiar_estado.php?id=<?= $tarea['id'] ?>&estado=en_progreso" class="btn btn-sm btn-primary">
                                                Reactivar
                                            </a>
                                        <?php endif; ?>

                                    </div>
                                </div>
                            </div>
                        <?php endforeach; ?>
                    </div>

                </div>
            </div>
        <?php endforeach; ?>
    </div>

</div>
</body>
</html>