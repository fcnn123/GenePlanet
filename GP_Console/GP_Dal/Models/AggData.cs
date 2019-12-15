using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GP_Dal.Models
{
    [DataContract]
    public class AggData
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public float? Min { get; set; }

        [DataMember]
        public float? Max { get; set; }

        [DataMember]
        public DateTime LastSync { get; set; }
    }
}
