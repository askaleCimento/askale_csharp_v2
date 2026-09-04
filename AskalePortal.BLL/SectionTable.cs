using AskalePortal.Data.ResponseModels;
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
		public class SectionTable : BaseBLL<AskalePortal.Data.Models.SectionTable>
		{
            public SectionTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public List<SectionTableSaveDto>? GetAllDto()
            {
                return dal.Get(u => u.enabled == true).Select(u => new SectionTableSaveDto
                {
                    enabled = u.enabled,
                    id = u.Id,
                    companyId = u.companyId,
                    createdDate = (u.createdDate ?? DateTime.Now).ToString("dd.MM.yyyy"),
                    createdUserId = u.createdUserId,
                    section = u.section,
                    updateDate = (u.updatedDate ?? DateTime.Now).ToString("dd.MM.yyyy"),
                    updatedUserId = u.updatedUserId
                }).ToList();
            }
        }
	}

}
