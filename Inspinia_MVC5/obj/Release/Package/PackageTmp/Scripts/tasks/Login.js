$('#btn_login').click(function () {

    $("#form_login").validate();
    var formulario = $("#form_login");

    if (formulario.valid()) {
        preLoad();
        $.ajax({
            type: "POST",
            url: $(this).data('request-url'),
            dataType: "json",
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: new FormData($("#form_login")[0]),
            success: function (data) {

                if (data.login == true)
                    window.location.href = data.redirectUrl;
                else {
                    posLoad();
                    Notiflix.Notify.Failure('Usuário ou senha inválido!');
                }

            },
            error: function () {
                posLoad();
                Notiflix.Notify.Failure('Erro!');
            }
        });
    };

});


//upload foto
$('#MyProfileImageCNH').click(function () {
    $('#fileUploaderControlCNH').click();
    $("#fileUploaderControlCNH").change(function () {
        var formato = this.files[0].type;
        if (formato != "image/jpeg" && formato != "image/jpg") {
            alert('formato')
            return false;
        }
        var file = document.getElementById('fileUploaderControlCNH').files[0];
        var readImg = new FileReader();
        readImg.readAsDataURL(file);
        readImg.onload = function (e) {
            $('#MyProfileImageCNH').attr('src', e.target.result).fadeIn();
        }
    });
});


$('#btn_add').click(function () {
    $('#fileAnexo').click();
});

$('#btn_registrar').click(function () {
    $("#form_registro").validate();
    var formulario = $("#form_registro");
    if (formulario.valid()) {

        $.ajax({
            type: "POST",
            url: $(this).data('request-url'),
            dataType: "json",
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: new FormData($("#form_registro")[0]),
            success: function (data) {
                Notiflix.Report.Success('TASKS', 'Registro salvo com sucesso', 'Ok', function () { window.location.href = data.redirectUrl });
            },
            error: function () {
                Notiflix.Notify.Failure('Houve um falha');
            }
        });
    }
});
