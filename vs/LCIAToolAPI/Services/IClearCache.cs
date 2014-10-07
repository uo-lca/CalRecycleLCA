using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IClearCache
    {
        void Clear(int fragmentId, int scenarioId);
    }
}
