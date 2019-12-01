//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// #define DEBUG_LOGGER_SPEED
#define USE_APPENDALLTEXT

using JEM.Core.Configuration;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

#if !USE_APPENDALLTEXT
using System.Text;
#endif

namespace JEM.Core.Debugging
{
    /// <summary>
    ///     JEM Log Type.
    ///     Defines a log type of JEMLogger.
    /// </summary>
    public enum JEMLogType
    {
        /// <summary>
        ///     Info log type.
        /// </summary>
        Log,

        /// <summary>
        ///     Warning log type.
        /// </summary>
        Warning,

        /// <summary>
        ///     Error log type.
        /// </summary>
        Error,

        /// <summary>
        ///     Exception log type.
        /// </summary>
        Exception,

        /// <summary>
        ///     Assertion log type.
        /// </summary>
        Assert
    }

    /// <summary>
    ///     Log append event.
    /// </summary>
    /// <param name="source">Source of log.</param>
    /// <param name="type">Type of log.</param>
    /// <param name="value">Value of log.</param>
    /// <param name="stackTrace">Stack trace of log.</param>
    public delegate void JEMLogAppend(string source, JEMLogType type, string value, string stackTrace);

    /// <summary>
    ///     JEM Logger.
    ///     A logs controller class.
    /// </summary>
    public static class JEMLogger
    {
        /// <summary>
        ///     Initializes JEMLogger.
        ///     This Method should be called at very beginning of application life to catch all the logs.
        /// </summary>
        public static void Start()
        {
#if !USE_APPENDALLTEXT
            if (_stream != null)
            {
                throw new InvalidOperationException();
            }
#endif

            // Set enable state.
            var cfg = InternalJEMConfiguration.Resolve();
            Disabled = cfg.NoLogs;

            // Set file prefix.
            LoggerFilePrefix = Process.GetCurrentProcess().ProcessName;

            // Get stream file.
            _streamFile = BuildFilePatch(string.Empty); //BuildFilePatch("log");
#if !USE_APPENDALLTEXT
            // Initialize writer.
            _stream = new FileStream(_streamFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            _writer = new StreamWriter(_stream, Encoding.UTF8);
#endif

            // Print notify log
            Log($"JEMLogger session started: {DateTime.Now:g}", "JEM");

#if DEBUG && DEBUG_LOGGER_SPEED
            var sw = Stopwatch.StartNew();
            for (int index = 0; index < 200; index++)
            {
                Log($"Log_speed_test", "JEM");
            }

            sw.Stop();
            Log($"The speed of logger is: {sw.Elapsed.TotalMilliseconds:0.000}ms", "JEM");
#endif
        }

        /// <summary>
        ///     Stops the JEMLogger.
        ///     This Method should be called at very end of application life.
        /// </summary>
        public static void Stop()
        {
#if !USE_APPENDALLTEXT
            // Stop writer.
            _stream?.Dispose();
            _stream = null;
#endif
        }

        /// <summary>
        ///     Clears logger directory. from all log files.
        /// </summary>
        public static void ClearLoggerDirectory(bool makeBackup = true)
        {
#if !USE_APPENDALLTEXT
            if (_stream != null)
            {
                throw new InvalidOperationException($"You can only clear logger's directory if JEMLogger is not started.");
            }
#endif

            if (Directory.Exists(LoggerWorkDirectory))
            {
                var previousFiles = Directory.GetFiles(LoggerWorkDirectory);
                if (makeBackup)
                {
                    // delete prev files
                    for (var index = 0; index < previousFiles.Length; index++)
                    {
                        var file = previousFiles[index];
                        if (!file.EndsWith($"_prev.{LogFileExtension}")) continue;
                        File.Delete(file);
                    }

                    previousFiles = Directory.GetFiles(LoggerWorkDirectory);
                    for (var index = 0; index < previousFiles.Length; index++)
                    {
                        var file = previousFiles[index];
                        if (!file.EndsWith($".{LogFileExtension}")) continue;
                        var dir = Path.GetDirectoryName(file);
                        var name = Path.GetFileNameWithoutExtension(file);
                        var newFile = $"{dir}{JEMProgram.DirectorySeparator}{name}_prev.{LogFileExtension}";
                        File.Move(file, newFile);
                    }
                }
                else
                {
                    // just delete all .log files from logger work directory
                    for (var index = 0; index < previousFiles.Length; index++)
                    {
                        if (!previousFiles[index].EndsWith($".{LogFileExtension}")) continue;
                        File.Delete(previousFiles[index]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(LoggerWorkDirectory);
            }
        }

        /// <summary>
        ///     Fixes logger work directory by checking if working directory not exists.
        /// </summary>
        private static bool FixLoggerWorkDirectory()
        {
            try
            {
                if (!Directory.Exists(LoggerWorkDirectory))
                    Directory.CreateDirectory(LoggerWorkDirectory);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Builds full path of file.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <returns>Created path of file.</returns>
        private static string BuildFilePatch(string fileName)
        {
            var prefix = LoggerFilePrefix;
            if (JEMProgram.IsLinux)
            {
                if (prefix.Contains(Environment.CurrentDirectory))
                    prefix = prefix.Remove(0, Environment.CurrentDirectory.Length);
            }

            return $"{LoggerWorkDirectory}{prefix}{fileName}.{LogFileExtension}";
        }

        /// <summary>
        ///     Builds line for file text.
        /// </summary>
        /// <param name="lineContent">Content of line.</param>
        private static string BuildFileLine(string lineContent) => lineContent + Environment.NewLine;
        
        /// <summary>
        ///     Builds comment line for file text.
        /// </summary>
        /// <param name="comment">Comment content.</param>
        private static string BuildFileCommentLine(string comment) => comment != "" ? BuildFileLine($"\t--->\n{comment}") : string.Empty;
        
        /// <summary>
        ///     Gets the stackTrace.
        ///     If originalTrace parameter is null or empty, Logger will try to generate new StackTrace from this point.
        /// </summary>
        private static string GetTrace(string originalTrace) => string.IsNullOrEmpty(originalTrace) ? new StackTrace().ToString() : originalTrace;

        /// <summary>
        ///     Appends log.
        /// </summary>
        /// <param name="source">Source of log.</param>
        /// <param name="type">Type of log.</param>
        /// <param name="value">Value of log.</param>
        /// <param name="stackTrace">Stack trace of log.</param>
        /// <returns>Is log been successfully appended?</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public static bool AppendLog(string source, JEMLogType type, string value, string stackTrace)
        {
#if !DEBUG
            try
            {
#endif
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (!Enum.IsDefined(typeof(JEMLogType), type))
                    throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(JEMLogType));
                if (Disabled) return false;
#if !USE_APPENDALLTEXT
                if (_stream == null) return false;
#else
                if (_streamFile == null) return false;
#endif
                if (source == null) source = "UNKNOWN";
                if (!FixLoggerWorkDirectory()) return false;

                var fileText = BuildFileLine(DateTimeNowText + $"[{type}][{source.ToUpper()}]: {value}");
                if (AlwaysThrowStacktrace || type == JEMLogType.Exception)
                    fileText += BuildFileCommentLine(stackTrace);

#if !USE_APPENDALLTEXT
                _writer.Write(fileText);
#else
                File.AppendAllText(_streamFile, fileText);
#endif

                OnLogAppended?.Invoke(source, type, value, stackTrace);
                return true;
#if !DEBUG
            }
            catch (Exception)
            {
                // Logs should never throw any error.
                return false;
            }
#endif
        }

#region EXTERNAL

        /// <summary>
        ///     Append log.
        /// </summary>
        /// <param name="value">Value of log.</param>
        /// <param name="source">Name of source of this log.</param>
        /// <param name="originalTrace">Original trace.</param>
        /// <returns>Is log been successfully appended by system?</returns>
        public static bool Log(string value, string source = "EXTERNAL", string originalTrace = null) =>
            AppendLog(source, JEMLogType.Log, value, GetTrace(originalTrace));
        
        /// <summary>
        ///     Append warning log.
        /// </summary>
        /// <param name="value">Value of log.</param>
        /// <param name="source">Name of source of this log.</param>
        /// <param name="originalTrace">Original trace.</param>
        /// <returns>Is log been successfully appended by system?</returns>
        public static bool LogWarning(string value, string source = "EXTERNAL", string originalTrace = null) =>
            AppendLog(source, JEMLogType.Warning, value, GetTrace(originalTrace));
        
        /// <summary>
        ///     Append error log.
        /// </summary>
        /// <param name="value">Value of log.</param>
        /// <param name="source">Name of source of this log.</param>
        /// <param name="originalTrace">Original trace.</param>
        /// <returns>Is log been successfully appended by system?</returns>
        public static bool LogError(string value, string source = "EXTERNAL", string originalTrace = null) =>
            AppendLog(source, JEMLogType.Error, value, GetTrace(originalTrace));
        
        /// <summary>
        ///     Append exception log.
        /// </summary>
        /// <param name="ex">Exception to write.</param>
        /// <param name="source">Name of source of this log.</param>
        /// <returns>Is log been successfully appended by system?</returns>
        public static bool LogException(Exception ex, string source = "EXTERNAL") =>
            AppendLog(source, JEMLogType.Exception, ex.Message, ex.StackTrace);      

        /// <summary>
        ///     Append exception log.
        /// </summary>
        /// <param name="condition">Condition of exception.</param>
        /// <param name="stackTrace">Stack trace of exception.</param>
        /// <param name="source">Name of source of this log.</param>
        /// <returns>Is log been successfully appended by system?</returns>
        public static bool LogException(string condition, string stackTrace, string source = "EXTERNAL") =>
            AppendLog(source, JEMLogType.Exception, condition, stackTrace);
        
        /// <summary>
        ///     Append exception log.
        /// </summary>
        /// <param name="ex">Exception to write.</param>
        /// <param name="customCondition">Condition of exception.</param>
        /// <param name="source">Name of source of this log.</param>
        /// <returns>Is log been successfully appended by system?</returns>
        public static bool LogException(Exception ex, string customCondition, string source) =>
            AppendLog(source, JEMLogType.Exception, customCondition, ex.Message + "\n" + ex.StackTrace);
        
        /// <summary>
        ///     Append log of <see cref="JEMLogType.Assert"/> type.
        /// </summary>
        /// <param name="value">Value of log.</param>
        /// <param name="source">Name of source of this log.</param>
        /// <param name="originalTrace">Original trace.</param>
        /// <returns>Is log been successfully appended by system?</returns>
        public static bool LogAssert(string value, string source = "EXTERNAL", string originalTrace = null) =>
            AppendLog(source, JEMLogType.Assert, value, GetTrace(originalTrace));

#endregion

        private static string _streamFile;
#if !USE_APPENDALLTEXT
        private static FileStream _stream;
        private static StreamWriter _writer;
#endif

        /// <summary>
        ///     Event called when new log has been received.
        /// </summary>
        public static event JEMLogAppend OnLogAppended;

        /// <summary>
        ///     Name of logger directory.
        /// </summary>
        public static string LoggerWorkDirectoryName = $@"{JEMProgram.DirectorySeparator}logs{JEMProgram.DirectorySeparator}";

        /// <summary>
        ///     Log file extension.
        /// </summary>
        public static string LogFileExtension = "log";

        /// <summary>
        ///     If true, the Logger wil ignore every log received.
        /// </summary>
        public static bool Disabled { get; set; } = false;

        /// <summary>
        ///     If true, Logger will try to deliver stackTrace for every log type.
        /// </summary>
        public static bool AlwaysThrowStacktrace { get; set; } = false;

        /// <summary>
        ///     Logger's custom root directory.
        ///     If empty, logger will get default current directory.
        /// </summary>
        public static string LoggerCustomDirectory { get; set; } = string.Empty;

        /// <summary>
        ///     Prefix of logger files. Based on process name.
        /// </summary>
        public static string LoggerFilePrefix { get; set; } = string.Empty;

        /// <summary>
        ///     DateTime.Now string formatted for text.
        /// </summary>
        public static string DateTimeNowText => $"[{DateTime.Now:HH:mm:ss}]";

        /// <summary>
        ///     Full path of logger work directory.
        /// </summary>
        public static string LoggerWorkDirectory =>
            $"{(string.IsNullOrEmpty(LoggerCustomDirectory) ? Environment.CurrentDirectory : LoggerCustomDirectory)}{LoggerWorkDirectoryName}";
    }
}