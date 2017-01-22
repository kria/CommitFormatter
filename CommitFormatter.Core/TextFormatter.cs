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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adrup.CommitFormatter.Core
{
    public class TextFormatter
    {
        private int _subjectWidth;
        private int _bodyWidth;
        private bool _blankSecondLine;

        public TextFormatter(int subjectWidth, int bodyWidth, bool blankSecondLine)
        {
            _subjectWidth = subjectWidth;
            _bodyWidth = bodyWidth;
            _blankSecondLine = blankSecondLine;
        }

        public string Wrap(string text, int caretIndex, out int caretIndexDelta)
        {
            caretIndexDelta = 0;
            int tokenizeIndexDelta = 0;
            var sb = new StringBuilder();
            var chars = text.ToCharArray();
            int currentIndex = 0;
            int currentLineNum = 1;

            var tokens = new Queue<string>(TextHelper.Tokenize(text, caretIndex, out tokenizeIndexDelta));
            caretIndexDelta += tokenizeIndexDelta;

            int currentWidth = 0;
            while (tokens.Count > 0)
            {
                string token = tokens.Dequeue();

                if (token == "\r" || token == "\n")
                {
                    currentWidth = 0;
                    sb.Append(token);
                    currentIndex += token.Length;
                    if (token == "\n") currentLineNum++;
                }
                else
                {
                    if (_blankSecondLine && currentWidth == 0 && currentLineNum == 2)
                    {
                        AddVirtualNewline(sb, caretIndex, ref currentIndex, ref currentLineNum, ref caretIndexDelta, ref currentWidth);
                    }
                    if (currentWidth > 0 && currentWidth + token.Length > GetLineMaxWidth(currentLineNum))
                    {
                        AddVirtualNewline(sb, caretIndex, ref currentIndex, ref currentLineNum, ref caretIndexDelta, ref currentWidth);
                    }
                    else if (currentWidth > 0 && token != " " && tokens.Count > 0 && tokens.Peek() == " "
                        && currentWidth + token.Length == GetLineMaxWidth(currentLineNum)) // if last char of row is a space we break before the last word so next line won't begin with a space
                    {
                        AddVirtualNewline(sb, caretIndex, ref currentIndex, ref currentLineNum, ref caretIndexDelta, ref currentWidth);
                    }

                    currentWidth += token.Length;
                    sb.Append(token);
                    currentIndex += token.Length;
                }
            }

            return sb.ToString();
        }

        private void AddVirtualNewline(StringBuilder sb, int caretIndex, ref int currentIndex, ref int currentLineNum, ref int caretIndexDelta, ref int currentWidth)
        {
            sb.Append('\n');
            if (currentIndex <= caretIndex + caretIndexDelta) caretIndexDelta++;
            currentIndex++;
            currentWidth = 0;
            currentLineNum++;

            if (_blankSecondLine && currentWidth == 0 && currentLineNum == 2)
            {
                AddVirtualNewline(sb, caretIndex, ref currentIndex, ref currentLineNum, ref caretIndexDelta, ref currentWidth);
            }
        }

        private int GetLineMaxWidth(int lineNum)
        {
            switch (lineNum)
            {
                case 1: return _subjectWidth;
                case 2: return _blankSecondLine ? 0 : _bodyWidth;
                default: return _bodyWidth;
            }
        }

        public int CountLineCharsLeft(string text, int caretIndex)
        {
            var chars = text.ToCharArray();
            int rowStartIndex = 0;
            int currentLineNum = 1;
            bool isStartSet = false;

            for (var i = caretIndex; i > 0; i--)
            {
                if (chars[i - 1] == '\r' || chars[i - 1] == '\n')
                {
                    if (!isStartSet)
                    {
                        rowStartIndex = i;
                        isStartSet = true;
                    }
                    if (chars[i - 1] == '\n') currentLineNum++;
                }
            }

            int rowEndIndex = caretIndex;
            while (rowEndIndex < chars.Length)
            {
                if (chars[rowEndIndex] == '\r' || chars[rowEndIndex] == '\n')
                {
                    break;
                }
                rowEndIndex++;
            }

            int charsLeft = GetLineMaxWidth(currentLineNum) - (rowEndIndex - rowStartIndex);

            return charsLeft;
        }
    }
}
