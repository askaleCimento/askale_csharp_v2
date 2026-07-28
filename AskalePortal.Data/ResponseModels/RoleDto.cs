using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class RoleDto
    {
        public int? id { get; set; }

        public string? title { get; set; }

        public string? description { get; set; }

        public string? companies { get; set; }

        public bool? approval { get; set; }

        public bool? enabled { get; set; }

        public DateTime? createdDate { get; set; }

        public int? createdUserId { get; set; }

        public DateTime? updatedDate { get; set; }

        public int? updatedUserId { get; set; }
    }
}
