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

using Latvian.Morphology;

using NUnit.Framework;

namespace Latvian.Tests.Morphology
{
    [TestFixture]
    public class LuMiiMorphologyTests
    {
        [Test]
        public void roku()
        {
            LuMiiMorphology morphology = new LuMiiMorphology();
            LuMiiTag[] tags = morphology.Analyze("roku").ToArray();
            Assert.AreEqual(3, tags.Length);
            Assert.IsTrue(tags.Any(t => t.Lemma == "roka" && t.Msd == "ncfsa4"));
            Assert.IsTrue(tags.Any(t => t.Lemma == "roka" && t.Msd == "ncfpg4"));
            Assert.IsTrue(tags.Any(t => t.Lemma == "rakt" && t.Msd == "vmnipt11san"));
        }

        [Test]
        public void pokeplerksts()
        {
            LuMiiMorphology morphology = new LuMiiMorphology();
            LuMiiTag[] tags = morphology.Analyze("pokeplerksts").ToArray();
            Assert.AreEqual(0, tags.Length);
        }
    }
}
