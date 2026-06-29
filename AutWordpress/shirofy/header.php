<!DOCTYPE html>
<html <?php language_attributes(); ?>>
<head>
    <meta charset="<?php bloginfo('charset'); ?>">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <?php wp_head(); ?>
</head>
<body <?php body_class(); ?>>

<?php
$current_page = basename(get_permalink());
$pages = array(
    array('url' => home_url('/'),               'label' => 'Inicio'),
    array('url' => home_url('/servicios'),       'label' => 'Servicios'),
    array('url' => home_url('/comunidad'),       'label' => 'Comunidad'),
    array('url' => home_url('/carreras'),        'label' => 'Carreras'),
    array('url' => home_url('/puestos'),         'label' => 'Puestos Disponibles'),
);
?>

<nav class="navbar-shirofy">
    <div class="container d-flex justify-content-between align-items-center">

        <a href="<?php echo home_url('/'); ?>" class="brand">
            <img src="<?php echo get_template_directory_uri(); ?>/img/logoempresa.png"
                 alt="Shirofy"
                 onerror="this.style.display='none'">
            <span class="brand-name">Shirofy</span>
        </a>

        <!-- Mobile toggle -->
        <button class="navbar-toggler d-md-none" onclick="toggleMenu()">
            <span></span><span></span><span></span>
        </button>

        <!-- Nav links -->
        <ul class="nav-links d-none d-md-flex" id="navLinks">
            <?php foreach ($pages as $page): ?>
                <li>
                    <a href="<?php echo $page['url']; ?>"
                       class="<?php echo (get_permalink() == $page['url'] || $_SERVER['REQUEST_URI'] == parse_url($page['url'], PHP_URL_PATH)) ? 'active' : ''; ?>">
                        <?php echo $page['label']; ?>
                    </a>
                </li>
            <?php endforeach; ?>
        </ul>

    </div>

    <!-- Mobile menu -->
    <div class="container d-md-none">
        <ul class="nav-links flex-column" id="mobileMenu" style="display:none!important">
            <?php foreach ($pages as $page): ?>
                <li>
                    <a href="<?php echo $page['url']; ?>">
                        <?php echo $page['label']; ?>
                    </a>
                </li>
            <?php endforeach; ?>
        </ul>
    </div>
</nav>

<script>
function toggleMenu() {
    var menu = document.getElementById('mobileMenu');
    menu.style.display = menu.style.display === 'none' || menu.style.display === '' 
        ? 'flex' : 'none';
    menu.style.flexDirection = 'column';
}
</script>