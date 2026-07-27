using AskalePortal.Constants;
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
        public class AttachedFiles : BaseBLL<AskalePortal.Data.Models.AttachedFile>
        {
            public AttachedFiles(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region Get

            public List<AskalePortal.Data.Models.AttachedFile> Get(int? moduleID,int? targetID, int? createdByUserID)
            {
                var q = dal.Get(k => (k.moduleId == moduleID || moduleID == null) &&
                                     (k.targetId == targetID || targetID == null) &&
                                     (k.createdUserId == createdByUserID || createdByUserID == null) &&
                                     k.enabled == true);
                return q.ToList();
            }

            #endregion Get


            #region AddList

            public async void AddList(List<AskalePortal.Data.Models.AttachedFile> lstFiles)
            {
                lstFiles.ForEach(k => k.enabled = true);

                await dal.AddList(lstFiles);
            }

         

            #endregion AddList
            public List<AttachedFile> GetByModuleID(int moduleId)
            {
                List<AttachedFile> list = dal.Get(k=> k.moduleId == moduleId && k.enabled==true).ToList();
                return list;
            }
        

            public List<AttachedFile> getByModuleIdAndTargetId(int moduleId, int targetId)
            {
                return dal.Get(k => k.enabled == true && k.moduleId == moduleId && k.targetId == targetId).ToList();
            }
        }
    }

    
}
