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

        Stream sourceStream = Console.OpenStandardInput();

        for (int i = 0; i < segments.Count; i++)
        {
            var segmentArgs = segments[i];

            RedirectionConfig config = RedirectionParser.Parse(segmentArgs);

            if (config.Arguments.Count == 0) continue;

            string cmdName = config.Arguments[0];
            string[] cmdArgs = config.Arguments.Skip(1).ToArray();

            Stream destStream;
            bool isLast = (i == segments.Count - 1);
            Stream errorStream;

            // Output Stream Setup
            if (config.StdOutPath != null)
            {
                var mode = config.AppendStdOut ? FileMode.Append : FileMode.Create;
                destStream = new FileStream(config.StdOutPath, mode, FileAccess.Write);
            }
            else if (isLast)
            {
                destStream = Console.OpenStandardOutput();
            }
            else
            {
                var pipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
                pipes.Add(pipe);
                destStream = pipe;
            }

            // Error Stream Setup
            if (config.StdErrPath != null)
            {
                var mode = config.AppendStdErr ? FileMode.Append : FileMode.Create;
                errorStream = new FileStream(config.StdErrPath, mode, FileAccess.Write);
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
                    Console.Error.WriteLine($"{cmdName}: {ex.Message}");
                }
                finally
                {
                    // Clean up output/error streams owned by this command
                    if (currentOut is PipeStream || currentOut is FileStream) currentOut.Dispose();
                    if (currentErr is FileStream) currentErr.Dispose();

                    // Clean up input stream if it was a client pipe created in previous loop
                    if (currentIn is PipeStream) currentIn.Dispose();
                }
            }));

            // Prepare Input for NEXT command
            if (!isLast)
            {
                if (destStream is AnonymousPipeServerStream pipeServer)
                {
                    // Create the client (Reader) handle
                    var pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeServer.GetClientHandleAsString());

                    // RESTORED: This is critical. It releases the server's reference to the client handle.
                    // Without this, the pipe state may not correctly signal EOF to the reader when the server closes.
                    pipeServer.DisposeLocalCopyOfClientHandle();

                    sourceStream = pipeClient;
                }
                else
                {
                    sourceStream = Stream.Null;
                }
            }
        }


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