// CÓDIGO CORREGIDO (Opción A: Ruta absoluta)
document.addEventListener('DOMContentLoaded', () => {
    const botonesReserva = document.querySelectorAll('.Reserva');
    botonesReserva.forEach(boton => {
        boton.addEventListener('click', () => {
            // Asumiendo que el controlador es 'Home' y la acción es 'Calendario'
            window.location.href = '/Home/Calendario'; // <--- CAMBIO CLAVE
        });
    });
});