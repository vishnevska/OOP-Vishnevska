using System;
using System.Collections.Generic;
using IndependentWork7;
using Xunit;

namespace IndependentWork7.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_And_Get_ShouldReturnAddedValue()
        {
            var cache = new Cache<int, string>(3);

            cache.Add(1, "One");
            cache.Add(2, "Two");

            Assert.Equal("One", cache.Get(1));
            Assert.Equal("Two", cache.Get(2));
        }

        [Fact]
        public void Count_ShouldReflectCurrentNumberOfElements()
        {
            var cache = new Cache<string, int>(3);
            Assert.Equal(0, cache.Count);

            cache.Add("A", 10);
            Assert.Equal(1, cache.Count);

            cache.Add("B", 20);
            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void ContainsKey_ShouldReturnTrueForExistingAndFalseForMissing()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("k1", "v1");

            Assert.True(cache.ContainsKey("k1"));
            Assert.False(cache.ContainsKey("k2"));
        }

        [Fact]
        public void Get_NonExistentKey_ShouldThrowKeyNotFoundException()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "Data");

            Assert.Throws<KeyNotFoundException>(() => cache.Get(99));
        }

        [Fact]
        public void Add_WhenCapacityExceeded_ShouldEvictOldestItem_FIFO()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("first", "1");
            cache.Add("second", "2");

            cache.Add("third", "3");

            Assert.Equal(2, cache.Count);
            Assert.False(cache.ContainsKey("first"));
            Assert.True(cache.ContainsKey("second"));
            Assert.True(cache.ContainsKey("third"));
            Assert.Throws<KeyNotFoundException>(() => cache.Get("first"));
        }

        [Fact]
        public void Add_ExistingKey_ShouldUpdateValueWithoutEvicting()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("k1", "old");
            cache.Add("k2", "val2");

            cache.Add("k1", "new");

            Assert.Equal(2, cache.Count);
            Assert.Equal("new", cache.Get("k1"));
            Assert.True(cache.ContainsKey("k2"));
        }

        [Fact]
        public void Constructor_InvalidCapacity_ShouldThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<int, int>(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<int, int>(-5));
        }
    }
}