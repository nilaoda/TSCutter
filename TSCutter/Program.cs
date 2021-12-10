using MediaInfo.DotNetWrapper.Enumerations;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using static TSCutter.Utils;

namespace TSCutter
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                // Create a root command with some options
                var rootCommand = new RootCommand
                {
                    new Argument<string>(
                        "Output",
                        "设置输出文件. 使用 - 可以输出到stdout"),
                    new Option<string>(
                        new string[]{ "--input", "-i" },
                        "设置输入文件."),
                    new Option<string>(
                        new string[]{ "--input-dur", "-D" },
                        "设置输入文件的长度. [hh:mm:ss]"),
                    new Option<string>(
                        new string[]{ "--start-time", "-ss" },
                        "设置起始时间. [hh:mm:ss]"),
                    new Option<string>(
                        new string[]{ "--stop-time", "-to" },
                        "设置停止时间. [hh:mm:ss]"),
                    new Option<string>(
                        new string[]{ "--duration", "-t" },
                        "设置输出长度. [hh:mm:ss]"),
                };

                rootCommand.Description = "通过文件时长推算大小来切割文件的工具(只适合恒定码率的TS档)";

                // Note that the parameters of the handler method are matched according to the names of the options
                rootCommand.Handler = CommandHandler.Create<Options>(options =>
                {
                    RunOptions(options);
                });

                return rootCommand.InvokeAsync(args).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        private static void RunOptions(Options options)
        {
            //是否输出到stdout
            var stdout = false || options.Output == "-";
            var input = options.Input;
            //Console.WriteLine(option.ToString());
            if (!File.Exists(input))
                throw new Exception("文件不存在! " + input);
            var fileSize = new FileInfo(input).Length;
            double totalSeconds = 0;
            if (string.IsNullOrEmpty(options.InputDur))
            {
                try
                {
                    using (var mediaInfo = new MediaInfo.DotNetWrapper.MediaInfo())
                    {
                        mediaInfo.Open(input);
                        double.TryParse(mediaInfo.Get(StreamKind.General, 0, "Duration"), out totalSeconds);
                        totalSeconds /= 1000;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("MediaInfo解析失败! " + ex.Message);
                }
            }
            Console.WriteLine(totalSeconds + "秒");
            var timeStartStr = options.StartTime;
            var timeEndtStr = options.StopTime;
            double startSeconds = ParseDur(timeStartStr).TotalSeconds;
            double endSeconds = ParseDur(timeEndtStr).TotalSeconds;
            if (endSeconds == 0 && options.Duration != "0") 
            {
                double durSeconds = ParseDur(options.Duration).TotalSeconds;
                endSeconds = startSeconds + durSeconds;
            }
            timeStartStr = ConvertSeconds((int)startSeconds);
            timeEndtStr = ConvertSeconds((int)endSeconds);
            //每秒大小
            double sizePerSec = fileSize / totalSeconds;
            long startPostition = (long)(startSeconds * sizePerSec);
            long endPostition = (long)(endSeconds * sizePerSec);
            if (endPostition == 0) endPostition = fileSize;
            using Stream inputStream = File.OpenRead(input);
            var outputPath = GetValidFileName(options.Output);
            if (string.IsNullOrEmpty(outputPath))
                outputPath = Path.GetFileNameWithoutExtension(input) + $"_[{timeStartStr}-{timeEndtStr}]" + Path.GetExtension(input);

            Console.WriteLine($"Input: {Path.GetFullPath(input)}");
            Console.WriteLine($"FileRange: {startPostition}-{endPostition}");
            if(!stdout)
                Console.WriteLine($"Output: {Path.GetFullPath(outputPath)}");
            Console.WriteLine("开始输出...");

            Stream outStream = null;
            if (stdout) outStream = Console.OpenStandardOutput();
            else outStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            
            inputStream.Seek(startPostition, SeekOrigin.Begin);
            var buffer = new byte[8192];
            var nowPosition = startPostition;
            int size = 0;
            while ((size = inputStream.Read(buffer, 0, buffer.Length)) > 0 && inputStream.Position <= endPostition)
            {
                outStream.Write(buffer);
                if (inputStream.Position - nowPosition > 100 * 1024 * 1024)
                {
                    if (!stdout)
                        Console.WriteLine($"{outStream.Position / 1024 / 1024}MB");
                    nowPosition = inputStream.Position;
                }
            }
        }
    }
}
