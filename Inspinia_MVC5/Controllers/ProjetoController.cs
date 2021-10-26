using Dapper;
using Inspinia_MVC5.Dapper;
using Inspinia_MVC5.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Inspinia_MVC5.Controllers
{
   
  
    public class ProjetoController : Controller
    {
        private static readonly Modulo_Model modulo_Model_global = new Modulo_Model();

        // GET: Projeto
        public ActionResult Projetos()
        {
            ListaProjetosParcial("");


            //alimentar o combobox de cliente
            ListaClienteParcial();

            return View();
        }       

        public ActionResult ListaProjetosParcial(string projeto)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Projeto", projeto);
            parameters.Add("@Usu_email", email);
            List<Modulo_Model> result = DapperORM.ReturnList<Modulo_Model>("Stb_Modulo_Acompanhamento", parameters).ToList();
            ViewBag.ListaModulos = result;
            return PartialView("_ListProjetos");
        }



        public ActionResult Projeto_detalhe(int id)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            modulo_Model_global.Mod_id = id;
           
            //usuarios participantes
            DynamicParameters par_mod_usuaarios = new DynamicParameters();
            par_mod_usuaarios.Add("@VloCampo", "Mod_Usuarios");
            par_mod_usuaarios.Add("@VloBusca1", id);
            List<Modulo_Model> result = DapperORM.ReturnList<Modulo_Model>("STb_Modulo_Localizar", par_mod_usuaarios).ToList();
            ViewBag.ListaParticipantes = result;

        
            TarefaViewModel tarefaView = new TarefaViewModel();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Mod_Id");
            parameters.Add("@VloBusca1", id);
            tarefaView.Modulo_M = DapperORM.ReturnList<Modulo_Model>("STb_Modulo_Localizar", parameters).FirstOrDefault<Modulo_Model>();

            ////Lista de tarefas
            DynamicParameters par_demandas = new DynamicParameters();
            par_demandas.Add("@VloCampo", "Tar_Mod_id");
            par_demandas.Add("@VloBusca1", id);
            par_demandas.Add("@VloBusca2", email);
            List<Demandas_Model> result_tarefa = DapperORM.ReturnList<Demandas_Model>("STb_Tarefas_Localizar", par_demandas).ToList();
            ViewBag.ListaTarefas = result_tarefa;

            //Anexo
            DynamicParameters par_anexo = new DynamicParameters();
            par_anexo.Add("@VloCampo", "Mod_Anexo");
            par_anexo.Add("@VloBusca1", id);
            par_anexo.Add("@VloBusca2", "");
            List<Anexo_Tarefa_Model> result_anexo = DapperORM.ReturnList<Anexo_Tarefa_Model>("STb_Anexo_Tarefa_Localizar", par_anexo).ToList();
            ViewBag.ListaAnexo = result_anexo;

            _Usuario_participante("0");


            return View(tarefaView);
        }

       
        public ActionResult _Usuario_participante(string id)
        {

            //Usuário_Tarefa
            DynamicParameters par_usuario_tar = new DynamicParameters();
            par_usuario_tar.Add("@VloCampo", "Tar_Id");
            par_usuario_tar.Add("@VloBusca1", id);
            par_usuario_tar.Add("@VloBusca2", "");
            List<Usuario_Model> result_usuario_tarefa = DapperORM.ReturnList<Usuario_Model>("STb_Usuario_Localizar", par_usuario_tar).ToList();
            ViewBag.UsuarioParticipante = result_usuario_tarefa;
            return PartialView("_Usuario_participante");
        }


        public ActionResult ClienteLista()
        {
   
            //Usuário
            DynamicParameters par_cliente_lista = new DynamicParameters();
            par_cliente_lista.Add("@VloCampo", "TODOS");
            par_cliente_lista.Add("@VloBusca1", "");
            par_cliente_lista.Add("@VloBusca2", "");
            List<Cliente_Model> result_cliente_lista = DapperORM.ReturnList<Cliente_Model>("STb_Cliente_Localizar", par_cliente_lista).ToList();
            ViewBag.clientelista = result_cliente_lista;
            
            return PartialView("_ClienteLista");
        }


        [HttpPost]
        public JsonResult CadastrarCliente(Cliente_Model Modulo)
        {           

            try
            {
                var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Cli_Descricao", Modulo.Cli_Descricao);
                DapperORM.ExecuteWithouReturn("STb_Cliente_Inserir", parameters);          
                return Json(new { erro = false });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
           
        }


        public ActionResult ListaClienteParcial()
        {
            DynamicParameters par_cliente_lista = new DynamicParameters();

            par_cliente_lista.Add("@VloCampo", "TODOS");
            par_cliente_lista.Add("@VloBusca1", "");
            par_cliente_lista.Add("@VloBusca2", "");

            List<Cliente_Model> result_cliente_lista = DapperORM.ReturnList<Cliente_Model>("STb_Cliente_Localizar", par_cliente_lista).ToList();
            ViewBag.clientelista = result_cliente_lista;
            return PartialView("_ListCliente");
        }
    }
}