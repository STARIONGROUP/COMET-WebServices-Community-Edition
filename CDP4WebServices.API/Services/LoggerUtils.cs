// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerUtils.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services
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
