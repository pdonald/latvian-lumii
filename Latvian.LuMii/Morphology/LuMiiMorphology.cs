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
using System.IO.Compression;

using lv.semti.morphology.analyzer;
using lv.semti.morphology.attributes;

using ikvm.io;

namespace Latvian.Morphology
{
    public class LuMiiMorphology
    {
        internal Analyzer analyzer;

        public LuMiiMorphology()
        {
            using (Stream lexicon = new GZipStream(typeof(LuMiiMorphology).Assembly.GetManifestResourceStream("Latvian.LuMii.Morphology.Resources.Lexicon.xml.gz"), CompressionMode.Decompress))
            using (Stream lexiconCore = new GZipStream(typeof(LuMiiMorphology).Assembly.GetManifestResourceStream("Latvian.LuMii.Morphology.Resources.Lexicon_core.xml.gz"), CompressionMode.Decompress))
            using (Stream lexiconValerijs = new GZipStream(typeof(LuMiiMorphology).Assembly.GetManifestResourceStream("Latvian.LuMii.Morphology.Resources.Lexicon_valerijs.xml.gz"), CompressionMode.Decompress))
            //using (Stream lexiconOnomastica = new GZipStream(typeof(LuMiiMorphology).Assembly.GetManifestResourceStream("Latvian.LuMii.Morphology.Resources.Lexicon_onomastica.xml.gz"), CompressionMode.Decompress))
            using (Stream exceptions = typeof(LuMiiMorphology).Assembly.GetManifestResourceStream("Latvian.LuMii.Morphology.Resources.Exceptions.txt"))
            using (InputStreamWrapper lexiconWrapper = new InputStreamWrapper(lexicon))
            using (InputStreamWrapper lexiconCoreWrapper = new InputStreamWrapper(lexiconCore))
            using (InputStreamWrapper lexiconValerijsWrapper = new InputStreamWrapper(lexiconValerijs))
            //using (InputStreamWrapper lexiconOnomasticaWrapper = new InputStreamWrapper(lexiconOnomastica))
            using (InputStreamWrapper exceptionsWrapper = new InputStreamWrapper(exceptions))
            {
                analyzer = new Analyzer(lexiconWrapper, new[] { lexiconCoreWrapper, lexiconValerijsWrapper, /* lexiconOnomasticaWrapper */ }, exceptionsWrapper);
                analyzer.enablePrefixes = true;
                analyzer.enableDiminutive = true;
                analyzer.enableVocative = true;
                //analyzer.enableGuessing = true;
                //analyzer.enableAllGuesses = true;
            }
        }

        public IEnumerable<LuMiiTag> Analyze(string word)
        {
            foreach (AttributeValues form in analyzer.analyze(word).wordforms)
            {
                yield return new LuMiiTag(MarkupConverter.toKamolsMarkup(form), form.getValue(AttributeNames.i_Lemma));
            }
        }
    }
}
