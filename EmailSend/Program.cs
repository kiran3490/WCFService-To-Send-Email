using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;
using EmailSend.EmailServRef;

namespace EmailSend
{
    class Program
    {
        static void Main(string[] args)
        {
            string emailFrom = "kirann1656@gmail.com";
            string password = "kiran7698485645";
            string emailTo = "kiran3490@yahoo.com";
            string fileAttachmentPath = @"C:\TextFile.txt";
            int result = -1;

            try
            {
                using (EmailServRef.MailClient client = new EmailServRef.MailClient())
                {
                    List<EmailServRef.FileAttachment> allAttachments = new List<EmailServRef.FileAttachment>();
                    EmailServRef.FileAttachment attachment = new EmailServRef.FileAttachment();
                    attachment.Info = new FileInfo(fileAttachmentPath);
                    attachment.FileContentBase64 = Convert.ToBase64String(File.ReadAllBytes(fileAttachmentPath));
                    allAttachments.Add(attachment);

                    result = Mail.SendEmail(emailFrom, password, new string[] { emailTo }, null,
                      "It works!!!", "Body text", false, allAttachments.ToArray());
                    Console.WriteLine("Result: " + result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

    }
}
