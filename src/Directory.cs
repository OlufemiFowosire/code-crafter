internal class Directory
{
    public static bool IsDirectory(string path)
    {
        return System.IO.Directory.Exists(path);
    }

    public static bool ChangeDirectory(string path)
    {
        try
        {
            Environment.CurrentDirectory = path;
            return true;
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"cd: {path} no such file or directory");
            return false;
        }
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
        else
        {
            return ResolvePath(args[0]);
        }
    }

    public static void PrintError(string message)
    {
        Console.WriteLine($"{message}");
    }

    public static bool ValidateDirectory(string path)
    {
        if (!IsDirectory(path))
        {
            PrintError($"no such file or directory: {path}");
            return false;
        }
        return true;
    }

    public static bool ChangeToTargetDirectory(string[] args)
    {
        string targetDirectory = GetTargetDirectory(args);
        if (!ValidateDirectory(targetDirectory))
        {
            return false;
        }
        return ChangeDirectory(targetDirectory);
    }

    public static bool IsValidTargetDirectory(string[] args)
    {
        string targetDirectory = GetTargetDirectory(args);
        return ValidateDirectory(targetDirectory);
    }
    public static void PrintTargetDirectory(string[] args)
    {
        Console.WriteLine(GetTargetDirectory(args));
    }

    public static void PrintParentDirectory()
    {
        Console.WriteLine(GetParentDirectory());
    }
    public static void PrintCurrentDirectory()
    {
        Console.WriteLine(GetCurrentDirectory());
    }
}