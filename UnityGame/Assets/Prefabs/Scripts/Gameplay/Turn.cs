using System.Collections.Generic;
using System.Linq;

namespace Gameplay
{
    public class Turn
    {
        public readonly int Number;
        public readonly Stack<IChange> Changelog = new Stack<IChange>();
        
        public Turn(int number)
        {
            Number = number;
        }

        public IEnumerable<IChange> IterateChangesFromNewestToOldest()
        {
            // Stack iterated in from newest to oldest
            return Changelog;
        }
    }
}