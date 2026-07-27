using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
  
        public class FiyatOnayiDtoOutput
        {
            [SapName("OUTPUT")]
            public List<FiyatOnayiList>? fiyatOnayiList { get; set; }
        }

        public class FiyatOnayiList
        {
        [SapName("selkz")]
        public string? selkz{get;set;}
        [SapName("color")]
        public string? color{get;set;}
        [SapName("mandt")]
        public string? mandt{get;set;}
        [SapName("kschl")]
        public string? kschl{get;set;}
        [SapName("knumh")]
        public string? knumh{get;set;}
        [SapName("kopos")]
        public int? kopos{get;set;}
        [SapName("vkorg")]
        public string? vkorg{get;set;}
        [SapName("vtweg")]
        public string? vtweg{get;set;}
        [SapName("kunwe")]
        public string? kunwe{get;set;}
        [SapName("kunnr")]
        public string? kunnr{get;set;}
        [SapName("bzirk")]
        public string? bzirk{get;set;}
        [SapName("regio")]
        public string? regio{get;set;}
        [SapName("zterm")]
        public string? zterm{get;set;}
        [SapName("augru")]
        public string? augru{get;set;}
        [SapName("matnr")]
        public string? matnr{get;set;}
        [SapName("datbi")]
        public string? datbi{get;set;}
        [SapName("steps")]
        public int? steps{get;set;}
        [SapName("datab")]
        public string? datab{get;set;}
        [SapName("changenr")]
        public string? changenr{get;set;}
        [SapName("kbstat")]
        public string? kbstat{get;set;}
        [SapName("wiid")]
        public int? wiid{get;set;}
        [SapName("releasecomplete")]
        public string? releasecomplete{get;set;}
        [SapName("rejected")]
        public string? rejected{get;set;}
        [SapName("uname")]
        public string? uname{get;set;}
        [SapName("datum")]
        public string? datum{get;set;}
        [SapName("uzeit")]
        public string? uzeit{get;set;}
        [SapName("message")]
        public string? message{get;set;}
        [SapName("nextuser")]
        public string? nextuser{get;set;}
        [SapName("valuenew")]
        public string? valuenew{get;set;}
        [SapName("valueold")]
        public string? valueold{get;set;}
        [SapName("dbtabname")]
        public string? dbtabname{get;set;}
        [SapName("cukyold")]
        public string? cukyold{get;set;}
        [SapName("cukynew")]
        public string? cukynew{get;set;}
        [SapName("loekz")]
        public string? loekz{get;set;}
        [SapName("name1kunwe")]
        public string? name1kunwe{get;set;}
        [SapName("name1kunnr")]
        public string? name1kunnr{get;set;}
        [SapName("maktx")]
        public string? maktx{get;set;}
        [SapName("bztxt")]
        public string? bztxt{get;set;}
        [SapName("bezei")]
        public string? bezei{get;set;}
    }
    
}
