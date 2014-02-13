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

using System.Linq;

using Latvian.Tokenization;

using NUnit.Framework;

namespace Latvian.Tests.Tokenization
{
    [TestFixture]
    public class LuMiiTokenizerTests
    {
        [Test]
        public void Tokenize()
        {
            string text = "Es eju.";
            string[] tokens = new LuMiiTokenizer().Tokenize(text).Select(t => t.Text).ToArray();
            Assert.AreEqual(3, tokens.Length);
        }

        [Test]
        public void TokenizeSentences()
        {
            string text = "Es eju. Labrīt.";
            string[][] sentences = new LuMiiTokenizer().TokenizeSentences(text).Select(s => s.Select(t => t.Text).ToArray()).ToArray();
            Assert.AreEqual(2, sentences.Length);
            Assert.AreEqual(3, sentences[0].Length);
            Assert.AreEqual(2, sentences[1].Length);
        }
    }
}
