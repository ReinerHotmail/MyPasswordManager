using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text;

namespace MyPasswordManager
{
    public class CPwDat: IEquatable<CPwDat>, IComparable<CPwDat>
    {
        public string Title { get; set; }
        public string WebAdr { get; set; }
        public string User { get; set; }
        public string PW { get; set; }
        public string Opt1 { get; set; }
        public string Opt2 { get; set; }

        public int CompareTo([AllowNull] CPwDat other)
        {
            return String.Compare(Title, other.Title);
        }

        public bool Equals([AllowNull] CPwDat other)
        {
            return Title == other.Title;
        }

        public override string ToString()
        {
            return Title + ";" + WebAdr + ";" + User + ";" + PW + ";" + Opt1 + ";" + Opt2;
        }


    }
}
