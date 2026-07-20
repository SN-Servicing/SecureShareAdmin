using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Snsc.SecureShareAdmin.Configuration;

namespace Snsc.SecureShareAdmin.Email;

public sealed class NewAccountEmailSender
{
    private const string SesFromAddress = "customserv@snsc.com";
    private const string WelcomeSubject = "New SNSC Secure Share Account";
    private const string ResetSubject = "SNSC Secure Share Password Reset";
    private const string SiteToken = "##SITE##";

    private readonly SnConfigClient _snConfig;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public NewAccountEmailSender(SnConfigClient snConfig, IWebHostEnvironment webHostEnvironment)
    {
        _snConfig = snConfig;
        _webHostEnvironment = webHostEnvironment;
    }

    public NewAccountEmailResult SendWelcomeEmail(string userEmail)
    {
        return SendForgotPasswordLinkEmail(userEmail, "NewAccount.htm", WelcomeSubject);
    }

    public NewAccountEmailResult SendPasswordResetEmail(string userEmail)
    {
        return SendForgotPasswordLinkEmail(userEmail, "ResetPassword.htm", ResetSubject);
    }

    private NewAccountEmailResult SendForgotPasswordLinkEmail(
        string userEmail,
        string templateFileName,
        string subject)
    {
        string? forgotPasswordUrl = ResolveForgotPasswordUrl(_snConfig.EnvironmentName);
        if (forgotPasswordUrl == null)
        {
            return NewAccountEmailResult.Skipped();
        }

        try
        {
            string body = LoadTemplate(templateFileName)
                .Replace(SiteToken, forgotPasswordUrl, StringComparison.Ordinal);
            SendViaSes(userEmail.Trim(), subject, body);
            return NewAccountEmailResult.Success();
        }
        catch (Exception ex)
        {
            return NewAccountEmailResult.Failed(ex.Message);
        }
    }

    private void SendViaSes(string toAddress, string subject, string htmlBody)
    {
        var credentials = new BasicAWSCredentials(
            _snConfig.GetAmazonAccessKeyId(),
            _snConfig.GetAmazonSecretAccessKey());
        RegionEndpoint region = RegionEndpoint.GetBySystemName(_snConfig.GetAmazonRegion());

        using var client = new AmazonSimpleEmailServiceClient(credentials, region);
        var request = new SendEmailRequest
        {
            Source = SesFromAddress,
            Destination = new Destination
            {
                ToAddresses = new List<string> { toAddress }
            },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body
                {
                    Html = new Content
                    {
                        Charset = "UTF-8",
                        Data = htmlBody
                    }
                }
            }
        };

        client.SendEmailAsync(request).GetAwaiter().GetResult();
    }

    private string LoadTemplate(string fileName)
    {
        string path = Path.Combine(_webHostEnvironment.ContentRootPath, "Email", "Templates", fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Email template '{fileName}' was not found.", path);
        }

        return File.ReadAllText(path);
    }

    internal static string? ResolveForgotPasswordUrl(string environmentName)
    {
        string host = environmentName.Trim().ToLowerInvariant() switch
        {
            "production" => "secureshare.snsc.com",
            "qaregression" => "secureshare-stg.snsc.com",
            _ => string.Empty
        };

        return string.IsNullOrEmpty(host)
            ? null
            : $"https://{host}/Account/ForgotPassword";
    }
}
