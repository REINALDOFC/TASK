using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Models
{
    public class Atividade_Model
    {
        public int HisAti_id { get; set; }
        public int HisAti_Tar_Id { get; set; }
        public string HisAti_Descricao { get; set; }
        public string HisAti_Dt_cadastro { get; set; }
        public string Usu_Foto { get; set; }
        public string Usu_Nome { get; set; }
        public int HisAti_Total_msg { get; set; }


    }
}