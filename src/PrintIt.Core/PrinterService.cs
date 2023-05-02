using System;
using System.Collections;
using System.Collections.Generic;
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

        public string[] GetPrinterPageSizes(string printerPath)
        {
            var allSizeNames = new List<string>();
            var settings = new PrinterSettings
            {
                PrinterName = printerPath,
            };
            foreach (PaperSize paperSize in settings.PaperSizes)
            {
                allSizeNames.Add($"{paperSize.PaperName}, {paperSize.Height}, {paperSize.Width}, {paperSize.Kind}, {paperSize.RawKind}");
            }

            return allSizeNames.ToArray();
        }

        public string[] GetPrinterPageSources(string printerPath)
        {
            var allSourceNames = new List<string>();
            var settings = new PrinterSettings
            {
                PrinterName = printerPath,
            };
            foreach (PaperSource paperSource in settings.PaperSources)
            {
                allSourceNames.Add($"{paperSource.SourceName}, {paperSource.Kind}, {paperSource.RawKind}");
            }

            return allSourceNames.ToArray();
        }

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

        string[] GetPrinterPageSizes(string printerPath);

        string[] GetPrinterPageSources(string printerPath);

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
