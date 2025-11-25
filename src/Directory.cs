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
            Console.WriteLine($"cd: no such file or directory: {path}");
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
        else if (args[0] == "~")
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
        Console.WriteLine($"cd: {message}");
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

    public static void PrintCurrentDirectory()
    {
        Console.WriteLine(GetCurrentDirectory());
    }

    public static string GetAbsolutePath(string path)
    {
        return Path.GetFullPath(ResolvePath(path));
    }

    public static string GetAbsoluteCurrentDirectory()
    {
        return GetAbsolutePath(GetCurrentDirectory());
    }

    public static string GetAbsoluteParentDirectory()
    {
        return GetAbsolutePath(GetParentDirectory());
    }

    public static string GetAbsoluteTargetDirectory(string[] args)
    {
        return GetAbsolutePath(GetTargetDirectory(args));
    }

    public static bool IsValidTargetDirectory(string[] args)
    {
        string targetDirectory = GetTargetDirectory(args);
        return ValidateDirectory(targetDirectory);
    }

    public static bool ChangeToAbsoluteTargetDirectory(string[] args)
    {
        string targetDirectory = GetAbsoluteTargetDirectory(args);
        if (!ValidateDirectory(targetDirectory))
        {
            return false;
        }
        return ChangeDirectory(targetDirectory);
    }

    public static void PrintAbsoluteCurrentDirectory()
    {
        Console.WriteLine(GetAbsoluteCurrentDirectory());
    }

    public static void PrintAbsoluteTargetDirectory(string[] args)
    {
        Console.WriteLine(GetAbsoluteTargetDirectory(args));
    }

    public static void PrintAbsoluteParentDirectory()
    {
        Console.WriteLine(GetAbsoluteParentDirectory());
    }

    public static void PrintTargetDirectory(string[] args)
    {
        Console.WriteLine(GetTargetDirectory(args));
    }

    public static void PrintParentDirectory()
    {
        Console.WriteLine(GetParentDirectory());
    }
}