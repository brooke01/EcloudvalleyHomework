using EcloudvalleyHomework.Controllers;
using EcloudvalleyHomework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string? connectString = configuration.GetConnectionString("DefaultConnection");
            var options = new DbContextOptionsBuilder<AwsDbContext>().UseSqlServer(connectString).Options;
            AwsDbContext context = new AwsDbContext(options);
            var homeController = new HomeController(configuration, context);

            // Act
            var expect = await homeController.UsageAmount(147878817734, new DateTime(2020, 04, 01), new DateTime(2020, 04, 02)) as OkObjectResult;
            var expectValue = (Dictionary<string, Dictionary<string, decimal>>)expect.Value;
            var result = await homeController.UsageAmount(147878817734, new DateTime(2020, 04, 01), new DateTime(2020, 04, 02)) as OkObjectResult;
            var resultValue = (Dictionary<string, Dictionary<string, decimal>>)result.Value;

            // Assert
            Assert.Equal(expectValue, resultValue);

            // Assert
            //expectValue.Should().BeEquivalentTo(resultValue, options => options.ExcludingMissingMembers());
        }
    }
}