

function ActualizarSelectCategorias(urlObtenerCategorias) {

    $('#TipoOperacionId').change(async function () {
        //seleccionar el valor
        const valorSeleccionado = $(this).val();

        const respuesta = await fetch(
            urlObtenerCategorias,
            {
                method: 'POST',
                body: valorSeleccionado,
                headers: {
                    'Content-Type': 'application/json'
                }
            });

        const json = await respuesta.json();
        console.log(json);

        const opciones =
            json.map(categoria => `<option value=${categoria.value}>${categoria.text}</option>`);

        $('#CategoriaId').html(opciones);
    })
}