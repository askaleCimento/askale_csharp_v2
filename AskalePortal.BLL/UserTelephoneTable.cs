using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
        public class UserTelephoneTable : BaseBLL<AskalePortal.Data.Models.UserTelephoneTable>
        {

            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            public UserTelephoneTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }

            public PageReturn<UserTelephoneTableDto>? FilterPageableDto(FilterPageParam<PressAnnouncementDtoParameter> filterPageParam)
            {
                PageReturn<UserTelephoneTableDto>? result = new PageReturn<UserTelephoneTableDto>();
                int pageSize = filterPageParam.size ?? 10;
                int pageNumber = filterPageParam.page ?? 0;

                IQueryable<UserTelephoneTableDto> query = dal.dB.AdminUser.Where(k => k.enabled).Select(u => new UserTelephoneTableDto
                {
                    companyName = u.company.vtext,
                    factoryInternal = u.UserTelephoneTable.First().factoryInternal,
                    factoryNumber = u.UserTelephoneTable.First().factoryNumber,
                    id = u.UserTelephoneTable.First().Id,
                    name = u.name,
                    phoneNumber = u.UserTelephoneTable.First().phoneNumber,
                    shortCode = u.UserTelephoneTable.First().shortCode,
                    shortDescription = u.shortDescription,
                    userId = u.Id,


                }).OrderByDescending(u=>u.id);

                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)
                    .ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;


                return result;
            }

            public Data.Models.UserTelephoneTable? getByUserId(int userId)
            {
                Data.Models.UserTelephoneTable? userTelephoneTable = dal.Get(u => u.userId == userId && u.enabled == true).FirstOrDefault();
                return userTelephoneTable;
            }
        }
    }
}
