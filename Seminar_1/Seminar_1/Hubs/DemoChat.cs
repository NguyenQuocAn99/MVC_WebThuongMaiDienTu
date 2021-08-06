using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Seminar_1.Hubs
{
    [HubName("chat")]
    public class DemoChat : Hub
    {

        //Mình tạo ra phương thức Connect sẽ được connect từ server vào vói tham số name
        public void Connect(string name)
        {
            //trả về cho client phương thức  connect
            Clients.Caller.connect(name);
        }
        public void Message(string name, string message)
        {
            //trả về cho client pt message vs 2 tham số
            Clients.All.message(name, message);
            //Clients.User(id).message(name, message);

        }
    }
}