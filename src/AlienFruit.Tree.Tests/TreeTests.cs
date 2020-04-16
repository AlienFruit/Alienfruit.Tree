using AlienFruit.Tree.Extensions;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AlienFruit.Tree.Tests
{
    public class TreeTests
    {
        [Fact]
        public void Tree_select_from_tree_hould_returns_all_names_of_tree_items()
        {
            // Arrange
            var items = BuildTree();

            // Action
            var tree = items.ToTree(x => x.Children);

            // Assert
            tree.Select(x => x.Name).Should().BeEquivalentTo(new[]
            {
                "item1",
                "child1",
                "childChild2",
                "Item2"
            });
        }

        [Fact]
        public void Tree_should_reset_enumerator_after_foreach()
        {
            // Arrange
            var items = BuildTree();

            // Action
            var tree = items.ToTree(x => x.Children);
            var count1 = tree.Count();
            var count2 = tree.Count();

            // Assert
            count2.Should().Be(count2);
        }

        [Fact]
        public void Tree_foreEach_test()
        {
            //Arrange
            var items = BuildTree();
            var tree = items.ToTree(x => x.Children);

            //Action
            var result = new List<(string name, string parrentName)>();
            tree.ForEach((c, p) => result.Add((c.Name, p?.Name)));

            //Assert
            result.Should().BeEquivalentTo(new[]
            {
                ("item1", null),
                ("child1", "item1"),
                ("childChild2", "child1"),
                ("Item2", null)
            });
        }


        private List<Item> BuildTree()
            => new List<Item>
            {
                new Item
                {
                    Name = "item1",
                    Children = new []
                    {
                        new Item
                        {
                            Name = "child1",
                            Children = new []
                            {
                                new Item
                                {
                                    Name = "childChild2",
                                    Children = Enumerable.Empty<Item>()
                                }
                            }
                        }
                    }
                },
                new Item
                {
                    Name = "Item2",
                    Children = null
                }
            };

        private class Item
        {
            public string Name { get; set; }
            public IEnumerable<Item> Children { get; set; }
        }
    }
}
