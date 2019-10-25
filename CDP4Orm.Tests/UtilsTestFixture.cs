// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilsTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   This the Dao utility test class
// </summary>
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

            CollectionAssert.AreEquivalent(expected, Utils.ParseHstoreString(hstoreSource));
        }

        [Test]
        public void VerifySpaceIndifferenceDeserialization()
        {
            const string hstoreSource = "\"TestKey1 \" =>   \"TestValue1\"   , \"TestKey2\"=>\"TestValue2\"";
            var expected = new Dictionary<string, string> { { "TestKey1", "TestValue1" }, { "TestKey2", "TestValue2" } };

            CollectionAssert.AreEquivalent(expected, Utils.ParseHstoreString(hstoreSource));
        }

        [Test]
        public void VerifyComplicatedString()
        {
            const string hstoreSource = "\"TestKey1 \" =>   \"TestValue1\"   , \"TestKey2\"=>\"TestValue2\"";
            var expected = new Dictionary<string, string> { { "TestKey1", "TestValue1" }, { "TestKey2", "TestValue2" } };

            CollectionAssert.AreEquivalent(expected, Utils.ParseHstoreString(hstoreSource));
        }

        [Test]
        public void VerifyEnumDeserialization()
        {
            const string enumSource = "Assembly";
            const AttributeTargets expected = AttributeTargets.Assembly;

            Assert.AreEqual(expected, Utils.ParseEnum<AttributeTargets>(enumSource));
        }

        [Test]
        public void VerifyEnumArrayDeserialization()
        {
            const string enumSource = "Assembly,Class";
            var expected = new List<AttributeTargets> { AttributeTargets.Assembly, AttributeTargets.Class };

            CollectionAssert.AreEquivalent(expected, Utils.ParseEnumArray<AttributeTargets>(enumSource));
        }

        [Test]
        public void VerifyOrderedListDeserialization()
        {
            var orderdListSource = new[,] { { "0", "1" }, { "item0", "item1" } };
            var expected = (IEnumerable)new List<OrderedItem>
                                            {
                                                new OrderedItem { K = 0, V = "item0" },
                                                new OrderedItem { K = 1, V = "item1" }
                                            };

            CollectionAssert.AreEqual(expected, Utils.ParseOrderedList<string>(orderdListSource));
        }

        [Test]
        public void VerifyStringEscape()
        {
            const string unescapedString = "This is not valid for \"hstore\" persistence";
            const string expectedEscapedString = "This is not valid for \\\"hstore\\\" persistence";

            Assert.AreEqual(expectedEscapedString, unescapedString.Escape());
        }

        [Test]
        public void TestValueArrayStringConversion()
        {
            var valuearray = new[] { "123\"(,)\"", "abc\\" };
            const string expected = "{\"123\\\"(,)\\\"\";\"abc\\\\\"}";

            var test = new ValueArray<string>(valuearray);

            var res = test.ToHstoreString();
            Assert.AreEqual(expected, res);

            var back = res.FromHstoreToValueArray<string>();
            Assert.AreEqual(valuearray[0], back[0]);
            Assert.AreEqual(valuearray[1], back[1]);
        }

        [Test]
        public void Verify_that_a_ValueArray_is_serialized_and_deserialized([ValueSource(nameof(TestStrings))] string input)
        {
            var valueArray = new ValueArray<string>(new List<string> { input });
            var hstore = valueArray.ToHstoreString();
            var result = hstore.FromHstoreToValueArray<string>();

            Assert.AreEqual(valueArray, result, "ValueArray creation failed for string \"{0}\"", input);

            var resultjson = result.ToHstoreString();

            Assert.AreEqual(hstore, resultjson, "Hstore creation failed for string \"{0}\"", input);
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
