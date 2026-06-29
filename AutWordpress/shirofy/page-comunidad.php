<?php
/*
Template Name: Comunidad
*/
get_header(); ?>

<section class="hero">
    <div class="container">
        <h1>Compromiso con la Comunidad</h1>
        <p>Trabajamos juntos para construir una comunidad más saludable.</p>
    </div>
</section>

<!-- COMPROMISO -->
<section class="section">
    <div class="container">
        <div class="row align-items-center g-5">
            <div class="col-lg-6">
                <h2 class="section-title">Nuestro compromiso</h2>
                <div class="divider"></div>
                <p class="mb-3">
                    En Shirofy, nos comprometemos activamente con nuestra comunidad y buscamos 
                    marcar una diferencia más allá de nuestras instalaciones.
                </p>
                <p class="mb-3">
                    Participamos en programas de educación y concienciación sobre la salud, 
                    colaboramos con organizaciones locales y apoyamos iniciativas que promueven 
                    estilos de vida saludables.
                </p>
                <p>
                    Creemos en el poder de la colaboración y en trabajar juntos para construir 
                    una comunidad más saludable y próspera para todos.
                </p>
            </div>
            <div class="col-lg-6">
                <div class="row g-3">
                    <?php
                    $compromisos = array(
                        array('icon' => '📚', 'texto' => 'Programas de educación en salud'),
                        array('icon' => '🤝', 'texto' => 'Colaboración con organizaciones locales'),
                        array('icon' => '🌱', 'texto' => 'Iniciativas de vida saludable'),
                        array('icon' => '👨‍👩‍👧', 'texto' => 'Atención a grupos vulnerables'),
                    );
                    foreach ($compromisos as $c): ?>
                        <div class="col-6">
                            <div style="background: var(--primary-light); border-radius:12px; padding:20px; text-align:center">
                                <div style="font-size:2rem; margin-bottom:8px"><?php echo $c['icon']; ?></div>
                                <p style="font-size:0.85rem; margin:0; font-weight:600; color:var(--text-dark)">
                                    <?php echo $c['texto']; ?>
                                </p>
                            </div>
                        </div>
                    <?php endforeach; ?>
                </div>
            </div>
        </div>
    </div>
</section>

<!-- INVESTIGACIÓN -->
<section class="section section-alt">
    <div class="container">
        <div class="row align-items-center g-5">
            <div class="col-lg-6 order-lg-2">
                <h2 class="section-title">Investigación y desarrollo</h2>
                <div class="divider"></div>
                <p class="mb-3">
                    En nuestro afán por avanzar en la medicina y mejorar continuamente la 
                    atención médica, invertimos en investigación y desarrollo.
                </p>
                <p class="mb-3">
                    Colaboramos con instituciones académicas y participamos en estudios clínicos 
                    para mantenernos a la vanguardia de los avances médicos.
                </p>
                <p>
                    Buscamos constantemente nuevas formas de mejorar la eficacia de nuestros 
                    tratamientos y servicios para el beneficio de nuestros pacientes.
                </p>
            </div>
            <div class="col-lg-6 order-lg-1">
                <div style="background: var(--primary); border-radius:20px; padding:48px; text-align:center; color:white">
                    <div style="font-size:4rem; margin-bottom:16px">🔬</div>
                    <h4 style="font-weight:700; margin-bottom:8px">Innovación continua</h4>
                    <p style="opacity:0.85; margin:0">
                        Colaborando con instituciones académicas para estar siempre 
                        a la vanguardia de la medicina.
                    </p>
                </div>
            </div>
        </div>
    </div>
</section>

<?php get_footer(); ?>