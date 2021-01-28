using System;
using System.Collections.Generic;
using System.Text;

namespace EjercicioBeClevar
{
    class Beclever
    {
        public string WID { get; set; }

        public string Reference_ID { get; set; }

        public string Name { get; set; }

        public string Organization_Subtpe_ID { get; set; }

        public string External_ID { get; set; }

        public string IsInactive { get; set; }

        public string Organization_Reference_ID { get; set; }

        public string Superior_Organization { get; set; }

        
        public override string ToString() {
            return "ID:" + this.WID + ";"+ "Reference_ID:" + this.Reference_ID + ";"+ "Name:" + this.Name + ";"
                +"SubType_ID:"+this.Organization_Subtpe_ID + ";"+ "External_ID:" + this.External_ID + 
                ";" + "Inactive:" + this.IsInactive + ";" + "Org_Ref_ID:" + this.Organization_Reference_ID + ";" +
                "Superior_Org:" + this.Superior_Organization + ".";
        }
    }






}

