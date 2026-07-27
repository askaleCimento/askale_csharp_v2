using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AskalePortal.BLL
{
    public class SendNotification
    {
        //#region SendEmail

        //public static string SendEmail(string toEmail, string toName, string subject, string content)
        //{
        //    BLLActions.Configs.WebMethods bllConfig = new BLLActions.Configs.WebMethods();

        //    string domain = bllConfig.GetByKey(1, "Website.URL").configValue;
        //    string smtp = bllConfig.GetByKey(1, "Email.SmtpClientAddress").configValue;
        //    string fromEmail = bllConfig.GetByKey(1, "Email.SenderEmail").configValue;
        //    string fromName = bllConfig.GetByKey(1, "Email.SenderName").configValue;
        //    string fromPass = bllConfig.GetByKey(1, "Email.SenderEmailPassword").configValue;
        //    int port = DataReader.GetInt32(bllConfig.GetByKey(1, "Email.SmtpPort").configValue);
        //    bool useSSL = DataReader.GetBoolean(bllConfig.GetByKey(1, "Email.SmtpRequiresSsl").configValue);

        //    string m = "<html style=\"height:100%\">" +
        //               "<head>" +
        //                    "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />" +
        //                    "<meta http-equiv=\"Content-Language\" content=\"TR\" />" +
        //                    "<title>" + subject + "</title>" +
        //                    "<style>" +
        //                        "body { color:#000000;background-color: #fff; border-width:0px; margin-top: 0px; margin-bottom: 0px; margin-left: 0px; margin-right: 0px; padding-top: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; font-family: Arial, Helvetica, sans-serif; text-align: left; font-size:13px; color:#000000}" +
        //                    "</style>" +
        //               "</head>" +
        //               "<body style=\"height:100%\">" +
        //                    "<table width=\"699\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" align=\"center\">" +
        //                    "<tr>" +
        //                        "<td width=\"699\">" +
        //                            "<img src=\"" + domain + "/_Resources/Mail/Notification/baslik.jpg\" width=\"699\" height=\"135\" alt=\"\" style=\"display: block; border: none\" />" +
        //                        "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                        "<td width=\"699\">" +
        //                            "<img style=\"display: block\" src=\"" + domain + "/_Resources/Mail/Notification/ust_cizgi.jpg\" width=\"699\" height=\"22\" alt=\"\" />" +
        //                        "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                        "<td width=\"699\">" +
        //                            "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"699\">" +
        //                                "<tr>" +
        //                                    "<td bgcolor=\"#333333\" width=\"23\"></td>" +
        //                                    "<td bgcolor=\"#e0e0e0\" width=\"1\"></td>" +
        //                                    "<td bgcolor=\"#575757\" width=\"14\"></td>" +
        //                                    "<td style=\"width: 623px;\">" +
        //                                        "<table border=\"0\" cellpadding=\"10\" cellspacing=\"0\" width=\"623\">" +
        //                                           " <tr>" +
        //                                                "<td width=\"623\" style=\"font-family: Arial, Helvetica, sans-serif; text-align: left; font-size:13px; color:#000000\">" + content + "</td>" +
        //                                            "</tr>" +
        //                                        "</table>" +
        //                                    "</td>" +
        //                                    "<td bgcolor=\"#575757\" width=\"13\"></td>" +
        //                                    "<td bgcolor=\"#e0e0e0\" width=\"1\"></td>" +
        //                                    "<td bgcolor=\"#333333\" width=\"24\"></td>" +
        //                                "</tr>" +
        //                            "</table>" +
        //                        "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                        "<td width=\"699\">" +
        //                            "<img style=\"display: block\" src=\"" + domain + "/_Resources/Mail/Notification/alt_cizgi.jpg\" width=\"699\" height=\"28\" alt=\"\" />" +
        //                        "</td>" +
        //                    "</tr>" +
        //                    "</table>" +
        //               "</body>" +
        //               "</html>";


        //    MailAddress gonderenEmail = new MailAddress(fromEmail, fromName);
        //    MailAddress alanEmail = new MailAddress(toEmail, toName);

        //    System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(gonderenEmail, alanEmail);

        //    msg.Subject = subject;
        //    msg.Body = content;
        //    msg.IsBodyHtml = true;
        //    System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient("smtp.yandex.com", port);
        //    sc.UseDefaultCredentials = false;
        //    sc.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    sc.Credentials = new System.Net.NetworkCredential(fromEmail, fromPass);
        //    if (useSSL)
        //        sc.EnableSsl = true;

        //    sc.Send(msg);


        //    return string.Empty;
        //}

        //#endregion SendEmail

        #region SendSMS

        public static string SendSMS(string number, string msg)
        {
            string[] numberList = number.Split(';');
            string tostring = string.Empty;
            foreach (var item in numberList)
            {
                tostring += "<TO>" + item + "</TO>";
            }
            msg = msg.Replace("İ", "I").Replace("Ü", "U").Replace("Ş", "S").Replace("Ğ", "G").Replace("Ö", "O").Replace("Ç", "C");
            msg = msg.Replace("ı", "i").Replace("ü", "u").Replace("ş", "s").Replace("ğ", "g").Replace("ö", "o").Replace("ç", "c");

            number = number.Replace("(", "").Replace(")", "").Replace(" ", "");

            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            
            xml += "<SMS-InsRequest>";
            xml += "<CLIENT user=\"askalebim\" pwd=\"askalebim01\" />";
            xml += "<INSERTMSG text=\"" + msg + "\">";
            xml += tostring;
            xml += "</INSERTMSG>";
            xml += "</SMS-InsRequest>";

            string url = "http://www.postaguvercini.com/api_xml/Sms_insreq.asp";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return PostXMLTransaction(url, doc);
        }

        public static string PostXMLTransaction(string URL, XmlDocument Doc)
        {
            //System.Net.WebClient wc = new System.Net.WebClient();
            //Byte[] bresp;
            //Byte[] bdata = System.Text.Encoding.UTF8.GetBytes(Doc.InnerXml);
            //wc.Headers.Add("Content-Type", "text/xml");
            //bresp = wc.UploadData(URL,bdata);
            //string resp = System.Text.Encoding.UTF8.Getstring(bresp);


            HttpWebRequest yeniRequest;
            Stream istekStream;

            yeniRequest = (HttpWebRequest)WebRequest.Create(URL);

            byte[] bytes;
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");
            //System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("ISO-8859-9");
            bytes = System.Text.Encoding.ASCII.GetBytes(Doc.InnerXml);
            yeniRequest.Method = "POST";
            yeniRequest.ContentLength = bytes.Length;
            yeniRequest.ContentType = "text/xml";

            istekStream = yeniRequest.GetRequestStream();
            istekStream.Write(bytes, 0, bytes.Length);
            istekStream.Close();

            WebResponse webResp = yeniRequest.GetResponse();
            Stream respStream = webResp.GetResponseStream();

            string responseData = "";
            byte[] buffer = new byte[10000];
            int len = 0, r = 1;
            while (r > 0)
            {
                r = respStream.Read(buffer, len, 10000 - len);
                len += r;
            }
            respStream.Close();
            responseData = encoding.GetString(buffer, 0, len);

            return responseData;
        }

        #endregion SendSMS
    }
}
