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

namespace Latvian.Tagging.Corpora
{
    public class LuMiiCorpus
    {
        public LuMiiCorpus()
        {
            IgnoredSentences = new List<Sentence>();
        }

        public List<Sentence> IgnoredSentences
        {
            get;
            private set;
        }

        public IEnumerable<Sentence> Load(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                foreach (Sentence sentence in Load(stream))
                    yield return sentence;
        }

        public IEnumerable<Sentence> Load(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                Sentence sentence = null;
                bool ignore = false;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    if (line == "<s>")
                    {
                        if (sentence != null && sentence.Count > 0)
                        {
                            if (!ignore)
                                yield return sentence;
                            if (ignore)
                                IgnoredSentences.Add(sentence);
                        }

                        sentence = new Sentence();
                        ignore = false;
                    }
                    else if (line == "</s>")
                    {
                        if (sentence == null)
                            throw new InvalidDataException();

                        if (sentence.Count > 0)
                        {
                            if (!ignore)
                                yield return sentence;
                            if (ignore)
                                IgnoredSentences.Add(sentence);
                        }

                        sentence = null;
                    }
                    else
                    {
                        if (sentence == null)
                            throw new InvalidDataException();

                        string[] values = line.Trim().Split('\t');

                        if (values.Length < 2)
                            throw new InvalidDataException();

                        if (values.Skip(1).Contains("N/A"))
                            ignore = true;

                        sentence.Add(new Token(
                            token: values[0],
                            possibleTags: values.Skip(1).Distinct().OrderBy(t => t).Select(t => new Tag(t)).ToArray(),
                            correctTag: new Tag(values[1]),
                            sentence: sentence));
                    }
                }

                if (sentence != null && sentence.Count > 0)
                {
                    if (!ignore)
                        yield return sentence;
                    if (ignore)
                        IgnoredSentences.Add(sentence);
                }
            }
        }
    }
}
