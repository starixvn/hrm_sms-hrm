// --------------------------------------------------------------------------
// <copyright file="EmailService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.18">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService.Services
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;

    public static class EmailService
    {
        public static void SendEmail(string receiverEmail, string mailCC, string subject, string body)
        {
            if (!IsValidEmail(receiverEmail))
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = receiverEmail,
                MailCc = mailCC,
                Subject = subject,
                Body = body
            };

            using (var client = new MailDeliver())
            {
                client.SendMail(mailItem);
            }
        }

        public static string ReadTextFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new System.ArgumentNullException("filePath");
            }

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }
    }
}
