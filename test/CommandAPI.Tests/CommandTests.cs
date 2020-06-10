using System;
using Xunit;
using CommandAPI.Models;

namespace CommandAPI.Tests
{
    public class CommandTests
    {
        [Fact]
        public void CanChangeHowTo()
        {
			//Arrange
			var testCommand = new Command
			{
				HowTo = "Do something awesome",
				Platform = "xUnit",
				CommandLine = "dotnet test"
			};

			//Act
			testCommand.HowTo = "Execute Unit Tests";

			//Assert
			Assert.Equal("Execute Unit Tests", testCommand.HowTo);
		}

		[Fact]
        public void CanChangePlatform()
        {
			//Arrange
			var testCommand = new Command
			{
				HowTo = "Do something awesome",
				Platform = "xUnit",
				CommandLine = "dotnet test"
			};

			//Act
			testCommand.Platform = "NUnit";

			//Assert
			Assert.Equal("NUnit", testCommand.Platform);
		}

		[Fact]
        public void CanChangeCommandLine()
        {
			//Arrange
			var testCommand = new Command
			{
				HowTo = "Do something awesome",
				Platform = "xUnit",
				CommandLine = "dotnet test"
			};

			//Act
			testCommand.CommandLine = "dotnet --help";

			//Assert
			Assert.Equal("dotnet --help", testCommand.CommandLine);
		}
	}
}
