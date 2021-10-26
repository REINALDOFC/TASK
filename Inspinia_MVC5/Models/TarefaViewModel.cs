using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Models
{
    public class TarefaViewModel
    {

        public Demandas_Model Demandas_M { get; set; }
        
        public Atividade_Model Atividade_M { get; set;}

        public CheckList_Model CheckList_M { get; set;}

        public Modulo_Model Modulo_M { get; set; }

        public Usuario_Model Usuario_M { get; set; }
    }
}