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
using System.Linq;

using Latvian.Morphology;

using java.util;

using lv.semti.morphology.analyzer;

namespace Latvian.Tokenization
{
    public class LuMiiTokenizer : ITokenizer, ISentenceTokenizer
    {
        LuMiiMorphology morphology;

        public LuMiiTokenizer()
            : this(new LuMiiMorphology())
        {
        }

        public LuMiiTokenizer(LuMiiMorphology morphology)
        {
            this.morphology = morphology;
        }

        public IEnumerable<Token> Tokenize(IEnumerable<char> text)
        {
            foreach (Word word in Splitting.tokenize(morphology.analyzer, new string(text.ToArray())))
            {
                yield return new Token { Text = word.getToken() };
            }
        }

        public IEnumerable<IEnumerable<Token>> TokenizeSentences(IEnumerable<char> text)
        {
            foreach (LinkedList sentence in Splitting.tokenizeSentences(morphology.analyzer, new string(text.ToArray())))
            {
                yield return sentence.OfType<Word>().Select(word => new Token { Text = word.getToken() });
            }
        }
    }
}
