using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class ChangeUserPasswordInputParams
    {
        
            [SapName("IV_USERNAME")]
            public string? IV_USERNAME { get; set; }

            [SapName("IV_PASSWORD")]
            public string? IV_PASSWORD { get; set; }

            [SapName("IV_LOCK")]
            public string? IV_LOCK { get; set; }

        }
}
