function Func_Crud(url, modulo) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ modulo }),
        url: url,// '@Url.Action("CadastrarModulo", "Painel")',
        success: function (data) {
            if (data.erro) {
                Notiflix.Notify.Failure('Houve um falha');
            } else {
                Notiflix.Notify.Success('Modulo cadastrado com sucesso!');
               // ListaTarefas_load();
            }
        },
        error: function () {
            Notiflix.Notify.Failure('Houve um falha');
        }
    });


}

function ListaTarefas_load(url) {
    $.ajax({
        url: '@Url.Action("DemandaPartial", "Demandas")',
        method: "POST",
        success: function (result) {
            $('#div_partialview').html(result)
        },
        erro: function () {
            alert('Erro')
        }

    });
}