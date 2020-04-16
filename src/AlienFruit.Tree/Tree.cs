using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AlienFruit.Tree
{
    public class Tree<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> source;
        private readonly Func<T, IEnumerable<T>> childrenSelector;
        public Tree(IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            this.source = source;
            this.childrenSelector = childrenSelector;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this.source.GetEnumerator(), this.childrenSelector);

        public void ForEach(Action<T, T> action) => ForEach((c, p, i) => action(c, p), this.source, default, 0);

        public void ForEach(Action<T, T, int> action) => ForEach(action, this.source, default, 0);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void ForEach(Action<T, T, int> action, IEnumerable<T> nodes, T parrentNode, int layerIndex)
        {
            foreach (var node in nodes.ToList())
            {
                action(node, parrentNode, layerIndex);
                if (this.childrenSelector(node) != null && this.childrenSelector(node).Any())
                    ForEach(action, this.childrenSelector(node), node, layerIndex + 1);
            }
        }

        private class Enumerator : IEnumerator<T>
        {

            private readonly Func<T, IEnumerable<T>> childrenSelector;
            private readonly IEnumerator<T> firstEnumerator;
            private readonly Stack<IEnumerator<T>> enumerators;

            private T currentItem;

            public Enumerator(IEnumerator<T> firstEnumerator, Func<T, IEnumerable<T>> childrenSelector)
            {
                enumerators = new Stack<IEnumerator<T>>();
                this.childrenSelector = childrenSelector;
                this.firstEnumerator = firstEnumerator;
                enumerators.Push(firstEnumerator);
                this.currentItem = firstEnumerator.Current;
            }

            public T Current => this.currentItem;

            object IEnumerator.Current => Current;

            public void Dispose() => this.firstEnumerator.Dispose();

            public bool MoveNext()
            {
                if (currentItem != null)
                {
                    var children = childrenSelector(this.currentItem);
                    if (children != null && children.Any())
                    {
                        this.enumerators.Push(children.GetEnumerator());
                    }
                }

                if (!this.enumerators.Any())
                    return false;

                var crntEnum = this.enumerators.Peek();
                if (crntEnum.MoveNext())
                {
                    this.currentItem = crntEnum.Current;
                    return true;
                }
                else
                {
                    if (!crntEnum.Equals(this.firstEnumerator))
                        crntEnum.Dispose();
                    this.enumerators.Pop();
                    return MoveNext();
                }
            }

            public void Reset() => throw new NotImplementedException();
        }
    }
}
