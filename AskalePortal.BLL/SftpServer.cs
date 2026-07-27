using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;
namespace AskalePortal.BLL
{
        public interface ISftpServer
        {
            void SendFileWithRetry(
                string host,
                string username,
                string password,
                string localFilePath,
                string remoteFilePath
            );
        }

        public class SftpServer : ISftpServer
        {
            public void SendFileWithRetry(
                string host,
                string username,
                string password,
                string localFilePath,
                string remoteFilePath
            )
            {
                using var client = new SftpClient(host, username, password);
                client.Connect();

                using var fileStream = File.OpenRead(localFilePath);
                client.UploadFile(fileStream, remoteFilePath, true);

                client.Disconnect();
            }
        }
    
}
