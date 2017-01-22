/*
 * CommitFormatter - http://github.com/kria/CommitFormatter
 * 
 * Copyright (C) 2015 Kristian Adrup
 * 
 * This file is part of CommitFormatter.
 * 
 * CommitFormatter is free software: you can redistribute it and/or modify 
 * it under the terms of the GNU General Public License as published by 
 * the Free Software Foundation, either version 3 of the License, or (at 
 * your option) any later version. See included file COPYING for details.
 */

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adrup.CommitFormatter.Core;

namespace Adrup.CommitFormatter.Tests
{
    [TestClass]
    public class TokenizeTests
    {

        [TestMethod]
        public void Tokenize_StandardText()
        {
            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit." };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_SingleLeadingWhitespace()
        {
            string text = " Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { " ", "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit." };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_MultipleLeadingWhitespace()
        {
            string text = "   Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { " ", " ", " ", "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit." };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_SingleTrailingWhitespace()
        {
            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit.", " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_MultipleTrailingWhitespace()
        {
            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.   ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit.", " ", " ", " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_ContainsCRLF()
        {
            string text = "Lorem ipsum dolor sit amet, \r\nconsectetur adipiscing elit.   ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "\r", "\n", "consectetur", " ", "adipiscing", " ", "elit.", " ", " ", " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_ContainsLF_ShouldRemoveLF()
        {
            string text = "Lorem ipsum dolor sit amet, \nconsectetur adipiscing elit.   ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit.", " ", " ", " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_ContainsSequentialLF_ShouldRemoveLF()
        {
            string text = "Lorem ipsum dolor sit amet, \n\nconsectetur adipiscing elit.   ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit.", " ", " ", " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_SingleLF_ShouldReturnEmpty()
        {
            string text = "\n";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void Tokenize_SingleLFWithTrailingWhitespace()
        {
            string text = "\n ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_SingleLFWithLeadingAndTrailingWhitespace()
        {
            string text = " \n ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { " ", " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_SingleCRLF()
        {
            string text = "\r\n";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "\r", "\n" };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_CRLFWithTrailingWhitespace()
        {
            string text = "\r\n ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "\r", "\n", " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_CRLFWithLeadingAndTrailingWhitespace()
        {
            string text = " \r\n ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { " ", "\r", "\n", " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_EmptyString()
        {
            string text = "";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void Tokenize_SingleWhitespaceOnly()
        {
            string text = " ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_MultipleWhitespaceOnly()
        {
            string text = "   ";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { " ", " ", " " };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Tokenize_ContainsLFBeforeCaret_ShouldDecreaseOffset()
        {
            string text = "Lorem ipsum dolor sit amet, \n\nconsectetur adipiscing elit.   ";
            int caretIndex = 35;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit.", " ", " ", " " };
            CollectionAssert.AreEqual(expected, actual);
            Assert.AreEqual(-2, caretIndexDelta);
        }

        [TestMethod]
        public void Tokenize_ContainsLFJustBeforeCaret_ShouldDecreaseOffset()
        {
            string text = "Lorem ipsum dolor sit amet, \n\nconsectetur adipiscing elit.   ";
            int caretIndex = 30;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit.", " ", " ", " " };
            CollectionAssert.AreEqual(expected, actual);
            Assert.AreEqual(-2, caretIndexDelta);
        }

        [TestMethod]
        public void Tokenize_ContainsLFJustAfterCaret_ShouldNotChangeOffset()
        {
            string text = "Lorem ipsum dolor sit amet, \n\nconsectetur adipiscing elit.   ";
            int caretIndex = 28;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit.", " ", " ", " " };
            CollectionAssert.AreEqual(expected, actual);
            Assert.AreEqual(0, caretIndexDelta);
        }

        [TestMethod]
        public void Tokenize_ContainsLFJusteforeAndAfterCaret_ShouldDecreaseOffsetByOne()
        {
            string text = "Lorem ipsum dolor sit amet, \n\nconsectetur adipiscing elit.   ";
            int caretIndex = 29;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Lorem", " ", "ipsum", " ", "dolor", " ", "sit", " ", "amet,", " ", "consectetur", " ", "adipiscing", " ", "elit.", " ", " ", " " };
            CollectionAssert.AreEqual(expected, actual);
            Assert.AreEqual(-1, caretIndexDelta);
        }

        [TestMethod]
        public void Tokenize_Unicode()
        {
            string text = "Эи еюж ютроквюы окюррырэт, \nпюрто нонюмэш номинави эи вяш";
            int caretIndex = 28;
            int caretIndexDelta = 0;
            var actual = TextHelper.Tokenize(text, caretIndex, out caretIndexDelta);
            var expected = new[] { "Эи", " ", "еюж", " ", "ютроквюы", " ", "окюррырэт,", " ", "пюрто", " ", "нонюмэш", " ", "номинави", " ", "эи", " ", "вяш" };
            CollectionAssert.AreEqual(expected, actual);
            Assert.AreEqual(-1, caretIndexDelta);
        }

        //
    }
}
