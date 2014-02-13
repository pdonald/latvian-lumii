// Copyright (C) 2014 Pēteris Ņikiforovs
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Latvian.Tokenization.Tokens
{
    public abstract class LuMiiExceptionToken : PatternToken
    {
        private const string ResourceName = "Latvian.LuMii.Morphology.Resources.Exceptions.txt";

        protected IEnumerable<string> Exceptions
        {
            get
            {
                using (StreamReader reader = new StreamReader(typeof(LuMiiExceptionToken).Assembly.GetManifestResourceStream(ResourceName)))
                {
                    while (!reader.EndOfStream)
                        yield return reader.ReadLine();
                }
            }
        }

        protected string MakePattern(IEnumerable<string> words)
        {
            if (!words.Any())
                return null;

            var patterns = words.Select(word => "[" + string.Join("].[", word.Select(c => char.ToLower(c) != char.ToUpper(c) ? char.ToLower(c) + "" + char.ToUpper(c) : c.ToString())) + "]");
            return "(" + string.Join(")|(", patterns) + ")";
        }
    }

    public class LuMiiAbbreviationToken : LuMiiExceptionToken
    {
        public override string Pattern
        {
            get { return MakePattern(Exceptions.Where(w => (w.Contains('.') || w.Contains('-') || w.Contains('/')) && !w.Contains(']'))); }
        }
    }

    public class LuMiiCollocationToken : LuMiiExceptionToken
    {
        public override string Pattern
        {
            get { return MakePattern(Exceptions.Where(w => w.Contains(' ') && !w.Contains('.') && !w.Contains(']'))); }
        }
    }
}
