using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SapNwRfc;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class SAPConnectionData :
            BaseBLL<AskalePortal.Data.Models.Config>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;

            public SAPConnectionData(
                IConfiguration configuration,
                IWebHostEnvironment env)
                : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }

            public SapConnection? sapConnection(
                IConfiguration configuration,
                IWebHostEnvironment env)
            {
                Configs bllConfigs =
                    new Configs(_configuration, _env);

                Data.Models.Config? config =
                    bllConfigs.GetByID(1);

                if (config == null)
                {
                    return null;
                }

                SapConnection sapConnection =
                    new SapConnection(
                        new SapConnectionParameters()
                        {
                            AppServerHost = config.appServerHost,
                            Client = config.client,
                            Language = config.language,
                            IdleTimeout = config.idleTimeout,
                            SystemId = config.systemId,
                            SystemNumber = config.systemNumber,
                            User = config.userName,
                            Password = config.password,
                            PoolSize = config.poolSize,
                            Name = config.name,
                            SapRouter = config.sapRouter
                        });

                // ÖNEMLİ:
                // new SapConnection() sadece nesneyi oluşturur.
                // Gerçek RFC bağlantısı burada açılır.
                sapConnection.Connect();

                return sapConnection;
            }
        }
    }
}