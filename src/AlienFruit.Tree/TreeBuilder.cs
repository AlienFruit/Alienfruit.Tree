using System;
using System.Collections.Generic;
using System.Linq;

namespace AlienFruit.Tree
{
    public class TreeBuilder<T> where T : ITreeItem<T>
    {

        private readonly Func<T, long> idSelector;
        private readonly Func<T, long> idParentSelector;

        private List<T> source;
        private Predicate<T> buildPredicate;
        private IEnumerable<T> selectedRootElements;

        public int ItemsCount { get; private set; }
        public IEnumerable<T> Tree => source;

        public TreeBuilder(IEnumerable<T> source, Func<T, long> idSelector, Func<T, long> idParentSelector)
        {
            this.source = source.ToList();
            this.idSelector = idSelector;
            this.idParentSelector = idParentSelector;
        }

        public TreeBuilder<T> Build()
        {
            ItemsCount = 0;
            var rootNodes = (this.selectedRootElements ?? GetRootList()).ToList();
            this.source = BuildTree(rootNodes).ToList();
            return this;
        }

        public TreeBuilder<T> SelectBranches(IEnumerable<long> rootItemsIds)
        {
            this.selectedRootElements = this.source.Where(x => rootItemsIds.Contains(this.idSelector(x))).ToList();
            return this;
        }

        public TreeBuilder<T> SelectBranch(long rootItemId)
        {
            this.selectedRootElements = this.source.Where(x => this.idSelector(x) == rootItemId).ToList();
            return this;
        }

        public TreeBuilder<T> SelectBranch(Func<T, bool> predicate)
        {
            var items = this.source.Where(predicate);
            if (!items.Any())
                throw new ArgumentException("No any items was not found in the collection");
            this.selectedRootElements = items.ToList();
            return this;
        }

        public TreeBuilder<T> Build(Predicate<T> predicate)
        {
            this.buildPredicate = predicate;
            Build();
            return this;
        }

        public TreeBuilder<T> RemoveAll(Predicate<T> match)
        {
            var oncemore = true;
            while (oncemore)
            {
                oncemore = false;
                ForEach((item, list) =>
                {
                    if (match(item))
                    {
                        var isRemoved = list.Remove(item);
                        ItemsCount--;

                        if (list.Count == 0 && isRemoved)
                            oncemore = true;
                    }
                });
            }
            return this;
        }

        public void ForEach(Action<T, List<T>> action) => ForEach(action, this.source);

        public IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            var result = new List<TResult>();
            ForEach((a, l) => result.Add(selector(a)));
            return result;
        }

        private void ForEach(Action<T, List<T>> action, List<T> nodes)
        {
            foreach (var node in nodes.ToList())
            {
                action(node, nodes);
                var nodeChildren = node.Children.ToList();
                ForEach(action, nodeChildren);
                node.Children = nodeChildren;
            }
        }

        private IEnumerable<T> BuildTree(IEnumerable<T> nodes)
        {
            var result = new List<T>();
            foreach (var node in nodes)
            {
                if (this.buildPredicate != null && !this.buildPredicate(node))
                    continue;

                var childNodes = this.source.Where(x => this.idParentSelector(x) == this.idSelector(node)
                    && this.idParentSelector(x) != this.idSelector(x));

                node.Children = BuildTree(childNodes);
                ItemsCount++;
                result.Add(node);
            }
            return result;
        }

        public static bool HasChildren(T item) => item.Children != null && item.Children.Any();

        private List<T> GetRootList()
            => this.source
                .Where(x => !this.source.Any(y => this.idSelector(y) == this.idParentSelector(x)) || this.idSelector(x) == this.idParentSelector(x))
                .ToList();
    }
}
