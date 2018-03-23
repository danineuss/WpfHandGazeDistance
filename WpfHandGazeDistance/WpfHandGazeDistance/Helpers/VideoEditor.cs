using System;
using System.Diagnostics;
using System.IO;

namespace WpfHandGazeDistance.Helpers
{
    public class VideoEditor
    {
        private string _ffmpegPath = @"C:\FFmpeg\bin\ffmpeg.exe";

        /// <summary>
        /// This will start a process to launch the FFmpeg library in order to extract a video
        /// snippet from a large video file and then save the snippet to a defined location on disc.
        /// </summary>
        /// <param name="inputPath">Path to the main video file.</param>
        /// <param name="outputPath">Output path of the resulting video snippet.</param>
        /// <param name="startTime">Number of seconds for the starting time.</param>
        /// <param name="endTime">Number of seconds for the end time.</param>
        public void CutVideo(string inputPath, string outputPath, float startTime, float endTime)
        {   
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo(_ffmpegPath)
                {
                    Arguments = FfmpegArgumentString(inputPath, outputPath, startTime, endTime),
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            process.Start();

            StreamReader reader = process.StandardError;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
            process.Close();
            Console.WriteLine("Video cut (" + startTime.ToString() + ")");
        }

        /// <summary>
        /// This will concat a string for the FFmpeg arguments, telling it to cut a video snippet.
        /// </summary>
        /// <param name="inputPath">Path to the main video file.</param>
        /// <param name="outputPath">Output path of the resulting video snippet.</param>
        /// <param name="startTime">Number of seconds for the starting time.</param>
        /// <param name="endTime">Number of seconds for the end time.</param>
        /// <returns>FFmpeg arguments as a string.</returns>
        private static string FfmpegArgumentString(
            string inputPath, string outputPath, float startTime, float endTime)
        {
            string argumentString = "-i " + inputPath + " -ss ";
            argumentString += TimeToString(startTime) + " -t " + TimeToString(endTime - startTime);
            argumentString += " " + outputPath;

            return argumentString;
        }

        /// <summary>
        /// This will convert a float value of seconds into a string of type "hh:mm:ss.d"
        /// with "dd" being the last point.
        /// </summary>
        /// <param name="inputSeconds">Number of seconds, including decimal points</param>
        /// <returns>Time as string of format "hh:mm:ss.d"</returns>
        private static string TimeToString(float inputSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(inputSeconds);
            string timeString = timeSpan.ToString(@"hh\:mm\:ss");

            string decimalValue = (inputSeconds - Math.Floor(inputSeconds)).ToString();
            decimalValue = decimalValue.Remove(0, 1);
            if (decimalValue.Length > 2) decimalValue = decimalValue.Remove(2, decimalValue.Length - 2);
            timeString += decimalValue;

            return timeString;
        }
    }
}
