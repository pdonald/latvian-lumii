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
using System.Diagnostics;
using System.Linq;
using System.IO;

using Latvian.Morphology;
using Latvian.Tagging;
using Latvian.Tagging.Corpora;
using Latvian.Tagging.Evaluation;

using NUnit.Framework;

namespace Latvian.Tests.Tagging
{
    [TestFixture]
    public class LuMiiTaggerTests
    {
        private const string MorphoTrain = "Latvian.LuMii.Tests.Tagging.Resources.MorphoTrain.txt";
        private const string MorphoTest = "Latvian.LuMii.Tests.Tagging.Resources.MorphoTest.txt";
        private const string Morpho2Train = "Latvian.LuMii.Tests.Tagging.Resources.Morpho2Train.txt";
        private const string Morpho2Test = "Latvian.LuMii.Tests.Tagging.Resources.Morpho2Test.txt";
        private const string TaggerTrain = "Latvian.LuMii.Tests.Tagging.Resources.TaggerTrain.txt";
        private const string TaggerTest = "Latvian.LuMii.Tests.Tagging.Resources.TaggerTest.txt";

        private static readonly string[] ModelLatestData = new[] { Morpho2Train, Morpho2Test };

        [Test]
        public void CrossValidation_10Fold_Morpho()
        {
            CrossValidation("morpho", 0.93, 10, LoadAnalyzedCorpus(MorphoTrain), LoadAnalyzedCorpus(MorphoTest));
        }

        [Test]
        public void CrossValidation_10Fold_Morpho2()
        {
            CrossValidation("morpho2", 0.93, 10, LoadAnalyzedCorpus(Morpho2Train), LoadAnalyzedCorpus(Morpho2Test));
        }

        [Test]
        public void CrossValidation_10Fold_Tagger()
        {
            CrossValidation("tagger data", 0.92, 10, LoadUnanalyzedCorpus(TaggerTrain), LoadUnanalyzedCorpus(TaggerTest));
        }

        public void CrossValidation(string name, double minAccuracy, int folds, params IEnumerable<Sentence>[] sentences)
        {
            CrossValidation<LuMiiTagger> evaluation = new CrossValidation<LuMiiTagger>();
            evaluation.Folds = folds;
            evaluation.Randomize = true;
            evaluation.RandomSeed = 1;
            foreach (Sentence[] s in sentences)
                evaluation.Sentences.AddRange(s);

            Assert.Greater(evaluation.Sentences.Count, 0);
            
            var results = evaluation.Evaluate();

            Debug.WriteLine("{0}-fold cross validation for {1}", evaluation.Folds, name);
            Debug.WriteLine("{0} sentences, {1} tokens", evaluation.Sentences.Count, evaluation.Sentences.SelectMany(t => t).Count());
            Debug.WriteLine("Mean: {0:0.00}% [{1:0.00}..{2:0.00} @ 99%]", results.Mean, results.ConfidenceIntervalAt99.Lower, results.ConfidenceIntervalAt99.Upper);
            foreach (var fold in results.OrderBy(f => f.Fold))
                Debug.WriteLine("Fold {0}: {1:0.00}%", fold.Fold, fold.CorrectPercentage);
            Debug.WriteLine("Duration: {0}", results.Duration);

            Assert.Greater(results.Mean, minAccuracy < 1 ? minAccuracy * 100 : minAccuracy);
            Assert.Less(results.Mean, 0.97 * 100);
        }

        [Test]
        public void Split_Morpho()
        {
            Split("morpho", 0.93, LoadAnalyzedCorpus(MorphoTrain), LoadAnalyzedCorpus(MorphoTest));
        }

        [Test]
        public void Split_Morpho2()
        {
            Split("morpho2", 0.93, LoadAnalyzedCorpus(Morpho2Train), LoadAnalyzedCorpus(Morpho2Test));
        }

        [Test]
        public void Split_Tagger()
        {
            Split("tagger", 0.92, LoadUnanalyzedCorpus(TaggerTrain), LoadUnanalyzedCorpus(TaggerTest));
        }

        public void Split(string name, double minAccuracy, Sentence[] train, Sentence[] test)
        {
            Assert.Greater(train.Length, 0);
            Assert.Greater(test.Length, 0);
            Assert.Greater(train.Length, test.Length);

            LuMiiTagger tagger = new LuMiiTagger();

            Stopwatch trainTimer = new Stopwatch();
            trainTimer.Start();
            tagger.Train(train);
            trainTimer.Stop();

            Stopwatch tagTimer = new Stopwatch();
            tagTimer.Start();
            tagger.Tag(test);
            tagTimer.Stop();

            Token[] tokens = test.SelectMany(t => t).ToArray();
            double accuracy = (double)tokens.Count(t => t.IsTagCorrect) / tokens.Count();

            Debug.WriteLine("Split validation for " + name);
            Debug.WriteLine("Train: {0} sentences, {1} tokens", train.Length, train.SelectMany(t => t).Count());
            Debug.WriteLine("Test: {0} sentences, {1} tokens", test.Length, test.SelectMany(t => t).Count());
            Debug.WriteLine("Accuracy: {0:0.00}%", accuracy * 100);
            Debug.WriteLine("Train duration: {0} or {1:0} ms", trainTimer.Elapsed, trainTimer.ElapsedMilliseconds);
            Debug.WriteLine("Tag duration: {0} or {1:0} ms", tagTimer.Elapsed, tagTimer.ElapsedMilliseconds);
            Debug.WriteLine("Tag speed: {0:0.00} tokens/s", tokens.Length / tagTimer.Elapsed.TotalSeconds);

            Assert.Greater(accuracy, minAccuracy);
            Assert.Less(accuracy, 0.97);
        }

        [Test]
        public void LoadSave()
        {
            string trainResource = Morpho2Train;
            string testResource = Morpho2Test;
            double minAccuracy = 0.93;

            Sentence[] train = LoadAnalyzedCorpus(trainResource);
            Sentence[] test = LoadAnalyzedCorpus(testResource);
            Sentence[] test2 = LoadAnalyzedCorpus(testResource);

            Assert.Greater(train.Length, 0);
            Assert.Greater(test.Length, 0);
            Assert.Greater(train.Length, test.Length);

            string filename = Path.GetTempFileName();

            LuMiiTagger tagger = new LuMiiTagger();
            tagger.Train(train);
            tagger.Save(filename);
            tagger.Tag(test);

            LuMiiTagger tagger2 = new LuMiiTagger();
            tagger2.Load(filename);
            tagger2.Tag(test2);

            Token[] tokens = test.SelectMany(t => t).ToArray();
            Token[] tokens2 = test2.SelectMany(t => t).ToArray();

            double accuracy = (double)tokens.Count(t => t.IsTagCorrect) / tokens.Count();
            double accuracy2 = (double)tokens2.Count(t => t.IsTagCorrect) / tokens2.Count();

            Assert.AreEqual(accuracy, accuracy2, 0.0001);
            Assert.Greater(accuracy, minAccuracy);
            Assert.Less(accuracy, 0.97);

            File.Delete(filename);
        }

        [Test]
        public void Model_Latest()
        {
            LuMiiCorpus corpus = new LuMiiCorpus();
            List<Sentence> all = new List<Sentence>();
            foreach (string resource in ModelLatestData)
                all.AddRange(corpus.Load(this.GetType().Assembly.GetManifestResourceStream(resource)).ToArray());

            LuMiiTagger tagger = new LuMiiTagger();
            tagger.Load();
            tagger.Tag(all);
            
            Token[] tokens = all.SelectMany(t => t).ToArray();
            double accuracy = (double)tokens.Count(t => t.IsTagCorrect) / tokens.Count();

            Assert.Greater(accuracy, 0.99);
        }

        [Test]
        public void TagSpeed()
        {
            string trainResource = Morpho2Train;
            string testResource = Morpho2Test;
            int maxTokenCount = 1000000;
            double minAccuracy = 0.93;

            Sentence[] train = LoadAnalyzedCorpus(trainResource);
            Sentence[] test = LoadAnalyzedCorpus(testResource);

            List<Sentence> all = new List<Sentence>();
            int allTokenCount = 0;
            while (allTokenCount < maxTokenCount)
            {
                Sentence s = new Sentence();

                foreach (Sentence sentence in test)
                {
                    foreach (Token token in sentence)
                    {
                        s.Add(new Token(token));
                        allTokenCount++;
                        if (allTokenCount >= maxTokenCount) break;
                    }

                    if (allTokenCount >= maxTokenCount) break;
                }

                all.Add(s);

                if (allTokenCount >= maxTokenCount) break;
            }

            Assert.AreEqual(maxTokenCount, all.SelectMany(t => t).Count());

            LuMiiTagger tagger = new LuMiiTagger();
            tagger.Train(train);

            Stopwatch timer = new Stopwatch();
            timer.Start();
            tagger.Tag(all);
            timer.Stop();

            Token[] tokens = all.SelectMany(t => t).ToArray();
            double accuracy = (double)tokens.Count(t => t.IsTagCorrect) / tokens.Count();

            Assert.AreEqual(maxTokenCount, tokens.Length);

            Debug.WriteLine("Accuracy: {0:0.00}%", accuracy * 100);
            Debug.WriteLine("Tokens: {0}%", tokens.Length);
            Debug.WriteLine("Tag duration: {0} or {1:0} ms", timer.Elapsed, timer.ElapsedMilliseconds);
            Debug.WriteLine("Tag speed: {0:0.00} tokens/s", tokens.Length / timer.Elapsed.TotalSeconds);

            Assert.Greater(accuracy, minAccuracy);
            Assert.Less(accuracy, 0.97);
        }

        private Sentence[] LoadAnalyzedCorpus(string resourceName)
        {
            LuMiiCorpus corpus = new LuMiiCorpus();
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream(resourceName))
                return corpus.Load(stream).ToArray();
        }

        private Sentence[] LoadUnanalyzedCorpus(string resourceName)
        {
            LuMiiCorpus corpus = new LuMiiCorpus();
            LuMiiMorphology morphology = new LuMiiMorphology(); 
            
            Sentence[] sentences = null;
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream(resourceName))
                sentences = corpus.Load(stream).ToArray();

            int total = 0;
            int wrong = 0;

            List<Sentence> goodSentences = new List<Sentence>();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            foreach (Sentence sentence in sentences)
            {
                bool ignore = false;

                foreach (Token token in sentence)
                {
                    token.PossibleTags = morphology.Analyze(token.Text).ToArray();
                    
                    if (!token.PossibleTags.Any(t => t.Equals(token.CorrectTag)))
                    {
                        ignore = true;
                        break;
                    }
                    
                    Assert.Contains(token.CorrectTag, token.PossibleTags);
                }

                if (!ignore)
                {
                    goodSentences.Add(sentence);
                }
            }
            watch.Stop();
            Debug.WriteLine(watch.Elapsed);

            return goodSentences.ToArray();
        }
    }
}
