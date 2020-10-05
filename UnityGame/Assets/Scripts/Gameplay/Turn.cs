using System.Collections.Generic;
using System.Linq;

namespace Gameplay
{
    public class Turn
    {
        public readonly int Number;
        public bool IsCompleted { get; private set; }
        
        private readonly LinkedList<ICommand> _commandQueue = new LinkedList<ICommand>();
        private readonly Stack<IChange> _changelog = new Stack<IChange>();
        
        public Turn(int number)
        {
            Number = number;
        }

        public IEnumerable<IChange> IterateChangesFromNewestToOldest()
        {
            // Stack iterated in from newest to oldest
            return _changelog;
        }

        public void PushCommandEarly(ICommand command)
        {
            _commandQueue.AddFirst(command);
        }
        
        public void PushCommand(ICommand command)
        {
            _commandQueue.AddLast(command);
        }

        public ICommand PopCommand()
        {
            if (_commandQueue.Count > 0)
            {
                var first = _commandQueue.First.Value;
                _commandQueue.RemoveFirst();
                return first;
            }
            return null;
        }

        public void LogChange(IChange change)
        {
            _changelog.Push(change);
        }

        public void Complete()
        {
            IsCompleted = true;
        }
    }
}