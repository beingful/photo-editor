function loadImage(formName) {
    var byteCharacters = atob(imageBase64Js);

    var byteNumbers = new Array(byteCharacters.length);

    for (var i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    var byteArray = new Uint8Array(byteNumbers);

    var blob = new Blob([byteArray], { type: 'image/jpeg' });

    var file = new File([blob], "uploadedFile.jpg", { type: 'image/jpeg' });
    var container = new DataTransfer();
    container.items.add(file);

    document.getElementById('fileInput').files = container.files;

    $('#'.concat(formName)).trigger('submit');
}


