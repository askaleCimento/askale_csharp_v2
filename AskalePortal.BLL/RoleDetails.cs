using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class RoleDetails : BaseBLL<Data.Models.RoleDetail>
        {
            private readonly IMapper _mapper;
            public RoleDetails(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _mapper = mapper;
            }
            #region GetByRoleID

            public List<Data.Models.RoleDetail> GetByRoleID(int? roleId)
            {
                var q = dal.Get(k => k.roleId == roleId && k.enabled == true);
                return q.ToList();
            }

            #endregion GetByRoleID

            #region GetByRoleIDAndModuleID

            public Data.Models.RoleDetail? GetByRoleIDAndModuleID(int roleId, int moduleID)
            {
                var q = dal.Get(k => k.roleId == roleId && k.moduleId == moduleID && k.enabled == true);
                return q.FirstOrDefault();
            }

            #endregion GetByRoleIDAndModuleID

            #region AddList

            public void AddList(List<Data.Models.RoleDetail> lstRoleDetail)
            {
                lstRoleDetail.ForEach(k => k.enabled = true);

                dal.AddList(lstRoleDetail);
            }

            #endregion AddList

            #region UpdateList

            public void UpdateList(List<Data.Models.RoleDetail> lstRoleDetail)
            {
                dal.UpdateList(lstRoleDetail);
            }

            public List<RoleDetailSaveDto> getByRoleId(int roleId)
            {
                return dal.Get(u => u.enabled && u.roleId == roleId).Select(u=> new RoleDetailSaveDto() {
                canAdd=u.canAdd,
                canApprove=u.canApprove,
                canDelete=u.canDelete,
                canEdit = u.canEdit,
                canSee = u.canSee,
                canSeeLogs = u.canSeeLogs,
                createdDate=u.createdDate.ToString(),
                createdUserId = u.createdUserId,
                enabled = u.enabled,
                id = u.Id,
                moduleId = u.moduleId,
                roleId  = u.roleId,
                updateDate=u.updatedDate.ToString(),
                updatedUserId = u.updatedUserId,
                }).ToList();
            }

            public List<RoleDetailSaveDto> getByRoleDetailRoleId(int roleId)
            {
                return dal.Get(u => u.enabled && u.roleId == roleId).Select(u => new RoleDetailSaveDto()
                {
                    canAdd = u.canAdd,
                    canApprove = u.canApprove,
                    canDelete = u.canDelete,
                    canEdit = u.canEdit,
                    canSee = u.canSee,
                    canSeeLogs = u.canSeeLogs,
                    createdDate = u.createdDate.ToString(),
                    createdUserId = u.createdUserId,
                    enabled = u.enabled,
                    id = u.Id,
                    moduleId = u.moduleId,
                    roleId = u.roleId,
                    updateDate = u.updatedDate.ToString(),
                    updatedUserId = u.updatedUserId,
                }).ToList();
            }

            public async Task<ActionResult<int>> delete(int moduleId, int roleId)
            {
                RoleDetail? detail = GetByRoleIDAndModuleID(roleId, moduleId);
                if (detail != null)
                {
                    detail.enabled = false;
                    detail.canSee = false; 
                    detail.canAdd = false; 
                    detail.canEdit = false; 
                    detail.canDelete = false; 
                    detail.canApprove = false; 
                    detail.canSeeLogs = false;

                    await Update(detail);
                    return 1;
                }
                return 0;
            }

            public HashSet<RoleDetail> getByModuleId(int moduleId)
            {
                var liste = dal.Get(u => u.enabled == true && u.moduleId == moduleId).ToHashSet();
                return liste;
            }

            #endregion UpdateList
        }
    }

    
}
