using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcaDataModel
{
    public interface IObjectState
    {
        ObjectState State { get; set; }
    }
}
