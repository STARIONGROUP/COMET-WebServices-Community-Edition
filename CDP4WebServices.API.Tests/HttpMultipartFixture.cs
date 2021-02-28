// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpMultipartFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using NUnit.Framework;

    [TestFixture]
    public class HttpMultipartFixture
    {
        private const string Boundary = "----NancyFormBoundary";

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Should_locate_all_boundaries()
        {
            // Given
            var stream = BuildInputStream(null, 10);
            var multipart = new HttpMultipart(stream, Boundary);

            // When
            var boundaries = multipart.GetBoundaries();

            // Then
            Assert.AreEqual(10, boundaries.Count());
        }

        [Test]
        public void Should_locate_boundary_when_it_is_not_at_the_beginning_of_stream()
        {
            // Given
            var stream = BuildInputStream("some padding in the stream", 1);
            var multipart = new HttpMultipart(stream, Boundary);

            // When
            var boundaries = multipart.GetBoundaries();

            // Then
            Assert.AreEqual(1, boundaries.Count());
        }

        //http://www.freesoft.org/CIE/RFC/1521/16.htm
        [Test]
        public void Should_preserve_the_content_of_the_file_even_though_there_is_data_at_the_end_of_the_multipart()
        {
            // Given
            var expected = "wazaa";

            var stream = new MemoryStream(BuildMultipartFileValues(new Dictionary<string, Tuple<string, string, string>>
            {
                { "sample.txt", new Tuple<string, string, string>("content/type", expected, "name")}
            }, null, "epilogue"));

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { "content-type", new[] { "multipart/form-data; boundary=----NancyFormBoundary" } }
            };

            // When
            var request = new Request("POST", new Url { Path = "/" }, CreateRequestStream(stream), headers);

            // Then
            var fileValue = request.Files.Single().Value;
            var actualBytes = new byte[fileValue.Length];
            fileValue.Read(actualBytes, 0, (int)fileValue.Length);

            var actual = Encoding.ASCII.GetString(actualBytes);

            Assert.AreEqual(expected, actual);
        }

        //http://www.freesoft.org/CIE/RFC/1521/16.htm
        [Test]
        public void Should_have_a_file_with_the_correct_data_in_it()
        {
            // Given
            var expected = "wazaa";

            var stream = new MemoryStream(BuildMultipartFileValues(new Dictionary<string, Tuple<string, string, string>>
            {
                { "sample.txt", new Tuple<string, string, string>("content/type", expected, "name")}
            }));

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { "content-type", new[] { "multipart/form-data; boundary=----NancyFormBoundary" } }
            };

            // When
            var request = new Request("POST", new Url { Path = "/", Scheme = "http" }, CreateRequestStream(stream), headers);


            // Then
            var fileValue = request.Files.Single().Value;
            var actualBytes = new byte[fileValue.Length];
            fileValue.Read(actualBytes, 0, (int)fileValue.Length);

            var actual = Encoding.ASCII.GetString(actualBytes);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Should_have_a_file_with_the_correct_data_in_it_using_quotes()
        {
            // Given
            var expected = "wazaa";

            var stream = new MemoryStream(BuildMultipartFileValues(new Dictionary<string, Tuple<string, string, string>>
            {
                { "sample.txt", new Tuple<string, string, string>("content/type", expected, "name")}
            }, null, null, true));

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { "content-type", new[] { "multipart/form-data; boundary=\"----NancyFormBoundary\"" } }
            };

            // When
            var request = new Request("POST", new Url { Path = "/", Scheme = "http" }, CreateRequestStream(stream), headers);


            // Then
            var fileValue = request.Files.Single().Value;
            var actualBytes = new byte[fileValue.Length];
            fileValue.Read(actualBytes, 0, (int)fileValue.Length);

            var actual = Encoding.ASCII.GetString(actualBytes);

            Assert.AreEqual(expected, actual);
        }

        //http://www.freesoft.org/CIE/RFC/1521/16.htm
        [Test]
        public void Should_preserve_the_content_of_the_file_even_though_there_is_data_at_the_beginning_of_the_multipart()
        {
            // Given
            var expected = "wazaa";

            var stream = new MemoryStream(BuildMultipartFileValues(new Dictionary<string, Tuple<string, string, string>>
            {
                { "sample.txt", new Tuple<string, string, string>("content/type", expected, "name")}
            }, "preamble", null));

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { "content-type", new[] { "multipart/form-data; boundary=----NancyFormBoundary" } }
            };

            // When
            var request = new Request("POST", new Url { Path = "/", Scheme = "http" }, CreateRequestStream(stream), headers);


            // Then
            var fileValue = request.Files.Single().Value;
            var actualBytes = new byte[fileValue.Length];
            fileValue.Read(actualBytes, 0, (int)fileValue.Length);

            var actual = Encoding.ASCII.GetString(actualBytes);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void If_the_stream_ends_with_carriage_return_characters_it_should_not_affect_the_multipart()
        {
            // Given
            var expected = "#!/usr/bin/env rake\n# Add your own tasks in files placed in lib/tasks ending in .rake,\n# for example lib/tasks/capistrano.rake, and they will automatically be available to Rake.\n\nrequire File.expand_path('../config/application', __FILE__)\n\nOnlinebackupWebclient::Application.load_tasks";
            var data = string.Format("--69989\r\nContent-Disposition: form-data; name=\"Stream\"; filename=\"Rakefile\"\r\nContent-Type: text/plain\r\n\r\n{0}\r\n--69989--\r\n", expected);
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(data));

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                {"Content-Type", new [] { "multipart/form-data; boundary=69989"} },
                {"Content-Length", new [] {"403"} }
            };

            // When
            var request = new Request("POST", new Url { Path = "/", Scheme = "http" }, CreateRequestStream(stream), headers);

            // Then
            var fileValue = request.Files.Single().Value;
            var actualBytes = new byte[fileValue.Length];
            fileValue.Read(actualBytes, 0, (int)fileValue.Length);

            var actual = Encoding.ASCII.GetString(actualBytes);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Should_limit_the_number_of_boundaries()
        {
            // Given
            var stream = BuildInputStream(null, StaticConfiguration.RequestQueryFormMultipartLimit + 10);
            var multipart = new HttpMultipart(stream, Boundary);

            // When
            var boundaries = multipart.GetBoundaries();

            // Then
            Assert.AreEqual(StaticConfiguration.RequestQueryFormMultipartLimit, boundaries.Count());
        }

        //
        private static HttpMultipartSubStream BuildInputStream(string padding, int numberOfBoundaries)
        {
            return BuildInputStream(padding, numberOfBoundaries, (i, b) => InsertRandomContent(b), null);
        }

        private static HttpMultipartSubStream BuildInputStream(string padding, int numberOfBoundaries, Action<int, StringBuilder> insertContent, string dataAtTheEnd)
        {
            var memory = new MemoryStream(BuildRandomBoundaries(padding, numberOfBoundaries, insertContent, dataAtTheEnd));

            return new HttpMultipartSubStream(memory, 0, memory.Length);
        }

        private static byte[] BuildRandomBoundaries(string padding, int numberOfBoundaries, Action<int, StringBuilder> insertContent, string dataAtTheEnd)
        {
            var boundaryBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(padding))
            {
                boundaryBuilder.Append(padding);
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
            }

            for (var index = 0; index < numberOfBoundaries; index++)
            {
                boundaryBuilder.Append("--");
                boundaryBuilder.Append("----NancyFormBoundary");
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');

                insertContent(index, boundaryBuilder);

                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
            }

            boundaryBuilder.Append('\r');
            boundaryBuilder.Append('\n');
            boundaryBuilder.AppendFormat("------NancyFormBoundary--{0}", dataAtTheEnd);

            var bytes = Encoding.ASCII.GetBytes(boundaryBuilder.ToString());
            return bytes;
        }

        private static void InsertRandomContent(StringBuilder builder)
        {
            var random =
                new Random((int)DateTime.Now.Ticks);

            for (var index = 0; index < random.Next(1, 200); index++)
            {
                builder.Append((char)random.Next(0, 255));
            }
        }

        private static byte[] BuildMultipartFileValues(Dictionary<string, Tuple<string, string, string>> formValues, string preamble, string epilogue, bool surroundWithQuotes = false)
        {
            var boundaryBuilder = new StringBuilder();

            boundaryBuilder.Append(preamble);
            foreach (var key in formValues.Keys)
            {
                var name = key;
                var filename = formValues[key].Item3;
                if (surroundWithQuotes)
                {
                    name = "\"" + name + "\"";
                    filename = "\"" + filename + "\"";
                }

                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.Append("--");
                boundaryBuilder.Append("----NancyFormBoundary");
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.AppendFormat("Content-Disposition: form-data; name={1}; filename={0}", name, filename);
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.AppendFormat("Content-Type: {0}", formValues[key].Item1);
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.Append(formValues[key].Item2);
            }

            boundaryBuilder.Append('\r');
            boundaryBuilder.Append('\n');
            boundaryBuilder.AppendFormat("------NancyFormBoundary--{0}", epilogue);

            var bytes =
                Encoding.ASCII.GetBytes(boundaryBuilder.ToString());

            return bytes;
        }

        private static RequestStream CreateRequestStream()
        {
            return CreateRequestStream(new MemoryStream());
        }

        private static RequestStream CreateRequestStream(Stream stream)
        {
            return RequestStream.FromStream(stream);
        }

        private static byte[] BuildMultipartFileValues(Dictionary<string, Tuple<string, string, string>> formValues)
        {
            return BuildMultipartFileValues(formValues, null, null);
        }
    }
}