// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilsTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using CDP4Common.Types;

    using CDP4Orm.Dao;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="Utils"/> class
    /// </summary>
    [TestFixture]
    public class UtilsTestFixture
    {
        [Test]
        public void VerifyDeserializationOfHstore()
        {
            const string hstoreSource = "\"TestKey1\" => \"TestValue1\", \"TestKey2\" => \"TestValue2\"";
            var expected = new Dictionary<string, string> { { "TestKey1", "TestValue1" }, { "TestKey2", "TestValue2" } };

            Assert.That(Utils.ParseHstoreString(hstoreSource), Is.EquivalentTo(expected));
        }

        [Test]
        public void VerifySpaceIndifferenceDeserialization()
        {
            const string hstoreSource = "\"TestKey1 \" =>   \"TestValue1\"   , \"TestKey2\"=>\"TestValue2\"";
            var expected = new Dictionary<string, string> { { "TestKey1", "TestValue1" }, { "TestKey2", "TestValue2" } };

            Assert.That(Utils.ParseHstoreString(hstoreSource), Is.EquivalentTo(expected));
        }

        [Test]
        public void VerifyComplicatedString()
        {
            const string hstoreSource = "\"TestKey1 \" =>   \"TestValue1\"   , \"TestKey2\"=>\"TestValue2\"";
            var expected = new Dictionary<string, string> { { "TestKey1", "TestValue1" }, { "TestKey2", "TestValue2" } };

            Assert.That(Utils.ParseHstoreString(hstoreSource), Is.EquivalentTo(expected));
        }

        [Test]
        public void VerifyEnumDeserialization()
        {
            const string enumSource = "Assembly";
            const AttributeTargets expected = AttributeTargets.Assembly;

            Assert.That(Utils.ParseEnum<AttributeTargets>(enumSource), Is.EqualTo(expected));
        }

        [Test]
        public void VerifyEnumArrayDeserialization()
        {
            const string enumSource = "Assembly,Class";
            var expected = new List<AttributeTargets> { AttributeTargets.Assembly, AttributeTargets.Class };

            Assert.That(Utils.ParseEnumArray<AttributeTargets>(enumSource), Is.EquivalentTo(expected));
        }

        [Test]
        public void VerifyOrderedListDeserialization()
        {
            var orderdListSource = new[,] { { "0", "1" }, { "item0", "item1" } };
            
            var expected = (IEnumerable)new List<OrderedItem>
                                            {
                                                new() { K = 0, V = "item0" },
                                                new() { K = 1, V = "item1" }
                                            };

            Assert.That(Utils.ParseOrderedList<string>(orderdListSource), Is.EquivalentTo(expected));
        }

        [Test]
        public void VerifyStringEscape()
        {
            const string unescapedString = "This is not valid for \"hstore\" persistence";
            const string expectedEscapedString = "This is not valid for \\\"hstore\\\" persistence";

            Assert.That(unescapedString.Escape(), Is.EqualTo(expectedEscapedString));
        }

        [Test]
        public void TestValueArrayStringConversion()
        {
            var valuearray = new[] { "123\"(,)\"", "abc\\" };
            const string expected = "{\"123\\\"(,)\\\"\";\"abc\\\\\"}";

            var test = new ValueArray<string>(valuearray);

            var res = test.ToHstoreString();
            
            Assert.That(res, Is.EqualTo(expected));

            var back = res.FromHstoreToValueArray<string>();

            Assert.That(back[0], Is.EqualTo(valuearray[0]));
            Assert.That(back[1], Is.EqualTo(valuearray[1]));
        }

        [Test]
        public void Verify_that_a_ValueArray_is_serialized_and_deserialized([ValueSource(nameof(TestStrings))] string input)
        {
            var valueArray = new ValueArray<string>(new List<string> { input });
            var hstore = valueArray.ToHstoreString();
            var result = hstore.FromHstoreToValueArray<string>();

            Assert.That(result, Is.EqualTo(valueArray), $"ValueArray creation failed for string \"{input}\"");

            var resultjson = result.ToHstoreString();

            Assert.That(resultjson, Is.EqualTo(hstore), $"Hstore creation failed for string \"{input}\"");
        }

        private const string JsonString = @"{""widget"": {
                ""debug"": ""on"",
                ""window"": {
                    ""title"": ""Sample Konfabulator Widget"",
                    ""name"": ""main_window"",
                    ""width"": 500,
                    ""height"": 500
                },
                ""image"": { 
                    ""src"": ""Images/Sun.png"",
                    ""name"": ""sun1"",
                    ""hOffset"": 250,
                    ""vOffset"": 250,
                    ""alignment"": ""center""
                },
                ""text"": {
                    ""data"": ""Click Here"",
                    ""size"": 36,
                    ""style"": ""bold"",
                    ""name"": ""text1"",
                    ""hOffset"": 250,
                    ""vOffset"": 100,
                    ""alignment"": ""center"",
                    ""onMouseUp"": ""sun1.opacity = (sun1.opacity / 100) * 90;""
                }
            }}";

        private const string XmlString = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <bookstore>
                <book category=""cooking"">
                    <title lang=""en"">Everyday food</title>
                    <author>Some great cook</author>
                    <year>2005</year>
                    <price>30.00</price>
                    <data><![CDATA[Within this Character Data block I can
                        use double dashes as much as I want (along with <, &, ', and "")
                        *and* %MyParamEntity; will be expanded to the text
                        ""Has been expanded"" ... however, I can't use
                        the CEND sequence.If I need to use CEND I must escape one of the
                        brackets or the greater-than sign using concatenated CDATA sections.
                        ]]></data>
                </book>
                <book category=""children"">
                    <title lang=""en"">Harry the child</title>
                    <author>Some child author</author>
                    <year>2005</year>
                    <price>29.99</price>
                </book>
                <book category=""web"">
                    <title lang=""en"">Learning XML</title>
                    <author>Some XML expert</author>
                    <year>2003</year>
                    <price>39.95</price>
                </book>
            </bookstore>";

        private static readonly string[] TestStrings = new string[]
        {
            "value with trailing spaces  ",
            "value with trailing space ",
            " value with leading spaces",
            "  value with leading space",
            "\t\t\tvalue with leading and trailing tabs \t",
            "\nvalue with leading and trailing linebreaks \r",
            "=2*(2+2)",
            "=2*\n(2+2)",
            "=2*\r(2+2)",
            "=2*\r\n(2+2)",
            "=2*\n\r(2+2)",
            "= 2 * \n ( 2 + 2 )",
            "=2*\b(2+2)",
            "=2*\f(2+2)",
            "=2*\t(2+2)",
            "Ar54WbBu + yhw - R:G!d)C!X_H % Vy ? V",
            "qm+L/{hp,qU[F\nnSyFymmZ\n+F(G/pP8@",
            "JSfJzH!U5:*wcnzT+{a5-L&+Xaq[g4",
            "EfRKJ[*A%uiM9MJ_h-z?9X(KYJQ/xL",
            "B_Dw+Tw.7g,.36]7(j8(k3/hxX,K_y",
            "qKt_C}@).D!ik.4W48ESR}w*VGvaub",
            "33CDr2NPZ[fJQ]p?aXT2L{giUUm}g#",
            "mpb-!ump7S{D)]Z9B@S([FXMRSq/9S",
            "D,VeZQRnV/}?}*qxMeX}N7*%R]!Tf/",
            "L$X7@P,JhcYM,-e4Z5,!ft.UbC[Y{n",
            "QWuAr.P$RUCf(NiV{7}tcwnia:.Fnp",
            "L%%t?cdpa?g#-PE4w6=[yU72Cgxz:f",
            ",GCeVX=$6R,(JJW[mLd4uF@{,Yr%NL",
            "i?5,/.G%D,M3im?8:,+ju}(CMh_E77",
            "}8Bn)rtS4BGTWThmT,=nu,q{[H?):9",
            "ScVmbHjSB[HS$8A*C{awPvvp{%@5Xr",
            "wy6bDVDuim}YLhB24=[y6!4vpM2pTw",
            "f:][.LfcN#(gH=Dq$6Lcp7TWQP7LH!",
            "!&.v8L44$ep69u+W-_5jq?DV@fi($H",
            "?_uB5Z(U$B6,cVPMPJv%q}d[+2PAMZ",
            "[_*q5d$U{qE7}r_7$fdf$h5yBFpPG+",
            XmlString,
            JsonString
        };
    }
}
