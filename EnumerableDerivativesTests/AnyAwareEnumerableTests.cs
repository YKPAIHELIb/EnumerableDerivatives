namespace EnumerableDerivativesTests;

using EnumerableDerivatives;

public class AsAnyAwareEnumerableTests
{
    [Fact]
    public void AsAnyAwareEnumerable_ShouldThrowOnNullEnumerable()
    {
        IEnumerable<int> enumerable = null;
        Assert.Throws<ArgumentNullException>(() => enumerable.AsAnyAwareEnumerable());
    }

    [Fact]
    public void AsAnyAwareEnumerable_Dispose_ShouldNotThrow()
    {
        var anyAware = new List<int> { 1, 2, 3 }.AsAnyAwareEnumerable();

        // It should not throw any exception
        anyAware.Dispose();
    }

    [Fact]
    public void AsAnyAwareEnumerable_IsAny_ShouldReturnTrueForNonEmpty()
    {
        using var anyAware = new List<int> { 1, 2, 3 }.AsAnyAwareEnumerable();

        Assert.True(anyAware.IsAny);
    }

    [Fact]
    public void AsAnyAwareEnumerable_IsAny_ShouldReturnFalseForEmpty()
    {
        using var anyAware = new List<int>().AsAnyAwareEnumerable();

        Assert.False(anyAware.IsAny);
    }

    [Fact]
    public void AsAnyAwareEnumerable_Enumerator_ShouldIterateCorrectly()
    {
        using var anyAware = new List<int> { 1, 2, 3 }.AsAnyAwareEnumerable();
        var list = anyAware.ToList();

        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void AsAnyAwareEnumerable_MultipleEnumeratorCall_ShouldResetEnumerator()
    {
        using var anyAware = new List<int> { 1, 2, 3 }.AsAnyAwareEnumerable();

        // First call
        var first = anyAware.First();
        Assert.Equal(1, first);

        // Second call
        var second = anyAware.First();
        Assert.Equal(1, second);
    }

    [Fact]
    public void AsAnyAwareEnumerable_MixingIsAnyWithEnumeration_ShouldPreserveEnumeration()
    {
        using var anyAware = new List<int> { 1, 2, 3 }.AsAnyAwareEnumerable();

        // Check IsAny first
        Assert.True(anyAware.IsAny);

        // Then enumerate
        var firstItem = anyAware.First();
        Assert.Equal(1, firstItem);
    }

    [Fact]
    public void AsAnyAwareEnumerable_MixingEnumerationWithIsAny_ShouldPreserveIsAnyValue()
    {
        using var anyAware = new List<int> { 1, 2, 3 }.AsAnyAwareEnumerable();

       // Enumerate first
        var firstItem = anyAware.First();
        Assert.Equal(1, firstItem);

        // Check IsAny afterwards
        Assert.True(anyAware.IsAny);
    }

    [Fact]
    public void AsAnyAwareEnumerable_MixingIsAnyWithMultipleEnumerations_ShouldResetEnumeration()
    {
        using var anyAware = new List<int> { 1, 2, 3 }.AsAnyAwareEnumerable();

        // Check IsAny first
        Assert.True(anyAware.IsAny);

        // Enumerate twice
        var firstItem = anyAware.First();
        Assert.Equal(1, firstItem);

        var secondItem = anyAware.Skip(1).First();
        Assert.Equal(2, secondItem);
    }

    [Fact]
    public void AsAnyAwareEnumerable_IsAnyAfterFullEnumeration_ShouldReturnTrue()
    {
        using var anyAware = new List<int> { 1, 2, 3 }.AsAnyAwareEnumerable();

        // Fully enumerate the collection
        var list = anyAware.ToList();
        Assert.Equal(3, list.Count);

        // Check IsAny afterwards
        Assert.True(anyAware.IsAny);
    }

    [Fact]
    public void AsAnyAwareEnumerable_EmptyList_IsAnyAfterEnumeration_ShouldReturnFalse()
    {
        using var anyAware = new List<int>().AsAnyAwareEnumerable();

        // Enumerate
        var list = anyAware.ToList();
        Assert.Empty(list);

        // Check IsAny afterwards
        Assert.False(anyAware.IsAny);
    }

    [Fact]
    public void AsAnyAwareEnumerable_MixingIsAnyWithEnumeration_ShouldNotEnumerateTwiceInternally()
    {
        // List to log enumeration
        var log = new List<int>();

        var source = new List<int> { 1, 2, 3 };

        // Use Select to log enumeration
        var trackedEnumerable = source.Select(item =>
        {
            log.Add(item);
            return item;
        });

        using var anyAware = trackedEnumerable.AsAnyAwareEnumerable();

        // Check IsAny first
        Assert.True(anyAware.IsAny);

        // Then enumerate
        var list = anyAware.ToList();

        // Verify items
        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);

        // Ensure enumeration was not done twice internally
        // Each item should only appear once in the log
        Assert.Equal(3, log.Count);
        Assert.Equal(1, log[0]);
        Assert.Equal(2, log[1]);
        Assert.Equal(3, log[2]);
    }
}
