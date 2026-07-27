using AskalePortal.Data.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class ResponseMyList
    {
        public InternalCorrespondenceSaveDto? dahiliYazismaTable { get; set; }
        public string? kanalGorusuFirst { get; set; }
    }
}
