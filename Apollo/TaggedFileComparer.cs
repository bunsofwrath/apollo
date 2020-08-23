using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Apollo
{
    public class TaggedFileComparer : IEqualityComparer<TaggedFile>
    {
        public bool Equals([AllowNull] TaggedFile x, [AllowNull] TaggedFile y)
        {
            if (x == null && y == null)
                return true;
            
            if (x == null || y == null)
                return false;

            if (!x.FileName.Equals(y.FileName, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!x.MD5Hash.SequenceEqual(y.MD5Hash))
                return false;

            return true;
        }

        public int GetHashCode([DisallowNull] TaggedFile obj)
            => obj.FileName.GetHashCode()
            ^ obj.MD5Hash.Select(b => Convert.ToInt32(b)).Sum();
    }
}
