using System;
using Xunit;


namespace FlightPlanner.Core.Types.Tests
{
    public class IcaoCodeTests
    {
        [Fact]
        public void Constructor()
        {
            new IcaoCode("ESGJ");
        }

        [Theory]
        [InlineData("JKG")]
        [InlineData("esgj")]
        [InlineData("ESG1")]
        public void InvalidCode(string input)
        {
            Assert.Throws<ArgumentException>(
                () => new IcaoCode(input)
            );
        }
    }
}
