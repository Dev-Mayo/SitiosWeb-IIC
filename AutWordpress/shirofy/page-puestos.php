<?php
/*
Template Name: Puestos Disponibles
*/

define('OFE_DB_HOST', 'mysql-admin-personal-iic-2026-admin-personal-iic-2026.k.aivencloud.com');
define('OFE_DB_PORT', 16341);
define('OFE_DB_NAME', 'EMP');
define('OFE_DB_USER', 'avnadmin');
define('OFE_DB_PASS', 'AVNS_D9NXIT8nECYcHW1YV31');

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
            function ofe_conectar_bd() {
                $mysqli = mysqli_init();
                $mysqli->ssl_set(null, null, null, null, null);
                $conectado = @$mysqli->real_connect(
                    OFE_DB_HOST,
                    OFE_DB_USER,
                    OFE_DB_PASS,
                    OFE_DB_NAME,
                    OFE_DB_PORT,
                    null,
                    MYSQLI_CLIENT_SSL
                );
                return $conectado ? $mysqli : null;
            }

            $puestos = array();
            $errorPuestos = '';

            $conn = ofe_conectar_bd();

            if ($conn === null) {
                $errorPuestos = 'No se pudieron cargar los puestos disponibles en este momento.';
            } else {
                if ($conn->multi_query("CALL sp_listar_puestos_disponibles()")) {
                    do {
                        if ($resultado = $conn->store_result()) {
                            while ($fila = $resultado->fetch_assoc()) {
                                $puestos[] = array(
                                    'id'      => $fila['puesto_id'],
                                    'nombre'  => $fila['nombre'],
                                    'salario' => $fila['salario'],
                                    'jefe'    => $fila['nombre_jefe'] ?? 'Sin asignar'
                                );
                            }
                            $resultado->free();
                        }
                    } while ($conn->more_results() && $conn->next_result());
                } else {
                    $errorPuestos = 'No se pudieron cargar los puestos disponibles en este momento.';
                }

                $conn->close();
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
                                <a href="<?php echo home_url('/registro-oferente/?id=' . urlencode($p['id'])); ?>"
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