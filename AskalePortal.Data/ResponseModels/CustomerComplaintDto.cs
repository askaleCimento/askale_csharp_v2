using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class CustomerComplaintDto
    {
       public int? id{get;set;}
       public string? companyName{get;set;}
       public string? categoryName{get;set;}
       public string? customerCode{get;set;}
       public string? customerName{get;set;}
       public string? complaintName{get;set;}
       public string? malzemeTuru{get;set;}
       public double? malzemeMiktari{get;set;}
       public string? aciklama{get;set;}
       public string? musteriTemsilcisi{get;set;}
       public string? musteriTel{get;set;}
       public string? musteriEmail{get;set;}
       public string? olusturmaTarihi{get;set;}
    }
}
