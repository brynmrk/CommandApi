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

		//ACTION 2 Tests: GET /api/commands/{id}
		//TEST 2.1: Null Object Value Result
		[Fact]
		public void GetCommandItemReturnsNullResultWhenInvalidID()
		{
			//Arrange
			//DB should be empty, any ID will be invalid

			//Act
			var result = controller.GetCommandItem(0);

			//Assert
			Assert.Null(result.Value);
		}

		//TEST 2.2: 404 Not Found Return Code
		[Fact]
		public void GetCommandItemReturns404NotFoundWhenInvalidID()
		{
			//Arrange
			//DB should be empty, any ID will be invalid

			//Act
			var result = controller.GetCommandItem(0);

			//Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		//TEST 2.3: Correct Return Type
		[Fact]
		public void GetCommandItemReturnsTheCorrectType()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			dbContext.CommandItems.Add(command);
			dbContext.SaveChanges();

			var cmdId = command.Id;

			//Act
			var result = controller.GetCommandItem(cmdId);

			//Assert
			Assert.IsType<ActionResult<Command>>(result);
		}

		//TEST 2.4: Correct Resource Returned
		[Fact]
		public void GetCommandItemReturnsTheCorrectResouce()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			dbContext.CommandItems.Add(command);
			dbContext.SaveChanges();

			var cmdId = command.Id;

			//Act
			var result = controller.GetCommandItem(cmdId);

			//Assert
			Assert.Equal(cmdId, result.Value.Id);
		}

		//ACTION 3: CREATE A NEW RESOURCE
		//TEST 3.1: Object count increments by 1
		[Fact]
		public void PostCommandItemObjectCountIncrementWhenValidObject()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			var oldCount = dbContext.CommandItems.Count();

			//Act
			var result = controller.PostCommandItem(command);

			//Assert
			Assert.Equal(oldCount + 1, dbContext.CommandItems.Count());
		}

		//TEST 3.2: 201 Created Return Code
		[Fact]
		public void PostCommandItemReturns201CreatedWhenValidObject()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};

			//Act
			var result = controller.PostCommandItem(command);

			//Assert
			Assert.IsType<CreatedAtActionResult>(result.Result);
		}

		//ACTION 4: UPDATE AN EXISTING RESOURCE
		//TEST 4.1: Attribute is updated
		[Fact]
		public void PutCommandItem_AttributeUpdated_WhenValidObject()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			dbContext.CommandItems.Add(command);
			dbContext.SaveChanges();
			var cmdId = command.Id; command.HowTo = "UPDATED";

			//Act
			controller.PutCommandItem(cmdId, command);
			var result = dbContext.CommandItems.Find(cmdId);

			//Assert
			Assert.Equal(command.HowTo, result.HowTo);
		}

		//TEST 4.2: 204 No Content Return Code
		[Fact]
		public void PutCommandItem_Returns204_WhenValidObject()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			dbContext.CommandItems.Add(command);
			dbContext.SaveChanges();
			var cmdId = command.Id; command.HowTo = "UPDATED";

			//Act
			var result = controller.PutCommandItem(cmdId, command);

			//Assert
			Assert.IsType<NoContentResult>(result);
		}

		//TEST 4.3: 400 Bad Request Return Code
		[Fact]
		public void PutCommandItem_Returns400_WhenInvalidObject()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			dbContext.CommandItems.Add(command);
			dbContext.SaveChanges();
			var cmdId = command.Id+1; command.HowTo = "UPDATED";

			//Act
			var result = controller.PutCommandItem(cmdId, command);

			//Assert
			Assert.IsType<BadRequestResult>(result);
		}

		//TEST 4.4: Object remains unchanged
		[Fact]
		public void PutCommandItem_AttributeUnchanged_WhenInvalidObject()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			dbContext.CommandItems.Add(command);
			dbContext.SaveChanges();

			var command2 = new Command
			{
				Id = command.Id,
				HowTo = "UPDATED",
				Platform = "UPDATED",
				CommandLine = "UPDATED"
			};

			//Act
			controller.PutCommandItem(command.Id + 1, command2);
			var result = dbContext.CommandItems.Find(command.Id);

			//Assert
			Assert.Equal(command.HowTo, result.HowTo);
		}

		//ACTION 5: DELETE AN EXISTING RESOURCE
		//TEST 5.1: Object Count Decrements by 1
		[Fact]
		public void DeleteCommandItem_ObjectsDecrement_WhenValidObjectID()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			dbContext.CommandItems.Add(command);
			dbContext.SaveChanges();
			var cmdId = command.Id;
			var objCount = dbContext.CommandItems.Count();

			//Act
			controller.DeleteCommandItem(cmdId);

			//Assert
			Assert.Equal(objCount-1, dbContext.CommandItems.Count());
		}

		//TEST 5.2: 200 OK Return Code
		[Fact]
		public void DeleteCommandItem_Returns200OK_WhenValidObjectID()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			dbContext.CommandItems.Add(command);
			dbContext.SaveChanges();
			var cmdId = command.Id;

			//Act
			var result = controller.DeleteCommandItem(cmdId);

			//Assert
			Assert.Null(result.Result);
		}

		//TEST 5.3: 400 Bad Request Return Code
		[Fact]
		public void DeleteCommandItem_Returns404NotFound_WhenValidObjectID()
		{
			//Arrange

			//Act
			var result = controller.DeleteCommandItem(-1);

			//Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		//TEST 5.4: Object count remains unchanged
		[Fact]
		public void DeleteCommandItem_ObjectCountNotDecremented_WhenValidObjectID()
		{
			//Arrange
			var command = new Command
			{
				HowTo = "Do Somethting",
				Platform = "Some Platform",
				CommandLine = "Some Command"
			};
			dbContext.CommandItems.Add(command);
			dbContext.SaveChanges();
			var cmdId = command.Id;
			var objCount = dbContext.CommandItems.Count();

			//Act
			var result = controller.DeleteCommandItem(cmdId+1);

			//Assert
			Assert.Equal(objCount, dbContext.CommandItems.Count());
		}
	}
}
