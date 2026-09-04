using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ISGUygunsuzlukKaynagiTable : BaseBLL<AskalePortal.Data.Models.ISGUygunsuzlukKaynagiTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public ISGUygunsuzlukKaynagiTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public List<Data.Models.ISGUygunsuzlukKaynagiTable> findAllByEnabled(bool enabled, int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(userId);
                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? rd = bllRoleDetails.GetByRoleIDAndModuleID(user.roleId,(int)CommonConstants.MODULES.MAVI_YAKA);

                if (rd == null || (rd != null && !rd.canSee))
                {
                    return dal.Get(u => u.enabled == enabled).ToList();
                }
                return dal.Get(u=>u.enabled==enabled&& u.maviYakaMi==true).ToList();
            }

            public List<AskalePortal.Data.Models.ISGUygunsuzlukKaynagiTable> GetAllByMaviYaka()
            {
                return dal.Get(u => u.maviYakaMi == true && u.enabled == true).ToList();
            }
        }
    }
}
