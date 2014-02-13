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

using System.IO;

namespace Latvian.Tagging
{
    public class LuMiiTagger : LatvianTagger
    {
        private const string ModelResourceName = "Latvian.LuMii.Tagging.Resources.Model-2014-02-10-93.42-morpho.bin.gz";

        public LuMiiTagger()
            : base()
        {
        }

        public LuMiiTagger(string filename)
            : base(filename)
        {
        }

        public void Load()
        {
            using (Stream stream = typeof(LuMiiTagger).Assembly.GetManifestResourceStream(ModelResourceName))
                Load(stream);
        }
    }
}
