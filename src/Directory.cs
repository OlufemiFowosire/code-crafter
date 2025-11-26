internal class Directory
{
    public static bool IsDirectory(string path)
    {
        return System.IO.Directory.Exists(path);
    }

    public static bool ChangeDirectory(string path)
    {
        bool status = false;
        try
        {
            Environment.CurrentDirectory = path;
            status = true;
        }
        catch (DirectoryNotFoundException)
        {
            throw;
        }
        return status;
    }

    public static string GetHomeDirectory()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    public static string GetCurrentDirectory()
    {
        return Environment.CurrentDirectory;
    }

    public static string GetParentDirectory()
    {
        return System.IO.Directory.GetParent(GetCurrentDirectory())?.FullName ?? GetCurrentDirectory();
    }

    public static string ResolvePath(string path)
    {
        if (Path.IsPathRooted(path))
        {
            return path;
        }
        return Path.Combine(GetCurrentDirectory(), path);
    }

    public static string GetTargetDirectory(string[] args)
    {
        if (args.Length == 0)
        {
            return GetHomeDirectory();
        }
        else if (args[0] == "..")
        {
            return GetParentDirectory();
        }
        else if (args[0] == "~")
        {
            return GetHomeDirectory();
        }
        else
        {
            return ResolvePath(args[0]);
        }
    }

    public static bool ChangeToTargetDirectory(string[] args)
    {
        string targetDirectory = GetTargetDirectory(args);
        return ChangeDirectory(targetDirectory);
    }

    public static void PrintCurrentDirectory()
    {
        Console.WriteLine(GetCurrentDirectory());
    }

    public static IEnumerable<string> ListDirectories(string path)
    {
        return System.IO.Directory.EnumerateDirectories(path);
    }

    public static IEnumerable<string> ListFiles(string path)
    {
        return System.IO.Directory.EnumerateFiles(path);
    }
}