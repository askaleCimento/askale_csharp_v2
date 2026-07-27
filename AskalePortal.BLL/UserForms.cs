
using AskalePortal.Constants;
using AutoMapper;
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
        public class UserForms : BaseBLL<AskalePortal.Data.Models.UserForm>
        {
            private readonly IWebHostEnvironment _env;
            private readonly IConfiguration _configuration;
            private readonly IMapper _mapper;
            public UserForms(IConfiguration configuration, IWebHostEnvironment env,IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }
            #region GetAll

            public List<AskalePortal.Data.Models.UserForm> GetByUserID(int userID)
            {
                var q = dal.Get(k => k.userId==userID && k.enabled == true).OrderByDescending(k => k.Id);
                return q.ToList();
            }

            public AskalePortal.Data.Models.UserForm GetByUserIDAndFormTypeID(int userID, int formTypeID, string term)
            {
                var q = dal.Get(k => k.userId == userID && k.userFormTypeId == formTypeID && k.term == term && k.enabled == true).OrderByDescending(k => k.Id);
                return q.FirstOrDefault() ?? new AskalePortal.Data.Models.UserForm();
            }

            public List<AskalePortal.Data.Models.UserForm> GetAll(AskalePortal.Data.Models.AdminUser currentUser, int? specificToUserID, int? userFormTypeID, int pageNumber, int pageSize)
            {
                var q = dal.Get(k =>
                 (k.userId == specificToUserID || specificToUserID == null || specificToUserID == 0)
                && (k.userFormTypeId == userFormTypeID || userFormTypeID == null || userFormTypeID == 0)
                && k.enabled == true);

                if(currentUser.roleId != 1) //superadmin
                {
                    BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                    AskalePortal.Data.Models.RoleDetail? rd = bllRoleDetails.GetByRoleIDAndModuleID(currentUser.roleId, (int)CommonConstants.MODULES.PERFORMANCE);

                    if(rd != null)
                    {
                        if(rd.canEdit)
                        {
                            return q.OrderByDescending(k => k.Id).Skip(pageNumber * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            BLLActions.AdminUsers bllUsers = new AdminUsers(_configuration, _env, _mapper);
                            List<int> newList = bllUsers.GetMyPersonelIDList(currentUser.Id);

                            q = q.Where(x => newList.Contains(x.userId)).OrderByDescending(k => k.Id);
                        }
                    }
                    else
                    {
                        return [];
                    }
                }

                return q.OrderByDescending(k => k.Id).Skip(pageNumber * pageSize).Take(pageSize).ToList();
            }

            public List<AskalePortal.Data.Models.UserForm> GetAllForReport(string term, int? specificToUserID, List<int> userFormTypeID, int pageNumber, int pageSize)
            {
                var q = dal.Get(k =>
                (k.term.Contains(term) || string.IsNullOrEmpty(term)) &&
                 (k.userId == specificToUserID || specificToUserID == null || specificToUserID == 0)
                && (userFormTypeID.Contains(k.userFormTypeId))
                && k.enabled == true);

                return q.OrderByDescending(k => k.Id).Skip(pageNumber * pageSize).Take(pageSize).ToList();
            }

            #endregion GetAll
        }
    }
}
