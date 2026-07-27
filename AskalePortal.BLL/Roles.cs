using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.SAP.InputParams;
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
        public class Roles : BaseBLL<AskalePortal.Data.Models.Role>
        {
            private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public Roles(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }
            #region GetAll

            public List<AskalePortal.Data.Models.Role> GetAll(bool? approval)
            {
                var q = dal.Get(k => (k.approval == approval || approval == null) &&
                                     k.enabled == true);
                return q.ToList();
            }

            public List<RoleDto> getAllFilter(FilterParam<RoleListParameter> filterParam)
            {
                string? title = filterParam?.liste?.title;
                string? description = filterParam?.liste?.description;
                List<RoleDto> liste = dal.Get(u => u.enabled == true && (title != null ? u.title.Contains(title) : true) &&
                (description != null ? u.description.Contains(description) : true)).OrderByDescending(u => u.Id).Select(u => new RoleDto()
                {
                    title = u.title,
                    description = u.description,
                    approval = u.approval,
                    companies = u.companies,
                    createdDate = u.createdDate,
                    createdUserId = u.createdUserId,
                    enabled = u.enabled,
                    Id = u.Id,
                    updatedDate = u.updatedDate,
                    updatedUserId = u.updatedUserId,
                }).ToList();
                return liste;
            }

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.Role> GetAllWithPage(string searchQuery, bool? approval, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || k.description.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     (k.approval == approval || approval == null) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            public List<IdandText> GetIdandText(int userId)
            {
                BLLActions.AdminUsers adminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                Data.Models.AdminUser? adminUser = adminUsers.GetByID(userId);
                if (adminUser?.roleId == 1)
                {
                    return dal.Get(u => u.enabled).Select(u => new IdandText() { id = u.Id, text = u.title }).ToList();
                }
                else
                {
                    return dal.Get(u => u.enabled && u.Id != 1).Select(u => new IdandText() { id = u.Id, text = u.title }).ToList();

                }
            }

            #endregion GetAllWithPage
       
        
        
        }
    }


}
