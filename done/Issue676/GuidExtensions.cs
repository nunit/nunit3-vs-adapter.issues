using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUnitTest
{
    // From https://stackoverflow.com/questions/30404965/increment-guid-in-c-sharp
    internal static class GuidExtensions
    {

        static int[] byteOrder = {15, 14, 13, 12, 11, 10, 9, 8, 6, 7, 4, 5, 0, 1, 2, 3};

        internal static Guid NextGuid(this Guid guid)
        {
            var bytes = guid.ToByteArray();
            var canIncrement = byteOrder.Any(i => ++bytes[i] != 0);
            return new Guid(canIncrement ? bytes : new byte[16]);
        }
    }
}
