<?php
/*
Template Name: Carreras
*/
get_header(); ?>

<section class="hero">
    <div class="container">
        <h1>¡Únete a Nosotros!</h1>
        <p>Forma parte de un equipo apasionado comprometido con la excelencia médica.</p>
        <a href="<?php echo home_url('/puestos'); ?>" class="btn-hero">
            Ver Puestos Disponibles
        </a>
    </div>
</section>

<!-- ÚNETE -->
<section class="section">
    <div class="container">
        <div class="row align-items-center g-5">
            <div class="col-lg-6">
                <h2 class="section-title">¿Por qué unirte?</h2>
                <div class="divider"></div>
                <p class="mb-3">
                    En Shirofy, valoramos y reconocemos el talento y el compromiso de nuestros 
                    profesionales. Si compartes nuestra pasión por la atención médica de calidad 
                    y estás interesado en formar parte de nuestro equipo, te invitamos a explorar 
                    las oportunidades de empleo disponibles.
                </p>
                <p>
                    Esperamos contar con personas dedicadas y entusiastas que se sumen a nuestro 
                    objetivo de proporcionar una atención médica excepcional a nuestra comunidad.
                </p>
                <a href="<?php echo home_url('/puestos'); ?>"
                   style="display:inline-block; margin-top:16px; background:var(--primary); color:white; padding:12px 28px; border-radius:8px; text-decoration:none; font-weight:600">
                    Explorar puestos
                </a>
            </div>
            <div class="col-lg-6">
                <div style="background: var(--primary-light); border-radius:20px; padding:40px">
                    <div style="font-size:3rem; margin-bottom:12px; text-align:center">👥</div>
                    <h4 style="color:var(--primary); font-weight:700; text-align:center; margin-bottom:20px">
                        Nuestro equipo te espera
                    </h4>
                    <ul style="list-style:none; padding:0; margin:0">
                        <?php
                        $razones = array(
                            'Ambiente de trabajo colaborativo',
                            'Profesionales comprometidos con la excelencia',
                            'Crecimiento profesional continuo',
                            'Impacto real en la comunidad',
                        );
                        foreach ($razones as $r): ?>
                            <li style="padding: 8px 0; border-bottom: 1px solid rgba(26,173,148,0.2); display:flex; align-items:center; gap:10px">
                                <span style="color:var(--primary); font-weight:700">✓</span>
                                <?php echo $r; ?>
                            </li>
                        <?php endforeach; ?>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</section>

<!-- BENEFICIOS -->
<section class="section section-alt">
    <div class="container">
        <div class="text-center mb-5">
            <h2 class="section-title">Beneficios de trabajar con nosotros</h2>
            <div class="divider mx-auto"></div>
            <p class="section-subtitle">
                Valoramos a nuestro equipo y nos esforzamos por crear un entorno 
                de trabajo enriquecedor y gratificante.
            </p>
        </div>

        <div class="row g-4">
            <?php
            $beneficios = array(
                array('icon' => '📈', 'titulo' => 'Desarrollo profesional',
                    'desc' => 'Oportunidades de capacitación, programas de desarrollo y apoyo para participar en conferencias especializadas.'),
                array('icon' => '⚖️', 'titulo' => 'Equilibrio trabajo-vida',
                    'desc' => 'Horarios flexibles y opciones de trabajo remoto cuando sea posible para gestionar sus responsabilidades personales.'),
                array('icon' => '🤝', 'titulo' => 'Ambiente colaborativo',
                    'desc' => 'Cultura basada en la colaboración y el trabajo en equipo, valorando la diversidad de ideas y experiencias.'),
                array('icon' => '💰', 'titulo' => 'Compensación competitiva',
                    'desc' => 'Salario justo y beneficios adicionales incluyendo seguro médico, planes de jubilación y programas de bienestar.'),
                array('icon' => '🖥️', 'titulo' => 'Tecnología de vanguardia',
                    'desc' => 'Trabaje con herramientas y equipos médicos de última generación para brindar atención de calidad.'),
                array('icon' => '💛', 'titulo' => 'Cultura de apoyo',
                    'desc' => 'Equipo que se preocupa por su bienestar con comunicación abierta, respeto mutuo y ambiente positivo.'),
            );
            foreach ($beneficios as $b): ?>
                <div class="col-md-6 col-lg-4">
                    <div class="card-shirofy">
                        <div class="card-body">
                            <div class="card-icon"><?php echo $b['icon']; ?></div>
                            <div class="card-title"><?php echo $b['titulo']; ?></div>
                            <p class="text-muted" style="font-size:0.9rem; margin:0">
                                <?php echo $b['desc']; ?>
                            </p>
                        </div>
                    </div>
                </div>
            <?php endforeach; ?>
        </div>
    </div>
</section>

<!-- CTA final -->
<section class="section" style="background:var(--primary); color:white; text-align:center">
    <div class="container">
        <h2 style="font-size:2rem; font-weight:800; margin-bottom:12px">
            ¿Listo para dar el siguiente paso?
        </h2>
        <p style="opacity:0.9; max-width:500px; margin: 0 auto 28px">
            Explore nuestros puestos disponibles y comience su camino en Shirofy.
        </p>
        <a href="<?php echo home_url('/puestos'); ?>" class="btn-hero">
            Ver Puestos Disponibles
        </a>
    </div>
</section>

<?php get_footer(); ?>