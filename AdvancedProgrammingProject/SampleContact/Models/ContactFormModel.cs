using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleContact.Models
{
    public class ContactFormModel
    {
        public string IP { get; set; } = "not-set";
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Comments { get; set; }
        public string LogSerialized
        {
            get
            {
                return $"IP: { IP } | Name: {Name} | Email: {Email} | Phone: {Phone} | Comments: {Comments}";
            }
        }
    }
}
