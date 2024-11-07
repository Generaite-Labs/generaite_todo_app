using ToDo.Domain.Entities;

namespace ToDo.Tests.Domain;

public class EntityTests
{
    private class TestEntity : Entity<int>
    {
        public TestEntity(int id) : base(id) { }
    }

    private class StringIdEntity : Entity<string>
    {
        public StringIdEntity(string id) : base(id) { }
    }

    private class GuidIdEntity : Entity<Guid>
    {
        public GuidIdEntity(Guid id) : base(id) { }
    }

    [Fact]
    public void Entity_WithSameId_ShouldBeEqual()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(1);

        // Act & Assert
        Assert.Equal(entity1, entity2);
        Assert.True(entity1 == entity2);
        Assert.False(entity1 != entity2);
    }

    [Fact]
    public void Entity_WithDifferentId_ShouldNotBeEqual()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(2);

        // Act & Assert
        Assert.NotEqual(entity1, entity2);
        Assert.False(entity1 == entity2);
        Assert.True(entity1 != entity2);
    }

    [Theory]
    [InlineData("string-id")]
    [InlineData("another-id")]
    public void Entity_WithStringId_ShouldCompareCorrectly(string id)
    {
        // Arrange
        var entity1 = new StringIdEntity(id);
        var entity2 = new StringIdEntity(id);
        var entity3 = new StringIdEntity("different-id");

        // Assert
        Assert.Equal(entity1, entity2);
        Assert.NotEqual(entity1, entity3);
    }

    [Fact]
    public void Entity_WithGuidId_ShouldCompareCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new GuidIdEntity(id);
        var entity2 = new GuidIdEntity(id);
        var entity3 = new GuidIdEntity(Guid.NewGuid());

        // Assert
        Assert.Equal(entity1, entity2);
        Assert.NotEqual(entity1, entity3);
    }

    [Fact]
    public void ToString_ShouldIncludeTypeAndId()
    {
        // Arrange
        var entity = new TestEntity(42);
        var expected = "TestEntity [Id=42]";

        // Act
        var result = entity.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetHashCode_ShouldBeConsistentWithEquals()
    {
        // Arrange
        var id = 42;
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }
} 