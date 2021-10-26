using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Models
{
    public class Demandas_Model 
    {
        [Display(Name = "Tarefa ID")]
        public int Tar_Id { get; set; }

        [Display(Name = "Modulo ID")]
        public int Tar_Mod_Id { get; set; }
        public string Tar_Mod_Descricao { get; set; }        

        [Display(Name = "Titulo")]
        public string Tar_Titulo { get; set; }
        
        [Display(Name = "Descricao")]
        public string Tar_Descricao { get; set; }
       
        [Display(Name = "Número da SS")]
        public string Tar_NSolicitacao { get; set; }

        [Display(Name = "Data do Cadastro")]
        public string Tar_DTCadastro { get; set; }

        [Display(Name = "Prazo")]
        public string Tar_DTPrazo { get; set; }

        [Display(Name = "Status")]
        public int Tar_Status { get; set; }

        [Display(Name = "Nível")]
        public string Tar_Nivel { get; set; }
        public int Anexo { get; set; }
        public int Atividade { get; set; }

        public string Vencimento { get; set; }

        public string CheckList_total { get; set; }

        public string Sta_Descricao { get; set; }

        public string UltimoComentario { get; set; }

        public int UsuTar_Usu_Id { get; set; }
        public int UsuTar_Tar_id { get; set; }
    }
}