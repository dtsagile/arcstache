using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ArcStache
{
    /// <summary>
    /// Summary description for LogUtil
    /// </summary>
    /// <remarks>
    /// Default location for log file is in the "Logs" folder located in the directory the calling DLL exists in.
    /// </remarks>
    public class ComLogUtil
    {
        private enum LogLevel
        {
            Debug,
            Error,
            Info,
            Warn
        }

        private const string _separator = "   ";
        private string _directoryPath = "";

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ComLogUtil"/> class. Log file will be created in a "Logs" folder located alongside the running DLL.
        /// </summary>
        public ComLogUtil()
        {
            // Create directory if it doesn't exist
            _directoryPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location), "Logs");
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            // Create a default filename
            this.FileName = "_log.txt";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the log file.
        /// </summary>
        /// <value>The name of the log file.</value>
        public string FileName { get; set; }
        //{ get; set {
        //    if (!string.IsNullOrEmpty(value) && !value.ToLower().EndsWith(".txt"))
        //    {
        //        value = value + ".txt";
        //    };
        //}
        //}

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath
        {
            get
            {
                return System.IO.Path.Combine(_directoryPath, this.FileName);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Logs the specified message by creating new log file or appending message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        private void Log(LogLevel level, string message)
        {
            // Format the log message
            // Wrap error messages in empty lines for easy viewing in log file
            StringBuilder formattedMsg = new StringBuilder();
            if (level == LogLevel.Error) { formattedMsg.AppendLine(); }
            formattedMsg.Append(string.Format("{0}{1}{2}{3}{4}", DateTime.Now.ToString(), _separator, level.ToString(), _separator, message));
            if (level == LogLevel.Error) { formattedMsg.AppendLine(); }

            // Create or append message to log file
            if (!File.Exists(this.FilePath))
            {
                File.WriteAllText(this.FilePath, formattedMsg.ToString() + Environment.NewLine);
            }
            else
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(this.FilePath, true))
                {
                    file.WriteLine(formattedMsg.ToString());
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Logs the info.
        /// </summary>
        /// <param name="className">Name of the source class.</param>
        /// <param name="publicMethodName">Name of the public method.</param>
        /// <param name="message">The message.</param>
        public void LogDebug(string className, string publicMethodName, string message)
        {
            this.Log(LogLevel.Debug, string.Format("{0}::{1}{2}{3}", className, publicMethodName, _separator, message));
        }

        /// <summary>
        /// Logs the info.
        /// </summary>
        /// <param name="className">Name of the source class.</param>
        /// <param name="publicMethodName">Name of the public method.</param>
        /// <param name="message">The message.</param>
        public void LogInfo(string className, string publicMethodName, string message)
        {
            this.Log(LogLevel.Info, string.Format("{0}::{1}{2}{3}", className, publicMethodName, _separator, message));
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="className">Name of the source class.</param>
        /// <param name="publicMethodName">Name of the public method.</param>
        /// <param name="parameters">The method parameters.</param>
        /// <param name="ex">The exception.</param>
        public void LogError(string className, string publicMethodName, string parameters, Exception ex)
        {
            this.Log(LogLevel.Error, string.Format("{0}::{1}{2}params: {3}{4}Exception: {5}", className, publicMethodName, Environment.NewLine, parameters, Environment.NewLine, ex.ToString()));
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="className">Name of the source class.</param>
        /// <param name="publicMethodName">Name of the public method.</param>
        /// <param name="parameters">The method parameters.</param>
        /// <param name="ex">The exception.</param>
        public void LogError(string className, string publicMethodName, string parameters, string message, Exception ex)
        {
            this.Log(LogLevel.Error, string.Format("{0}::{1}{2}message: {3}{4}params: {5}{6}Exception: {7}", className, publicMethodName, Environment.NewLine, message, Environment.NewLine, parameters, Environment.NewLine, ex.ToString()));
        }


        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="className">Name of the source class.</param>
        /// <param name="publicMethodName">Name of the public method.</param>
        /// <param name="message">The message.</param>
        public void LogWarning(string className, string publicMethodName, string message)
        {
            this.Log(LogLevel.Warn, string.Format("{0}::{1}{2}{3}", className, publicMethodName, _separator, message));
        }

        /// <summary>
        /// Logs the parameters.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="publicMethodName">Name of the public method.</param>
        /// <param name="parameters">The parameters.</param>
        public void LogParameters(string className, string publicMethodName, string parameters)
        {
            this.Log(LogLevel.Debug, string.Format("{0}::{1}{2}params: {3}", className, publicMethodName, _separator, parameters));
        }

        #endregion

    }
}

