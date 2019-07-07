using System.Collections.Generic;
using DeOlho.ETL;
using DeOlho.ETL.Destinations;
using FluentAssertions;
using Moq;
using Xunit;
using System.Linq;

namespace UnitTests
{
    public class DestinationsTest
    {
        
        [Fact]
        public async void NothingDestination_Execute_IStep()
        {
            var mockStep = new Mock<Step<string>>();
            mockStep
                .Setup(_ => _.Execute())
                .ReturnsAsync(new StepValue<string>("teste", null));
            
            var result = (await mockStep.Object.Load(() => new NothingDestination())).Value;
            result.Should().Be("teste");
        }

        [Fact]
        public async void NothingDestination_Collection_Execute_IStep()
        {
            var strings = new List<StepValue<string>>()
            {
                new StepValue<string>("teste1", null),
                new StepValue<string>("teste2", null),
            };

            var results = (await strings.Load(() => new NothingDestination()));
            results.Should().HaveCount(2);
            results.ToList()[0].Value.Should().Be("teste1");
            results.ToList()[1].Value.Should().Be("teste2");
        }

    }
}