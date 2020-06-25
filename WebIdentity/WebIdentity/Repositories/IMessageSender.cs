using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebIdentity.Repositories
{
   public interface IMessageSender
    {
         Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false);
    }
}
