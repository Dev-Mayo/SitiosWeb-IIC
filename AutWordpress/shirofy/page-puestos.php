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
            $puestos = array(
                array(
                    'nombre' => 'Desarrollador',
                    'disponible' => true,
                    'salario' => '1200',
                    'jefe' => 'David'
                ),
                array(
                    'nombre' => 'Analista de Sistemas',
                    'disponible' => true,
                    'salario' => '1500',
                    'jefe' => 'Rafa'
                ),
                array(
                    'nombre' => 'Administrador de Base de Datos',
                    'disponible' => false,
                    'salario' => '1800',
                    'jefe' => 'Diego'
                ),
                array(
                    'nombre' => 'Técnico de Redes',
                    'disponible' => true,
                    'salario' => '1000',
                    'jefe' => 'Steven'
                ),
            );
            foreach ($puestos as $p): ?>
                <div class="col-md-6 col-lg-4">
                    <div class="card-shirofy puesto-card <?php echo !$p['disponible'] ? 'puesto-card-disponible' : ''; ?>">
                        <div class="card-body">
                            <div class="card-title">
                                <a href="<?php echo home_url('/puestos/' . urlencode($p['nombre'])); ?>" 
                                   class="puesto-link"
                                   <?php echo !$p['disponible'] ? 'disabled style="pointer-events:none; opacity:0.6;"' : ''; ?>>
                                    <?php echo $p['nombre']; ?>
                                </a>
                                <?php if (!$p['disponible']): ?>
                                    <span class="badge bg-danger ms-2">No disponible</span>
                                <?php endif; ?>
                            </div>
                            <p class="text-muted" style="font-size:0.9rem; margin-top:8px">
                                <strong>Salario:</strong> <?php echo $p['salario']; ?>
                            </p>
                            <p class="text-muted" style="font-size:0.85rem; margin-top:4px">
                                <strong>Jefe:</strong> <?php echo $p['jefe']; ?>
                            </p>
                        </div>
                    </div>
                </div>
            <?php endforeach; ?>
        </div>
    </div>
</section>

<?php get_footer(); ?>
