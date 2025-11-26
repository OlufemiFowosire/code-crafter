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
        // 1. Split tokens by "|"
        var segments = SplitPipeline(allTokens);
        if (segments.Count == 0) return;

        var tasks = new List<Task>();
        var pipes = new List<AnonymousPipeServerStream>();

        // The input for the current command (Starts with Console Input or Empty)
        // We use Console.OpenStandardInput() to allow interactive piping if needed, 
        // but for this shell, usually Stream.Null or non-interactive Console In.
        Stream sourceStream = Stream.Null;

        for (int i = 0; i < segments.Count; i++)
        {
            var segmentArgs = segments[i];
            bool isLast = (i == segments.Count - 1);

            // 2. Parse Redirection for THIS specific command segment
            // We use your existing RedirectionParser logic here
            RedirectionConfig config = RedirectionParser.Parse(segmentArgs);

            if (config.Arguments.Count == 0) continue;

            string cmdName = config.Arguments[0];
            string[] cmdArgs = config.Arguments.Skip(1).ToArray();

            // 3. Determine Output Stream
            Stream destStream;
            Stream errorStream;

            // Handle Output Redirection (>)
            if (config.StdOutPath != null)
            {
                var mode = config.AppendStdOut ? FileMode.Append : FileMode.Create;
                destStream = new FileStream(config.StdOutPath, mode, FileAccess.Write);
            }
            else if (isLast)
            {
                // If it's the last command and no redirection, write to Console
                destStream = Console.OpenStandardOutput();
            }
            else
            {
                // Otherwise, pipe to the next command
                var pipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
                pipes.Add(pipe);
                destStream = pipe;
            }

            // Handle Error Redirection (2>)
            if (config.StdErrPath != null)
            {
                var mode = config.AppendStdErr ? FileMode.Append : FileMode.Create;
                errorStream = new FileStream(config.StdErrPath, mode, FileAccess.Write);
            }
            else
            {
                errorStream = Console.OpenStandardError();
            }

            // 4. Create Command
            // We handle the creation logic here to catch errors per-command
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

            // Capture context for the Task closure
            Stream currentIn = sourceStream;
            Stream currentOut = destStream;
            Stream currentErr = errorStream;

            // 5. Run Command Asynchronously
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    await command.ExecuteAsync(cmdArgs, currentIn, currentOut, currentErr);
                }
                catch (Exception ex)
                {
                    // Ensure errors in async tasks are reported
                    Console.Error.WriteLine($"{cmdName}: {ex.Message}");
                }
                finally
                {
                    // CRITICAL: Close pipes when done writing so the next command knows data ended.
                    // Do NOT close Console.Out/Error
                    if (currentOut is PipeStream || currentOut is FileStream)
                    {
                        currentOut.Dispose();
                    }
                    if (currentErr is FileStream)
                    {
                        currentErr.Dispose();
                    }
                    // We don't dispose currentIn here usually, as it's owned by the previous pipe's creator 
                    // or it is the Client end of the pipe.
                    if (currentIn is PipeStream)
                    {
                        currentIn.Dispose();
                    }
                }
            }));

            // 6. Setup Input for NEXT command
            if (!isLast && destStream is AnonymousPipeServerStream pipeServer)
            {
                // create the "Read" end of the pipe for the next command
                var pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeServer.GetClientHandleAsString());
                sourceStream = pipeClient;
            }
        }

        // Wait for all pipeline stages to finish
        await Task.WhenAll(tasks);

        // Cleanup Server Pipes (Client pipes disposed in tasks)
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