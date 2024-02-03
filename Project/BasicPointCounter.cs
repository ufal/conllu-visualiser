using System;
using System.Collections.Generic;
using System.Drawing;
using ConlluVisualiser;

namespace GraphVisualiser
{
    namespace BasicVisualiser
    {
        class BasicPointCounter
        {
            /// <summary>
            /// The half of the space which rests in the TreePanel after counting the graph width.
            /// </summary>
            protected float LeftSpace { get; set; } = 0;
            /// <summary>
            /// The height of each floor of the graph.
            /// </summary>
            public float LevelDifference { get; protected set; }
            /// <summary>
            /// The count of floors in the graph.
            /// </summary>
            public int MaximumHeight { get; protected set; } = 1;
            /// <summary>
            /// The constant that determines the minimum height of floor.
            /// </summary>
            public const int minLevelDiff = 10;
            /// <summary>
            /// The constant that determines the minimal space between two words.
            /// </summary>
            protected const float minSpace = 10;
            /// <summary>
            /// Returns the form size of the word in the current representation.
            /// </summary>
            protected virtual IVisitor FormSizeGetter { get; }
            /// <summary>
            /// The constant on whose index in <seealso cref="StartsOfWords"/> the graph width is saved.
            /// </summary>
            protected const string ExtraID = "EXTRA";
            /// <summary>
            /// The counted x-coordinates of word points in the sentence. 
            /// Key Value pairs means the word Ids and its starting place according to the width of the preceding words.
            /// </summary>
            protected Dictionary<string, float> StartsOfWords;

            /// <summary>
            /// Creates the point counter and initializes it according to the representation.
            /// </summary>
            /// <param name="schema"></param>
            public BasicPointCounter(GraphicsSchema schema)
            {
                FormSizeGetter = new FormSizeBasic(schema);
            }

            /// <summary>
            /// Creates the point counter without initialization.
            /// </summary>
            protected BasicPointCounter() { }

            /// <summary>
            /// Prepares the points of the words in the <paramref name="sentence"/>. Actualizes the point property of all words.
            /// </summary>
            /// <param name="sentence">The sentence whose word points are counted.</param>
            /// <param name="width">The optimal width of the space where the sentence is drawn.</param>
            /// <param name="height">The optimal height of the space where the sentence is drawn.</param>
            /// <param name="g">The graphics that is used to visualise the sentence.</param>
            /// <param name="schema">The color schema according to which the graph is drawn.</param>
            /// <returns>The graph width.</returns>
            public virtual float CountAndCreatePoints(ISentence sentence, int width, int height, Graphics g, GraphicsSchema schema)
            {
                // Prepares x-coordinates of all the words.
                float graphWidth = PrepareXes(sentence, g);
                MaximumHeight = 1;
                // Counts the levels in which the words will be visualised in the graph.
                HeightOfSubtrees(sentence.Root, 1);
                //Space which remains for each level in the graph view. 
                // If the graph is too tall, the minimum spaces are used and the vertical scrollbar will appear.
                LevelDifference = height / (Math.Max(MaximumHeight - 1, 1));
                if (LevelDifference < schema.SizeOfPoint.Height + schema.BoldFont.Height + minLevelDiff)
                    LevelDifference = schema.SizeOfPoint.Height + schema.BoldFont.Height + minLevelDiff;
                // Space which rests on the left side of the graph in case of the thin graph.
                // Graph is situated in the middle of the panel.
                // If the graph is too wide, the left space is 0.
                LeftSpace = (width - graphWidth) / 2;
                if (LeftSpace < 0) LeftSpace = 0;
                // Prepares the information to find out the x and y coordinates of all words
                PreparePoints(sentence, minLevelDiff);
                return graphWidth;
            }

            /// <summary>
            /// Returns the position of the root of the sentence according the graph width.
            /// </summary>
            /// <param name="schema">The graphics schema of the graph.</param>
            /// <returns>The point of the root.</returns>
            public PointF GetRootPoint(GraphicsSchema schema)
            {
                return new PointF(LeftSpace + StartsOfWords[ExtraID] / 2f - schema.SizeOfPoint.Width / 2f, minLevelDiff);
            }

            /// <summary>
            /// Returns the size that the written form of the <paramref name="word"/> needs to be visualised .
            /// The width of visualised word is at least as big as the <see cref="GraphicsSchema.SizeOfPoint"/> width is.
            /// If the word is not shown, the size is 0.
            /// </summary>
            /// <param name="word">The word whose form size is being counted.</param>
            /// <param name="g">The graphics on which the word would be written - the form size is counted in relation to this graphics.</param>
            /// <returns>The maximum of the size of the word form and the size of point. 0, if the word is not visualised.</returns>
            public SizeF GetFormSize(IWord word, Graphics g)
            {
                return (SizeF)word.Accept(FormSizeGetter, g);
            }


            /// <summary>
            /// Counts the coordinates of the word point according to prepared 
            /// <seealso cref="StartsOfWords"/>, <seealso cref="LevelDifference"/> and <seealso cref="LeftSpace"/>
            /// </summary>
            /// <param name="word">The word whose point is counted</param>
            /// <returns></returns>
            private PointF GetPoint(IWord word, int minLevelDiff)
            {
                // Add left space to the counted start to shift the graph to the middle
                float x = StartsOfWords[word.Id] + LeftSpace;
                float y = LevelDifference * (word.GetWordPoint().Level - 1) + minLevelDiff;
                return new PointF(x, y);
            }

            /// <summary>
            /// Counts the graph levels of the words in the sentence
            /// </summary>
            /// <param name="subRoot">The root of the subtree.</param>
            /// <param name="actualLevel">The level of the root of the subtree.</param>
            protected virtual void HeightOfSubtrees(ITreeWord subRoot, int actualLevel)
            {
                // Saves the root Level
                subRoot.GetWordPoint().Level = actualLevel;
                // All children will have the same lavel as their parent plus one
                foreach (ITreeWord child in subRoot.Children.Values)
                {
                    HeightOfSubtrees(child, actualLevel + 1);
                }
                // We test if this level is the maximum in the found levels.
                if (actualLevel > MaximumHeight)
                {
                    MaximumHeight = actualLevel;
                }
            }

            /// <summary>
            /// Counts the starting positions of all words in the sentence and saves the results into the <seealso cref="StartsOfWords"/>.
            /// </summary>
            /// <param name="sentence">The sentence whose graph is going to be visualised.</param>
            /// <returns>The final width of the graph.</returns>
            protected float PrepareXes(ISentence sentence, Graphics g)
            {
                // Initialize the dictionary. The initializing word will start in 0.
                StartsOfWords = new Dictionary<string, float>
                {
                    ["0"] = 0
                };
                IWord lastWord = sentence.Root;
                for (int i = 1; i < sentence.CountWords; i++)
                {
                    IWord word = sentence.GetWord(i);
                    // Saves the start of the word according to the end of the preceding word
                    SaveStartOfWord(word.Id, lastWord, g);
                    // This word will be the preceding in the next iteration
                    lastWord = word;
                }
                // To find out the final graph width, we will add the extra word. 
                // The position where the next word would start is the graph width.
                SaveStartOfWord(ExtraID, lastWord, g);
                // Never returns 0, if there is at least one visualised word (the root is in each sentence).
                return StartsOfWords[ExtraID];
            }

            /// <summary>
            /// Saves the start of the word to  <seealso cref="StartsOfWords"/> according to the preceding word. 
            /// </summary>
            /// <param name="id">Id of the current word</param>
            /// <param name="preceding">The preceding word</param>
            protected virtual void SaveStartOfWord(string id, IWord preceding, Graphics g)
            {
                // Gets the form size according to the representation and the word type
                // If the word should not be visualised, the returned value is 0.
                float form_size = GetFormSize(preceding, g).Width;
                if (!StartsOfWords.ContainsKey(id))
                {
                    StartsOfWords[id] = 0;
                }
                StartsOfWords[id] = Math.Max(StartsOfWords[preceding.Id] + form_size, StartsOfWords[id]);
                // If the word should be visualised, we add the space between the words.
                if (form_size != 0)
                    StartsOfWords[id] += minSpace;
            }

            /// <summary>
            /// Method that counts the points of all words and saves the point to the word objects.
            /// If the word is not visualised in the current representation, its point is set to default.
            /// </summary>
            /// <param name="sentence">The sentence whose word points are counted.</param>
            protected void PreparePoints(ISentence sentence, int minLevelDiff)
            {
                IWord last = sentence.Root;
                // Saves the points to all visualised words
                for (int i = 0; i < sentence.CountWords; i++)
                {
                    IWord word = sentence.GetWord(i);
                    // If the x-coordinates are identical, the last point should not be visualised. Its point is set to be empty.
                    if (StartsOfWords[word.Id] == StartsOfWords[last.Id])
                        last.GetWordPoint().SetPoint(new PointF());
                    // Sets the word point to the counted point.
                    word.GetWordPoint().SetPoint(GetPoint(word, minLevelDiff));
                    last = word;
                }
            }
        }
    }
}
