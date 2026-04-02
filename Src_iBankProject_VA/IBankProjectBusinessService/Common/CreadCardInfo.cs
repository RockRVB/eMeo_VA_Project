using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LogProcessorService;
using System.Diagnostics;


namespace VTMModelLibrary
{
    public sealed class CreadCardInfo : PropertyChangedBase
    {
        public string err_code { get; set; }
        public string err_msg { get; set; }
        public string name { get; set; }
        public string idcard { get; set; }
        public string mobile { get; set; }
        public string open_no { get; set; }
        public string sex { get; set; }
        public string vipno { get; set; }
        public string card_type { get; set; }
        public string birth { get; set; }
    }

    public sealed class OtherService4ExhibitionClass : PropertyChangedBase
    {
        public bool Tanswer1Yes { get; set; }
        public bool Tanswer1No { get; set; }
        public bool Tanswer2Yes { get; set; }
        public bool Tanswer2No { get; set; }
        public bool Tanswer3Yes { get; set; }
        public bool Tanswer3No { get; set; }
        public bool Tanswer4Yes { get; set; }
        public bool Tanswer4No { get; set; }
        public bool Tanswer5Yes { get; set; }
        public bool Tanswer5No { get; set; }
        public bool Tanswer6Yes { get; set; }
        public bool Tanswer6No { get; set; }
    }
}
