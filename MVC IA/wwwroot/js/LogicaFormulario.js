document.addEventListener('DOMContentLoaded', () => {

    // =======================================================
    // 1. CAMPOS DE TEXTO (Nombre, Apellido, Email, Teléfono, Descripción)
    // =======================================================

    const inputNombre = document.getElementById('nombre');
    const inputApellido = document.getElementById('apellido');
    const inputEmail = document.getElementById('email');
    const inputTelefono = document.getElementById('telefono');
    const inputProblema = document.getElementById('problema');
    const inputFabricacion = document.getElementById('fabricacion');
    const selectPais = document.getElementById('pais');

    // Elementos de visualización
    const displayNombreCompleto = document.getElementById('displayNombreCompleto');
    const displayEmail = document.getElementById('displayEmail');
    const displayTelefono = document.getElementById('displayTelefono');
    const displayProblema = document.getElementById('displayProblema');
    const displayFabricacion = document.getElementById('displayFabricacion');
    const displayPais = document.getElementById('displayPais');

    // Función para actualizar el nombre completo (se llama en cada input de nombre/apellido)
    const actualizarNombreCompleto = () => {
        if (displayNombreCompleto) {
            const nombre = inputNombre?.value || '';
            const apellido = inputApellido?.value || '';

            // Muestra un guion o espacio si no hay nada escrito
            const texto = (nombre.trim() || apellido.trim()) ? `${nombre} ${apellido}`.trim() : '---';
            displayNombreCompleto.textContent = texto;
        }
    };

    // Escuchadores de eventos para los campos de texto:
    if (inputNombre) inputNombre.addEventListener('input', actualizarNombreCompleto);
    if (inputApellido) inputApellido.addEventListener('input', actualizarNombreCompleto);

    if (inputEmail && displayEmail) {
        inputEmail.addEventListener('input', () => { displayEmail.textContent = inputEmail.value || '---'; });
    }
    if (inputTelefono && displayTelefono) {
        inputTelefono.addEventListener('input', () => { displayTelefono.textContent = inputTelefono.value || '---'; });
    }
    if (inputProblema && displayProblema) {
        inputProblema.addEventListener('input', () => {
            // Limita la vista previa a un máximo de caracteres si es muy larga
            let texto = inputProblema.value.substring(0, 100);
            displayProblema.textContent = texto || 'Esperando descripción...';
        });
    }
    if (inputFabricacion && displayFabricacion) {
        inputFabricacion.addEventListener('input', () => { displayFabricacion.textContent = inputFabricacion.value || '---'; });
    }

    // Escuchador para el SELECT de País
    if (selectPais && displayPais) {
        selectPais.addEventListener('change', () => {
            // Usa textContent para mostrar el texto de la opción seleccionada, no el 'value'
            displayPais.textContent = selectPais.options[selectPais.selectedIndex].textContent || '---';
        });
    }


    // =======================================================
    // 2. RADIO BUTTONS (Tipo de Equipo y Sistema Operativo)
    // =======================================================
    const displayTipoEquipo = document.getElementById('displayTipoEquipo');
    const displayOS = document.getElementById('displayOS');

    // Tipo de Equipo
    document.querySelectorAll('input[name="tipo_equipo"]').forEach(radio => {
        radio.addEventListener('change', (event) => {
            if (displayTipoEquipo) {
                // Obtiene el texto de la etiqueta asociada al radio button
                const labelText = event.target.parentNode.textContent.trim();
                displayTipoEquipo.textContent = labelText;
            }
        });
    });

    // Sistema Operativo
    document.querySelectorAll('input[name="so"]').forEach(radio => {
        radio.addEventListener('change', (event) => {
            if (displayOS) {
                // Obtiene el texto de la etiqueta asociada al radio button
                const labelText = event.target.parentNode.textContent.trim();
                displayOS.textContent = labelText;
            }
        });
    });

    // Opcional: Ejecuta las funciones una vez al cargar para mostrar valores pre-cargados (si existen)
    actualizarNombreCompleto();
    if (selectPais) selectPais.dispatchEvent(new Event('change'));

    const displayFechaHora = document.getElementById('displayFechaHora');

    if (displayFechaHora) {
        // Creamos un objeto de fecha y hora actual
        const fechaActual = new Date();

        // Opciones para formatear el día, mes y año
        const opcionesFecha = {
            day: '2-digit', // 01, 02, ... 
            month: 'long',  // Octubre, Noviembre...
            year: 'numeric' // 2025
        };

        // Opciones para formatear la hora (asumiendo que quieres el formato 24h)
        const opcionesHora = {
            hour: '2-digit',
            minute: '2-digit',
            hour12: false // Formato 24 horas
        };

        // Obtener el formato de la fecha y hora según la ubicación del usuario
        // Usamos 'es-AR' para obtener meses en español, si necesitas otro, cámbialo.
        const fechaFormateada = fechaActual.toLocaleDateString('es-AR', opcionesFecha);
        const horaFormateada = fechaActual.toLocaleTimeString('es-AR', opcionesHora);

        // Combinamos y lo inyectamos en el elemento
        displayFechaHora.textContent = `${fechaFormateada}, ${horaFormateada}`;
    }
});