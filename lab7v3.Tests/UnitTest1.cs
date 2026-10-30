using System;
using System.Linq;
using Xunit;
using lab7v3;

namespace lab7v3.Tests
{
    public class InMemoryRepositoryTests
    {
        [Fact]
        public void Add_ShouldAddEntityToRepository()
        {
            var repository = new InMemoryRepository<Order>();
            var order = new Order(1, DateTime.Now, 500m, 1);

            repository.Add(order);

            var result = repository.GetById(1);
            Assert.NotNull(result);
            Assert.Equal(500m, result.TotalAmount);
        }

        [Fact]
        public void GetById_ExistingId_ShouldReturnEntity()
        {
            var repository = new InMemoryRepository<Order>();
            var order = new Order(1, DateTime.Now, 1000m, 2);
            repository.Add(order);

            var result = repository.GetById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ShouldReturnNull()
        {
            var repository = new InMemoryRepository<Order>();

            var result = repository.GetById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllEntities()
        {
            var repository = new InMemoryRepository<Order>();
            repository.Add(new Order(1, DateTime.Now, 100m, 1));
            repository.Add(new Order(2, DateTime.Now, 200m, 2));

            var result = repository.GetAll();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void Update_ExistingEntity_ShouldUpdateData()
        {
            var repository = new InMemoryRepository<Order>();
            repository.Add(new Order(1, DateTime.Now, 300m, 1));

            var updatedOrder = new Order(1, DateTime.Now, 999m, 1);
            repository.Update(updatedOrder);

            var result = repository.GetById(1);
            Assert.Equal(999m, result.TotalAmount);
        }

        [Fact]
        public void Update_NonExistingEntity_ShouldNotChangeRepositoryState()
        {
            var repository = new InMemoryRepository<Order>();
            repository.Add(new Order(1, DateTime.Now, 300m, 1));

            var nonExistingOrder = new Order(99, DateTime.Now, 999m, 1);
            repository.Update(nonExistingOrder);

            Assert.Null(repository.GetById(99));
            Assert.Single(repository.GetAll());
        }

        [Fact]
        public void Delete_ExistingEntity_ShouldRemoveEntity()
        {
            var repository = new InMemoryRepository<Order>();
            repository.Add(new Order(1, DateTime.Now, 100m, 1));

            repository.Delete(1);

            Assert.Null(repository.GetById(1));
            Assert.Empty(repository.GetAll());
        }

        [Fact]
        public void Delete_NonExistingEntity_ShouldDoNothing()
        {
            var repository = new InMemoryRepository<Order>();
            repository.Add(new Order(1, DateTime.Now, 100m, 1));

            repository.Delete(999);

            Assert.Single(repository.GetAll());
        }
    }
}