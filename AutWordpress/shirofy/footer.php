<footer class="footer">
    <div class="container">
        <div class="row g-4">

            <div class="col-md-4">
                <h5>Shirofy</h5>
                <p style="font-size:0.9rem">
                    Empresa líder en servicios médicos, comprometida con la 
                    atención de calidad y el bienestar de nuestros pacientes.
                </p>
            </div>

            <div class="col-md-2">
                <h5>Navegación</h5>
                <a href="<?php echo home_url('/'); ?>">Inicio</a>
                <a href="<?php echo home_url('/servicios'); ?>">Servicios</a>
                <a href="<?php echo home_url('/comunidad'); ?>">Comunidad</a>
                <a href="<?php echo home_url('/carreras'); ?>">Carreras</a>
                <a href="<?php echo home_url('/puestos'); ?>">Puestos</a>
            </div>

            <div class="col-md-3">
                <h5>Contacto</h5>
                <p style="font-size:0.9rem">
                    📍 Cartago, Costa Rica<br>
                    📞 +506 2550-0000<br>
                    ✉️ info@shirofy.com
                </p>
            </div>

            <div class="col-md-3">
                <h5>Horario</h5>
                <p style="font-size:0.9rem">
                    Lunes a Viernes<br>
                    7:00 am – 5:00 pm<br><br>
                    Emergencias 24/7
                </p>
            </div>

        </div>

        <div class="footer-bottom">
            <p class="mb-0">
                &copy; <?php echo date('Y'); ?> Shirofy. 
                Todos los derechos reservados.
            </p>
        </div>
    </div>
</footer>

<?php wp_footer(); ?>
</body>
</html>