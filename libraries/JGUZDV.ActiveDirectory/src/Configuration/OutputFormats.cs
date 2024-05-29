using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JGUZDV.ActiveDirectory.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static class OutputFormats
{
    public static class ByteArrays
    {
        public const string Base64 = "base64";
        public const string Guid = "guid";
        public const string SDDL = "sddl";
    }

    public static class Long
    {
        public const string FileTime = "filetime";
    }
}
