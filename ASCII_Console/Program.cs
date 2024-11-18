using System;
using System.Text;
using OpenCvSharp;

namespace ASCII_Console
{
    internal class Program
    {
        // dark to light character
        private static readonly char[] ASCII_CHARS = { '@', '%', '#', '*', '+', '=', '-', ':', '.', ' ' };

        static void Main(string[] args)
        {
            Console.WriteLine("GIF to ASCII Art Converter");

            // console size 
            try
            {
                Console.SetWindowSize(120, 40); // Width and height (adjust if needed)
                Console.BufferWidth = 120;
                Console.BufferHeight = 40;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to resize console: {ex.Message}");
            }

            string gifPath = @"C:\Users\giova\Downloads\miku-hatsune.gif"; // Path to GIF
            ConvertGifToAscii(gifPath);

            Console.WriteLine("Conversion complete. Press any key to exit...");
            Console.ReadKey();
        }

        static void ConvertGifToAscii(string gifPath)
        {
            using (var capture = new VideoCapture(gifPath))
            {
                if (!capture.IsOpened())
                {
                    Console.WriteLine("Error: Unable to open the GIF file.");
                    return;
                }

                Console.WriteLine($"Processing GIF: {gifPath}");
                Console.WriteLine($"Frame count: {capture.FrameCount}, FPS: {capture.Fps}");

                Mat frame = new Mat();
                int frameIndex = 0;

                // Aspect ratio correction factor (fine-tuned for console fonts)
                double aspectRatioCorrection = 0.4;

                while (true) // Infinite loop to replay the GIF
                {
                    capture.Set(VideoCaptureProperties.PosFrames, 0); // Reset to the first frame for looping

                    while (capture.Read(frame) && !frame.Empty())
                    {
                        // Dynamically calculate console size
                        int consoleWidth = Console.WindowWidth;
                        int consoleHeight = Console.WindowHeight;

                        // Resize for better ASCII output with aspect ratio correction
                        int targetWidth = consoleWidth - 1; // Leave space for line breaks
                        int targetHeight = (int)((consoleHeight - 2) * aspectRatioCorrection); // Adjust for aspect ratio

                        Cv2.Resize(frame, frame, new Size(targetWidth, targetHeight));

                        // Convert the frame to grayscale
                        Mat grayFrame = new Mat();
                        Cv2.CvtColor(frame, grayFrame, ColorConversionCodes.BGR2GRAY);

                        // Convert the grayscale frame to ASCII art
                        string asciiArt = ConvertFrameToAscii(grayFrame);

                        // Display ASCII art in the console
                        Console.Clear();
                        CenterAsciiArt(asciiArt, consoleHeight);
                        Console.WriteLine($"Frame {++frameIndex}/{capture.FrameCount}");

                        // Wait to match GIF frame rate
                        Cv2.WaitKey(1000 / (int)capture.Fps); // Frame delay
                    }
                }
            }
        }

        static string ConvertFrameToAscii(Mat grayFrame)
        {
            StringBuilder asciiArt = new StringBuilder();

            for (int y = 0; y < grayFrame.Rows; y++)
            {
                for (int x = 0; x < grayFrame.Cols; x++)
                {
                    // Get pixel intensity
                    byte pixelValue = grayFrame.At<byte>(y, x);

                    // Map intensity to an ASCII character
                    char asciiChar = ASCII_CHARS[pixelValue * ASCII_CHARS.Length / 256];
                    asciiArt.Append(asciiChar);
                }
                asciiArt.AppendLine();
            }

            return asciiArt.ToString();
        }

        static void CenterAsciiArt(string asciiArt, int consoleHeight)
        {
            // Split the ASCII art into lines
            string[] lines = asciiArt.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Calculate vertical padding to center the art
            int verticalPadding = (consoleHeight - lines.Length) / 2;

            // Print blank lines for vertical centering
            for (int i = 0; i < verticalPadding; i++)
            {
                Console.WriteLine();
            }

            // Print the ASCII art
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }
    }
}
