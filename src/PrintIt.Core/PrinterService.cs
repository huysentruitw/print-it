using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Printing;
using System.IO;
using System.Linq;

namespace PrintIt.Core
{
    internal sealed class PrinterService : IPrinterService
    {
        private readonly ICommandService _commandService;

        public PrinterService(ICommandService commandService)
        {
            _commandService = commandService;
        }

        public string[] GetInstalledPrinters()
            => PrinterSettings.InstalledPrinters.Cast<string>().ToArray();

        public void InstallPrinter(string printerPath)
        {
            if (string.IsNullOrEmpty(printerPath))
                throw new ArgumentException("Path cannot be null or empty", nameof(printerPath));

            string cmdPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "rundll32.exe");
            CommandExecutionResult executionResult = _commandService.Execute(cmdPath, "printui.dll,PrintUIEntry", "/in", $"/n\"{printerPath}\"");

            // ReSharper disable once InvertIf
            if (!executionResult.Success)
            {
                var message = string.Join(Environment.NewLine, executionResult.Output);
                throw new InstallPrinterFailedException(printerPath, message);
            }
        }
    }

    public interface IPrinterService
    {
        string[] GetInstalledPrinters();

        void InstallPrinter(string printerPath);
    }

    public sealed class InstallPrinterFailedException : Exception
    {
        public InstallPrinterFailedException(string printerPath, string message)
            : base($"Failed to install printer '{printerPath}' with message: {message}")
        {
            PrinterPath = printerPath;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
        public string PrinterPath { get; }
    }
}
