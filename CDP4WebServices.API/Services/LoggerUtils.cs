// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerUtils.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using CDP4Authentication;

    /// <summary>
    /// The logger utils helper class
    /// </summary>
    public static class LoggerUtils
    {
        /// <summary>
        /// The unauthenticated subject constant.
        /// </summary>
        public const string UnauthenticatedSubject = "unauthenticated";

        /// <summary>
        /// The log success constant.
        /// </summary>
        public const string SuccesLog = "success";

        /// <summary>
        /// The log failure constant.
        /// </summary>
        public const string FailureLog = "failure";

        /// <summary>
        /// Construct a log message.
        /// </summary>
        /// <param name="subject">
        /// The authenticated subject (user) that triggered the log entry.
        /// </param>
        /// <param name="subjectHostAddress">
        /// The subject host address.
        /// </param>
        /// <param name="success">
        /// Successful request or not.
        /// </param>
        /// <param name="message">
        /// The log message
        /// </param>
        /// <returns>
        /// The formatted log entry.
        /// </returns>
        public static string GetLogMessage(
            string subject,
            string subjectHostAddress,
            bool success,
            string message)
        {
            return string.Format(
                "[{0}{1}] [{2}]|{3}",
                subject,
                !string.IsNullOrWhiteSpace(subjectHostAddress) ? string.Format("@{0}", subjectHostAddress) : string.Empty,
                success ? SuccesLog : FailureLog,
                message);
        }

        /// <summary>
        /// Construct a log message.
        /// </summary>
        /// <param name="subject">
        /// The authenticated subject (user) that triggered the log entry.
        /// </param>
        /// <param name="subjectHostAddress">
        /// The subject host address.
        /// </param>
        /// <param name="success">
        /// Successful request or not.
        /// </param>
        /// <param name="message">
        /// The log message.
        /// </param>
        /// <returns>
        /// The formatted log entry.
        /// </returns>
        public static string GetLogMessage(
            AuthenticationPerson subject,
            string subjectHostAddress,
            bool success, 
            string message)
        {
            var subjectString = subject == null
                                    ? UnauthenticatedSubject
                                    : string.Format("{0}({1})", subject.UserName, subject.Iid);
            return GetLogMessage(subjectString, subjectHostAddress, success, message);
        }
    }
}
