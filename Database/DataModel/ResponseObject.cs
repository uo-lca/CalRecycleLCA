using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcaDataModel
{
    public class ResponseObject : Entity
    {
        public string Status { get; set; }
        public string ConfirmationId { get; set; }
        public string Message { get; set; }
    }
}
