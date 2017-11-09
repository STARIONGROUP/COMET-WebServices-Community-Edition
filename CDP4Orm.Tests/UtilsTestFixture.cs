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
    using System.Diagnostics.CodeAnalysis;

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
            const string HstoreSource = "\"TestKey1\" => \"TestValue1\", \"TestKey2\" => \"TestValue2\"";
            var expected = new Dictionary<string, string> { { "TestKey1", "TestValue1" }, { "TestKey2", "TestValue2" } };

            CollectionAssert.AreEquivalent(expected, Utils.ParseHstoreString(HstoreSource));
        }

        [Test]
        public void VerifySpaceIndifferenceDeserialization()
        {
            const string HstoreSource = "\"TestKey1 \" =>   \"TestValue1\"   , \"TestKey2\"=>\"TestValue2\"";
            var expected = new Dictionary<string, string> { { "TestKey1", "TestValue1" }, { "TestKey2", "TestValue2" } };

            CollectionAssert.AreEquivalent(expected, Utils.ParseHstoreString(HstoreSource));
        }

        [Test]
        public void VerifyEnumDeserialization()
        {
            const string EnumSource = "Assembly";
            const AttributeTargets Expected = System.AttributeTargets.Assembly;
            
            Assert.AreEqual(Expected, Utils.ParseEnum<System.AttributeTargets>(EnumSource));
        }

        [Test]
        public void VerifyEnumArrayDeserialization()
        {
            const string EnumSource = "Assembly,Class";
            var expected = new List<AttributeTargets> { AttributeTargets.Assembly, AttributeTargets.Class };

            CollectionAssert.AreEquivalent(expected, Utils.ParseEnumArray<System.AttributeTargets>(EnumSource));
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
            const string UnescapedString = "This is not valid for \"hstore\" persistence";
            const string ExpectedEscapedString = "This is not valid for \\\"hstore\\\" persistence";

            Assert.AreEqual(ExpectedEscapedString, UnescapedString.Escape());
        }
    }
}
