using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace PrintIt.Core
{
    internal sealed class CommandService : ICommandService
    {
        private readonly Func<ProcessStartInfo, Process> _startProcess;
        private readonly ILogger<CommandService> _logger;

        public CommandService(ILogger<CommandService> logger)
            : this(Process.Start, logger)
        {
        }

        [ExcludeFromCodeCoverage]
        internal CommandService(
            Func<ProcessStartInfo, Process> startProcess,
            ILogger<CommandService> logger)
        {
            _startProcess = startProcess ?? throw new ArgumentNullException(nameof(_startProcess));
            _logger = logger;
        }

        public CommandExecutionResult Execute(string fileName, params string[] arguments)
        {
            _logger.LogInformation("Executing {FileName} with arguments {Arguments}", fileName, arguments);

            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = string.Join(" ", arguments),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            using var process = _startProcess(startInfo);

            if (process == null)
            {
                return new CommandExecutionResult(Array.Empty<string>(), -1);
            }

            process.WaitForExit();

            return new CommandExecutionResult(
                output: ReadLinesWithContent(process.StandardOutput)
                    .Concat(ReadLinesWithContent(process.StandardError))
                    .ToArray(),
                exitCode: process.ExitCode);
        }

        private static IEnumerable<string> ReadLinesWithContent(StreamReader streamReader)
        {
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine()?.Trim() ?? string.Empty;
                if (!string.Empty.Equals(line))
                    yield return line;
            }
        }
    }

    public interface ICommandService
    {
        CommandExecutionResult Execute(string fileName, params string[] arguments);
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
    public sealed class CommandExecutionResult
    {
        internal CommandExecutionResult(string[] output, int exitCode)
        {
            Output = output;
            ExitCode = exitCode;
        }

        public string[] Output { get; }

        public int ExitCode { get; }

        public bool Success => ExitCode == 0;
    }
}
