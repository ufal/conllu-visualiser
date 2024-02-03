using System;
using System.Collections.Generic;
using System.Drawing;
using ConlluVisualiser;
using GraphVisualiser.BasicVisualiser;

namespace GraphVisualiser
{
    namespace EnhancedVisualiser
    {
        class EnhancedPointCounter : BasicPointCounter
        {
            /// <summary>
            /// The dictionary where all words with Deps attribute are saved. 
            /// Each word owns the set of deps parents and the levels, where the joining line between the word and parent lies.
            /// </summary>
            private Dictionary<IWord, Dictionary<string, int>> LevelsOfDeps = new Dictionary<IWord, Dictionary<string, int>>();
            /// <summary>
            /// The map of filled places in all existing levels. 
            /// True field means that in the proper level and index, there lies a joining line between some words.
            /// </summary>
            private List<List<bool>> filled = new List<List<bool>>();
            /// <summary>
            /// The set sorted by the levels.
            /// Each level owns a set of child/parent pairs that have a joining line on that level.
            /// </summary>
            public SortedList<int, List<KeyValuePair<string, string>>> SortedLevels { get; private set; }
            /// <summary>
            /// Returns the form size of the word in the current representation.
            /// </summary>
            protected override IVisitor FormSizeGetter { get; }
            /// <summary>
            /// The graphics schema used by the current designer - colors, font, size of point.
            /// </summary>
            private readonly GraphicsSchema Schema;

            /// <summary>
            /// Creates and initializes the point counter of the enhanced representation of the graph.
            /// </summary>
            /// <param name="schema"></param>
            public EnhancedPointCounter(GraphicsSchema schema)
            {
                FormSizeGetter = new FormSizeEnhanced(schema);
                Schema = schema;
            }
            /// <summary>
            /// Prepares the points of the words in the <paramref name="sentence"/>. Actualizes the point property of all words.
            /// </summary>
            /// <param name="sentence">The sentence whose word points are counted.</param>
            /// <param name="width">The optimal width of the space where the sentence is drawn.</param>
            /// <param name="height">The optimal height of the space where the sentence is drawn.</param>
            /// <param name="g">The graphics that is used to visualise the sentence.</param>
            /// <param name="schema">The color schema according to which the graph is drawn.</param>
            /// <returns>The graph width.</returns>
            public override float CountAndCreatePoints(ISentence sentence, int width, int height, Graphics g, GraphicsSchema schema)
            {
                // Initializes the structs to count the new coordinates.
                LevelsOfDeps.Clear();
                filled.Clear();
                // Prepares x-coordinates of all the words.
                float graphWidth = PrepareXes(sentence, g) + schema.SizeOfPoint.Width + minSpace;
                MaximumHeight = 1;
                // Counts the levels in which the words will be visualised in the graph.
                LevelsInEnhanced(sentence);
                //Space which remains for each level in the graph view. 
                // If the graph is too tall, the minimum spaces are used and the vertical scrollbar will appear.
                LevelDifference = height / (Math.Max(MaximumHeight - 1, 1));
                if (LevelDifference < schema.SizeOfPoint.Height + schema.BoldFont.Height + minLevelDiff)
                    LevelDifference = schema.SizeOfPoint.Height + schema.BoldFont.Height + minLevelDiff;
                // Space which rests on the left side of the graph if the graph in case of the thin graph.
                // Graph is situated in the middle of the panel.
                // If the graph is too wide, the left space is 0.
                LeftSpace = ((width - graphWidth) / 2);
                if (LeftSpace < 0) LeftSpace = 0;
                // Creates a place for a root point before the word points. 
                LeftSpace += schema.SizeOfPoint.Width + minSpace;
                // Prepares the information to find out the x and y coordinates of all words
                PreparePoints(sentence, minLevelDiff);
                // Sorts the levels and prepares the SortedLevels collection to allow draw the lines.
                SortLevels();
                return graphWidth + schema.SizeOfPoint.Width + minSpace;
            }

            /// <summary>
            /// Sorts the deps according to their levels.
            /// </summary>
            private void SortLevels()
            {
                SortedLevels = new SortedList<int, List<KeyValuePair<string, string>>>();
                // Goes over all the words and saves all deps according to their level.
                foreach (var word in LevelsOfDeps)
                {
                    foreach (var dep in word.Value)
                    {
                        if (!SortedLevels.ContainsKey(dep.Value))
                            SortedLevels[dep.Value] = new List<KeyValuePair<string, string>>();
                        SortedLevels[dep.Value].Add(new KeyValuePair<string, string>(word.Key.Id, dep.Key));
                    }
                }
            }

            /// <summary>
            /// Returns the level of the dependency line between <paramref name="child"/> and <paramref name="parent"/>.
            /// </summary>
            /// <param name="child">The child node dependant on the parent.</param>
            /// <param name="parent">The parent node.</param>
            /// <returns>The level of the joining line. -1, if the words are not joined.</returns>
            public int GetLevelDep(IWord child, IWord parent)
            {
                if (!LevelsOfDeps.ContainsKey(child) || !LevelsOfDeps[child].ContainsKey(parent.Id))
                    return -1;
                return LevelsOfDeps[child][parent.Id] + 1;
            }

            /// <summary>
            /// Counts the levels of lines in the enhanced representation.
            /// </summary>
            /// <param name="sentence">The sentence whose line levels are counted.</param>
            protected void LevelsInEnhanced(ISentence sentence)
            {
                for (int i = 0; i < sentence.CountWords; i++)
                {
                    // For each word
                    IWord current = sentence.GetWord(i);
                    // For all deps finds the minimum level, where the line can lie so that it does not overlap any other line.
                    foreach (var dep in current.Info.Deps)
                    {
                        int lev = 0;
                        // Repeat until the level is not found
                        while (!TryAddToLevel(current, sentence.GetWord(dep.Key), lev, sentence))
                        {
                            lev++;
                        }
                        // Adds the dependency line level to the collection
                        if (!LevelsOfDeps.ContainsKey(current))
                        {
                            LevelsOfDeps[current] = new Dictionary<string, int>();
                        }
                        LevelsOfDeps[current][dep.Key] = lev;
                    }
                }
                // Assigns the same level to all the words. The points lie in the middle of the levels.
                for (int i = 0; i < sentence.CountWords; i++)
                {
                    IWord current = sentence.GetWord(i);
                    current.GetWordPoint().Level = filled.Count / 2 + 1;
                }
                // Counts the maximum height as the sum of the count of levels and one for the line with points.
                MaximumHeight = filled.Count + 1;
            }

            /// <summary>
            /// Tries to add the dependency line between <paramref name="word"/> 
            /// and <paramref name="dep"/> at the given <paramref name="level"/>.
            /// </summary>
            /// <param name="word">The word whose line level is searched for.</param>
            /// <param name="dep">The word whose line level is searched for.</param>
            /// <param name="level">The level where the line tries to be situated.</param>
            /// <param name="sentence">The sentence where the words lie in.</param>
            /// <returns>True if the line can be in this level; otherwise, false.</returns>
            private bool TryAddToLevel(IWord word, IWord dep, int level, ISentence sentence)
            {
                // Assumes that the dep parameter is before the word parameter in the sentence. 
                // If not, do the same action in the swapped order.
                if (sentence.GetIndexOf(word) < sentence.GetIndexOf(dep))
                {
                    return TryAddToLevel(dep, word, level, sentence);
                }
                // If there is some other line in the given level.
                if (filled.Count > level)
                {
                    // Checks if all the space for the line in the level is empty. If not, returns false.
                    for (int i = sentence.GetIndexOf(dep); i < sentence.GetIndexOf(word); i++)
                    {
                        // Some other line lies on the place that is needed to draw this line.
                        if (filled[level][i])
                        {
                            return false;
                        }
                    }
                }
                // If the level is empty, initializes it to falses.
                else
                {
                    filled.Add(new List<bool>(new bool[sentence.CountWords]));
                }
                // All places that the line passes through are marked as occupied.
                for (int i = sentence.GetIndexOf(dep); i < sentence.GetIndexOf(word); i++)
                {
                    filled[level][i] = true;
                }
                return true;
            }

            /// <summary>
            /// Counts the starting X-coordinate of the word with a given <paramref name="id"/>.
            /// It takes into account all deps lines and the texts on them and ensures that all the deps texts fit on their lines.
            /// </summary>
            /// <param name="id">The id of the word whose coordinate is being counted.</param>
            /// <param name="preceding">The preceding word in the sentence.</param>
            /// <param name="g">The graphics where the word is drawn to.</param>
            protected override void SaveStartOfWord(string id, IWord preceding, Graphics g)
            {
                // Checks all deps of the preceding word.
                foreach (var dep in preceding.Info.Deps)
                {
                    // Gets the size that the deps text need to fit on the line.
                    float width = g.MeasureString(dep.Value, Schema.DepFont).Width;
                    if (!StartsOfWords.ContainsKey(dep.Key))
                    {
                        StartsOfWords[dep.Key] = 0;
                    }
                    // If there is a line whose dep word is on the left side of the word,
                    // the word must start at least on the place where the line ends. 
                    // The length of the line is at least the width of the text and the width of the point. 
                    if (GetFloatId(dep.Key) < GetFloatId(preceding.Id))
                    {
                        // The start will be the maximum of the two values.
                        StartsOfWords[preceding.Id] =
                            Math.Max(StartsOfWords[dep.Key] + width + 2 * Schema.SizeOfPoint.Width, StartsOfWords[preceding.Id]);
                    }
                    // If the dep word is on the right side of word, the same thing is done, but in the swapped order.
                    else
                    {
                        StartsOfWords[dep.Key] =
                            Math.Max(StartsOfWords[preceding.Id] + width + 2 * Schema.SizeOfPoint.Width, StartsOfWords[dep.Key]);
                    }
                }
                // Shifts the start of the word according to the word form widths.
                base.SaveStartOfWord(id, preceding, g);
            }

            /// <summary>
            /// Makes the float number from id to find out the order in the sentence.
            /// </summary>
            /// <param name="id">The id that is going to be transformed to float.</param>
            /// <returns>The float number that describes the id. If the id is in the bad format, returns 0.</returns>
            private float GetFloatId(string id)
            {
                // If the id can be transformed to float narmally, returns it.
                if (float.TryParse(id, out float floatId))
                {
                    return floatId;
                }
                // For the multiword tokens returns the first part of their id.
                string[] splitted = id.Split('-');
                if (splitted.Length == 2)
                {
                    return int.Parse(splitted[0]);
                }
                // If the id is in the bad format, returns 0.
                return 0;
            }
        }
    }
}
