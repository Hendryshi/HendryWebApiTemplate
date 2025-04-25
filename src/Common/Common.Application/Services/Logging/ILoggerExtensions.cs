using Common.Domain.Logging;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text;

namespace Common.Application.Services.Logging
{
    public static class ILoggerExtensions
    {
        private const string BaseLogType = LogTypes.Application;

        private static IDisposable BeginLogScope(ILogger logger, string logType, Dictionary<string, object> additionalFields = null)
        {
            return logger.BeginScope(GetScopeDictionary(logType, additionalFields));
        }

        private static Dictionary<string, object> GetScopeDictionary(string logType, Dictionary<string, object> additionalFields = null)
        {
            var dic = additionalFields ?? new Dictionary<string, object>();

            dic.Add("LogType", logType);
            return dic;
        }

        #region logging methods
        #region LogResult
        /// <summary>
        /// Log all reasons (Errors and success)
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="result"></param>
        /// <param name="reasonMessage"></param>
        /// <param name="level"></param>
        public static void LogResult(this ILogger logger, ResultBase result, Func<IReason, string> reasonMessage = null, LogLevel level = LogLevel.Debug,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogResult_internal(logger, result, reasonMessage, level, memberName, sourceFilePath, sourceLineNumber);
        }

        #region Specific LogType
        public static void LogResultApplication(this ILogger logger, ResultBase result, Func<IReason, string> reasonMessage = null, LogLevel level = LogLevel.Debug,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogResult_internal(logger, result, reasonMessage, level, memberName, sourceFilePath, sourceLineNumber, logType: LogTypes.Application);
        }

        public static void LogResultSecurity(this ILogger logger, ResultBase result, Func<IReason, string> reasonMessage = null, LogLevel level = LogLevel.Debug,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogResult_internal(logger, result, reasonMessage, level, memberName, sourceFilePath, sourceLineNumber, logType: LogTypes.Security);
        }

        public static void LogResultAudit(this ILogger logger, ResultBase result, Func<IReason, string> reasonMessage = null, LogLevel level = LogLevel.Debug,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogResult_internal(logger, result, reasonMessage, level, memberName, sourceFilePath, sourceLineNumber, logType: LogTypes.Audit);
        }

        public static void LogResultMonitoring(this ILogger logger, ResultBase result, Func<IReason, string> reasonMessage = null, LogLevel level = LogLevel.Debug,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogResult_internal(logger, result, reasonMessage, level, memberName, sourceFilePath, sourceLineNumber, logType: LogTypes.Monitoring);
        }
        #endregion

        /// <summary>
        /// Log all reasons (Errors and success)
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="result"></param>
        /// <param name="reasonMessage"></param>
        /// <param name="level"></param>

        private static void LogResult_internal<TReason>(this ILogger logger, ResultBase result, Func<TReason, string> reasonMessage = null, LogLevel level = LogLevel.Debug,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           string logType = BaseLogType) where TReason : IReason
        {
            result.Reasons.ForEach(reason =>
            {
                if (reason is TReason reasonCasted)
                {
                    var messageBuilder = new StringBuilder();
                    var message = reasonMessage != null ? reasonMessage.Invoke(reasonCasted) : reason.Message;
                    messageBuilder.Append(message.FormatForContext(memberName, sourceFilePath, sourceLineNumber));

                    //If reason is error type, it can have children reasons
                    List<ExceptionalError> exceptionalErrors = null;
                    if (reason is Error error)
                    {
                        exceptionalErrors = new List<ExceptionalError>();

                        error.Reasons.ForEach(r =>
                        {

                            if (r is ExceptionalError exceptionalError)
                            {
                                exceptionalErrors.Add(exceptionalError);
                            }
                            else
                            {
                                messageBuilder.AppendLine(r.Message.FormatForContext(memberName, sourceFilePath, sourceLineNumber));
                            }
                        });
                    }

                    using (BeginLogScope(logger, logType))
                    {
                        logger.Log(level, messageBuilder.ToString());

                        if (exceptionalErrors != null)
                        {
                            foreach (var exceptionalError in exceptionalErrors)
                            {
                                logger.Log(level, exceptionalError.Exception, exceptionalError.Message.FormatForContext(memberName, sourceFilePath, sourceLineNumber));
                            }
                        }
                    }

                }

            });
        }

        #endregion

        #region private
        private static string FormatForException(this string message, Exception ex)
        {
            return $"{message}: {(ex != null ? ex.ToString() : "")}";
        }

        private static string FormatForContext(
            this string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0)
        {
            var filePath = sourceFilePath;
            var methodName = memberName;

            return $"{filePath}: Line {sourceLineNumber} [{methodName}] {message}";
        }
        #endregion

        public static void Trace(this ILogger logger,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           string logType = BaseLogType,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
        {
            using (BeginLogScope(logger, logType, additionalFields))
            {
                logger.Log(LogLevel.Trace, message
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args);
            }
        }

        #region Trace
        public static void Trace(this ILogger logger,
         Exception ex,
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0,
         string logType = BaseLogType,
         Dictionary<string, object> additionalFields = null,
         params object[] args)
        {
            using (BeginLogScope(logger, logType, additionalFields))
            {
                logger.Log(LogLevel.Trace, ex, message
                  .FormatForException(ex)
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
               );
            }
        }
        #region Specific LogType
        public static void TraceAudit(this ILogger logger,
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0,
         Dictionary<string, object> additionalFields = null,
         params object[] args)
            => Trace(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);

        public static void TraceAudit(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Trace(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);
        public static void TraceApplication(this ILogger logger,
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0,
         Dictionary<string, object> additionalFields = null,
         params object[] args)
            => Trace(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);

        public static void TraceApplication(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Trace(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);
        public static void TraceSecurity(this ILogger logger,
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0,
         Dictionary<string, object> additionalFields = null,
         params object[] args)
            => Trace(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);

        public static void TraceSecurity(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Trace(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);
        public static void TraceMonitoring(this ILogger logger,
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0,
         Dictionary<string, object> additionalFields = null,
         params object[] args)
            => Trace(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);

        public static void TraceMonitoring(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Trace(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);
        #endregion
        #endregion

        #region Debug
        public static void Debug(this ILogger logger,
          string message,
          [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0,
          string logType = BaseLogType,
          Dictionary<string, object> additionalFields = null,
          params object[] args)
        {
            using (BeginLogScope(logger, logType, additionalFields))
            {
                logger.Log(LogLevel.Debug,
               message
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
               );
            }
        }

        public static void Debug(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           string logType = BaseLogType,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
        {
            using (BeginLogScope(logger, logType, additionalFields))
            {
                logger.Log(LogLevel.Debug, ex,
               message
                  .FormatForException(ex)
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
               );
            }
        }

        #region Specific LogTypes
        public static void DebugAudit(this ILogger logger,
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0,
         Dictionary<string, object> additionalFields = null,
         params object[] args) => Debug(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);

        public static void DebugAudit(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args) => Debug(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);

        public static void DebugApplication(this ILogger logger,
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0,
         Dictionary<string, object> additionalFields = null,
         params object[] args) => Debug(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);

        public static void DebugApplication(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args) => Debug(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);

        public static void DebugSecurity(this ILogger logger,
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0,
         Dictionary<string, object> additionalFields = null,
         params object[] args) => Debug(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);

        public static void DebugSecurity(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args) => Debug(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);

        public static void DebugMonitoring(this ILogger logger,
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0,
         Dictionary<string, object> additionalFields = null,
         params object[] args) => Debug(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);

        public static void DebugMonitoring(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args) => Debug(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);

        #endregion
        #endregion

        #region Information
        public static void Information(this ILogger logger,
          string message,
          [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0,
          string logType = BaseLogType,
          Dictionary<string, object> additionalFields = null,
          params object[] args)
        {
            using (BeginLogScope(logger, logType, additionalFields))
            {
                logger.Log(LogLevel.Information,
               message
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
               );
            }
        }

        public static void Information(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           string logType = BaseLogType,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
        {
            using (BeginLogScope(logger, logType, additionalFields))
            {
                logger.Log(LogLevel.Information, ex,
               message
                  .FormatForException(ex)
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
               );
            }
        }

        #region Specific LogTypes
        public static void InformationAudit(this ILogger logger,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Information(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);

        public static void InformationAudit(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Information(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);
        public static void InformationApplication(this ILogger logger,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Information(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);

        public static void InformationApplication(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Information(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);
        public static void InformationSecurity(this ILogger logger,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Information(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);

        public static void InformationSecurity(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Information(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);
        public static void InformationMonitoring(this ILogger logger,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Information(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);

        public static void InformationMonitoring(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Information(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);
        #endregion
        #endregion

        #region Warning
        public static void Warning(this ILogger logger,
           string message,
          [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0,
          string logType = BaseLogType,
          Dictionary<string, object> additionalFields = null,
          params object[] args)
        {
            using (BeginLogScope(logger, logType, additionalFields))
            {
                logger.Log(LogLevel.Warning,
               message
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
               );
            }
        }

        public static void Warning(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           string logType = BaseLogType,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
        {
            using (BeginLogScope(logger, logType, additionalFields))
            {
                logger.Log(LogLevel.Warning, ex,
               message
                  .FormatForException(ex)
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
               );
            }
        }

        #region Specific LogTypes

        public static void WarningAudit(this ILogger logger,
           string message,
          [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0,
          Dictionary<string, object> additionalFields = null,
          params object[] args)
            => Warning(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);

        public static void WarningAudit(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Warning(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);

        public static void WarningApplication(this ILogger logger,
           string message,
          [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0,
          Dictionary<string, object> additionalFields = null,
          params object[] args)
            => Warning(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);

        public static void WarningApplication(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Warning(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);

        public static void WarningSecurity(this ILogger logger,
           string message,
          [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0,
          Dictionary<string, object> additionalFields = null,
          params object[] args)
            => Warning(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);

        public static void WarningSecurity(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Warning(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);

        public static void WarningMonitoring(this ILogger logger,
           string message,
          [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0,
          Dictionary<string, object> additionalFields = null,
          params object[] args)
            => Warning(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);

        public static void WarningMonitoring(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Warning(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);
        #endregion
        #endregion

        #region Error
        public static void Error(this ILogger logger,
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0,
         string logType = BaseLogType,
         Dictionary<string, object> additionalFields = null,
         params object[] args)
        {
            using (BeginLogScope(logger, logType, additionalFields))
            {
                logger.Log(LogLevel.Error,
               message
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
              );
            }
        }

        public static void Error(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           string logType = BaseLogType,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
        {
            using (BeginLogScope(logger, logType, additionalFields))
            {
                logger.Log(LogLevel.Error, ex,
               message
                  .FormatForException(ex)
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
               );
            }
        }

        #region Specific LogTypes
        public static void ErrorAudit(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0,
        Dictionary<string, object> additionalFields = null,
        params object[] args)
            => Error(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);

        public static void ErrorAudit(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Error(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);
        public static void ErrorApplication(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0,
        Dictionary<string, object> additionalFields = null,
        params object[] args)
            => Error(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);

        public static void ErrorApplication(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Error(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);
        public static void ErrorSecurity(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0,
        Dictionary<string, object> additionalFields = null,
        params object[] args)
            => Error(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);

        public static void ErrorSecurity(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Error(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);
        public static void ErrorMonitoring(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0,
        Dictionary<string, object> additionalFields = null,
        params object[] args)
            => Error(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);

        public static void ErrorMonitoring(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           Dictionary<string, object> additionalFields = null,
           params object[] args)
            => Error(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);
        #endregion
        #endregion

        #region Critical
        public static void Critical(this ILogger logger,
          string message,
          [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0,
          string logType = BaseLogType,
          object[] additionalFields = null,
          params object[] args)
        {
            using (BeginLogScope(logger, logType))
            {
                logger.Log(LogLevel.Critical,
              message
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
               );
            }
        }

        public static void Critical(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           string logType = BaseLogType,
           object[] additionalFields = null,
           params object[] args)
        {
            using (BeginLogScope(logger, logType))
            {
                logger.Log(LogLevel.Critical, ex,
              message
                  .FormatForException(ex)
                  .FormatForContext(memberName, sourceFilePath, sourceLineNumber), args
               );
            }
        }

        #region Specific LogTypes
        public static void CriticalAudit(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0,
        object[] additionalFields = null,
        params object[] args)
            => Critical(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);

        public static void CriticalAudit(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           object[] additionalFields = null,
           params object[] args)
            => Critical(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Audit, additionalFields, args);

        public static void CriticalApplication(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0,
        object[] additionalFields = null,
        params object[] args)
            => Critical(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);

        public static void CriticalApplication(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           object[] additionalFields = null,
           params object[] args)
            => Critical(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Application, additionalFields, args);

        public static void CriticalSecurity(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0,
        object[] additionalFields = null,
        params object[] args)
            => Critical(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);

        public static void CriticalSecurity(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           object[] additionalFields = null,
           params object[] args)
            => Critical(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Security, additionalFields, args);

        public static void CriticalMonitoring(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0,
        object[] additionalFields = null,
        params object[] args)
            => Critical(logger, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);

        public static void CriticalMonitoring(this ILogger logger,
           Exception ex,
           string message,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0,
           object[] additionalFields = null,
           params object[] args)
            => Critical(logger, ex, message, memberName, sourceFilePath, sourceLineNumber, LogTypes.Monitoring, additionalFields, args);
        #endregion
        #endregion
        #endregion
    }
}
