using System;
using Xunit;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CommandAPI.Controllers;
using CommandAPI.Models;

namespace CommandAPI.Tests
{
	public class CommandsControllerTests : IDisposable
	{
		DbContextOptionsBuilder<CommandContext> optionsBuilder;
		CommandContext dbContext;
		CommandsController controller;

		public CommandsControllerTests()
		{
			optionsBuilder = new DbContextOptionsBuilder<CommandContext>();
			optionsBuilder.UseInMemoryDatabase("UnitTestInMemBD");
			dbContext = new CommandContext(optionsBuilder.Options);

			controller = new CommandsController(dbContext);
		}

		public void Dispose()
		{
			optionsBuilder = null;
			foreach (var cmd in dbContext.CommandItems)
			{
				dbContext.CommandItems.Remove(cmd);
			}
			dbContext.SaveChanges();
			dbContext.Dispose();
			controller = null;
		}

		//ACTION 1 Tests: GET /api/commands
		//TEST 1.1 REQUEST OBJECTS WHEN NONE EXIST – RETURN "NOTHING"
		[Fact]
		public void GetCommandItems_ReturnsZeroItems_WhenDBIsEmpty()
		{
			//ACT
			var result = controller.GetCommandItems();

			//ASSERT
			Assert.Empty(result.Value);
		}

		//ACTION 1 Tests: GET /api/commands
		//TEST 1.2: RETURNING A COUNT OF 1 FOR A SINGLE COMMAND OBJECT
		[Fact]
		public void GetCommandItemsReturnsOneItemWhenDBHasOneObject()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			dbContext.CommandItems.Add(command); dbContext.SaveChanges();

			//Act
			var result = controller.GetCommandItems();

			//Assert
			Assert.Single(result.Value);
		}

		//ACTION 1 Tests: GET /api/commands
		//TEST 1.3: RETURNING A COUNT OF N FOR N COMMAND OBJECTS
		[Fact]
		public void GetCommandItemsReturnNItemsWhenDBHasNObjects()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};

			var command2 = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};

			dbContext.CommandItems.Add(command);
			dbContext.CommandItems.Add(command2);
			dbContext.SaveChanges();

			//Act
			var result = controller.GetCommandItems();

			//Assert
			Assert.Equal(2, result.Value.Count());
		}

		//ACTION 1 Tests: GET /api/commands
		//TEST 1.4: RETURNS THE EXPECTED TYPE
		[Fact]
		public void GetCommandItemsReturnsTheCorrectType()
		{
			//Arrange

			//Act
			var result = controller.GetCommandItems();

			//Assert
			Assert.IsType<ActionResult<IEnumerable<Command>>>(result);
		}
	}
}
