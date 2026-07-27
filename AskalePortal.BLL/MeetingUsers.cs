using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class MeetingUsers : BaseBLL<AskalePortal.Data.Models.MeetingUser>
        {
            public MeetingUsers(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.MeetingUser> GetAll()
            {
                var q = dal.Get(k =>  k.enabled == true)
                                     .OrderBy(k=>k.dataOrder);
                return q.ToList();
            }

            public List<AskalePortal.Data.Models.MeetingUser> GetForCode()
            {
                var q = dal.Get(k => k.Id != 0)
                                     .OrderBy(k => k.dataOrder);
                return q.ToList();
            }

            public List<MeetingUser> listAllMeetingUser()
            {
                return dal.Get(u=>u.enabled==true ||u.enabled==false).ToList();
            }

            #endregion GetAll

        }
    }
}