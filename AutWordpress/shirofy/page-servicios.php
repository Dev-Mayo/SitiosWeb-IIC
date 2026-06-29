<?php
/*
Template Name: Servicios
*/
get_header(); ?>

<!-- HERO -->
<section class="hero">
    <div class="container">
        <h1>Nuestros Servicios</h1>
        <p>Ofrecemos una amplia gama de servicios médicos integrales para cuidar su salud.</p>
    </div>
</section>

<!-- SERVICIOS -->
<section class="section">
    <div class="container">
        <div class="text-center mb-5">
            <h2 class="section-title">¿Qué ofrecemos?</h2>
            <div class="divider mx-auto"></div>
            <p class="section-subtitle">
                En Shirofy ofrecemos una amplia gama de servicios médicos para abordar 
                las necesidades de salud de nuestros pacientes.
            </p>
        </div>

        <div class="row g-4">
            <?php
            $servicios = array(
                array('icon' => '🩺', 'titulo' => 'Consultas médicas', 'desc' => 'Consultas médicas y exámenes físicos completos con especialistas de alto nivel.'),
                array('icon' => '💊', 'titulo' => 'Diagnóstico y tratamiento', 'desc' => 'Diagnóstico y tratamiento de enfermedades agudas y crónicas con tecnología de punta.'),
                array('icon' => '🛡️', 'titulo' => 'Atención preventiva', 'desc' => 'Atención preventiva y promoción de la salud para mantener su bienestar a largo plazo.'),
                array('icon' => '🔬', 'titulo' => 'Servicios de laboratorio', 'desc' => 'Servicios de laboratorio y pruebas diagnósticas con resultados precisos y rápidos.'),
                array('icon' => '📷', 'titulo' => 'Imágenes médicas', 'desc' => 'Servicios de imágenes médicas incluyendo radiografías y resonancias magnéticas.'),
                array('icon' => '⚕️', 'titulo' => 'Cirugías especializadas', 'desc' => 'Cirugías y procedimientos médicos especializados realizados por expertos certificados.'),
            );
            foreach ($servicios as $s): ?>
                <div class="col-md-6 col-lg-4">
                    <div class="card-shirofy">
                        <div class="card-body">
                            <div class="card-icon"><?php echo $s['icon']; ?></div>
                            <div class="card-title"><?php echo $s['titulo']; ?></div>
                            <p class="text-muted" style="font-size:0.9rem; margin:0">
                                <?php echo $s['desc']; ?>
                            </p>
                        </div>
                    </div>
                </div>
            <?php endforeach; ?>
        </div>
    </div>
</section>

<!-- NUESTRO EQUIPO -->
<section class="section section-alt">
    <div class="container">
        <div class="text-center mb-5">
            <h2 class="section-title">Nuestro equipo</h2>
            <div class="divider mx-auto"></div>
            <p class="section-subtitle">
                Contamos con un equipo multidisciplinario dedicado a brindar 
                una atención integral y de calidad.
            </p>
        </div>

        <div class="row g-4 justify-content-center">
            <?php
            $equipo = array(
                array('inicial' => 'DM', 'nombre' => 'Dr. Mario Rojas', 'puesto' => 'Director Médico'),
                array('inicial' => 'CE', 'nombre' => 'Dra. Carmen Elizondo', 'puesto' => 'Especialista en Cardiología'),
                array('inicial' => 'LP', 'nombre' => 'Dr. Luis Pérez', 'puesto' => 'Cirujano General'),
                array('inicial' => 'AV', 'nombre' => 'Dra. Ana Vargas', 'puesto' => 'Especialista en Pediatría'),
            );
            foreach ($equipo as $m): ?>
                <div class="col-6 col-md-3">
                    <div class="card-shirofy team-card">
                        <div class="team-avatar"><?php echo $m['inicial']; ?></div>
                        <div class="card-title"><?php echo $m['nombre']; ?></div>
                        <p class="text-muted" style="font-size:0.85rem; margin:0">
                            <?php echo $m['puesto']; ?>
                        </p>
                    </div>
                </div>
            <?php endforeach; ?>
        </div>

        <div class="text-center mt-5">
            <p class="text-muted">
                Nuestro equipo está altamente capacitado y se mantiene actualizado con los 
                avances médicos más recientes para ofrecer los mejores resultados posibles.
            </p>
        </div>
    </div>
</section>

<?php get_footer(); ?>