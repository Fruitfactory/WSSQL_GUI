using System.Collections.Generic;

namespace OF.Core.Interfaces
{
    public interface IRawSearchResult<T> where T : class, new ()
    {
        int Total { get; }

        int Took { get; }

        List<T> Documents { get; }

    }
}