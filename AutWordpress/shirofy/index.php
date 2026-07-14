<?php get_header(); ?>

<!-- HERO -->
<section class="hero">
    <div class="container">
        <h1>Bienvenidos a Shirofy</h1>
        <p>Líderes en servicios médicos integrales, comprometidos con su salud y bienestar.</p>
        <a href="<?php echo home_url('/puestos'); ?>" class="btn-hero">
            Ver Puestos Disponibles
        </a>
    </div>
</section>

<!-- QUIÉNES SOMOS -->
<section class="section">
    <div class="container">
        <div class="row align-items-center g-5">

            <div class="col-lg-6">
                <p class="text-uppercase fw-semibold" style="color:var(--primary); font-size:0.85rem; letter-spacing:1px">
                    Sobre nosotros
                </p>
                <h2 class="section-title">¿Quiénes somos?</h2>
                <div class="divider"></div>
                <p class="mb-3">
                    Bienvenidos a <strong>Shirofy</strong>, una empresa líder en el campo de los 
                    servicios médicos, comprometida con la atención de calidad y el bienestar 
                    de nuestros pacientes.
                </p>
                <p class="mb-3">
                    Con una pasión por la excelencia y un equipo de profesionales altamente 
                    capacitados, nos enorgullece brindar servicios médicos integrales y 
                    personalizados para satisfacer las necesidades de nuestra comunidad.
                </p>
                <p>
                    Creemos que todos merecen tener acceso a la atención médica de calidad. 
                    Nuestra misión es proporcionar servicios médicos excepcionales, promover la 
                    salud y el bienestar, y marcar una diferencia positiva en la vida de nuestros 
                    pacientes.
                </p>
            </div>

            <div class="col-lg-6">
                <div style="background: var(--primary-light); border-radius: 20px; padding: 48px; text-align:center">
                    <div style="font-size: 5rem; margin-bottom: 16px">🏥</div>
                    <h4 style="color: var(--primary); font-weight:700">Shirofy</h4>
                    <p class="text-muted mb-0">Más de 15 años cuidando a nuestra comunidad</p>
                </div>
            </div>

        </div>
    </div>
</section>

<!-- VALORES -->
<section class="section section-alt">
    <div class="container">
        <div class="text-center mb-5">
            <p class="text-uppercase fw-semibold" style="color:var(--primary); font-size:0.85rem; letter-spacing:1px">
                Lo que nos define
            </p>
            <h2 class="section-title">Nuestros valores fundamentales</h2>
            <div class="divider mx-auto"></div>
        </div>

        <div class="row g-4">

            <div class="col-md-6 col-lg-3">
                <div class="card-shirofy">
                    <div class="card-body">
                        <div class="card-icon">❤️</div>
                        <div class="card-title">Cuidado centrado en el paciente</div>
                        <p class="text-muted" style="font-size:0.9rem">
                            Colocamos a nuestros pacientes en el centro de todo lo que hacemos, 
                            brindando un cuidado compasivo, respetuoso y personalizado.
                        </p>
                    </div>
                </div>
            </div>

            <div class="col-md-6 col-lg-3">
                <div class="card-shirofy">
                    <div class="card-body">
                        <div class="card-icon">⭐</div>
                        <div class="card-title">Excelencia médica</div>
                        <p class="text-muted" style="font-size:0.9rem">
                            Nos comprometemos a mantener los más altos estándares de excelencia 
                            médica con profesionales altamente capacitados y especializados.
                        </p>
                    </div>
                </div>
            </div>

            <div class="col-md-6 col-lg-3">
                <div class="card-shirofy">
                    <div class="card-body">
                        <div class="card-icon">💡</div>
                        <div class="card-title">Tecnología avanzada</div>
                        <p class="text-muted" style="font-size:0.9rem">
                            Abrazamos la innovación y la tecnología para mejorar continuamente 
                            nuestros servicios con equipos médicos de última generación.
                        </p>
                    </div>
                </div>
            </div>

            <div class="col-md-6 col-lg-3">
                <div class="card-shirofy">
                    <div class="card-body">
                        <div class="card-icon">🔒</div>
                        <div class="card-title">Confidencialidad y ética</div>
                        <p class="text-muted" style="font-size:0.9rem">
                            Respetamos la privacidad y confidencialidad de nuestros pacientes 
                            cumpliendo los más altos estándares éticos y legales.
                        </p>
                    </div>
                </div>
            </div>

        </div>
    </div>
</section>

<!-- CTA -->
<section class="section" style="background: var(--primary); color: white; text-align:center">
    <div class="container">
        <h2 style="font-size:2rem; font-weight:800; margin-bottom:12px">
            ¿Listo para unirte a nuestro equipo?
        </h2>
        <p style="opacity:0.9; max-width:500px; margin: 0 auto 28px">
            Explore las oportunidades de empleo disponibles y forme parte de nuestra familia.
        </p>
        <a href="<?php echo home_url('/puestos'); ?>" 
           class="btn-hero">
            Ver Puestos Disponibles
        </a>
    </div>
</section>

<?php get_footer(); ?>