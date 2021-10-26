using Dapper;
using Inspinia_MVC5.Dapper;
using Inspinia_MVC5.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Claims;

namespace Inspinia_MVC5.Controllers
{
  
    public class PainelController : Controller
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["CONEXAO_TASK"].ToString();

        // GET: Painel
        public ActionResult Painel()
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "TODOS");
            parameters.Add("@VloBusca1", email);
      
            List<Modulo_Model> result = DapperORM.ReturnList<Modulo_Model>("STb_Modulo_Localizar", parameters).ToList();
            ViewBag.ListaModulo = result;
            return View();
        }


        [HttpPost]
        public JsonResult CadastrarModulo(Modulo_Model Modulo)
        {
            try
            {  
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

                    DynamicParameters parameters = new DynamicParameters();
                    
                    parameters.Add("@Mod_Descricao", Modulo.Mod_Descricao);
                    parameters.Add("@MOd_Usu_Email", email);
                    parameters.Add("@MOd_Cli_id", Modulo.Mod_Cli_id);
                    parameters.Add("@ResultadoCampoAutoIncremente", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    sqlCon.Execute("STb_Modulo_Inserir", parameters, commandType: CommandType.StoredProcedure);
                   
                    int Mod_Id = parameters.Get<int>("ResultadoCampoAutoIncremente");

                    return Json(new { erro = false, Mod_Id });
                }
                
            }
            catch (Exception ex)
            {
                return Json(new { erro = true, mensagem = "Ocorreu um erro ao criar a área." });
            }
        }


        public ActionResult Tarefas()
        {
            return View();
        }

        public ActionResult Projetos()
        {
            return View();
        }

        public ActionResult Projeto_detalhe(int id)
        {
            return View();
        }
    }
}