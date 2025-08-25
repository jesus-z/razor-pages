// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    const body = document.body;
    const sidebarToggle = document.querySelector('.sidebar-toggle');

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function (e) {
            e.preventDefault();
            // Alternar la clase en el body para mover el sidebar y el contenido
            if (body.classList.contains('sidebar-closed')) {
                body.classList.remove('sidebar-closed');
                body.classList.add('sidebar-open');
            } else {
                body.classList.remove('sidebar-open');
                body.classList.add('sidebar-closed');
            }
        });
    }
});