using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class Bdcmsgcool
    {
        [SapName("TCODE")]
       public string? TCODE    {get;set;}

        [SapName("DYNAME")]
        public string? DYNAME   {get;set;}

        [SapName("DYNUMB")]
        public string? DYNUMB   {get;set;}

        [SapName("MSGTYP")]
        public string? MSGTYP   {get;set;}

        [SapName("MSGSPRA")]
        public string? MSGSPRA  {get;set;}

        [SapName("MSGID")]
        public string? MSGID    {get;set;}

        [SapName("MSGNR")]
        public string? MSGNR    {get;set;}

        [SapName("MSGV1")]
        public string? MSGV1    {get;set;}

        [SapName("MSGV2")]
        public string? MSGV2    {get;set;}

        [SapName("MSGV3")]
        public string? MSGV3    {get;set;}

        [SapName("MSGV4")]
        public string? MSGV4    {get;set;}

        [SapName("ENV")]
        public string? ENV      {get;set;}

        [SapName("FLDNAME")]
        public string? FLDNAME { get; set; }
    }
}
