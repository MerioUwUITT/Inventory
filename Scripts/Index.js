//generate module to import jquery-confirm library from /scripts folder






document.addEventListener('DOMContentLoaded', function () {
    var nav = document.querySelector('.nav-extended');
    M.Sidenav.init(nav);
    // Inicializar las pestañas
    // Inicializar modales

    hideCategoryTabs();
    hideCategoryContents();

    var modals = document.querySelectorAll('.modal');
    M.Modal.init(modals);

    var checkbox = document.getElementById('perishableCheckbox');
    var datepickerContainer = document.getElementById('datepickerContainer');
    var quantityInput = document.getElementById('itemQuantity');
    var checkbox2 = document.getElementById('editperishableCheckbox');
    var datepickerContainer2 = document.getElementById('editdatepickerContainer');
    var quantityInput2 = document.getElementById('editItemQuantity');

    checkbox.addEventListener('change', function () {
        if (checkbox.checked) {
            datepickerContainer.style.display = 'block';
        } else {
            datepickerContainer.style.display = 'none';
        }
    });
    checkbox2.addEventListener('change', function () {
        if (checkbox2.checked) {
            datepickerContainer2.style.display = 'block';
        } else {
            datepickerContainer2.style.display = 'none';
        }
    });


    // Inicializa el datepicker
    var datepicker = document.getElementById('expirationDate');
    M.Datepicker.init(datepicker, {
        format: 'yyyy-mm-dd'
    });
    var datepicker2 = document.getElementById('editexpirationDate');
    M.Datepicker.init(datepicker2, {
        format: 'yyyy-mm-dd'
    });

    quantityInput.addEventListener('change', function () {
        if (quantityInput.value < 0) {
            jQuery.confirm({
                title: 'Error',
                content: 'La cantidad no puede ser menor que 0.',
                type: 'red',
                buttons: {
                    ok: function () {
                        quantityInput.value = 0;
                    }
                }
            });
        }
    });

    quantityInput2.addEventListener('change', function () {
        if (quantityInput2.value < 0) {
            alert('La cantidad no puede ser menor que 0.');
            quantityInput2.value = 0;
        }
    });

});
function enableEditFields() {
    var enableEditCheckbox = document.getElementById('enableEdit');
    var editLocationNameInput = document.getElementById('editLocationName');
    var editLocationDescriptionInput = document.getElementById('editLocationDescription');
    editLocationNameInput.disabled = !enableEditCheckbox.checked;
    editLocationDescriptionInput.disabled = !enableEditCheckbox.checked;
}
function hideCategoryTabs() {
    // Oculta las pestañas de categorías al iniciar el documento
    var categoryTabs = document.querySelectorAll('.tabs.tabs-transparent.location-tabs');
    categoryTabs.forEach(function (tab) {
        tab.style.display = 'none';
    });
}

function hideCategoryContents() {
    // Oculta los contenidos de categorías al iniciar el documento
    var categoryContents = document.querySelectorAll('[id^="categorycontent_"]');
    categoryContents.forEach(function (content) {
        content.style.display = 'none';
    });
}
function openLocationSettingsModal(name, id, description) {

    document.getElementById('editLocationName').value = name;
    document.getElementById('editLocationDescription').value = description;
    document.getElementById('editLocationId').value = id;
    M.Modal.getInstance(document.getElementById('LocationSettingsModal')).open();
}
function openCategorySettingsModal(locationid, name, id, description) {
    document.getElementById('editCategoryName').value = name;
    document.getElementById('editCategoryDescription').value = description;
    document.getElementById('editCategoryId').value = id;
    M.Modal.getInstance(document.getElementById('CategorySettingsModal')).open();

}
function openItemSettingsModal(name, id, description, quantity, perishable) {
    document.getElementById('editItemName').value = name;
    document.getElementById('editItemDescription').value = description;
    document.getElementById('editItemId').value = id;
    document.getElementById('editItemQuantity').value = quantity;
    document.getElementById('editperishableCheckbox').checked = perishable;
    
    if (perishable) {
        document.getElementById('editdatepickerContainer').style.display = 'block';
        //also change expiration date label to active class
        document.getElementById('editexpirationDateLabel').classList.add('active');

        var duedate = document.getElementById('expirationdate_' + id).value;
        document.getElementById('editdatepicker').value = duedate;
    } else {
        document.getElementById('editdatepickerContainer').style.display = 'none';
    }
    M.Modal.getInstance(document.getElementById('ItemSettingsModal')).open();
}
function openAddCategoryModal(locationId) {
    document.getElementById('addcategorylocationid').value = locationId;
    M.Modal.getInstance(document.getElementById('modalAddCategory')).open();
}
function openAddItemModal(categoryid) {
    document.getElementById('additemcategoryid').value = categoryid;
    M.Modal.getInstance(document.getElementById('modalAddItem')).open();
}

function ClickedNav(id) {
    hideCategoryTabs();
    hideCategoryContents();

    // Checar todos los elementos tablists
    var tabLists = document.querySelectorAll('.tabs.tabs-transparent.location-tabs');
    tabLists.forEach(function (tabList) {
        if (separateIds(id) === separateIds(tabList.id)) {
            tabList.style.display = 'block';
            M.Tabs.init(tabList);
        } else {
            tabList.style.display = 'none';
        }
    });
}

function ClickedTab(id) {
    hideCategoryContents();
    var categoryContents = document.querySelectorAll('[id^="categorycontent_"]');
    categoryContents.forEach(function (content) {
        if (separateIds(id) === separateIds(content.id)) {
            content.style.display = 'block';
            content.classList.add('slide-in');
        } else {
            content.style.display = 'none';
            content.classList.remove('slide-in');
        }
    });
}
function separateIds(id) {
    const parts = id.split('_');
    const name = parts[1].toLowerCase();
    const parsedId = parseInt(parts[2], 10);
    return name + '_' + parsedId;
}
function addLocation() {
    $.ajax({
        url: '/Home/AddLocation',
        type: 'POST',
        data: $('#addLocationForm').serialize(),
        success: function (result) {
            if (result.success) {
                alert(result.message);
                location.reload(true);
                // Puedes agregar lógica adicional después de agregar la ubicación
            } else {
                alert(result.message);
            }
        },
        error: function () {
            alert('Error adding location');
        }
    });
}
function editLocation() {
    // Obtener los datos necesarios desde la modal
    var id = document.getElementById('editLocationId').value;
    var name = document.getElementById('editLocationName').value;
    var description = document.getElementById('editLocationDescription').value;

    // Preparar los datos para enviar mediante AJAX
    var data = {
        id: id,
        name: name,
        description: description
    };

    // Realizar la petición AJAX
    $.ajax({
        url: '/Home/EditLocation',
        type: 'POST',
        data: data,
        success: function (result) {
            if (result.success) {
                alert(result.message);
                location.reload(true);

                // Puedes agregar lógica adicional después de editar la ubicación
            } else {
                alert(result.message);
            }
        },
        error: function () {
            alert('Error editing location');
        }
    });
}

// Agregar categoría
function addCategory() {
    // Obtener el ID de la ubicación desde la pestaña actual
    var locationId = $('.tabs.tabs-transparent.location-tabs:visible').find('input[type=hidden]').val();
    $('#locationId').val(locationId);

    $.ajax({
        url: '/Home/AddCategory',
        type: 'POST',
        data: $('#addCategoryForm').serialize(),
        success: function (result) {
            if (result.success) {
                alert(result.message);
                location.reload(true);

                // Puedes agregar lógica adicional después de agregar la categoría
            } else {
                alert(result.message);
            }
        },
        error: function () {
            alert('Error adding category');
        }
    });

}
function editCategory() {
    // Obtener los datos necesarios desde la modal
    var categoryId = document.getElementById('editCategoryId').value;
    var name = document.getElementById('editCategoryName').value;
    var description = document.getElementById('editCategoryDescription').value;

    // Preparar los datos para enviar mediante AJAX
    var data = {
        categoryId: categoryId,
        name: name,
        description: description
    };
    console.log(data);
    // Realizar la petición AJAX
    $.ajax({
        url: '/Home/EditCategory',
        type: 'POST',
        data: data,
        success: function (result) {
            if (result.success) {
                alert(result.message);
                location.reload(true);

            } else {
                alert(result.message);
            }
        },
        error: function () {
            //say why
            alert('Error editing category');
        }
    });
}
function addItem() {
    // Obtener los datos necesarios desde el formulario
    var categoryId = document.getElementById('additemcategoryid').value;
    var itemName = document.getElementById('itemName').value;
    var itemDescription = document.getElementById('itemDescription').value;
    var quantity = document.getElementById('itemQuantity').value;
    var perishable = document.getElementById('perishableCheckbox').checked;
    var expirationDate = document.getElementById('expirationDate').value;

    // Preparar los datos para enviar mediante AJAX
    var data = {
        categoryId: categoryId,
        itemName: itemName,
        itemDescription: itemDescription,
        quantity: quantity,
        perishable: perishable,
        expirationDate: expirationDate
    };

    // Realizar la petición AJAX
    $.ajax({
        url: '/Home/AddItem',
        type: 'POST',
        data: data,
        success: function (result) {
            if (result.success) {
                alert(result.message);
                location.reload(true);

                // Puedes agregar lógica adicional después de agregar el ítem
            } else {
                alert(result.message);
            }
        },
        error: function () {
            alert('Error adding item');
        }
    });
    


}
function editItem() {
    // Obtener los datos necesarios desde el formulario de edición de ítem
    var itemId = document.getElementById('editItemId').value;
    var itemName = document.getElementById('editItemName').value;
    var itemDescription = document.getElementById('editItemDescription').value;
    var quantity = document.getElementById('editItemQuantity').value;
    var perishable = document.getElementById('editperishableCheckbox').checked;
    var expirationDate = document.getElementById('editdatepicker').value;

    // Preparar los datos para enviar mediante AJAX
    var data = {
        itemId: itemId,
        itemName: itemName,
        itemDescription: itemDescription,
        quantity: quantity,
        perishable: perishable,
        expirationDate: expirationDate
    };

    // Realizar la petición AJAX
    $.ajax({
        url: '/Home/EditItem',
        type: 'POST',
        data: data,
        success: function (result) {
            if (result.success) {
                alert(result.message);
                location.reload(true);

                // Puedes agregar lógica adicional después de editar el ítem
            } else {
                alert(result.message);
            }
        },
        error: function () {
            alert('Error editing item');
        }
    });
}

function handleImageUpload(input, itemId) {
    const file = input.files[0];
    const formData = new FormData();
    formData.append('file', file);
    formData.append('itemId', itemId);

    // Realiza la solicitud AJAX
    $.ajax({
        type: 'POST',
        url: '/Home/UploadImage',
        data: formData,
        contentType: false,
        processData: false,
        success: function (result) {
            location.reload(true);
            console.log(result);
        },
        error: function (error) {
            // Manejar errores si es necesario
            console.error(error);
        }
    });
}
function triggerUpload(button) {
    // Encuentra el input de archivo asociado al botón clicado
    const inputFile = button.querySelector('.upload-input');
    // Simula el clic en el input de archivo
    inputFile.click();
}

//broser confirm called deleteItem that gets the item id andi if confirme calls the ajax function

function deleteItem(itemId) {
    //not ajax just a browser confirm
    if (confirm('Are you sure you want to delete this item?')) {
        $.ajax({
            url: '/Home/DeleteItem',
            type: 'POST',
            data: { itemId: itemId },
            success: function (result) {
                if (result.success) {
                    alert(result.message);
                    location.reload(true);
                    // Puedes agregar lógica adicional después de eliminar el ítem
                } else {
                    alert(result.message);
                }
            },
            error: function () {
                alert('Error deleting item');
            }
        });
    }
}
            
function deleteCategory() {
    //before ajax just a browser confirm
    if (confirm('Are you sure you want to delete this category? This will delete every item on this category!!!')) {
        //get id from modal
        var categoryId = document.getElementById('editCategoryId').value;
        // Realizar la petición AJAX
        $.ajax({
            url: '/Home/DeleteCategory',
            type: 'POST',
            data: { categoryId: categoryId },
            success: function (result) {
                if (result.success) {
                    alert(result.message);
                    location.reload(true);
                    // Puedes agregar lógica adicional después de eliminar la categoría
                } else {
                    alert(result.message);
                }
            },
            error: function () {
                alert('Error deleting category');
            }
        });
    }
}
function deleteLocation() {
    //before ajax just a browser confirm
    if (confirm('Are you sure you want to delete this location? this will delete every category and item related to it!!!')) {
        //get id from modal
        var locationId = document.getElementById('editLocationId').value;
        $.ajax({
            url: '/Home/DeleteLocation',
            type: 'POST',
            data: { locationId: locationId },
            success: function (result) {
                if (result.success) {
                    alert(result.message);
                    location.reload(true);
                    // Puedes agregar lógica adicional después de eliminar la ubicación
                } else {
                    alert(result.message);
                }
            },
            error: function () {
                alert('Error deleting location');
            }
        });
    }
}