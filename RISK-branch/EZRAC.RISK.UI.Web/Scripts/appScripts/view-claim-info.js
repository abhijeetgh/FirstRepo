

var isCollectable = $('#IsCollectable').val();

if (isCollectable.toLowerCase() == 'true') {
    $('#collectable').show();
    $('#non-collectable').hide();
} else {
    $('#collectable').hide();
    $('#non-collectable').show();
}

