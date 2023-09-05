using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public static class EmailSender 
{
    const string senderEmail = "reutsthesissender@outlook.com";
    const string senderPassword = "Reut12345";
    public static void SendEmail(string subject,string message,string receiverEmail,string CSVPath)
    {
        // Create mail
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(senderEmail);
        mail.To.Add(receiverEmail);
        mail.Subject = subject;
        mail.Body = message;
        Attachment att = new Attachment(CSVPath);
        mail.Attachments.Add(att);
        // Setup server 
        SmtpClient smtpServer = new SmtpClient("smtp-mail.outlook.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential(senderEmail, senderPassword) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                Debug.Log("Email success!");
                return true;
            };

        // Send mail to server, print results
        try
        {
            smtpServer.Send(mail);
        }
        catch (System.Exception e)
        {
            Debug.Log("Email error: " + e.Message);
        }
        finally
        {
            Debug.Log("Email sent!");
        }
    }
}
