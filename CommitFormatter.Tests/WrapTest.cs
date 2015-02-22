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
using Adrup.CommitFormatter;

namespace Adrup.CommitFormatter.Tests
{
    [TestClass]
    public class WrapTest
    {
        private const int SubjectWidth = 10;
        private const int BodyWidth = 15;

        [TestMethod]
        public void Wrap_StandardText()
        {
            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            int caretIndex = 0;
            int caretIndexDelta = 0;
            var actual = TextHelper.Wrap(text, caretIndex, SubjectWidth, BodyWidth, false, out caretIndexDelta);
            var expected = "Lorem \nipsum dolor \nsit amet, \nconsectetur \nadipiscing \nelit.";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Wrap_ContainsLF_ShouldDisregardExistingLF()
        {
            string text = "Lorem \nipsum \ndolor \nsit\n amet, \nconsectetur \nadipiscing\n elit.\n";
            int caretIndex = 14;
            int caretIndexDelta = 0;
            
            var actual = TextHelper.Wrap(text, caretIndex, SubjectWidth, BodyWidth, false, out caretIndexDelta);
            var expected = "Lorem \nipsum dolor \nsit amet, \nconsectetur \nadipiscing \nelit.";
            int charsLeft = TextHelper.CountLineCharsLeft(actual, caretIndex + caretIndexDelta, SubjectWidth, BodyWidth, false);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(-1, caretIndexDelta);
            Assert.AreEqual(3, charsLeft);
        }

        [TestMethod]
        public void Wrap_ContainsCRLF_ShouldKeepExistingCRLF()
        {
            string text = "Lorem ipsum dolor sit\r\n amet, consectetur adipiscing\n elit.\n";
            int caretIndex = 54;
            int caretIndexDelta = 0;
            var actual = TextHelper.Wrap(text, caretIndex, SubjectWidth, BodyWidth, false, out caretIndexDelta);
            var expected = "Lorem \nipsum dolor sit\r\n amet, \nconsectetur \nadipiscing \nelit.";
            int charsLeft = TextHelper.CountLineCharsLeft(actual, caretIndex + caretIndexDelta, SubjectWidth, BodyWidth, false);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(3, caretIndexDelta);
            Assert.AreEqual(10, charsLeft);
        }
    }
}
