using AskalePortal.Data.ResponseModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class EmailMessages : BaseBLL<Data.Models.EmailMessage>
        {
            public EmailMessages(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            #region Get

            public List<AskalePortal.Data.Models.EmailMessage> GetUnsend()
            {
                DateTime dt = DateTime.Now;
                var q = dal.Get(k => (k.isSent == false)  &&
                                     (k.plannedDate < dt) &&                     
                                     k.enabled == true);
                
                return q.ToList();
            }

            public void DeleteNextMessages(int meetingDetailID)
            {
                DateTime dt = DateTime.Now;
                var q = dal.Get(k => (k.isSent == false) &&
                                     k.meetingDetailId == meetingDetailID &&
                                     (k.plannedDate > dt) &&
                                     k.enabled == true).ToList();

                foreach (var item in q)
                {
                    Delete(item.Id);
                }
            }

            #endregion Get

            #region AddList

            public async void AddList(List<AskalePortal.Data.Models.EmailMessage> lstActionLog)
            {
                lstActionLog.ForEach(k => k.enabled = true);

                await dal.AddList(lstActionLog);
            }

            #endregion AddList

            public override void Delete(int ID)
            {
                DeletePermanently(ID);
            }

            public List<Data.Models.EmailMessage> findByEnabledAndSubject(string subject)
            {
                List<Data.Models.EmailMessage>? liste = dal.Get(u=>u.enabled &&u.subject.Contains(subject)&& u.isSent==false).ToList();
                return liste;
            }
        }
    }

    
}
