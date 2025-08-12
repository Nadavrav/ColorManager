$(document).ready(function () {
    loadColors();

    $('#btnAddNew').on('click', function () {
        resetForm();
        $('#formTitle').text('הוסף צבע חדש');
        $('#editFormContainer').slideDown();
    });

    $('#btnCancel').on('click', function () {
        $('#editFormContainer').slideUp();
    });

    $('#btnSave').on('click', saveColor);

    $('#colorsTable tbody').on('click', '.btn-delete', function () {
        const row = $(this).closest('tr');
        const colorId = row.data('id');
        if (confirm('האם אתה בטוח שברצונך למחוק צבע זה?')) {
            deleteColor(colorId);
        }
    });

    $('#colorsTable tbody').on('click', '.btn-edit', function () {
        const row = $(this).closest('tr');
        populateForm(row);
    });
    
$('#colorsTable tbody').sortable({
    axis: 'y', // Allow dragging only vertically
    update: function (event, ui) {
        // This function runs after a row has been dropped in a new position

        // Get the new order of IDs from the table rows
        const ids = $('#colorsTable tbody tr').map(function() {
            return $(this).data('id');
        }).get();

        // Send the new order to our backend API
        $.ajax({
            type: "POST",
            url: "/api/colors/updateorder",
            contentType: "application/json",
            data: JSON.stringify(ids),
            success: function() {
                console.log("Order saved successfully!");
                // Refresh the table to show the new order numbers from the server
                loadColors();
            },
            error: function() {
                alert('שגיאה בשמירת הסדר.');
            }
        });
    }
});

});

function loadColors() {
    $.ajax({
        type: "GET",
        url: "/api/colors",
        success: function (colors) {
            const tbody = $('#colorsTable tbody');
            tbody.empty();
            $.each(colors, function (i, color) {
                const stockStatus = color.isInStock ? '✔️' : '❌';
                const row = `<tr data-id="${color.id}">
                    <td class="color-name">${color.colorName}</td>
                    <td class="price">${color.price}</td>
                    <td class="hex-val" data-hex="${color.colorHex}"><div class="color-box" style="background-color:${color.colorHex};"></div> ${color.colorHex}</td>
                    <td class="in-stock" data-stock="${color.isInStock}">${stockStatus}</td>
                    <td class="order">${color.displayOrder}</td>
                    <td class="actions">
                        <button class="btn-edit">עריכה</button>
                        <button class="btn-delete">מחיקה</button>
                    </td>
                </tr>`;
                tbody.append(row);
            });
        },
        error: function(err) { alert('שגיאה בטעינת הנתונים.'); }
    });
}

function saveColor() {
    const colorId = $('#editColorId').val();
    const colorData = {
        colorName: $('#colorName').val(),
        colorHex: $('#colorHex').val(),
        price: parseFloat($('#price').val()),
        isInStock: $('#isInStock').is(':checked')
    };
    
    const method = colorId ? "PUT" : "POST";
    const url = colorId ? `/api/colors/${colorId}` : "/api/colors";

    $.ajax({
        type: method,
        url: url,
        contentType: "application/json",
        data: JSON.stringify(colorData),
        success: function () {
            $('#editFormContainer').slideUp();
            loadColors(); 
        },
        error: function(err) { alert('שגיאה בשמירת הנתונים.'); }
    });
}

function deleteColor(id) {
    $.ajax({
        type: "DELETE",
        url: `/api/colors/${id}`,
        success: function () {
            loadColors(); // Just reload the table for simplicity
        },
        error: function(err) { alert('שגיאה במחיקת הרשומה.'); }
    });
}

function resetForm() {
    $('#editColorId').val('');
    $('#colorName').val('');
    $('#colorHex').val('#000000');
    $('#price').val('');
    $('#isInStock').prop('checked', false);
}

function populateForm(row) {
    resetForm();
    $('#formTitle').text('עריכת צבע');
    $('#editColorId').val(row.data('id'));
    $('#colorName').val(row.find('.color-name').text());
    $('#colorHex').val(row.find('.hex-val').data('hex'));
    $('#price').val(parseFloat(row.find('.price').text()));
    $('#isInStock').prop('checked', row.find('.in-stock').data('stock'));
    $('#editFormContainer').slideDown();
}