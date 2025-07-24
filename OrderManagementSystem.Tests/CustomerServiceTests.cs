using System.Threading.Tasks;
using AutoMapper;
using Moq;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Models;
using OrderManagementSystem.Services;
using Xunit;

namespace OrderManagementSystem.Tests
{
    public class CustomerServiceTests
    {
        [Fact]
        public async Task CreateCustomerAsync_Throws_WhenCustomerIdTooLong()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            var repo = new Mock<IRepository<Customer>>();
            unitOfWork.Setup(u => u.Repository<Customer>()).Returns(repo.Object);
            repo.Setup(r => r.GetByIdAsync(It.IsAny<object>())).ReturnsAsync((Customer?)null);

            var mapper = new Mock<IMapper>();

            var service = new CustomerService(unitOfWork.Object, mapper.Object);
            var dto = new CreateCustomerDto { CustomerID = "TOOLONG", CompanyName = "Test" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateCustomerAsync(dto));
        }
    }
}
