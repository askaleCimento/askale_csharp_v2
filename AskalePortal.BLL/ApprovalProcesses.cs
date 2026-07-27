
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ApprovalProcesses : BaseBLL<AskalePortal.Data.Models.ApprovalProcess>
        {

            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public ApprovalProcesses(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public ApprovalProcess findByCompanyIdAndTypeIdAndEnabled(int companyId, int typeId)
            {
                return dal.Get(u => u.enabled && companyId == u.companyId && typeId == u.typeId).FirstOrDefault() ?? new ApprovalProcess();
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.ApprovalProcess> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.type.title);

                return q.ToList();
            }

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.ApprovalProcess> GetAllWithPage(string searchQuery, int activePage, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.type.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.type.title).Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();

                return q;
            }

            #endregion GetAllWithPage

            public AskalePortal.Data.Models.ApprovalProcess GetRelatedProcess(int companyID, int processTypeID, string dagitimKanali)
            {
                var q = dal.Get(k => k.companyId == companyID && k.typeId == processTypeID && k.dagitimKanali == dagitimKanali && k.enabled == true).FirstOrDefault();
                return q;
            }

            public List<ApprovalProcessSaveDto> listAllByEnabled(bool enabled)
            {
                return dal.Get(u => u.enabled == enabled).Select(u => new ApprovalProcessSaveDto()
                {
                    enabled = u.enabled,
                    companyId = u.companyId,
                    createdDate = (u.createdDate ?? DateTime.Now).ToString("dd.MM.yyyy"),
                    createdUserId = u.createdUserId,
                    dagitimKanali = u.dagitimKanali,
                    description = u.description,
                    id = u.Id,
                    typeId = u.typeId,
                    updateDate = (u.updatedDate ?? DateTime.Now).ToString("dd.MM.yyyy"),
                    updatedUserId = u.updatedUserId
                }).OrderByDescending(u=>u.id).ToList();
            }

            internal ApprovalProcess? findByCompanyIdAndTypeIdAndDagitimKanaliAndEnabled(int companyId, int processTypeId, string dagitimkanali, bool enabled)
            {
                return dal.Get(u => u.enabled == enabled && u.typeId == processTypeId
                && u.dagitimKanali.Contains(dagitimkanali) && u.companyId == companyId).FirstOrDefault();
            }

            internal AdminUser? GetNextUser(int currentUserId, int processId, bool enabled)
            {
                BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env,_mapper);
                ApprovalProcessDetail? approvalProcessDetail = bllApprovalProcessDetails
                .findByUserIdAndProcessIdAndEnabled(currentUserId, processId, enabled);

                int newDataOrder = (approvalProcessDetail?.dataOrder ?? 0) + 1;
                var user = (
    from a in dal.dB.AdminUser
    join b in dal.dB.ApprovalProcessDetail
        on a.Id equals b.userId
    where
        b.processId == processId &&
        b.dataOrder == newDataOrder &&
        a.enabled == enabled &&
        b.enabled == enabled
    select a
).FirstOrDefault();
                return user;

            }
        }
    }
}