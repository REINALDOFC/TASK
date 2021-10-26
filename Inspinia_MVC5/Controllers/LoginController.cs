using Dapper;
using Inspinia_MVC5.Dapper;
using Inspinia_MVC5.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Inspinia_MVC5.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
       
        private static string connectionString = ConfigurationManager.ConnectionStrings["CONEXAO_TASK"].ToString();
        // GET: Login
        public ActionResult Registro()
        {
            return View();
        }

       
        [HttpGet]
        public ActionResult Login()
        {
            
            if (User?.Identity.IsAuthenticated == true)
            {
                var Role = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role).Value;
             
                    return RedirectToAction("projetos", "projeto");
            }
               
            else

                return View();
        }



        [HttpPost]
        public JsonResult ValidarAcesso(Usuario_Model model)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@VloCampo", "Login");
                parameters.Add("@VloBusca1", model.Usu_Email);
                parameters.Add("@VloBusca2", model.Usu_Senha);

                var resultado = DapperORM.ReturnList<Usuario_Model>("STb_Usuario_Localizar", parameters).FirstOrDefault<Usuario_Model>();

                if (resultado != null)
                {
                    var identity = new ClaimsIdentity(new[]
                    {    new Claim(ClaimTypes.Name, resultado.Usu_Nome),
                         new Claim(ClaimTypes.Email, resultado.Usu_Email),
                         new Claim(ClaimTypes.Actor, resultado.Usu_Foto),
                          new Claim(ClaimTypes.Role, resultado.Usu_Nivel_id)

                     }, "ApplicationCookie",  ClaimTypes.Name, ClaimTypes.Role);

                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;

                    authManager.SignIn(identity);
                    return Json(new { error = false, login = true, redirectUrl = Url.Action("projetos", "projeto") }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { error = false, login = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }

        }



        public ActionResult LogOut()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("login", "login");
        }


        [HttpPost]
        public JsonResult Registar(Usuario_Model Modulo)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    var path = Path.Combine(Server.MapPath("~/Images/"));

                    HttpFileCollectionBase file = Request.Files;
                
                      HttpPostedFileBase files = file[0];
                      var foto = files.ContentLength;

                      DynamicParameters par_prazo = new DynamicParameters();
                      par_prazo.Add("@Usu_Nome", Modulo.Usu_Nome);
                      par_prazo.Add("@Usu_Email", Modulo.Usu_Email);
                      par_prazo.Add("@usu_Senha", Modulo.Usu_Senha);
                      par_prazo.Add("@Foto", foto);  
                      par_prazo.Add("@Resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                      sqlCon.Execute("Stb_Usuario_Inserir", par_prazo, commandType: CommandType.StoredProcedure);
                      var retorno = par_prazo.Get<int>("Resultado");

                      if (files.ContentLength != 0)
                      {
                          files.SaveAs(path + "IMG_"+retorno+".jpg");
                      }


                    return Json(new { erro = true, retorno , redirectUrl = Url.Action("Login", "Login") });
                }
               
            }

            catch (Exception ex)
            {
                return Json(new { erro = true });
            }
        }




    }
}