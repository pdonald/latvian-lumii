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
using Latvian.Tokenization.Tokens;

using NUnit.Framework;

namespace Latvian.Tests.Tokenization
{
    [TestFixture]
    public class LuMiiTokensTests
    {
        [Test]
        public void kā_arī()
        {
            LatvianTokenizer tokenizer = new LatvianTokenizer(compile: false);
            tokenizer.Add<LuMiiCollocationToken>();
            tokenizer.Compile();

            string text = "Es skrienu kā arī lecu.";

            Token[] tokens = tokenizer.Tokenize(text).ToArray();

            Assert.AreEqual(5, tokens.Length);
            Assert.AreEqual("kā arī", tokens[2].Text);
        }

        [Test]
        public void ļcien()
        {
            LatvianTokenizer tokenizer = new LatvianTokenizer(compile: false);
            tokenizer.Add<LuMiiAbbreviationToken>();
            tokenizer.Compile();

            string text = "Ļ.cien. kungs!";

            Token[] tokens = tokenizer.Tokenize(text).ToArray();

            Assert.AreEqual(3, tokens.Length);
            Assert.AreEqual("Ļ.cien.", tokens[0].Text);
        }

        [Test]
        public void kaut_jel_milj_kgs()
        {
            LatvianTokenizer tokenizer = new LatvianTokenizer(compile: false);
            tokenizer.Add<LuMiiAbbreviationToken>();
            tokenizer.Add<LuMiiCollocationToken>();
            tokenizer.Compile();

            string text = "kaut JEL 2 milj. mans k-gs";

            Token[] tokens = tokenizer.Tokenize(text).ToArray();

            Assert.AreEqual(5, tokens.Length);
            Assert.AreEqual("kaut JEL", tokens[0].Text);
            Assert.AreEqual("milj.", tokens[2].Text);
            Assert.AreEqual("k-gs", tokens[4].Text);
        }
    }
}
