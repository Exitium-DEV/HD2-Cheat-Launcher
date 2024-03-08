using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string steamDirectory = GetSteamInstallationPath();
        string steamOverride = "./steam_override.txt";

        if (File.Exists(steamOverride))
        {
            steamDirectory = File.ReadAllText(steamOverride);
        }
        

        if (string.IsNullOrEmpty(steamDirectory))
        {
            Console.WriteLine("Steam is not installed or its installation path couldn't be found");
            WaitForUserInput();
            return;
        }

        string gameDirectory = Path.Combine(steamDirectory, "steamapps", "common", "Helldivers 2", "bin");
        if (string.IsNullOrEmpty(gameDirectory))
        {
            Console.WriteLine("Could not find Helldivers 2 installation");
            WaitForUserInput();
            return;
        }

        if(Directory.GetCurrentDirectory().EndsWith("Helldivers 2\\bin"))
        {
            Console.WriteLine("\nFaulty installation detected!\n\nPlace this executable, and the original 'version.dll' in their own folder, elsewhere on your PC, and not the game files. The rest is handled for you!");
            WaitForUserInput();
            return;
        }

        string inputFilePath = "./version.dll";
        string outputFileTemp = "./version.dll.obf";
        string outputFileLocation = Path.Combine(gameDirectory, "version.dll");

        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine("Input file not found in the current directory.");
            WaitForUserInput();
            return;
        }

        byte[] randomGarbage = GenerateRandomBytes(1024);

        try
        {
            byte[] inputBytes = File.ReadAllBytes(inputFilePath);
            File.WriteAllBytes(outputFileTemp, inputBytes);
            using (FileStream stream = new FileStream(outputFileTemp, FileMode.Append))
            {
                stream.Write(randomGarbage, 0, randomGarbage.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error injecting random garbage: {ex.Message}");
            WaitForUserInput();
            return;
        }

        try
        {
            Console.WriteLine("Injecting modification into game");

            if (!File.Exists(outputFileLocation))
            {
                Console.WriteLine("\nWe notice this is your first installation...\n");
                Console.WriteLine("To uninstall the hack, simply delete the injected file 'version.dll' from your 'Helldivers 2\\bin' directory");
                Console.WriteLine("Continue to use this executable to start the game, as it re-hashes the DLL on each launch\n");
                Console.WriteLine("IMPORTANT: If your Steam installation directory is not default...\nMake a file 'steam_override.txt' containing the location of your installation directory, and restart the EXE\n\nEXAMPLE: D:\\Some\\Path\\Steam\n");
                WaitForUserInput();
            }

            if (File.Exists(outputFileLocation))
            {
                File.Delete(outputFileLocation);
            }

            File.Move(outputFileTemp, outputFileLocation);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error injecting DLL: {ex.Message}");
            WaitForUserInput();
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = "steam://rungameid/553850",
            UseShellExecute = true
        });
    }

    static byte[] GenerateRandomBytes(int length)
    {
        byte[] randomBytes = new byte[length];
        new Random().NextBytes(randomBytes);
        return randomBytes;
    }

    static string GetSteamInstallationPath()
    {
        string defaultSteamPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam");

        if (Directory.Exists(defaultSteamPath))
        {
            return defaultSteamPath;
        }
        else
        {
            Console.WriteLine($"Steam directory not found at the default location: {defaultSteamPath}");
            return null;
        }
    }

    static void WaitForUserInput()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
