using System.Collections.Generic;

namespace OF.Core.Interfaces
{
    public interface IRawSearchResult<T> where T : class, new ()
    {
        int Total { get; }

        int Took { get; }

        IEnumerable<T> Documents { get; }

    }
}