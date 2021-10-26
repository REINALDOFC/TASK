using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Models
{
    public class Usuario_Model
    {
        public int Usu_id { get; set; }
        public string Usu_Nome { get; set; }
        public string Usu_Foto { get; set; }
        public string Usu_Dt_Cadastro { get; set; }
        public string Usu_Email { get; set; }
        
        [DataType(DataType.Password)]
        public string Usu_Senha { get; set; }
        public string Usu_Nivel_id { get; set; }

        public int UsuTar_Tar_id { get; set; }
        public int UsuTar_Usu_id { get; set; }

        public string ReturnUrl { get; set; }

    }
}