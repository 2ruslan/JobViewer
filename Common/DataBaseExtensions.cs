using JobViewer.Model;
using System;
using System.Linq;

namespace JobViewer.Common
{
    public static class DataBaseExtensions
    {
        public static bool IsDataBase(this JobDbContext db,  string candidate)
            => db.DataBases.ToList()
                .Any(x => string.Equals(x.Name, candidate.TrimStart('[').TrimEnd(']'), StringComparison.InvariantCultureIgnoreCase));
    }
}
