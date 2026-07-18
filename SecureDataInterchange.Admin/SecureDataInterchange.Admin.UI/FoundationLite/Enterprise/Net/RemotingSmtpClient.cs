/********************************************
Copyright  2015, SN Servicing, Inc.
********************************************/

using System.Configuration;
using System.Net.Mail;

namespace Snsc.Enterprise.Net
{
    /// <summary>
    /// Minimal SMTP email client for SecureShare Admin.
    /// Sends emails using SMTP configuration from web.config.
    /// </summary>
    public static class RemotingSmtpClient
    {
        /// <summary>
        /// Sends an email message using the configured SMTP settings.
        /// </summary>
        /// <param name="message">The mail message to send.</param>
        public static void SendEmail(MailMessage message)
        {
            SmtpClient client = new SmtpClient();
            client.Send(message);
        }
    }
}
