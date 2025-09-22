document.addEventListener('DOMContentLoaded', () => {
    const loginBox = document.getElementById('login-box');
    const registerBox = document.getElementById('register-box');
    const showRegisterLink = document.getElementById('show-register');
    const showLoginLink = document.getElementById('show-login');

    // Función para mostrar el formulario de registro
    showRegisterLink.addEventListener('click', (e) => {
        e.preventDefault(); // Evita el comportamiento predeterminado del enlace
        loginBox.classList.add('hidden');
        registerBox.classList.remove('hidden');
    });

    // Función para mostrar el formulario de login
    showLoginLink.addEventListener('click', (e) => {
        e.preventDefault(); // Evita el comportamiento predeterminado del enlace
        registerBox.classList.add('hidden');
        loginBox.classList.remove('hidden');
    });
});