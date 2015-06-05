using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailManager.Models
{
    public class Message
    {


        public int statusCode { get; set; }
        public string message { get; set; }
        public string navTabId { get; set; }
        public string rel { get; set; }
        public string callbackType { get; set; }
        public string forwardUrl { get; set; }
        public string confirmMsg { get; set; }

    }
}