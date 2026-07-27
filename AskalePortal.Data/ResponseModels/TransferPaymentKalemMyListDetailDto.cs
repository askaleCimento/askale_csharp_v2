using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class TransferPaymentKalemMyListDetailDto
    {
        public int? id{get;set;}
        public string? henum{get;set;}
        public string? posnr{get;set;}
        public string? lifnr{get;set;}
        public string? firma{get;set;}
        public string? wrbtr{get;set;}
        public string? usnam{get;set;}
        public int? currentStateId{get;set;}
        public string? aenam{get;set;}
        public string? cpudt{get;set;}
        public string? iban{get;set;}
        public string? banka{get;set;}
        public string? brnch{get;set;}
        public string? bankn{get;set;}
        public List<ApprovedPerson>? listApprovedPerson { get;set;}
    }
}
