document.addEventListener('DOMContentLoaded', () => {
    // =======================================================
    // 1. VARIABLES GLOBALES Y ELEMENTOS DEL DOM
    // =======================================================
    const currentMonthYear = document.getElementById('currentMonthYear');
    const calendarGrid = document.getElementById('calendarGrid');
    const prevMonthBtn = document.getElementById('prevMonth');
    const nextMonthBtn = document.getElementById('nextMonth');
    const timeSlotsGrid = document.getElementById('timeSlotsGrid');
    const timeSlotsDate = document.getElementById('timeSlotsDate');
    const selectedDateTimeDisplay = document.getElementById('selectedDateTime');
    const nextButton = document.getElementById('nextButton');

    // Variables de estado
    let fechaActual = new Date(); // Inicia con la fecha de hoy
    let mesActual = fechaActual.getMonth();
    let anioActual = fechaActual.getFullYear();
    let diaSeleccionado = null;
    let horaSeleccionada = null;
    let fechaSeleccionadaCompleta = null; // Para enviar al formulario final

    // =======================================================
    // 2. FUNCIONES DE CALENDARIO
    // =======================================================

    /**
     * Dibuja los días del mes actual en el grid.
     */
    function renderCalendar() {
        calendarGrid.innerHTML = '';
        currentMonthYear.textContent = `${formatearMes(mesActual)} ${anioActual}`;

        const primerDiaDelMes = new Date(anioActual, mesActual, 1);
        const ultimoDiaDelMes = new Date(anioActual, mesActual + 1, 0);
        const diasPrevios = primerDiaDelMes.getDay(); // 0 (Dom) a 6 (Sáb)
        const totalDias = ultimoDiaDelMes.getDate();

        // 1. Relleno de días previos (vacíos)
        for (let i = 0; i < diasPrevios; i++) {
            calendarGrid.appendChild(crearElementoDia(null));
        }

        // 2. Días del mes actual
        for (let dia = 1; dia <= totalDias; dia++) {
            calendarGrid.appendChild(crearElementoDia(dia));
        }
    }

    /**
     * Crea un elemento <div> para un día del mes.
     */
    function crearElementoDia(dia) {
        const diaDiv = document.createElement('div');
        diaDiv.className = 'calendar-day';

        if (dia) {
            diaDiv.textContent = dia;
            diaDiv.classList.add('active-day');

            // Crea un objeto Date para verificar si es el pasado
            const fechaDia = new Date(anioActual, mesActual, dia);
            const hoy = new Date();
            hoy.setHours(0, 0, 0, 0);

            // Deshabilita y estiliza días pasados
            if (fechaDia < hoy) {
                diaDiv.classList.add('disabled-day');
            } else {
                diaDiv.dataset.dia = dia;
                diaDiv.addEventListener('click', handleDayClick);
            }
        }
        return diaDiv;
    }

    /**
     * Maneja el clic en un día activo.
     */
    function handleDayClick(event) {
        // Limpiar selección previa
        document.querySelectorAll('.calendar-day.selected').forEach(el => el.classList.remove('selected'));

        // Aplicar nueva selección
        event.target.classList.add('selected');

        // Resetear selección de hora
        horaSeleccionada = null;
        updateDetailsDisplay();

        // Obtener la fecha para la solicitud AJAX
        diaSeleccionado = event.target.dataset.dia;
        const fechaString = `${anioActual}-${mesActual + 1}-${diaSeleccionado}`; // Mes es 0-index en JS
        fechaSeleccionadaCompleta = fechaString; // Guardamos para el envío final

        // Llamar a la función para cargar horarios
        cargarHorariosDisponibles(fechaString, diaSeleccionado, formatearMes(mesActual));
    }

    // Función de ayuda para obtener el nombre del mes
    function formatearMes(mesIndex) {
        const meses = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];
        return meses[mesIndex];
    }

    // =======================================================
    // 3. NAVEGACIÓN DE MESES
    // =======================================================
    prevMonthBtn.addEventListener('click', () => {
        mesActual--;
        if (mesActual < 0) {
            mesActual = 11;
            anioActual--;
        }
        renderCalendar();
    });

    nextMonthBtn.addEventListener('click', () => {
        mesActual++;
        if (mesActual > 11) {
            mesActual = 0;
            anioActual++;
        }
        renderCalendar();
    });

    // =======================================================
    // 4. LÓGICA DE AJAX (Comunicación con C#)
    // =======================================================

    async function cargarHorariosDisponibles(fecha, dia, mes) {
        timeSlotsDate.textContent = `Buscando disponibilidad para el ${dia} de ${mes}...`;
        timeSlotsGrid.innerHTML = ''; // Limpiar
        nextButton.disabled = true;

        try {
            const response = await fetch('/Home/ObtenerDisponibilidad', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ fecha: fecha })
            });

            if (!response.ok) throw new Error('Respuesta de red no satisfactoria');

            const horarios = await response.json();
            renderizarHorarios(horarios, dia, mes);

        } catch (error) {
            console.error('Error al obtener disponibilidad:', error);
            timeSlotsDate.textContent = 'Error al cargar los horarios.';
        }
    }

    /**
     * Dibuja los botones de hora recibidos desde el servidor.
     */
    function renderizarHorarios(horarios, dia, mes) {
        timeSlotsGrid.innerHTML = '';

        if (!horarios || horarios.length === 0) {
            timeSlotsDate.textContent = `No hay disponibilidad para el ${dia} de ${mes}.`;
            return;
        }

        timeSlotsDate.textContent = `Selecciona una hora para el ${dia} de ${mes}:`;

        horarios.forEach(hora => {
            const botonHora = document.createElement('button');
            botonHora.className = 'time-slot-button';
            botonHora.textContent = hora;
            botonHora.dataset.hora = hora;

            botonHora.addEventListener('click', handleTimeClick);

            timeSlotsGrid.appendChild(botonHora);
        });
    }

    /**
     * Maneja el clic en un botón de hora disponible.
     */
    function handleTimeClick(event) {
        // Limpiar selección previa
        document.querySelectorAll('.time-slot-button.selected').forEach(el => el.classList.remove('selected'));

        // Aplicar nueva selección
        event.target.classList.add('selected');
        horaSeleccionada = event.target.dataset.hora;

        // Actualizar el panel de detalles y habilitar el botón "Siguiente"
        updateDetailsDisplay();
    }

    // =======================================================
    // 5. ACTUALIZACIÓN DEL PANEL DE DETALLES
    // =======================================================
    function updateDetailsDisplay() {
        if (diaSeleccionado && horaSeleccionada) {
            selectedDateTimeDisplay.textContent = `${diaSeleccionado} de ${formatearMes(mesActual)} ${anioActual}, de ${horaSeleccionada}`;
            nextButton.disabled = false;
        } else if (diaSeleccionado) {
            selectedDateTimeDisplay.textContent = `${diaSeleccionado} de ${formatearMes(mesActual)} ${anioActual}`;
            nextButton.disabled = true;
        } else {
            selectedDateTimeDisplay.textContent = 'Selecciona una fecha y hora';
            nextButton.disabled = true;
        }
    }
    function redirectToForm() {
        // La redirección aquí debe llevar al formulario de detalles del diagnóstico.
        // Usaremos los valores de las variables globales (fechaSeleccionadaCompleta y horaSeleccionada).

        if (fechaSeleccionadaCompleta && horaSeleccionada) {
            // Codifica los valores para pasarlos por URL (query string)
            const fecha = encodeURIComponent(fechaSeleccionadaCompleta);
            const hora = encodeURIComponent(horaSeleccionada);

            // Redirige al controlador Home/Diagnosticabtn con los datos
            window.location.href = `/Home/Diagnosticabtn?fecha=${fecha}&hora=${hora}`;
        } else {
            alert('Por favor, selecciona una fecha y una hora.');
        }
    }

    // =======================================================
    // 6. INICIALIZACIÓN
    // =======================================================
    renderCalendar();
});