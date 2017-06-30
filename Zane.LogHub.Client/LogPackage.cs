using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Zane.LogHub.Client
{
    internal class LogPackage:IEnumerable<Tuple<LogEntity, string>>
    {
        internal int ContentLength { get; private set; }
        private List<Tuple<LogEntity, string>> Logs = new List<Tuple<LogEntity, string>>();
        
        internal void Add(LogEntity log)
        {
            if (log==null)
            {
                return;
            }
            var jsonStr = log.ToJsonString();
            this.ContentLength += jsonStr.Length;
            Logs.Add(new Tuple<LogEntity, string>(log,jsonStr));
        }

        public IEnumerator<Tuple<LogEntity, string>> GetEnumerator()
        {
            return ((IEnumerable<Tuple<LogEntity, string>>)Logs).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Tuple<LogEntity, string>>)Logs).GetEnumerator();
        }
    }
}
