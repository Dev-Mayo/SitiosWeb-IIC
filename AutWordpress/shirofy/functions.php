<?php

function shirofy_enqueue_scripts() {
    // Bootstrap CSS
    wp_enqueue_style(
        'bootstrap',
        'https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css'
    );

    // Theme stylesheet
    wp_enqueue_style(
        'shirofy-style',
        get_stylesheet_uri(),
        array('bootstrap')
    );

    // Bootstrap JS
    wp_enqueue_script(
        'bootstrap-js',
        'https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js',
        array(),
        null,
        true
    );
}
add_action('wp_enqueue_scripts', 'shirofy_enqueue_scripts');

function shirofy_setup() {
    add_theme_support('title-tag');
    add_theme_support('post-thumbnails');
    add_theme_support('html5', array('search-form', 'comment-form', 'gallery'));

    register_nav_menus(array(
        'primary' => 'Menú Principal'
    ));
}
add_action('after_setup_theme', 'shirofy_setup');