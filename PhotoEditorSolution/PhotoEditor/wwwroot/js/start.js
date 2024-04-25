function readURL(input) {
    if (input.files && input.files[0]) {

        var reader = new FileReader();

        reader.readAsDataURL(input.files[0]);

        $('#imageForm').trigger('submit');
    }
}
