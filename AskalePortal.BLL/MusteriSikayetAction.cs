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
        public class MusteriSikayetAction : BaseBLL<AskalePortal.Data.Models.MusteriSikayetAction>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public MusteriSikayetAction(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }
            #region GetAll

            public List<AskalePortal.Data.Models.MusteriSikayetAction> GetAllAction()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.Id);
                return q.ToList();
            }
            public List<AskalePortal.Data.Models.MusteriSikayetAction> GetAllBySikayetId(int sikayetId)
            {
                var q = dal.Get(k => k.sikayetId == sikayetId && k.enabled == true);
                return q.ToList();
            }

            public async Task<CustomerComplaintActionSaveDto> save(CustomerComplaintActionSaveDto entity, int userId)
            {
                if (entity.id == null)
                {
                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    Data.Models.MusteriSikayetAction savedData = await Add(_mapper.Map<Data.Models.MusteriSikayetAction>(entity));
                    return _mapper.Map<CustomerComplaintActionSaveDto>(savedData);
                }
                else
                {
                    entity.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId;
                    await Update(_mapper.Map<Data.Models.MusteriSikayetAction>(entity));
                    return entity;
                }

            }

            internal List<CustomerComplaintActionDto> findAllBySikayetIdAndEnabled(int sikayetId, bool enabled)
            {
                var query = dal.Get(u =>
    u.enabled == enabled &&
    u.sikayetId == sikayetId)
                    .Select(a => new CustomerComplaintActionDto
                    {
                        id = a.Id,
                        sikayetId = a.sikayetId,
                        actionType = a.aksiyonTipi.aksiyonTipi,
                        companyName = a.company.vtext,
                        olusturanKisi = a.createdUser.name,
                        olusturmaTarihi = a.createdDate,
                        enabled = a.enabled,
                        actionDescription = a.actionDescription
                    })
                    .ToList();

                return query;
            }

            #endregion

        }
    }
}
