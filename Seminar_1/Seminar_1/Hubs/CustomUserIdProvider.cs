using Microsoft.AspNet.SignalR;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seminar_1.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            var userId = request.User.Identity.Name;
            return userId.ToString();
        }
    }
}