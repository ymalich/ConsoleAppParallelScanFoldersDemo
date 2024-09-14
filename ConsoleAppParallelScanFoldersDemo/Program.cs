using System.Collections.Concurrent;

namespace ConsoleAppPathScan1;

internal class Program
{
    private static void Main(string[] args)
    {
        var folder = @"c:\Program Files (x86)\";

        ParallelScanPathInternTaskEnter(folder, true, new InterlockedInt(Environment.ProcessorCount));

        var files = _files
            .SelectMany(x => x)
            .ToList();

        Console.WriteLine($"{files.Count} found");
        Console.ReadLine();
    }

    private static ConcurrentBag<string[]> _files = new ();

    private static bool IsOperationCancelled { get; }

    private static void ParallelScanPathInternTaskEnter(string path, bool recursive, InterlockedInt counter)
    {
        counter.Dec();

        var files = new List<string>();

        try
        {
            ParallelScanPathIntern(path, recursive, files, counter);
        }
        finally
        {
            counter.Inc();
        }

        if (files.Count > 0)
        {
            _files.Add(files.ToArray());
        }
    }

    private static void ParallelScanPathIntern(string path, bool recursive, List<string> foundFiles, InterlockedInt counter)
    {
        if (IsOperationCancelled) { return; }

        try
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Scanning '{path}', ({counter.Value})");

            const string searchPattern = "*";

            var files = Directory.EnumerateFiles(path, searchPattern);
            foreach (var file in files)
            {
                if (IsOperationCancelled) { return; }

                foundFiles.Add(file);
            }

            if (!recursive)
            {
                return;
            }

            var subDirectories = Directory.GetDirectories(path, searchPattern);
            foreach (var file in files)
            {
                if (IsOperationCancelled) { return; }

                foundFiles.Add(file);
            }

            var availableDegreeOfParallelism = counter.Value;
            if (subDirectories.Length > 1 && availableDegreeOfParallelism > 0)
            {
                try
                {
                    // parallel scan for SSDs, increment the counter, because the current thread has decremented it above, but it waits in ForEach,
                    // so give all N thread to continue in ForEach
                    availableDegreeOfParallelism = Math.Max(1, counter.Inc());

                    Parallel.ForEach(subDirectories, new ParallelOptions() { MaxDegreeOfParallelism = availableDegreeOfParallelism },
                        subDir =>
                        {
                            ParallelScanPathInternTaskEnter(subDir, true, counter);
                        });
                }
                finally
                {
                    counter.Dec();
                }
            }
            else
            {
                foreach (var subDir in subDirectories)
                {
                    ParallelScanPathIntern(subDir, true, foundFiles, counter);
                }
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
        }
    }
}