<?php
/*
Template Name: Puestos Disponibles
*/
get_header(); ?>

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
            <?php
            $wcf_url = "http://localhost:63602/PuestoService.svc/listarDisponibles";

            $ch = curl_init($wcf_url);
            curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
            curl_setopt($ch, CURLOPT_HTTPGET, true);
            curl_setopt($ch, CURLOPT_TIMEOUT, 5);
            curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
            curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false);

            $response = curl_exec($ch);
            $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
            curl_close($ch);

            $puestos = array();
            $errorPuestos = '';

            if ($response === false || $httpCode !== 200) {
                $errorPuestos = 'No se pudieron cargar los puestos disponibles en este momento.';
            } else {
                $resultado = json_decode($response, true);
                if (!empty($resultado['Success'])) {
                    foreach ($resultado['Puestos'] as $p) {
                        $puestos[] = array(
                            'id'      => $p['PuestoId'],
                            'nombre'  => $p['Nombre'],
                            'salario' => $p['Salario'],
                            'jefe'    => $p['Jefe']
                        );
                    }
                } else {
                    $errorPuestos = 'No se pudieron cargar los puestos disponibles en este momento.';
                }
            }

            if (!empty($errorPuestos)): ?>
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
                                <a href="<?php echo home_url('/puestos/' . urlencode($p['id'])); ?>"
                                   class="puesto-link">
                                    <?php echo esc_html($p['nombre']); ?>
                                </a>
                            </div>
                            <p class="text-muted" style="font-size:0.9rem; margin-top:8px">
                                <strong>Salario:</strong> <?php echo esc_html($p['salario']); ?>
                            </p>
                            <p class="text-muted" style="font-size:0.85rem; margin-top:4px">
                                <strong>Jefe:</strong> <?php echo esc_html($p['jefe']); ?>
                            </p>
                        </div>
                    </div>
                </div>
            <?php endforeach; endif; ?>
        </div>
    </div>
</section>

<?php get_footer(); ?>
