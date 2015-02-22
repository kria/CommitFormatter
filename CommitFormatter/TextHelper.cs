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

namespace Adrup.CommitFormatter
{
    public static class TextHelper
    {
        public static int CountLineCharsLeft(string text, int caretIndex, int subjectWidth, int bodyWidth, bool blankSecondLine)
        {
            var formatter = new TextFormatter(subjectWidth, bodyWidth, blankSecondLine);
            return formatter.CountLineCharsLeft(text, caretIndex);
        }

        public static string Wrap(string text, int caretIndex, int subjectWidth, int bodyWidth, bool blankSecondLine, out int caretIndexDelta)
        {
            var formatter = new TextFormatter(subjectWidth, bodyWidth, blankSecondLine);
            return formatter.Wrap(text, caretIndex, out caretIndexDelta);
        }

        public static string[] Tokenize(string text, int caretIndex, out int caretIndexDelta)
        {
            caretIndexDelta = 0;
            var tokens = new List<string>();
            var chars = text.ToCharArray();
            int latestWordBoundaryIndex = 0;

            bool inWhitespace = false;

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == chars.Length - 1) // end of text
                {
                    if (char.IsWhiteSpace(chars[i]))
                    {
                        if (!inWhitespace)
                        {
                            var sb = new StringBuilder();
                            sb.Append(chars, latestWordBoundaryIndex, i - latestWordBoundaryIndex);
                            sb.Replace("\n", "");
                            if (sb.Length > 0)
                                tokens.Add(sb.ToString());  
                        }

                        if (chars[i] == '\n' && (i == 0 || chars[i - 1] != '\r'))
                        {
                            if (i < caretIndex) caretIndexDelta--; // If linebreak is removed before caret - move back caret
                            continue; // Don't add virtual linebreaks that the plugin has set
                        }
                        else
                        {
                            tokens.Add(chars[i].ToString());
                            inWhitespace = true;
                        }
                    }
                    else // ends on non whitespace
                    {
                        if (inWhitespace) latestWordBoundaryIndex = i;
                        var sb = new StringBuilder();
                        sb.Append(chars, latestWordBoundaryIndex, i - latestWordBoundaryIndex + 1);
                        sb.Replace("\n", "");
                        if (sb.Length > 0)
                            tokens.Add(sb.ToString()); 
                    }

                }
                else if (chars[i] == '\n' && (i == 0 || chars[i - 1] != '\r'))
                {
                    if (i < caretIndex) caretIndexDelta--; // If linebreak is removed before caret - move back caret
                    continue; // Don't add virtual linebreaks that the plugin has set
                }
                else if (char.IsWhiteSpace(chars[i]))
                {
                    if (!inWhitespace && latestWordBoundaryIndex != i)
                    {
                        var sb = new StringBuilder();
                        sb.Append(chars, latestWordBoundaryIndex, i - latestWordBoundaryIndex);
                        sb.Replace("\n", "");
                        if (sb.Length > 0)
                            tokens.Add(sb.ToString()); 
                    }
                    tokens.Add(chars[i].ToString());
                    inWhitespace = true;
                }
                else
                {
                    if (inWhitespace) latestWordBoundaryIndex = i;
                    inWhitespace = false;
                }
            }

            return tokens.ToArray();
        }
    }
}
