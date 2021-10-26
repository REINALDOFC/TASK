using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Models
{
    public class Modulo_Model
    {
        public int Mod_id { get; set; }
        public string Mod_Descricao { get; set; }
        public string Mod_dt_criacao { get; set; }
        public string MOd_Usu_Email { get; set; }
        public string ResultadoCampoAutoIncremente { get; set; }
        public int Mod_Cli_id { get; set; }


        public int T_Tarefas { get; set; }
        public string Total_Checked { get; set; }
        public string Foto { get; set; }

        public string Mod_Cliente { get; set; }
        public string Mod_Criador { get; set; }
        public string Mod_Versao { get; set; }
    }
}