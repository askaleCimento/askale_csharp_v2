using AskalePortal.Data.Models;
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
        public class EmailMessagesKurumsal : BaseBLL<AskalePortal.Data.Models.EmailMessageKurumsal>
        {
            public EmailMessagesKurumsal(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region Get

            public List<AskalePortal.Data.Models.EmailMessageKurumsal> GetUnsend()
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

            public async void AddList(List<AskalePortal.Data.Models.EmailMessageKurumsal> lstActionLog)
            {
                lstActionLog.ForEach(k => k.enabled = true);

                await dal.AddList(lstActionLog);
            }

            #endregion AddList

            public override void Delete(int ID)
            {
                DeletePermanently(ID);
            }
        }
    }

    
}
