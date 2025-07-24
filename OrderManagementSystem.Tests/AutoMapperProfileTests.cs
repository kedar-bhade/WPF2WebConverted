using AutoMapper;
using OrderManagementSystem.Mapping;
using Xunit;

namespace OrderManagementSystem.Tests
{
    public class AutoMapperProfileTests
    {
        [Fact]
        public void AutoMapperConfiguration_IsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            config.AssertConfigurationIsValid();
        }
    }
}
