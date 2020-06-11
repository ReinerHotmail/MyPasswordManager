using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text;

namespace MyPasswordManager
{
    class PwDat
    {
        public string Title { get; set; }
        public string WebAdr { get; set; }
        public string User { get; set; }
        public string PW { get; set; }
        public string Opt1 { get; set; }
        public string Opt2 { get; set; }

 

        public override string ToString()
        {
            return Title + ";" + WebAdr + ";" + User + ";" + PW + ";" + Opt1 + ";" + Opt2;
        }


    }
}
