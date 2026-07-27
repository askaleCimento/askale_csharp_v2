using AskalePortal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class ActiveProcessDto
    {
       public int? id{get;set;}
       public bool? enabled{get;set;}
       public ProcessState? currentState{get;set;}
       public AdminUser? currentUser{get;set;}
       public AdminUser? userVekalet{get;set;}
       public ApprovalProcess? approvalProcess{get;set;}
       public List<ActiveProcessDetail>? listActiveProcessDetail{get;set;}
       public string? dagitimKanali{get;set;}
       public string? relatedData{get;set;}
       public string? relatedDataId{get;set;}
       public string? relatedDataDesc{get;set;}
       public string? relatedDataPrimary{get;set;}
       public string? relatedDataPrimaryId{get;set;}
       public string? relatedDataPrimaryDesc{get;set;}
       public string? relatedColumn{get;set;}
       public string? dataType{get;set;}
       public string? oldValue{get;set;}
       public string? newValue{get;set;}
       public string? description{get;set;}
       public string? customFields{get;set;}
       public string? disaprovecondition{get;set;}
       public int? oncekiArtirim{get;set;}
       public DateTime? createdDate{get;set;}
       public int? createdUserId{get;set;}
       public string? belgeTutari{get;set;}
       public double? avgDays{get;set;}
       public double? avgVade{get;set;}
    }
}
