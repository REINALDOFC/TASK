using Dapper;
using Inspinia_MVC5.Dapper;
using Inspinia_MVC5.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Inspinia_MVC5.Controllers
{  

    public class DemandasController : Controller
    {
        private static readonly Modulo_Model modulo_Model_global = new Modulo_Model();
        private static string connectionString = ConfigurationManager.ConnectionStrings["CONEXAO_TASK"].ToString();
        // GET: Demandas
        public ActionResult Demandas(int id, string nome)
        {
            Demandas_Model demandas = new Demandas_Model();
            modulo_Model_global.Mod_id = id;
            demandas.Tar_Mod_Id = id;
            ViewBag.NomeModulo = nome;
            return View(demandas);
        }

        public ActionResult DemandaPartial()
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Tar_Mod_id");
            parameters.Add("@VloBusca1", modulo_Model_global.Mod_id);
            parameters.Add("@VloBusca2", email);

            List<Demandas_Model> result = DapperORM.ReturnList<Demandas_Model>("STb_Tarefas_Localizar", parameters).ToList();
            ViewBag.ListaDemandas = result;
            return PartialView("DemandaPartialview");
         }



        //Melhorar!
        //passar o form para Parcial
        public ActionResult Cadastrartarefas(string id, string Mod_id)
        {
            if(Mod_id != "")
              modulo_Model_global.Mod_id = Convert.ToInt32(Mod_id);
            
 
            TarefaViewModel tarefa_model = new TarefaViewModel();
            Demandas_Model dm = new Demandas_Model();
   

            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;      

            //tarefas
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Tar_NSolicitacao");
            parameters.Add("@VloBusca1", id);
            parameters.Add("@VloBusca2", email);
            dm = DapperORM.ReturnList<Demandas_Model>("STb_Tarefas_Localizar", parameters).FirstOrDefault<Demandas_Model>();

            tarefa_model.Demandas_M = dm;

            CadastroTarefaParcial(id);
            HistoricoAtividadeParcial(id);
            CheckListParcial(id);
            CheckList_Item_Parcial(id);
            AnexoParcial(id);
            UsuarioParticipanteParcial(id);
            UsuarioListaParcial(id);

            return View(tarefa_model);
        }

        [HttpPost]
        public JsonResult SalvarTarefa(TarefaViewModel Modulo)
        {
            try
            {
                var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    var Tar_NSolicitacao = "";
                    int Tar_id = 0;

                    switch (Modulo.Demandas_M.Tar_Id)
                    {
                        case 0:                         
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@Tar_Mod_id", modulo_Model_global.Mod_id);
                            parameters.Add("@Tar_Titulo", Modulo.Demandas_M.Tar_Titulo);
                            parameters.Add("@Tar_Descricao", Modulo.Demandas_M.Tar_Descricao);
                            parameters.Add("@Tar_DTPrazo", Modulo.Demandas_M.Tar_DTPrazo);
                            parameters.Add("@Tar_Nivel", Modulo.Demandas_M.Tar_Nivel);
                            parameters.Add("@Usu_Email", email);

                            parameters.Add("@Tar_NSolicitacao", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);
                            parameters.Add("@Tar_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                            
                            sqlCon.Execute("STb_Tarefas_Inserir", parameters, commandType: CommandType.StoredProcedure);

                            Tar_NSolicitacao = parameters.Get<string>("Tar_NSolicitacao");
                            Tar_id = parameters.Get<int>("Tar_id");
                            break;
                        default:
                            DynamicParameters par_update = new DynamicParameters();
                            par_update.Add("@Tar_Id", Modulo.Demandas_M.Tar_Id);
                            par_update.Add("@Tar_Titulo", Modulo.Demandas_M.Tar_Titulo);
                            par_update.Add("@Tar_Descricao", Modulo.Demandas_M.Tar_Descricao);
                            sqlCon.Execute("STb_Tarefas_Alterar", par_update, commandType: CommandType.StoredProcedure);

                            Tar_NSolicitacao = Modulo.Demandas_M.Tar_NSolicitacao;
                            Tar_id = Modulo.Demandas_M.Tar_Id; 

                            break;
                    }                    
                    return Json(new { erro = false, tar_NSolicitacao = Tar_NSolicitacao, tar_id= Tar_id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { erro = true});
            }
        }

        [HttpPost]
        public JsonResult SalvarAtividade(Atividade_Model Modulo)
        {
            try
            {
                var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@HisAti_Descricao", Modulo.HisAti_Descricao);
                parameters.Add("@HisAti_Tar_Id", Modulo.HisAti_Tar_Id);
                parameters.Add("@Usu_email", email);
                DapperORM.ExecuteWithouReturn("STb_Historico_Atividade_Inserir", parameters);

                return Json(new { erro = false });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        [HttpPost]
        public JsonResult AddCheckList(CheckList_Model Modulo)
        {
           
            try
            {
                DynamicParameters par_Add_CheckLista = new DynamicParameters();
                par_Add_CheckLista.Add("@Check_Tar_id", Modulo.Check_Tar_id);
                par_Add_CheckLista.Add("@Check_Descricao", Modulo.Check_Descricao);
                DapperORM.ExecuteWithouReturn("STb_Checklist_Inserir", par_Add_CheckLista);
                
                return Json(new { erro = false });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        [HttpPost]
        public JsonResult DeleteCheckList(CheckList_Model Modulo)
        {
            try
            {
                DynamicParameters par_Add_CheckLista = new DynamicParameters();
                par_Add_CheckLista.Add("@Check_id", Modulo.Check_Tar_id);
                DapperORM.ExecuteWithouReturn("STb_Checklist_Delete", par_Add_CheckLista);

                return Json(new { erro = false });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        [HttpPost]
        public JsonResult Add_ItemCheckList(CheckList_Model Modulo)
        {
            try
            {
                DynamicParameters par_add_CheckLista = new DynamicParameters();
                par_add_CheckLista.Add("@Check_Tar_id", Modulo.Check_Tar_id);
                par_add_CheckLista.Add("@Check_Item_Descricao", Modulo.Check_Item_Descricao);
                DapperORM.ExecuteWithouReturn("STb_CheckList_Item_Inserir", par_add_CheckLista);
                
                //CheckListaItem
                DynamicParameters parCheckLista = new DynamicParameters();
                parCheckLista.Add("@VloCampo", "Tar_id_item");
                parCheckLista.Add("@VloBusca1", Modulo.Check_Tar_id);
                parCheckLista.Add("@VloBusca2", "");
                List<CheckList_Model> resultCheckitem = DapperORM.ReturnList<CheckList_Model>("STb_CheckList_Localizar", parCheckLista).ToList();
                ViewBag.checkListItem = resultCheckitem;

                return Json(new { erro = false, resultCheckitem });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        [HttpPost]
        public JsonResult DeleteItemCheckList(CheckList_Model Modulo)
        {
            try
            {
                DynamicParameters par_add_CheckLista = new DynamicParameters();
                par_add_CheckLista.Add("@Check_Item_id", Modulo.Check_Item_id);
                DapperORM.ExecuteWithouReturn("STb_Delete_Check_Item_Deletar", par_add_CheckLista);

                //CheckListaItem
                DynamicParameters parCheckLista = new DynamicParameters();
                parCheckLista.Add("@VloCampo", "Tar_id_item");
                parCheckLista.Add("@VloBusca1", Modulo.Check_Tar_id);
                parCheckLista.Add("@VloBusca2", "");
                List<CheckList_Model> resultCheckitem = DapperORM.ReturnList<CheckList_Model>("STb_CheckList_Localizar", parCheckLista).ToList();

                return Json(new { erro = false, resultCheckitem });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        [HttpPost]
        public JsonResult Checkted(CheckList_Model Modulo)
        {
            try
            {
                DynamicParameters par_Checkted = new DynamicParameters();
                par_Checkted.Add("@Check_Item_id", Modulo.Check_Item_id);
                par_Checkted.Add("@Check_Item_ted", Modulo.Check_Item_ted);
                par_Checkted.Add("@Tar_id", Modulo.Check_Tar_id);
                DapperORM.ExecuteWithouReturn("STb_Checklist_item_Alterar", par_Checkted);

               
                return Json(new { erro = false  });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        [HttpPost]
        public JsonResult AtualizarPrazo(Demandas_Model Modulo)
        {
            var data = DateTime.ParseExact(Modulo.Tar_DTPrazo, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
            try
            {
                DynamicParameters par_prazo = new DynamicParameters();
                par_prazo.Add("@Tar_Id", Modulo.Tar_Id);
                par_prazo.Add("@Tar_DTPrazo", data);
                DapperORM.ExecuteWithouReturn("STb_Prazo_Atualizar", par_prazo);

                return Json(new { erro = false });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        [HttpPost]
        public JsonResult AddUsuario(Usuario_Model Modulo)
        {
            try
            {
                DynamicParameters par_usuario = new DynamicParameters();
                par_usuario.Add("@Tar_Id", Modulo.UsuTar_Tar_id);
                par_usuario.Add("@Usu_Nome", Modulo.Usu_Nome);

                DapperORM.ExecuteWithouReturn("Stb_Usuario_Tarefa_Inserir", par_usuario);

                return Json(new { erro = false });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        [HttpPost]
        public JsonResult Movimentacao(Movimentacao_Model Modulo )
        {
            try
            {
                DynamicParameters par_usuario = new DynamicParameters();
                par_usuario.Add("@Coluna", Modulo.Coluna);
                par_usuario.Add("@Tar_NSolicitacao", Modulo.Tar_NSolicitacao);

                DapperORM.ExecuteWithouReturn("Stb_Tarefas_Movimentacao", par_usuario);

                return Json(new { erro = false });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        [HttpPost]
        public JsonResult AnexarDocumento()
        {
            try
            {
                var NumeroSS = Request.Params["NumeroSS"];
                var path = Path.Combine(Server.MapPath("~/Anexo/"+ NumeroSS+"/"));

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                HttpFileCollectionBase file = Request.Files;

                for (int i = 0; i < file.Count; i++)
                {
                    //string Nome = Request.Files.AllKeys[i];

                    HttpPostedFileBase files = file[i];

                    if (files.ContentLength != 0)
                    {
                        var NomeDoc = files.FileName;

                        DynamicParameters par_Checkted = new DynamicParameters();
                        par_Checkted.Add("@Tar_NSolicitacao", NumeroSS);
                        par_Checkted.Add("@Ane_Nome", NomeDoc);
                        DapperORM.ExecuteWithouReturn("STb_Anexo_Tarefa_Inserir", par_Checkted);

                        files.SaveAs(path + NomeDoc);
                    }
                }

                return Json(new { erro = false });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        [HttpPost]
        public JsonResult AtualizarNivel(Demandas_Model Modulo)
        {
           
            try
            {
                DynamicParameters par_prazo = new DynamicParameters();
                par_prazo.Add("@Tar_Id", Modulo.Tar_Id);
                par_prazo.Add("@Tar_Nivel", Modulo.Tar_Nivel);
                DapperORM.ExecuteWithouReturn("STb_Nivel_Atualizar", par_prazo);

                return Json(new { erro = false });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        public ActionResult CadastroTarefaParcial(string id)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            //tarefas
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Tar_NSolicitacao");
            parameters.Add("@VloBusca1", id);
            parameters.Add("@VloBusca2", email);
            Demandas_Model dm = DapperORM.ReturnList<Demandas_Model>("STb_Tarefas_Localizar", parameters).FirstOrDefault<Demandas_Model>();

            TarefaViewModel tarefa_model = new TarefaViewModel();
            tarefa_model.Demandas_M = dm;
            return PartialView("_NumeroDemana",tarefa_model);
        }

        public ActionResult HistoricoAtividadeParcial(string id)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            //tarefas
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Tar_NSolicitacao");
            parameters.Add("@VloBusca1", id);
            parameters.Add("@VloBusca2", email);
            List<Atividade_Model> resultAtividade = DapperORM.ReturnList<Atividade_Model>("STb_Historico_Atividade_Localizar", parameters).ToList();
            ViewBag.ListaAtividades = resultAtividade;
            
            if(resultAtividade.Count != 0)
            ViewBag.TotalMesagem = resultAtividade[0].HisAti_Total_msg;

            return PartialView("_HistoricoAtividade");
        }

        public ActionResult CheckListParcial(string id)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            //checklist
            DynamicParameters parCheckLista = new DynamicParameters();
            parCheckLista.Add("@VloCampo", "Check_Tar_id");
            parCheckLista.Add("@VloBusca1", id);
            parCheckLista.Add("@VloBusca2", "");
            List<CheckList_Model> resultchk = DapperORM.ReturnList<CheckList_Model>("STb_CheckList_Localizar", parCheckLista).ToList();
            ViewBag.checkList = resultchk;

            return PartialView("_CheckList");
        }

        public ActionResult CheckList_Item_Parcial(string id)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            //CheckListaItem
            DynamicParameters parCheckListaItem = new DynamicParameters();
            parCheckListaItem.Add("@VloCampo", "Check_Tar_id_item");
            parCheckListaItem.Add("@VloBusca1", id);
            parCheckListaItem.Add("@VloBusca2", "");
            List<CheckList_Model> resultCheckitem = DapperORM.ReturnList<CheckList_Model>("STb_CheckList_Localizar", parCheckListaItem).ToList();
            ViewBag.checkListItem = resultCheckitem;

            return PartialView("_CheckList_Item");
        }

        public ActionResult AnexoParcial(string id)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            
            DynamicParameters par = new DynamicParameters();
            par.Add("@VloCampo", "Tar_NSolicitacao");
            par.Add("@VloBusca1", id);
            par.Add("@VloBusca2", "");
            List<Anexo_Tarefa_Model> result_Anexo = DapperORM.ReturnList<Anexo_Tarefa_Model>("STb_Anexo_Tarefa_Localizar", par).ToList();
            ViewBag.anexo = result_Anexo;

            return PartialView("_Anexo");
        }

        public JsonResult DeleteAnexo(Anexo_Tarefa_Model Modulo)
        {

            try
            {
                DynamicParameters par_anexo = new DynamicParameters();
                par_anexo.Add("@Ane_id", Modulo.Ane_id);
                DapperORM.ExecuteWithouReturn("STb_anexo_tarefa_deletar", par_anexo);

                return Json(new { erro = false });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }

        


        public ActionResult UsuarioParticipanteParcial(string id)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            //Usuário_Tarefa
            DynamicParameters par_usuario_tar = new DynamicParameters();
            par_usuario_tar.Add("@VloCampo", "Tar_NSolicitacao");
            par_usuario_tar.Add("@VloBusca1", id);
            par_usuario_tar.Add("@VloBusca2", "");
            List<Usuario_Model> result_usuario_tarefa = DapperORM.ReturnList<Usuario_Model>("STb_Usuario_Localizar", par_usuario_tar).ToList();
            ViewBag.usuario = result_usuario_tarefa;
            return PartialView("_Usuario");
        }

        public ActionResult UsuarioListaParcial(string id)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            //Usuário
            DynamicParameters par_usuario_lista = new DynamicParameters();
            par_usuario_lista.Add("@VloCampo", "TODOS");
            par_usuario_lista.Add("@VloBusca1", id);
            par_usuario_lista.Add("@VloBusca2", "");
            List<Usuario_Model> result_usuario_lista = DapperORM.ReturnList<Usuario_Model>("STb_Usuario_Localizar", par_usuario_lista).ToList();
            ViewBag.usuariolista = result_usuario_lista;
            return PartialView("_UsuarioLista");
        }

        //USUARIO TAREFAS
        public ActionResult Demandas_Usuario(string id)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            //tarefas
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Usuarios_Tarefas");
            parameters.Add("@VloBusca1", id);
            parameters.Add("@VloBusca2", email);
            List<Demandas_Model> result_tarefa = DapperORM.ReturnList<Demandas_Model>("STb_Tarefas_Localizar", parameters).ToList();

            //if (result_tarefa[0].Tar_Id == 0)
            //    ViewBag.ListaTarefas = new List<Demandas_Model>();
            //else
            //    ViewBag.ListaTarefas = result_tarefa;

            DemandausuarioParcial(id);
            Usuario_Model us = DapperORM.ReturnList<Usuario_Model>("STb_Tarefas_Localizar", parameters).FirstOrDefault<Usuario_Model>();
            Modulo_Model md = DapperORM.ReturnList<Modulo_Model>("STb_Tarefas_Localizar", parameters).FirstOrDefault<Modulo_Model>();

            TarefaViewModel tarefa_model = new TarefaViewModel();
            tarefa_model.Usuario_M = us;
            tarefa_model.Modulo_M = md;



            return View(tarefa_model);
        }
              
        public ActionResult DemandausuarioParcial(string id)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Usuarios_Tarefas");
            parameters.Add("@VloBusca1", id);
            parameters.Add("@VloBusca2", email);
            List<Demandas_Model> result_tarefa = DapperORM.ReturnList<Demandas_Model>("STb_Tarefas_Localizar", parameters).ToList();

            if (result_tarefa[0].Tar_Id == 0)
                ViewBag.ListaTarefas = new List<Demandas_Model>();
            else
                ViewBag.ListaTarefas = result_tarefa;


            return PartialView("_demandas_usuario");
        }

        public ActionResult Download(string ext, string NumeroSS)
        {

            var numeroSS = NumeroSS;
            var path = Path.Combine(Server.MapPath("~/Anexo/" + NumeroSS + "/"));
           
            var contentType = "";

            var caminhoArquivo = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + NumeroSS + "\\" + ext;
            var extensao = Path.GetExtension(caminhoArquivo);
            var nomeArquivoV = ext;
            if (extensao.Equals(".pdf") || extensao.Equals(".PDF"))
                contentType = "application/pdf";
            if (extensao.Equals(".jpeg") || extensao.Equals(".JPEG"))
                contentType = "application/image";

            if (System.IO.File.Exists(caminhoArquivo))
            {
                return File(caminhoArquivo, contentType, nomeArquivoV);
            }

            return File("", "", "");
        }
       
        [HttpPost]
        public JsonResult Disvincular(Usuario_Model modulo)
        {
            try
            {
                DynamicParameters par_disvincular = new DynamicParameters();
                par_disvincular.Add("@UsuTar_Tar_id", modulo.UsuTar_Tar_id);
                par_disvincular.Add("@UsuTar_Usu_id", modulo.UsuTar_Usu_id);
                DapperORM.ExecuteWithouReturn("STb_Disvincular_Usuario_Tarefa", par_disvincular);

                return Json(new { erro = false });
            }
            catch (Exception)
            {

                return Json(new { erro = true });
                throw;
            }

        }




    }
}