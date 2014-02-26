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
            using (InputStreamWrapper lexicon = LoadResource("Latvian.LuMii.Morphology.Resources.Lexicon.xml.gz"))
            using (InputStreamWrapper lexiconCore = LoadResource("Latvian.LuMii.Morphology.Resources.Lexicon_core.xml.gz"))
            using (InputStreamWrapper lexiconValerijs = LoadResource("Latvian.LuMii.Morphology.Resources.Lexicon_valerijs.xml.gz"))
            using (InputStreamWrapper lexiconVietas = LoadResource("Latvian.LuMii.Morphology.Resources.Lexicon_vietas.xml.gz"))
            //using (InputStreamWrapper lexiconOnomasticaWrapper = LoadResource("Latvian.LuMii.Morphology.Resources.Lexicon_onomastica.xml.gz"))
            using (InputStreamWrapper exceptions = LoadResource("Latvian.LuMii.Morphology.Resources.Exceptions.txt", compressed: false))
            {
                analyzer = new Analyzer(lexicon, new[] { lexiconCore, lexiconValerijs, lexiconVietas, /* lexiconOnomastica */ }, exceptions);
                analyzer.enableVocative = true;
                analyzer.enableGuessing = true;
                analyzer.enablePrefixes = true;
                analyzer.enableAllGuesses = true;
            }
        }

        private static InputStreamWrapper LoadResource(string name, bool compressed = true)
        {
            Stream resource = typeof(LuMiiMorphology).Assembly.GetManifestResourceStream(name);
            Stream stream = compressed ? new GZipStream(resource, CompressionMode.Decompress) : resource;
            InputStreamWrapper wrapper = new InputStreamWrapper(stream);
            return wrapper;
        }

        public IEnumerable<LuMiiTag> Analyze(string word)
        {
            foreach (AttributeValues form in analyzer.analyze(word).wordforms)
            {
                form.removeNonlexicalAttributes();
                yield return new LuMiiTag(MarkupConverter.toKamolsMarkup(form), form.getValue(AttributeNames.i_Lemma));
            }
        }
    }
}
