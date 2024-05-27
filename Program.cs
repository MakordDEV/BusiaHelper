using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

class Program
{

    static void Main()
    {
        string inputDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "input");
        string outputDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "output");
        bool shouldRetry = false;

        do
        {
            try
            {
                if (!Directory.Exists(inputDirectoryPath))
                {
                    Console.WriteLine("Директория с исходными изображениями не найдена.");
                    return;
                }

                if (!Directory.Exists(outputDirectoryPath))
                {
                    Directory.CreateDirectory(outputDirectoryPath);
                }

                string[] files = Directory.GetFiles(inputDirectoryPath);

                if (files.Length == 0)
                {
                    Console.WriteLine("Нет изображений для конвертации в директории 'input'.");
                    return;
                }

                int convertedCount = 0;
                int skippedCount = 0;
                int errorCount = 0;

                Console.WriteLine("Идет конвертация изображений:");

                foreach (string inputFile in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFile);
                    string newFilePath = Path.Combine(outputDirectoryPath, fileName + ".webp");

                    if (File.Exists(newFilePath))
                    {
                        Console.WriteLine($"\nФайл '{fileName}.webp' уже существует в директории 'output' и был пропущен.");
                        skippedCount++;
                        continue;
                    }

                    Console.Write($"\rКонвертация {convertedCount + 1}/{files.Length}... {GetSpinnerAnimation(convertedCount)}");

                    try
                    {
                        using (var image = Image.Load(inputFile))
                        {
                            image.Mutate(x => x.Resize(512, 512));

                            image.Save(newFilePath, new WebpEncoder());
                            convertedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Произошла ошибка при конвертации файла '{fileName}': {ex.Message}");
                        errorCount++;
                    }
                }

                Console.WriteLine($"\nКонвертация завершена. Успешно сконвертировано: {convertedCount}, пропущено: {skippedCount}, с ошибкой: {errorCount}");
                shouldRetry = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                shouldRetry = true;
            }
            finally
            {
                if (shouldRetry)
                {
                    Console.WriteLine("Нажмите R, чтобы выполнить повторную конвертацию. Нажмите любую другую клавишу, чтобы закрыть это окно.");
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.KeyChar == 'R' || keyInfo.KeyChar == 'r')
                    {
                        Console.Write("\n");
                        shouldRetry = true;
                    }
                    else
                    {
                        shouldRetry = false;
                    }
                }
                else
                {
                    Console.WriteLine("Нажмите любую клавишу, чтобы закрыть это окно.");
                    Console.ReadKey();
                }
            }
        } while (shouldRetry);
    }

    static char GetSpinnerAnimation(int index)
    {
        char[] spinner = { '/', '|', '\\', '-' };
        return spinner[index % spinner.Length];
    }
}
