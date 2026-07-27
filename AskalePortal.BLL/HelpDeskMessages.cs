using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using Azure;
using AskalePortal.Data.Functions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static AskalePortal.Constants.CommonConstants;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class HelpDeskMessages : BaseBLL<AskalePortal.Data.Models.HelpDeskMessage>
        {
            private readonly IConfiguration _configuration;
            private  IWebHostEnvironment _env;
            public HelpDeskMessages(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }
           
            public List<AskalePortal.Data.Models.HelpDeskMessage> GetAllDemandId(int helpDeskDemandID)
            {
                var q = dal.Get(k => k.helpDeskDemandId== helpDeskDemandID && k.enabled == true).OrderBy(k => k.createdDate);

                return q.ToList();
            }

            public override List<AskalePortal.Data.Models.HelpDeskMessage> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.createdDate);

                return q.ToList();
            }

            public List<HelpDeskMessageDto>? listDemandId(int demandId)
            {

                List<HelpDeskMessageDto> listDeskMessages = dal.Get(k => k.helpDeskDemandId == demandId).Include(k => k.createdUser).Select(k => new HelpDeskMessageDto
                {
                    createdDate = k.createdDate,

                    id = k.Id,
                    imageUrl = k.createdUser.imageUrl,
                    message = k.message,
                    username = k.createdUser.username,

                }).ToList();


                string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                       _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "adminusers\\images\\");

                foreach (HelpDeskMessageDto deskMessageDto in listDeskMessages)
                {
                    BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);
                    List<AttachedFile> listAttachedFiles = bllAttachedFiles.getByModuleIdAndTargetId((int)MODULES.HELPDESK_MESSAGES, deskMessageDto.id);
                    List<FileNameAndTitle> listFileNameAndTitles = [];
                    foreach (AttachedFile attachedFile in listAttachedFiles)
                    {
                        FileNameAndTitle fileNameAndTitle = new FileNameAndTitle();
                        fileNameAndTitle.filename = attachedFile.filePath;
                        fileNameAndTitle.title = attachedFile.title;
                        listFileNameAndTitles.Add(fileNameAndTitle);
                    }

                    ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, deskMessageDto?.imageUrl??"", deskMessageDto?.imageUrl??"");
                    deskMessageDto!.imgPhoto = responseByteArray;
                    deskMessageDto.fileName = listFileNameAndTitles;
                }
                return listDeskMessages;

            }
        }
    }
}