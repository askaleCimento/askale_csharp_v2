using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
        public class ApprovalProcessDetails : BaseBLL<AskalePortal.Data.Models.ApprovalProcessDetail>
        {
            private readonly IMapper _mapper;
            public ApprovalProcessDetails(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _mapper = mapper;
            }
            #region GetAll

            public async Task<List<AskalePortal.Data.Models.ApprovalProcessDetail>> GetAll(int processId)
            {
                List<AskalePortal.Data.Models.ApprovalProcessDetail> list = dal.Get(k => k.processId == processId && k.enabled == true).OrderBy(k => k.dataOrder).ToList();
                for (int i = 0; i < list.Count; i++)
                {

                    ApprovalProcessDetail approvalProcessDetail = list[i];
                    approvalProcessDetail.dataOrder = (i + 1);
                    await Update(approvalProcessDetail);
                }
                return list;
            }

            public int? GetFirstUser(int processId)
            {
                var q = dal.Get(k => k.processId == processId && k.enabled == true).OrderBy(k => k.dataOrder).FirstOrDefault();
                if (q == null)
                    return null;
                else
                    return q.userId;
            }

            public int? GetNextUserId(int currentUserId, int processId)
            {
                var q = dal.Get(k => k.processId == processId && k.enabled == true).OrderBy(k => k.dataOrder).ToList();
                var currentUsersOrder = q.Where(c => c.userId == currentUserId).FirstOrDefault();
                var nextUser = q.Where(c => c.dataOrder > currentUsersOrder?.dataOrder).OrderBy(k => k.dataOrder).FirstOrDefault();

                if (nextUser == null)
                    return null;
                else
                    return nextUser.userId;
            }
            public int? GetLastUser(int dataOrder, int processId)
            {
                var nextUser = dal.Get(k => k.processId == processId && k.enabled == true && k.dataOrder == dataOrder).OrderBy(k => k.dataOrder).FirstOrDefault();
                return nextUser?.userId;
            }
            public List<AskalePortal.Data.Models.ApprovalProcessDetail> GetAll(int processId, int userId)
            {
                var q = dal.Get(k => k.processId == processId && k.userId == userId && k.enabled == true).OrderBy(k => k.dataOrder);

                return q.ToList();
            }

            public int GetUser(int companyId, int typeId)
            {

                return dal.Get(u => u.process.companyId == companyId && u.process.typeId == typeId && u.enabled == true).FirstOrDefault()!.userId;
            }

            public List<ApprovalProcessDetail> GetByUserId(int vekaletverenId)
            {
                return dal.Get(u => u.userId == vekaletverenId && u.enabled == true).ToList();
            }

            public int? GetNextUserId(int currentUserId, int processId, decimal newvalue)
            {
                var q = dal.Get(k => k.processId == processId && k.deger <= newvalue && k.enabled == true).OrderBy(k => k.dataOrder).ToList();
                var currentUsersOrder = q.Where(c => c.userId == currentUserId).FirstOrDefault();
                var nextUser = q.Where(c => c.dataOrder > currentUsersOrder?.dataOrder).OrderBy(k => k.dataOrder).FirstOrDefault();

                if (nextUser == null)
                    return null;
                else
                    return nextUser.userId;
            }

            public List<ApprovalProcessDetail> findByProcessIdAndEnabled(int processId, bool v)
            {
                throw new NotImplementedException();
            }

            public List<ApprovalProcessDetail> findByProcessIdAndEnabledOrderByDataOrderAsc(int processId)
            {
                return dal.Get(u => u.enabled && u.processId == processId).OrderBy(k => k.dataOrder).ToList() ?? [];
            }

            public AdminUser? GetNextUser(int? currentUserId, int? processId, bool enabled)
            {
                ApprovalProcessDetail approvalProcessDetail = dal.Get(u => u.userId == currentUserId && u.processId == processId && u.enabled == enabled).First();
                int newDataOrder = approvalProcessDetail.dataOrder + 1;

                AdminUser? nextUser = nextUser = dal.Get(b =>
        b.processId == processId &&
        b.dataOrder == newDataOrder &&
        b.enabled == enabled &&
        b.user.enabled == enabled
    )
    .Select(b => b.user)
    .FirstOrDefault();

                return nextUser;
            }

            public ApprovalProcessDetail? findByUserIdAndProcessIdAndEnabled(int currentUserId, int processId, bool enabled)
            {
                return dal.Get(u => u.userId == currentUserId && u.processId == processId && u.enabled == enabled).FirstOrDefault();
            }

            public ApprovalProcessDetail? findByProcessIdAndDataOrderAndEnabled(int processId, int dataOrder, bool enabled)
            {
                return dal.Get(u => u.processId == processId && u.dataOrder == dataOrder && u.enabled == enabled).FirstOrDefault();
            }

            internal ApprovalProcessDetail? findByProcessIdAndUserIdAndEnabled(int approvalProcessId, int userId, bool enabled)
            {
                return dal.Get(u => u.processId == approvalProcessId && u.userId == userId && u.enabled == enabled).FirstOrDefault();
            }

            public async Task<bool> changeOrder(int processId, int oldIndex, int newIndex)
            {
                try
                {
                    ApprovalProcessDetail? approvalProcessDetailFirst = dal.Get(u => u.processId == processId && u.dataOrder == oldIndex && u.enabled == true).FirstOrDefault();
                    if (approvalProcessDetailFirst != null)
                    {
                        ApprovalProcessDetail? approvalProcessDetailSecond = dal.Get(u => u.processId == processId && u.dataOrder == newIndex && u.enabled == true).FirstOrDefault();
                        if (approvalProcessDetailSecond != null)
                        {
                            approvalProcessDetailFirst.dataOrder = newIndex;
                            await Update(approvalProcessDetailFirst);
                            approvalProcessDetailSecond.dataOrder = oldIndex;
                            await Update(approvalProcessDetailSecond);
                            return true;
                        }
                        else
                        {
                            return false;
                        }


                    }
                    else
                    {
                        return false;
                    }


                }
                catch (Exception e)
                {
                    return false;
                }
            }

            public async Task<ApprovalProcessDetailSaveDto> save(ApprovalProcessDetailSaveDto entity, int userId)
            {
                if (entity.id == null)
                {
                    entity.createdUserId=userId;
                    entity.createdDate=DateTime.Now.ToString();

                    ApprovalProcessDetail? approvalProcessDetail = dal.Get(u => u.processId == entity.processId && u.enabled == true).OrderByDescending(u => u.dataOrder).FirstOrDefault();
                       
                    if (approvalProcessDetail == null)
                    {
                        entity.dataOrder=1;
                    }
                    else
                    {
                        int dataOrder = approvalProcessDetail.dataOrder;
                        entity.dataOrder=dataOrder + 1;
                    }
                    ApprovalProcessDetail? savedData = await Add(_mapper.Map<ApprovalProcessDetail>(entity));
                    return _mapper.Map<ApprovalProcessDetailSaveDto>(savedData);
                }
                else
                {
                    entity.updatedUserId=userId;
                    entity.updateDate=DateTime.Now.ToString();
                    await Update(_mapper.Map<ApprovalProcessDetail>(entity));
                    return entity;
                }
                
            }


            #endregion GetAll
        }
    }
}
