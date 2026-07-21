<?php
/*
Template Name: Puestos Disponibles
*/

require_once get_template_directory() . '/ET/PuestoET.php';
require_once get_template_directory() . '/Repositories/PuestoRepository.php';
require_once get_template_directory() . '/Services/PuestoService.php';

$puestos = array();
$errorPuestos = '';

try {
    $repository = new PuestoRepository();
    $service = new PuestoService($repository);

    $resultado = $service->obtenerPuestosDisponibles();

    if ($resultado['exito']) {
        $puestos = $resultado['puestos'];
    } else {
        $errorPuestos = $resultado['mensaje'];
    }

    $repository->cerrar();
} catch (Throwable $error) {
    $errorPuestos = 'No se pudieron cargar los puestos disponibles en este momento.';
}

get_header();
?>

<!-- HERO -->
<section class="hero">
    <div class="container">
        <h1>Puestos Disponibles</h1>
        <p>Explora nuestras oportunidades de empleo y forma parte de nuestro equipo.</p>
    </div>
</section>

<!-- PUESTOS -->
<section class="section">
    <div class="container">
        <div class="row g-4">

            <?php if (!empty($errorPuestos)): ?>
                <div class="col-12">
                    <p class="text-muted"><?php echo esc_html($errorPuestos); ?></p>
                </div>
            <?php elseif (empty($puestos)): ?>
                <div class="col-12">
                    <p class="text-muted">No hay puestos disponibles en este momento.</p>
                </div>
            <?php else: foreach ($puestos as $p): ?>
                <div class="col-md-6 col-lg-4">
                    <div class="card-shirofy puesto-card">
                        <div class="card-body">
                            <div class="card-title">
                                <a href="<?php echo home_url('/registro-oferente/?id=' . urlencode($p->id)); ?>"
                                   class="puesto-link">
                                    <?php echo esc_html($p->nombre); ?>
                                </a>
                            </div>
                            <p class="text-muted" style="font-size:0.9rem; margin-top:8px">
                                <strong>Salario:</strong> <?php echo esc_html($p->salario); ?>
                            </p>
                            <p class="text-muted" style="font-size:0.85rem; margin-top:4px">
                                <strong>Jefe:</strong> <?php echo esc_html($p->jefe); ?>
                            </p>
                        </div>
                    </div>
                </div>
            <?php endforeach; endif; ?>

        </div>
    </div>
</section>

<?php get_footer(); ?>