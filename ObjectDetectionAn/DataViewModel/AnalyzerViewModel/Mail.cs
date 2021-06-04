using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using System.Windows;
using MimeKit;
using MailKit.Security;


namespace ObjectDetectionAn.DataViewModel.FrameListViewModel
{
    public class Mail
    {
        MailKit.Net.Smtp.SmtpClient smtpClient;
        MimeMessage mailMessage;
        private string _host;
        private int _port;
        private string _email;
        private string _password;

        public Mail(string host, int port, string email, string password)
        {
            _host = host;
            _port = port;
            _email = email;
            _password = password;
        }

        public void SendMail(string _from, string _to, string _subject, string _text)
        {
            try
            {
                smtpClient = new MailKit.Net.Smtp.SmtpClient();
                smtpClient.Connect(_host, _port);
                smtpClient.Authenticate(_email, _password);

                mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress("", _from));

                foreach (var to in _to.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mailMessage.To.Add(new MailboxAddress("", to));
                }

                mailMessage.Subject = _subject;
                mailMessage.Body = new TextPart("html") { Text = _text };

                if(smtpClient != null)
                {
                    smtpClient.Send(mailMessage);
                    smtpClient.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "PP", "Er", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}