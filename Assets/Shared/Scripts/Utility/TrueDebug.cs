using UnityEngine;
using Conditional = System.Diagnostics.ConditionalAttribute;

namespace Shared.Utility
{
    public static class TrueDebug
    {
        /// <summary>
        ///   <para>Draws a line between specified start and end points.</para>
        /// </summary>
        /// <param name="start">Point in world space where the line should start.</param>
        /// <param name="end">Point in world space where the line should end.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="duration">How long the line should be visible for.</param>
        /// <param name="depthTest">Determines whether objects closer to the camera obscure the line.</param>
        [Conditional("DEBUG")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.0f, bool depthTest = true)
        {
            Debug.DrawLine(start, end, color, duration, depthTest);
        }

        /// <summary>
        ///   <para>Draws a line from start to start + dir in world coordinates.</para>
        /// </summary>
        /// <param name="start">Point in world space where the ray should start.</param>
        /// <param name="dir">Direction and length of the ray.</param>
        /// <param name="color">Color of the drawn line.</param>
        /// <param name="duration">How long the line will be visible for (in seconds).</param>
        /// <param name="depthTest">Determines whether objects closer to the camera obscure the line.</param>
        [Conditional("DEBUG")]
        public static void DrawRay(Vector3 start, Vector3 dir)
        {
            DrawRay(start, dir, Color.white);
        }

        /// <summary>
        ///   <para>Draws a line from start to start + dir in world coordinates.</para>
        /// </summary>
        /// <param name="start">Point in world space where the ray should start.</param>
        /// <param name="dir">Direction and length of the ray.</param>
        /// <param name="color">Color of the drawn line.</param>
        /// <param name="duration">How long the line will be visible for (in seconds).</param>
        /// <param name="depthTest">Determines whether objects closer to the camera obscure the line.</param>
        [Conditional("DEBUG")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration = 0.0f, bool depthTest = true)
        {
            Debug.DrawRay(start, dir, color, duration, depthTest);
        }

        /// <summary>
        ///   <para>Pauses the editor.</para>
        /// </summary>
        [Conditional("DEBUG")]
        public static void Break()
        {
            Debug.Break();
        }

        [Conditional("DEBUG")]
        public static void DebugBreak()
        {
            Debug.DebugBreak();
        }

#if DEBUG
        /// <summary>
        ///   <para>Populate an unmanaged buffer with the current managed call stack as a sequence of UTF-8 bytes, without allocating GC memory. Returns the number of bytes written into the buffer.</para>
        /// </summary>
        /// <param name="buffer">Target buffer to receive the callstack text</param>
        /// <param name="bufferMax">Max number of bytes to write</param>
        /// <param name="projectFolder">Project folder path, to clean up path names</param>
        public static unsafe int ExtractStackTraceNoAlloc(byte* buffer, int bufferMax, string projectFolder)
        {
            return Debug.ExtractStackTraceNoAlloc(buffer, bufferMax, projectFolder);
        }
#endif

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEBUG")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEBUG")]
        public static void Log(object message, Object context)
        {
            Debug.Log(message, context);
        }

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="logType">Type of message e.g. warn or error etc.</param>
        /// <param name="logOptions">Option flags to treat the log message special.</param>
        [Conditional("DEBUG")]
        public static void LogFormat(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="logType">Type of message e.g. warn or error etc.</param>
        /// <param name="logOptions">Option flags to treat the log message special.</param>
        [Conditional("DEBUG")]
        public static void LogFormat(Object context, string format, params object[] args)
        {
            Debug.LogFormat(context, format, args);
        }

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="logType">Type of message e.g. warn or error etc.</param>
        /// <param name="logOptions">Option flags to treat the log message special.</param>
        [Conditional("DEBUG")]
        public static void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args)
        {
            Debug.LogFormat(logType, logOptions, context, format, args);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEBUG")]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEBUG")]
        public static void LogError(object message, Object context)
        {
            Debug.LogError(message, context);
        }

        /// <summary>
        ///   <para>Logs a formatted error message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEBUG")]
        public static void LogErrorFormat(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }

        /// <summary>
        ///   <para>Logs a formatted error message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEBUG")]
        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
            Debug.LogErrorFormat(context, format, args);
        }

        /// <summary>
        ///   <para>Clears errors from the developer console.</para>
        /// </summary>
        [Conditional("DEBUG")]
        public static void ClearDeveloperConsole()
        {
            Debug.ClearDeveloperConsole();
        }

#if DEBUG
        /// <summary>
        ///   <para>Allows you to enable or disable the developer console.</para>
        /// </summary>
        public static bool developerConsoleEnabled
        {
            get => Debug.developerConsoleEnabled;
            set => Debug.developerConsoleEnabled = value;
        }

        /// <summary>
        ///   <para>Controls whether the development console is visible.</para>
        /// </summary>
        public static bool developerConsoleVisible
        {
            get => Debug.developerConsoleVisible;
            set => Debug.developerConsoleVisible = value;
        }
#endif

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="exception">Runtime Exception.</param>
        [Conditional("DEBUG")]
        public static void LogException(System.Exception exception, Object context = null)
        {
            Debug.LogException(exception, context);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEBUG")]
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEBUG")]
        public static void LogWarning(object message, Object context)
        {
            Debug.LogWarning(message, context);
        }

        /// <summary>
        ///   <para>Logs a formatted warning message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEBUG")]
        public static void LogWarningFormat(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }

        /// <summary>
        ///   <para>Logs a formatted warning message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEBUG")]
        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
            Debug.LogWarningFormat(context, format, args);
        }

        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }

        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, Object context)
        {
            Debug.Assert(condition, context);
        }

        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message)
        {
            Debug.Assert(condition, message);
        }

        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message, Object context)
        {
            Debug.Assert(condition, message, context);
        }

        /// <summary>
        ///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            Debug.AssertFormat(condition, format, args);
        }

        /// <summary>
        ///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void AssertFormat(bool condition, Object context, string format, params object[] args)
        {
            Debug.AssertFormat(condition, context, format, args);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertion(object message)
        {
            Debug.LogAssertion(message);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertion(object message, Object context)
        {
            Debug.LogAssertion(message, context);
        }

        /// <summary>
        ///   <para>Logs a formatted assertion message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertionFormat(string format, params object[] args)
        {
            Debug.LogAssertionFormat(format, args);
        }

        /// <summary>
        ///   <para>Logs a formatted assertion message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertionFormat(Object context, string format, params object[] args)
        {
            Debug.LogAssertionFormat(context, format, args);
        }

        /// <summary>
        ///   <para>In the Build Settings dialog there is a check box called "Development Build".</para>
        /// </summary>
        public static bool isDebugBuild => Debug.isDebugBuild;

#if DEBUG
        /// <summary>
        ///   <para>Returns any captured startup logs</para>
        /// </summary>
        public static Debug.StartupLog[] RetrieveStartupLogs()
        {
            return Debug.RetrieveStartupLogs();
        }

        /// <summary>
        ///   <para>Performs an integrity check of the currently running process and return discovered errors.</para>
        /// </summary>
        /// <param name="level">Thoroughness of integrity check.</param>
        public static string CheckIntegrity(IntegrityCheckLevel level)
        {
            return Debug.CheckIntegrity(level);
        }
#endif
    }
}