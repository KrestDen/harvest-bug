using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestBug
{
    struct User
    {
        public string userId;
        public string domain;
        public string online;
        public string can_write_private_message;
        public string screen_name;
        public string first_name;
        public string last_name;
        public string city;

        public override string ToString()
        {
            return first_name + " " + last_name + " " + GetUrl();
        }

        private string GetUrl()
        {
            return "https://vk.com/" + domain;
        }
    }
}
