using System;
using System.IO;
using FluentAssertions;
using Moq;
using Xunit;

namespace PrintIt.Core.Tests
{
    public sealed class PrinterServiceTests
    {
        [Fact]
        public void GetInstalledPrinters_ShouldReturnSomething()
        {
            // Arrange
            var printerService = new PrinterService(Mock.Of<ICommandService>());

            // Act
            string[] installedPrinters = printerService.GetInstalledPrinters();

            // Assert
            installedPrinters.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Fact]
        public void InstallPrinter_SomePrinterPath_ShouldExecuteCorrectCommand()
        {
            // Arrange
            var printerPath = @"\\RemoteComputer\Remote Printer";
            var runDll32Path =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "rundll32.exe");
            var commandServiceMock = new Mock<ICommandService>();
            commandServiceMock
                .Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(new CommandExecutionResult(new[] { "Success" }, 0));
            var printerService = new PrinterService(commandServiceMock.Object);

            // Act
            printerService.InstallPrinter(printerPath);

            // Assert
            commandServiceMock.Verify(x => x.Execute(runDll32Path, "printui.dll,PrintUIEntry", "/in", $"/n\"{printerPath}\""), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void InstallPrinter_InvalidPrinterPath_ShouldThrowArgumentException(string printerPath)
        {
            // Arrange
            var printerService = new PrinterService(Mock.Of<ICommandService>());

            // Act
            Action action = () => printerService.InstallPrinter(printerPath);

            // Assert
            action.Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be("printerPath");
        }
    }
}
