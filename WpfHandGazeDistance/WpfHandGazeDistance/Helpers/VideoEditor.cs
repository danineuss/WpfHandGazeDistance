using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WpfHandGazeDistance.Models;

namespace WpfHandGazeDistance.Helpers
{
    public class VideoEditor
    {
        private readonly Video _video;

        private readonly string _videoPath;

        private const string FfmpegPath = @"C:\FFmpeg\bin\ffmpeg.exe";

        private const string FfprobePath = @"C:\FFmpeg\bin\ffprobe.exe";

        public VideoEditor(string videoPath)
        {
            _videoPath = videoPath;
            _video = new Video(videoPath);
        }

        /// <summary>
        /// This will start a process to launch the FFmpeg library in order to extract a video
        /// snippet from a large video file and then save the snippet to a defined location on disc.
        /// </summary>
        /// <param name="outputPath">Output path of the resulting video snippet.</param>
        /// <param name="startTime">Number of seconds for the starting time.</param>
        /// <param name="endTime">Number of seconds for the end time.</param>
        public void CutVideo(string outputPath, float startTime, float endTime)
        {   
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo(FfmpegPath)
                {
                    Arguments = FfmpegArgumentString(outputPath, startTime, endTime),
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

        #region Private Members
        
        /// <summary>
        /// This will concat a string for the FFmpeg arguments, telling it to cut a video snippet.
        /// </summary>
        /// <param name="outputPath">Output path of the resulting video snippet.</param>
        /// <param name="startTime">Number of seconds for the starting time.</param>
        /// <param name="endTime">Number of seconds for the end time.</param>
        /// <returns>FFmpeg arguments as a string.</returns>
        private string FfmpegArgumentString(string outputPath, float startTime, float endTime)
        {
            string argumentString = "-i " + _videoPath + " -ss ";
            argumentString += TimeToString(startTime) + " -t " + TimeToString(endTime - startTime);
            argumentString += " -b:v " + ProbeVideo("bitrate:") + "k";
            argumentString += " -r " + _video.Fps;
            argumentString += " " + outputPath;
            Debug.Print(argumentString);

            return argumentString;
        }

        /// <summary>
        /// This function will FFprobe within a process in order to find out either the duration or the
        /// bitrate of a given video file.
        /// </summary>
        /// <param name="targetKey">Either "Duration:" or "bitrate:"</param>
        private string ProbeVideo(string targetKey)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo(FfprobePath)
                {
                    Arguments = _videoPath,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            process.Start();

            string output = process.StandardError.ReadToEnd();
            var encodingLines = output.Split(Environment.NewLine[0]).Where(
                    line => string.IsNullOrWhiteSpace(line) == false && string.IsNullOrEmpty(line.Trim()) == false)
                .Select(s => s.Trim()).ToList();

            string videoInfo = "";
            foreach (var line in encodingLines)
            {
                if (line.StartsWith("Duration"))
                {
                    videoInfo = GetValueFromItemData(line, targetKey);
                }
            }
            process.Close();
            return videoInfo;
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

        /// <summary>
        /// This function will take the list of elements within the ffprobe line which contains
        /// Duration, start and bitrate of the following format:
        /// Duration: 00:10:53.79, start: 0.000000, bitrate: 12816 kb/s
        /// </summary>
        /// <param name="ffprobeLine">The list of elements in the "Duration" string line.</param>
        /// <param name="targetKey">The element which is looked for ("Duration:" or "bitrate:")</param>
        /// <returns></returns>
        private static string GetValueFromItemData(string ffprobeLine, string targetKey)
        {
            var itemsOfData = ffprobeLine.Split(" "[0], "="[0]).Where(s => string.IsNullOrEmpty(s) == false).Select(s => s.Trim().Replace("=", string.Empty).Replace(",", string.Empty)).ToList();

            var key = itemsOfData.FirstOrDefault(i => i.ToUpper() == targetKey.ToUpper());

            if (key == null) return null;
            var index = itemsOfData.IndexOf(key);
            if (index >= itemsOfData.Count - 1) return null;
            return itemsOfData[index + 1];
        }

        #endregion

    }
}
