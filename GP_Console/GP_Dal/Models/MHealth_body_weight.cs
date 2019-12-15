using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP_Dal.Models
{
    public class MHealth_body_weight
    {
        public string shim { get; set; }
        public int timeStamp { get; set; }
        public Body[] body { get; set; }
    }

    public class Body
    {
        public Header header { get; set; }
        public Body1 body { get; set; }
    }

    public class Header
    {
        public string id { get; set; }
        public DateTime creation_date_time { get; set; }
        public Acquisition_Provenance acquisition_provenance { get; set; }
        public Schema_Id schema_id { get; set; }
    }

    public class Acquisition_Provenance
    {
        public string source_name { get; set; }
        public long external_id { get; set; }
    }

    public class Schema_Id
    {
        public string _namespace { get; set; }
        public string name { get; set; }
        public string version { get; set; }
    }

    public class Body1
    {
        public Effective_Time_Frame effective_time_frame { get; set; }
        public Body_Weight body_weight { get; set; }
    }

    public class Effective_Time_Frame
    {
        public DateTime date_time { get; set; }
    }

    public class Body_Weight
    {
        public string unit { get; set; }
        public float value { get; set; }
    }

}
