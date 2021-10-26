
//Criar Projetio
$('#btn_add_Projeto').click(function () {
    $('#exampleModal').modal('show');
    $("#edit_Modulo").focus();
    $("#div_cadcliente").hide();
    $("#btn_add_cliente").html('Add novo cliente')
})


//add novo cliente
$('#btn_add_cliente').click(function () {

    var visivel = $('#div_cadcliente').is(':visible');
    if (visivel) {

        $("#div_cadcliente").hide();
        $("#btn_add_cliente").html('Add novo cliente');

        //salvando 
        var cli_descricao = $('#edit_cliente').val();

        modulo = { Cli_Descricao: cli_descricao }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ modulo }),
            url: $(this).data('request-url'), //"/projeto/CadastrarCliente",
            success: function (data) {
                if (data.erro) {
                    posLoad();
                    Notiflix.Notify.Failure('Houve um falha');
                } else {
                    posLoad();
                    Notiflix.Notify.Success('Modulo cadastrado com sucesso!');
                    preencher_combobox_cliente();
                }
            },
            error: function () {
                posLoad();
                Notiflix.Notify.Failure('Houve um falha');
            }
        });

    }
    else {
        $("#div_cadcliente").show();
        $("#edit_cliente").focus();
        $("#btn_add_cliente").html('Add');
    }


});

//recolhendo a opção de add cliente
$('#Div_Lista_Cliente').click(function () {
    $("#div_cadcliente").hide();
    $("#btn_add_cliente").html('Add novo cliente');
})

//salvar projeto
function salvar_projeto()
{
    var mod_descricao = $('#edit_Modulo').val();
    var cod_Cliente = $('#combo_list_cliente').val();

    modulo = { Mod_descricao: mod_descricao, Mod_cli_id: cod_Cliente }

    preLoad();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ modulo }),
        url: $(this).data('request-url'),//"/Painel/CadastrarModulo",
        success: function (data) {
            if (data.erro) {
                posLoad();
                Notiflix.Notify.Failure('Houve um falha');
            } else {
                posLoad();
                Notiflix.Notify.Success('Modulo cadastrado com sucesso!');
                $('#exampleModal').modal('hide');
                $('#edit_Modulo').val("");
                pesquisar_projeto()
            }
        },
        error: function () {
            posLoad();
            Notiflix.Notify.Failure('Houve um falha');
        }
    });

}

//localizar projeto
function pesquisar_projeto() {

    var key = event.keyCode || event.charCode;
    if (key == 8)
    {
        if ($('#edit_pesquisar').val().length == 1) {
            $('#edit_pesquisar').val("")
        }
    }

    $.ajax({

        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ projeto: $('#edit_pesquisar').val() }),
        url: "/Projeto/ListaProjetosParcial",// '@Url.Action("ListaProjetosParcial", "Projeto")',

        success: function (result) {
            $('#Div_lista_processo').html(result);
        }
    });


}

function preencher_combobox_cliente()
    {
    //atualizando o combobox de cliente
    $.ajax({

        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Projeto/ListaClienteParcial",// '@Url.Action("ListaProjetosParcial", "Projeto")',

        success: function (result) {
            $('#Div_Lista_Cliente').html(result);
        }
    });
}



 //click buttom reflesh
$('#loading-example-btn').click(function () {
    pesquisar_projeto();
    $('#edit_pesquisar').val("");
    $('#edit_pesquisar').focus();
})


//pesquisar por letra 
var input = document.getElementById('edit_pesquisar');
    input.onkeydown = function () {
        pesquisar_projeto();
}



