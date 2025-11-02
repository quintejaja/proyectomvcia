//const mockAvailability = {
//    // Formato: 'YYYY-MM-DD'
//    '2025-05-26': ['1:30 p.m.', '2:00 p.m.', '2:30 p.m.', '3:00 p.m.', '3:30 p.m.', '4:00 p.m.', '4:30 p.m.', '5:00 p.m.'],
//    '2025-05-27': ['10:00 a.m.', '11:00 a.m.', '1:00 p.m.'],
//    '2025-06-05': ['10:30 a.m.', '11:00 a.m.', '11:30 a.m.']
//};

//// --- ESTADO INICIAL ---
//let currentDate = new Date();
//let selectedDate = null;
//let selectedTime = null;

//// --- ELEMENTOS DEL DOM ---
//const calendarGrid = document.getElementById('calendarGrid');
//const currentMonthYearEl = document.getElementById('currentMonthYear');
//const prevMonthBtn = document.getElementById('prevMonth');
//const nextMonthBtn = document.getElementById('nextMonth');
//const timeSlotsGrid = document.getElementById('timeSlotsGrid');
//const timeSlotsDateEl = document.getElementById('timeSlotsDate');
//const selectedDateTimeEl = document.getElementById('selectedDateTime');
//const nextButton = document.getElementById('nextButton');

//// --- FUNCIONES ---
//function renderCalendar() {
//    // Limpiamos el calendario
//    calendarGrid.innerHTML = '';

//    // Obtenemos el primer día del mes y el número de días
//    const startOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
//    const endOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
//    const numDaysInMonth = endOfMonth.getDate();
//    const firstDayIndex = startOfMonth.getDay(); // 0 = Domingo, 1 = Lunes, etc.

//    // Actualizamos el mes y año
//    const monthName = currentDate.toLocaleString('es-ES', { month: 'long' });
//    const year = currentDate.getFullYear();
//    currentMonthYearEl.textContent = `${monthName.charAt(0).toUpperCase() + monthName.slice(1)} ${year}`;

//    // Rellenamos los espacios vacíos del inicio
//    for (let i = 0; i < firstDayIndex; i++) {
//        const emptyCell = document.createElement('div');
//        emptyCell.classList.add('day-cell', 'disabled');
//        calendarGrid.appendChild(emptyCell);
//    }

//    // Creamos las celdas para cada día del mes
//    for (let day = 1; day <= numDaysInMonth; day++) {
//        const dayCell = document.createElement('div');
//        dayCell.classList.add('day-cell');
//        dayCell.textContent = day;

//        const dayDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), day);
//        const today = new Date();

//        // Formato para comparar con la disponibilidad
//        const formattedDate = dayDate.toISOString().slice(0, 10);

//        // Clases especiales para el día actual y los días pasados
//        if (dayDate.setHours(0, 0, 0, 0) === today.setHours(0, 0, 0, 0)) {
//            dayCell.classList.add('current-day');
//        }
//        if (dayDate < today) {
//            dayCell.classList.add('disabled');
//        }

//        // Si la fecha está seleccionada, le damos la clase 'active'
//        if (selectedDate && formattedDate === selectedDate) {
//            dayCell.classList.add('active');
//        }

//        // Asignamos el valor de la fecha para usarlo en el click
//        dayCell.dataset.date = formattedDate;

//        // Añadimos el evento de click
//        dayCell.addEventListener('click', () => {
//            if (!dayCell.classList.contains('disabled')) {
//                // Removemos 'active' del día anterior
//                const prevActive = calendarGrid.querySelector('.day-cell.active');
//                if (prevActive) {
//                    prevActive.classList.remove('active');
//                }
//                // Añadimos 'active' al día actual
//                dayCell.classList.add('active');

//                selectedDate = formattedDate;
//                selectedTime = null; // Reiniciamos la hora
//                renderTimeSlots(selectedDate);
//                updateDetails();
//            }
//        });

//        calendarGrid.appendChild(dayCell);
//    }
//}

//function renderTimeSlots(date) {
//    // Limpiamos los horarios
//    timeSlotsGrid.innerHTML = '';

//    // Obtenemos los horarios disponibles para la fecha
//    const timeSlots = mockAvailability[date] || [];

//    if (timeSlots.length > 0) {
//        timeSlotsDateEl.textContent = `Disponibilidad para: ${new Date(date).toLocaleDateString('es-ES', { weekday: 'long', day: 'numeric', month: 'long' })}`;
//        timeSlots.forEach(time => {
//            const timeSlotBtn = document.createElement('button');
//            timeSlotBtn.classList.add('time-slot');
//            timeSlotBtn.textContent = time;

//            if (selectedTime === time) {
//                timeSlotBtn.classList.add('active');
//            }

//            timeSlotBtn.addEventListener('click', () => {
//                const prevActive = timeSlotsGrid.querySelector('.time-slot.active');
//                if (prevActive) {
//                    prevActive.classList.remove('active');
//                }
//                timeSlotBtn.classList.add('active');
//                selectedTime = time;
//                updateDetails();
//            });

//            timeSlotsGrid.appendChild(timeSlotBtn);
//        });
//    } else {
//        timeSlotsDateEl.textContent = `No hay disponibilidad para esta fecha.`;
//    }
//}

//function updateDetails() {
//    if (selectedDate && selectedTime) {
//        const dateObj = new Date(selectedDate);
//        const formattedDate = dateObj.toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' });
//        selectedDateTimeEl.textContent = `${formattedDate}, ${selectedTime}`;
//        nextButton.disabled = false;
//    } else {
//        selectedDateTimeEl.textContent = `Selecciona una fecha y hora`;
//        nextButton.disabled = true;
//    }
//}

//// --- EVENTOS DE NAVEGACIÓN ---
//prevMonthBtn.addEventListener('click', () => {
//    currentDate.setMonth(currentDate.getMonth() - 1);
//    renderCalendar();
//});

//nextMonthBtn.addEventListener('click', () => {
//    currentDate.setMonth(currentDate.getMonth() + 1);
//    renderCalendar();
//});

//// --- INICIALIZACIÓN ---
//document.addEventListener('DOMContentLoaded', () => {
//    renderCalendar();
//    updateDetails();
//});

// ELIMINAR O COMENTAR la variable mockAvailability, ya no la necesitamos
// const mockAvailability = { ... }; 

// --- ESTADO INICIAL ---
let currentDate = new Date();
let selectedDate = null;
let selectedTime = null;

// --- ELEMENTOS DEL DOM ---
const calendarGrid = document.getElementById('calendarGrid');
const currentMonthYearEl = document.getElementById('currentMonthYear');
const prevMonthBtn = document.getElementById('prevMonth');
const nextMonthBtn = document.getElementById('nextMonth');
const timeSlotsGrid = document.getElementById('timeSlotsGrid');
const timeSlotsDateEl = document.getElementById('timeSlotsDate');
const selectedDateTimeEl = document.getElementById('selectedDateTime');
const nextButton = document.getElementById('nextButton');

// --- FUNCIONES ---
function renderCalendar() {
    // ... (El cuerpo de renderCalendar se mantiene igual, ya que maneja la UI del mes) ...
    calendarGrid.innerHTML = '';

    const startOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    const endOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
    const numDaysInMonth = endOfMonth.getDate();
    const firstDayIndex = startOfMonth.getDay();

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

                // *** CAMBIO CLAVE: Llama a la función asíncrona ***
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
    timeSlotsDateEl.textContent = `Buscando disponibilidad para: ${new Date(date).toLocaleDateString('es-ES', { weekday: 'long', day: 'numeric', month: 'long' })}`;

    try {
        const response = await fetch('/Home/ObtenerDisponibilidad', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            // Enviamos la fecha al backend para que el servidor genere los intervalos
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
    const dateObj = new Date(date);

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
        const dateObj = new Date(selectedDate);
        const formattedDate = dateObj.toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' });

        // Asumiendo que selectedTime es el intervalo 'HH:MM - HH:MM'
        selectedDateTimeEl.textContent = `${formattedDate}, ${selectedTime}`;

        nextButton.disabled = false;
    } else {
        selectedDateTimeEl.textContent = `Selecciona una fecha y hora`;
        nextButton.disabled = true;
    }
}

// --- EVENTOS DE NAVEGACIÓN ---
// ... (Se mantienen igual)
prevMonthBtn.addEventListener('click', () => {
    currentDate.setMonth(currentDate.getMonth() - 1);
    renderCalendar();
});

nextMonthBtn.addEventListener('click', () => {
    currentDate.setMonth(currentDate.getMonth() + 1);
    renderCalendar();
});

// --- INICIALIZACIÓN ---
document.addEventListener('DOMContentLoaded', () => {
    renderCalendar();
    updateDetails();
});

// Agrega esta función para la redirección (si aún no la tienes)
nextButton.addEventListener('click', () => {
    if (selectedDate && selectedTime) {
        const fecha = encodeURIComponent(selectedDate);
        const hora = encodeURIComponent(selectedTime);
        window.location.href = `/Home/Diagnosticabtn?fecha=${fecha}&hora=${hora}`;
    }
});
