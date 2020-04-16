using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AlienFruit.Tree.Tests
{
    public class TreeBuilderTests
    {
        [Fact]
        public void TreeBuilder_Build_should_build_correct_tree()
        {
            // Arrange
            var items = new[]
            {
                new Foo(12,0),
                new Foo(13,0),
                new Foo(0, 0),
                new Foo(1, 3),
                new Foo(14,13)
            };

            // Action
            var tree = new TreeBuilder<Foo>(items, x => x.Id, x => x.ParrentId).Build().Tree;

            // Assert
            tree.Should().BeEquivalentTo(new[]
            {
                new Foo(0,0)
                {
                    Children = new[]
                    {
                        new Foo(12,0),
                        new Foo(13,0)
                        {
                            Children = new []
                            {
                                new Foo(14,13)
                            }
                        }
                    }
                },
                new Foo(1,3)
            });

        }

        private class Foo : ITreeItem<Foo>
        {
            public long Id { get; set; }
            public long ParrentId { get; set; }
            public IEnumerable<Foo> Children { get; set; }

            public Foo(long id, long parrentId)
            {
                this.Id = id;
                this.ParrentId = parrentId;
                this.Children = Enumerable.Empty<Foo>();
            }
        }
    }
}
