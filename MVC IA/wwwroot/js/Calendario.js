const calendarGrid = document.getElementById('calendarGrid');
const currentMonthYearEl = document.getElementById('currentMonthYear');
const prevMonthBtn = document.getElementById('prevMonth');
const nextMonthBtn = document.getElementById('nextMonth');
const timeSlotsGrid = document.getElementById('timeSlotsGrid');
const timeSlotsDateEl = document.getElementById('timeSlotsDate');
const selectedDateTimeEl = document.getElementById('selectedDateTime');
const nextButton = document.getElementById('nextButton');

// --- VARIABLES GLOBALES DE ESTADO ---
let currentDate = new Date();
let selectedDate = null; // Almacena la fecha seleccionada (YYYY-MM-DD)
let selectedTime = null; // Almacena la franja horaria seleccionada (HH:mm - HH:mm)


// --- FUNCIONES ---
function renderCalendar() {
    // Limpiamos el calendario
    calendarGrid.innerHTML = '';

    // Obtenemos el primer día del mes y el número de días
    const startOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    const endOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
    const numDaysInMonth = endOfMonth.getDate();
    // Ajustamos getDay() para que la semana inicie en Lunes (0 = Lunes, 6 = Domingo)
    // En la plantilla, si 0 es Domingo, lo convertimos a 6 y el resto -1
    let firstDayIndex = startOfMonth.getDay() === 0 ? 6 : startOfMonth.getDay() - 1;

    // Actualizamos el mes y año
    const monthName = currentDate.toLocaleString('es-ES', { month: 'long' });
    const year = currentDate.getFullYear();
    currentMonthYearEl.textContent = `${monthName.charAt(0).toUpperCase() + monthName.slice(1)} ${year}`;

    // Rellenamos los espacios vacíos del inicio
    for (let i = 0; i < firstDayIndex; i++) {
        const emptyCell = document.createElement('div');
        emptyCell.classList.add('day-cell', 'disabled');
        calendarGrid.appendChild(emptyCell);
    }

    // Creamos las celdas para cada día del mes
    for (let day = 1; day <= numDaysInMonth; day++) {
        const dayCell = document.createElement('div');
        dayCell.classList.add('day-cell');
        dayCell.textContent = day;

        const dayDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), day);
        const today = new Date();

        // Formato para comparación (YYYY-MM-DD)
        const formattedDate = dayDate.toISOString().slice(0, 10);

        // Clases especiales para el día actual y los días pasados
        if (dayDate.setHours(0, 0, 0, 0) === today.setHours(0, 0, 0, 0)) {
            dayCell.classList.add('current-day');
        }
        // Usamos solo la comparación de fecha (día < hoy) para deshabilitar días pasados
        if (dayDate < today) {
            dayCell.classList.add('disabled');
        }

        // Si la fecha está seleccionada, le damos la clase 'active'
        if (selectedDate && formattedDate === selectedDate) {
            dayCell.classList.add('active');
        }

        // Asignamos el valor de la fecha para usarlo en el click
        dayCell.dataset.date = formattedDate;

        // Añadimos el evento de click
        dayCell.addEventListener('click', () => {
            if (!dayCell.classList.contains('disabled')) {
                // Remueve 'active' del día anterior
                const prevActive = calendarGrid.querySelector('.day-cell.active');
                if (prevActive) {
                    prevActive.classList.remove('active');
                }
                // Añade 'active' al día actual
                dayCell.classList.add('active');

                selectedDate = formattedDate;
                selectedTime = null; // Reinicia la hora

                // Llama a la función asíncrona que se comunica con C#
                fetchTimeSlotsFromBackend(selectedDate);

                updateDetails();
            }
        });

        calendarGrid.appendChild(dayCell);
    }
}

/**
 * @name fetchTimeSlotsFromBackend
 * @description Llama a la acción del controlador C# para obtener horarios disponibles.
 * @param {string} date - Fecha en formato YYYY-MM-DD.
 */
async function fetchTimeSlotsFromBackend(date) {
    // 1. Mostrar estado de carga
    timeSlotsGrid.innerHTML = '';
    const dateTitle = new Date(date + 'T00:00:00').toLocaleDateString('es-ES', { weekday: 'long', day: 'numeric', month: 'long' });
    timeSlotsDateEl.textContent = `Buscando disponibilidad para: ${dateTitle}`;

    try {
        // NOTA: Se asume que el backend está en /Home/ObtenerDisponibilidad
        const response = await fetch('/Home/ObtenerDisponibilidad', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            // Enviamos la fecha al backend
            body: JSON.stringify({ fecha: date })
        });

        if (!response.ok) throw new Error('Respuesta de red no satisfactoria');

        // La respuesta es el array de strings con los intervalos de 2 horas
        const timeSlots = await response.json();
        renderTimeSlots(timeSlots, date);

    } catch (error) {
        console.error('Error al obtener disponibilidad:', error);
        timeSlotsDateEl.textContent = 'Error al cargar los horarios.';
    }
}


/**
 * @name renderTimeSlots
 * @description Dibuja los botones de hora recibidos (intervalos de 2h).
 * @param {string[]} timeSlots - Array de strings con los horarios (ej: ['10:00 - 12:00']).
 * @param {string} date - Fecha seleccionada.
 */
function renderTimeSlots(timeSlots, date) {
    timeSlotsGrid.innerHTML = ''; // Limpiamos los horarios

    // Convertimos la fecha con una hora ficticia para evitar problemas de zona horaria
    const dateObj = new Date(date + 'T00:00:00');

    // Formato de fecha para el título de la columna de horarios
    const formattedTitle = dateObj.toLocaleDateString('es-ES', { weekday: 'long', day: 'numeric', month: 'long' });

    if (timeSlots.length > 0) {
        timeSlotsDateEl.textContent = `Disponibilidad para: ${formattedTitle}`;
        timeSlots.forEach(time => {
            const timeSlotBtn = document.createElement('button');
            timeSlotBtn.classList.add('time-slot');
            timeSlotBtn.textContent = time;

            if (selectedTime === time) {
                timeSlotBtn.classList.add('active');
            }

            timeSlotBtn.addEventListener('click', () => {
                const prevActive = timeSlotsGrid.querySelector('.time-slot.active');
                if (prevActive) {
                    prevActive.classList.remove('active');
                }
                timeSlotBtn.classList.add('active');
                selectedTime = time;
                updateDetails();
            });

            timeSlotsGrid.appendChild(timeSlotBtn);
        });
    } else {
        timeSlotsDateEl.textContent = `No hay disponibilidad para esta fecha.`;
    }
}

function updateDetails() {
    if (selectedDate && selectedTime) {
        // Lógica para actualizar los detalles del servicio
        const dateObj = new Date(selectedDate + 'T00:00:00');
        const formattedDate = dateObj.toLocaleDateString('es-ES', { weekday: 'long', day: 'numeric', month: 'long' });

        // Asumiendo que selectedDateTimeEl es el div de "Detalles del servicio"
        selectedDateTimeEl.innerHTML = `
            <strong>${formattedDate}, ${selectedTime}</strong><br>
            100% Online<br>
            Servicio con intervalo de 2 horas<br>
        `;
        nextButton.disabled = false; // Habilitar el botón Siguiente
    } else {
        selectedDateTimeEl.innerHTML = 'Selecciona una fecha y hora para ver los detalles.';
        nextButton.disabled = true; // Deshabilitar si no hay selección
    }
}

// --- MANEJO DE EVENTOS INICIALES ---
prevMonthBtn.addEventListener('click', () => {
    currentDate.setMonth(currentDate.getMonth() - 1);
    renderCalendar();
});

nextMonthBtn.addEventListener('click', () => {
    currentDate.setMonth(currentDate.getMonth() + 1);
    renderCalendar();
});

// --- LÓGICA DEL BOTÓN "SIGUIENTE" (LA PARTE CLAVE) ---
nextButton.addEventListener('click', () => {
    // 1. Validar que se haya seleccionado fecha y hora
    if (!selectedDate || !selectedTime) {
        alert('Por favor, selecciona una fecha y una franja horaria.');
        return;
    }

    // 2. Preparar los parámetros como C# los espera
    // selectedDate es "YYYY-MM-DD" (ej: "2025-11-12")
    // selectedTime es "HH:mm - HH:mm" (ej: "14:00 - 16:00")
    // Tu CitasController ya sabe cómo manejar estos dos formatos.

    const fecha = selectedDate;
    const hora = selectedTime;

    // 3. Codificar la URL y Redirigir al endpoint correcto
    const url = `/Citas/ConfirmacionReserva?fecha=${encodeURIComponent(fecha)}&hora=${encodeURIComponent(hora)}`;

    // Redirección a la nueva vista de C# MVC
    window.location.href = url;
});


// Inicialización
renderCalendar();
updateDetails(); // Para asegurar que el botón esté deshabilitado al inicio