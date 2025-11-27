using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;

public class PipelineExecutor
{
    public async Task ExecutePipelineAsync(string[] allTokens)
    {
        var segments = SplitPipeline(allTokens);
        if (segments.Count == 0) return;

        var tasks = new List<Task>();
        var pipes = new List<AnonymousPipeServerStream>();

        // FIX 1: Start with Console Input (allows interactive commands like 'cat' to work)
        Stream? sourceStream = Stream.Null;

        for (int i = 0; i < segments.Count; i++)
        {
            var segmentArgs = segments[i];
            bool isLast = (i == segments.Count - 1);

            RedirectionConfig config = RedirectionParser.Parse(segmentArgs);

            if (config.Arguments.Count == 0) continue;

            string cmdName = config.Arguments[0];
            string[] cmdArgs = config.Arguments.Skip(1).ToArray();

            Stream destStream;
            Stream errorStream;

            // Track if we created these streams so we know if we should dispose them
            bool shouldDisposeOut = false;
            bool shouldDisposeErr = false;

            // Output Stream Setup
            if (config.StdOutPath != null)
            {
                var mode = config.AppendStdOut ? FileMode.Append : FileMode.Create;
                destStream = new FileStream(config.StdOutPath, mode, FileAccess.Write);
                shouldDisposeOut = true; // We opened a file, we must close it
            }
            else if (isLast)
            {
                // Do not mark this for disposal!
                destStream = Console.OpenStandardOutput();
            }
            else
            {
                var pipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
                pipes.Add(pipe);
                destStream = pipe;
                shouldDisposeOut = true; // We created a pipe, we must close it
            }

            // Error Stream Setup
            if (config.StdErrPath != null)
            {
                var mode = config.AppendStdErr ? FileMode.Append : FileMode.Create;
                errorStream = new FileStream(config.StdErrPath, mode, FileAccess.Write);
                shouldDisposeErr = true;
            }
            else
            {
                errorStream = Console.OpenStandardError();
            }

            // Command Creation
            ICommand command;
            try
            {
                command = CommandFactory.Create(cmdName);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                continue;
            }

            Stream currentIn = sourceStream;
            Stream currentOut = destStream;
            Stream currentErr = errorStream;

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    await command.ExecuteAsync(cmdArgs, currentIn, currentOut, currentErr);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"{ex.Message}");
                }
                finally
                {
                    // FIX 2: Only dispose streams we explicitly flagged
                    if (shouldDisposeOut) currentOut.Dispose();
                    if (shouldDisposeErr) currentErr.Dispose();

                    // Clean up input stream if it was a client pipe created in previous loop
                    if (currentIn is PipeStream) currentIn.Dispose();
                }
            }));

            // Prepare Input for NEXT command
            if (!isLast)
            {
                if (destStream is AnonymousPipeServerStream pipeServer)
                {
                    var pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeServer.GetClientHandleAsString());

                    // Critical: Release server handle
                    pipeServer.DisposeLocalCopyOfClientHandle();

                    sourceStream = pipeClient;
                }
                else
                {
                    // FIX 3: Break the chain if redirecting to file
                    sourceStream = Stream.Null;
                }
            }
        }

        // Wait for all commands to complete
        await Task.WhenAll(tasks);

        foreach (var p in pipes) p.Dispose();
    }

    private List<string[]> SplitPipeline(string[] tokens)
    {
        var segments = new List<string[]>();
        var currentSegment = new List<string>();

        foreach (var token in tokens)
        {
            if (token == "|")
            {
                if (currentSegment.Count > 0)
                {
                    segments.Add(currentSegment.ToArray());
                    currentSegment = new List<string>();
                }
            }
            else
            {
                currentSegment.Add(token);
            }
        }

        if (currentSegment.Count > 0)
        {
            segments.Add(currentSegment.ToArray());
        }

        return segments;
    }
}