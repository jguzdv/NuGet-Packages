using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JGUZDV.ActiveDirectory.Converters
{
    internal class IntToStringConverter : IToStringConverter<int>
    {
        public string Convert(int value, string? outFormat)
            => value.ToString(outFormat ?? "0");
    }
}
