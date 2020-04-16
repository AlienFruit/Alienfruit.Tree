using System.Collections.Generic;

namespace AlienFruit.Tree
{
    public interface ITreeItem<T>
    {
        IEnumerable<T> Children { get; set; }
    }
}
