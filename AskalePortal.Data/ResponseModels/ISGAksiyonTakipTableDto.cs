using AskalePortal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class ISGAksiyonTakipTableDto
    {
        public ISGAksiyonTakipTableSaveDto? isgAksiyonTakipTable { get; set; }
        public int? sapBildirim { get; set; }   
    }
}
