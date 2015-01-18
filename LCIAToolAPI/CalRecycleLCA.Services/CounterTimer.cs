using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CalRecycleLCA.Services
{
    class CounterTimer : Stopwatch
    {
        private List<long> counts;
        private long startval;
        private List<String> notes;

        public CounterTimer()
        {
            counts = new List<long>();
            notes = new List<String>();
        }

        public void CStart()
        {
            startval = base.ElapsedMilliseconds;
            base.Start();
        }

        public void Click(String note = "")
        {
            counts.Add(base.ElapsedMilliseconds - startval);
            notes.Add(note);
        }

        public void CStop()
        {
            base.Stop();
            Click();
        }

        public List<long> Counts()
        {
            return counts;
        }

    }
}
