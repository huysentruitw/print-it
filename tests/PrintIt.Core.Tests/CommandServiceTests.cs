using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace PrintIt.Core.Tests
{
    public sealed class CommandServiceTests
    {
        [Fact]
        public void Execute_BasicCommand_ShouldReturnOutput()
        {
            // Arrange
            var commandService = new CommandService(Mock.Of<ILogger<CommandService>>());
            var fileName = $@"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\ipconfig.exe";

            // Act
            CommandExecutionResult executionResult = commandService.Execute(fileName);

            // Assert
            executionResult.Success.Should().BeTrue();
            executionResult.Output.Should().Contain("Windows IP Configuration");
        }

        [Fact]
        public void Execute_CommandWithArguments_ShouldReturnCorrectOutput()
        {
            // Arrange
            var commandService = new CommandService(Mock.Of<ILogger<CommandService>>());
            var fileName = $@"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\whoami.exe";
            var arguments = new[] { "/user" };

            // Act
            CommandExecutionResult executionResult = commandService.Execute(fileName, arguments);

            // Assert
            executionResult.Success.Should().BeTrue();
            executionResult.Output.Should().Contain("USER INFORMATION");
        }

        [Fact]
        public void Execute_ErrorInCommand_ShouldNotBeSuccessful()
        {
            // Arrange
            var commandService = new CommandService(Mock.Of<ILogger<CommandService>>());
            var fileName = $@"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\whoami.exe";
            var arguments = new[] { "blabla" };

            // Act
            CommandExecutionResult executionResult = commandService.Execute(fileName, arguments);

            // Assert
            executionResult.Success.Should().BeFalse();
            var output = string.Join(Environment.NewLine, executionResult.Output);
            output.Should().Contain("blabla");
        }

        [Fact]
        public void Execute_ProcessStartReturnsNull_ShouldNotBeSuccessful()
        {
            // Arrange
            var commandService = new CommandService(_ => null, Mock.Of<ILogger<CommandService>>());
            var fileName = $@"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\ipconfig.exe";

            // Act
            CommandExecutionResult executionResult = commandService.Execute(fileName);

            // Assert
            executionResult.Success.Should().BeFalse();
            executionResult.ExitCode.Should().Be(-1);
        }
    }
}
