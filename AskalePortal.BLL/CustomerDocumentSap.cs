using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class CustomerDocumentSap
        {
            private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;

            public CustomerDocumentSap(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }
            public CustomerDocumentDto[] getCustomerDocument(string kunnr)
            {
                try
                {

                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);


                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI013");


                        CustomerDocumentOutput? output = sapFunction.Invoke<CustomerDocumentOutput>(
                            input: new KunnrParams
                            {
                                kunnr = kunnr
                            }
                                 );
                        sapConn.Disconnect();
                        return output.OUTPUT ?? [];
                    }
                }
                catch 
                {
                    // TODO: handle exception
                }
                return [];
            }

        }
    }
}