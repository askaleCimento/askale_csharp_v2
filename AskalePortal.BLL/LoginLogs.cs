using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseParams;
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
        public class LoginLogs : BaseBLL<AskalePortal.Data.Models.LoginLog>
        {
            public LoginLogs(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public PageReturn<LoginLogFilterDto>? FilterPageableDto(FilterPageParam<LoginLogDtoRequest> filterPageParam)
            {

                PageReturn<LoginLogFilterDto>? result = new PageReturn<LoginLogFilterDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? username = filterPageParam.liste?.username;

                IQueryable<LoginLog> query = dal.Get(u => u.enabled &&
                (username == null || username == "" ? true : u.username.Contains(username))).OrderByDescending(u => u.Id);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new LoginLogFilterDto()
                    {
                        username = u.username,
                        createdDate = u.createdDate.ToString("dd.MM.yyyy HH:mm:ss"),
                        createdUserId = u.createdUserId,
                        enabled = u.enabled,
                        id = u.Id,
                        iP = u.iP,
                        success = u.isSuccess,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,

                    }).OrderByDescending(u => u.id).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }
            #region GetAllWithPage

            public List<AskalePortal.Data.Models.LoginLog> GetAllWithPage(string username, string password, string ip, DateTime? startDate, DateTime? endDate, bool? isssuccess,
                                                        int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.username.Contains(username) || string.IsNullOrEmpty(username)) &&
                                     (k.password.Contains(password) || string.IsNullOrEmpty(password)) &&
                                     (k.iP.Contains(ip) || string.IsNullOrEmpty(ip)) &&
                                     (k.createdDate >= startDate || startDate == null) &&
                                     (k.createdDate <= endDate || endDate == null) &&
                                     (k.isSuccess == isssuccess || isssuccess == null) &&
                                     k.enabled == true)
                                     .OrderByDescending(k => k.createdDate)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            #endregion GetAllWithPage
        }
    }


}
