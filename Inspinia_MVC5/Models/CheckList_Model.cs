using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Models
{
    public class CheckList_Model
    {
        public int Check_id { get; set; }
        public int Check_Tar_id { get; set; }
        public string Check_Descricao { get; set; }
        public string Total_Checked { get; set; }
        public int Check_Item_id { get; set; }
        public string Check_Item_Descricao { get; set; }
        public string Check_Item_ted { get; set; }
        public string Tar_NSolicitacao { get; set; }
    }
}