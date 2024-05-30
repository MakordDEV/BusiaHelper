using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

class Program
{
    static void Main()
    {
        // Starting
        welcome();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Идет конвертация изображений:");
        Console.ResetColor();

        Console.WriteLine($"------");

        string executableFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string appDirectory = Path.GetDirectoryName(executableFilePath);

        string inputDirectoryPath = Path.Combine(appDirectory, "input");
        string outputDirectoryPath = Path.Combine(appDirectory, "output");
        bool shouldRetry = false;

        do
        {
            try
            {
                // Checking directory step
                if (!Directory.Exists(inputDirectoryPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Директория с исходными изображениями не найдена.");
                    Console.ResetColor();
                    return;
                }

                if (!Directory.Exists(outputDirectoryPath))
                {
                    Directory.CreateDirectory(outputDirectoryPath);
                }

                string[] files = Directory.GetFiles(inputDirectoryPath);

                if (files.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Нет изображений для конвертации в директории 'input'.");
                    Console.ResetColor();
                    return;
                }

                // Count variables
                int convertedCount = 0;
                int skippedCount = 0;
                int errorCount = 0;

                // Working step
                foreach (string inputFile in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFile);
                    string newFilePath = Path.Combine(outputDirectoryPath, fileName + ".webp");

                    // Checking files
                    if (File.Exists(newFilePath))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"Файл '{fileName}.webp' уже существует в директории 'output' и был пропущен.");
                        Console.ResetColor();
                        skippedCount++;
                        // Sleep
                        Thread.Sleep(610);
                        continue;
                    }

                    // Convert message 
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"\rКонвертация {convertedCount + 1}/{files.Length}... {GetSpinnerAnimation(convertedCount)}");

                    // Resize and save 
                    try
                    {
                        using (var image = Image.Load(inputFile))
                        {
                            image.Mutate(x => x.Resize(512, 512));

                            image.Save(newFilePath, new WebpEncoder());
                            convertedCount++;
                        }
                    }

                    // Catch errors
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Произошла ошибка при конвертации файла '{fileName}': {ex.Message}");
                        Console.ResetColor();
                        errorCount++;
                    }

                    // Sleep
                    Thread.Sleep(610);
                }

                // Done message
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Конвертация завершена. Успешно сконвертировано: {convertedCount}, пропущено: {skippedCount}, с ошибкой: {errorCount}");
                Console.ResetColor();
                Console.WriteLine($"------\n");
                shouldRetry = true;
            }
            // Catch errors
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                Console.ResetColor();
                shouldRetry = true;
            }
            // Finally step
            finally
            {
                if (shouldRetry)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Нажмите R, чтобы выполнить повторную конвертацию. Нажмите любую другую клавишу, чтобы закрыть это окно.");
                    Console.ResetColor();
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.KeyChar == 'R' || keyInfo.KeyChar == 'r')
                    {
                        Console.Write("\n\n\n");
                        shouldRetry = true;
                    }
                    else
                    {
                        shouldRetry = false;
                    }
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Нажмите любую клавишу, чтобы закрыть это окно.");
                    Console.ResetColor();
                    Console.ReadKey();
                }
            }
        } while (shouldRetry);
    }

    // Welcome message
    static void welcome()
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine("BusiaHelper :3");
        Console.WriteLine("(C) Busia Studios & IKR Studios 2024\n");

        Console.ResetColor();

        Thread.Sleep(1500);

        return;
    }

    // Process animation
    static char GetSpinnerAnimation(int index)
    {
        char[] spinner = { '/', '|', '\\', '-' };
        return spinner[index % spinner.Length];
    }
}
