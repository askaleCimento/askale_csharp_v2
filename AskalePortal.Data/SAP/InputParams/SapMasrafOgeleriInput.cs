using AskalePortal.Data.SAP.Models;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class SapMasrafOgeleriInput
    {
        [SapName("LV_BWKEY")]
       public SelectOption[]? lvbwkey {  get; set; }

        [SapName("LV_PTYP")]
        public SelectOption[]? lvptyp { get; set; }

        [SapName("LV_KKZST")]
        public SelectOption[]? lvkkzst { get; set; }

        [SapName("LV_PRTYP")]
        public SelectOption[]? lvprtyp { get; set; }

        [SapName("LV_CATEG")]
        public SelectOption[]? lvcateg { get; set; }


        [SapName("LV_MATKL")]
        public SelectOption[]? lvmatkl { get; set; }


        [SapName("LV_BDATJ")]
        public SelectOption[]? lvbdatj { get; set; }


        [SapName("LV_POPER")]
        public SelectOption[]? lvpoper { get; set; }


        [SapName("LV_CURTP")]
        public string? lvcurtp { get; set; }




    }
}
