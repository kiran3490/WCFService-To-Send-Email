using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;
using System.Net;

namespace EmailService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Mail" in code, svc and config file together.
    public class Mail : IMail
    {
        private static string SMTP_SERVER = "smtp.gmail.com";
        private static int SMTP_PORT = 587;
        private static string TEMP_FOLDER = @"C:\temp\";

        public int SendEmail(string gmailUserAddress, string gmailUserPassword, string[] emailTo,
          string[] ccTo, string subject, string body, bool isBodyHtml, FileAttachment[] attachments)
        {
            int result = -100;
            if (gmailUserAddress == null || gmailUserAddress.Trim().Length == 0)
            {
                return 10;
            }
            if (gmailUserPassword == null || gmailUserPassword.Trim().Length == 0)
            {
                return 20;
            }
            if (emailTo == null || emailTo.Length == 0)
            {
                return 30;
            }

            string tempFilePath = "";
            List<string> tempFiles = new List<string>();

            SmtpClient smtpClient = new SmtpClient(SMTP_SERVER, SMTP_PORT);
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(gmailUserAddress, gmailUserPassword);

            using (MailMessage message = new MailMessage())
            //Message object must be disposed before deleting temp attachment files
            {
                message.From = new MailAddress(gmailUserAddress);
                message.Subject = subject == null ? "" : subject;
                message.Body = body == null ? "" : body;
                message.IsBodyHtml = isBodyHtml;

                foreach (string email in emailTo)
                {
                    //TODO: Check email is valid
                    message.To.Add(email);
                }

                if (ccTo != null && ccTo.Length > 0)
                {
                    foreach (string emailCc in ccTo)
                    {
                        //TODO: Check CC email is valid
                        message.CC.Add(emailCc);
                    }
                }
                //There is a better way!!! See bellow...
                //if (attachments != null && attachments.Length > 0)
                //{
                //    foreach (FileAttachment fileAttachment in attachments)
                //    {
                //        if (fileAttachment.Info == null || fileAttachment.FileContentBase64 == null)
                //        {
                //            continue;
                //        }

                //        tempFilePath = CreateTempFile(TEMP_FOLDER, fileAttachment.FileContentBase64);

                //        if (tempFilePath != null && tempFilePath.Length > 0)
                //        {
                //            Attachment attachment = new Attachment(tempFilePath, MediaTypeNames.Application.Octet);
                //            ContentDisposition disposition = attachment.ContentDisposition;
                //            disposition.FileName = fileAttachment.Info.Name;
                //            disposition.CreationDate = fileAttachment.Info.CreationTime;
                //            disposition.ModificationDate = fileAttachment.Info.LastWriteTime;
                //            disposition.ReadDate = fileAttachment.Info.LastAccessTime;
                //            disposition.DispositionType = DispositionTypeNames.Attachment;
                //            message.Attachments.Add(attachment);
                //            tempFiles.Add(tempFilePath);
                //        }
                //        else
                //        {
                //            return 50;
                //        }
                //    }
                //}


                if (attachments != null && attachments.Length > 0)
                {
                    foreach (FileAttachment fileAttachment in attachments)
                    {
                        byte[] bytes = System.Convert.FromBase64String(fileAttachment.FileContentBase64);
                        MemoryStream memAttachment = new MemoryStream(bytes);
                        Attachment attachment = new Attachment(memAttachment, fileAttachment.Info.Name);
                        message.Attachments.Add(attachment);
                    }
                } 

                try
                {
                    smtpClient.Send(message);
                    result = 0;
                }
                catch
                {
                    result = 60;
                }
            }

            DeleteTempFiles(tempFiles.ToArray());
            return result;
        }



        private static string CreateTempFile(string destDir, string fileContentBase64)
        {
            string tempFilePath = destDir + (destDir.EndsWith("\\") ?
              "" : "\\") + Guid.NewGuid().ToString();

            try
            {
                using (FileStream fs = new FileStream(tempFilePath, FileMode.Create))
                {
                    byte[] bytes = System.Convert.FromBase64String(fileContentBase64); ;
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch
            {
                return null;
            }

            return tempFilePath;
        }

        private static void DeleteTempFiles(string[] tempFiles)
        {
            if (tempFiles != null && tempFiles.Length > 0)
            {
                foreach (string filePath in tempFiles)
                {
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch { } //Do nothing
                    }
                }
            }
        }
    }
}
