using System;
using System.Collections.Generic;
using System.Linq;

namespace ConlluVisualiser
{
    /// <summary>
    /// The sentence factory that creates the new sentence according to the text from file in CoNLL-U format.
    /// </summary>
    class ConlluSentenceFactory : ISentenceFactory
    {
        /// <summary>
        /// The class that holds and processes information to create one sentence.
        /// </summary>
        class OneSentenceParts
        {
            /// <summary>
            /// The positions of word ids in the sentence.
            /// </summary>
            public Dictionary<string, int> Positions { get; } = new Dictionary<string, int>();
            /// <summary>
            /// The words (word attribute sets) in the sentence grouped by their parents. The parent id is allways the int value.
            /// </summary>
            public Dictionary<int, List<WordInfo>> Words { get; } = new Dictionary<int, List<WordInfo>>();
            /// <summary>
            /// The empty nodes grouped by their main word id. The main word id is allways the int value.
            /// </summary>
            Dictionary<int, List<EmptyNodeWord>> EmptyNodes = new Dictionary<int, List<EmptyNodeWord>>();
            /// <summary>
            /// The multiwords that join the words in the sentence. 
            /// On each idex of the word that is a part of a multiword token (according to <seealso cref="Positions"/>), 
            /// there is the proper multiword token.
            /// </summary>
            Dictionary<int, MultiWord> Joined { get; } = new Dictionary<int, MultiWord>();
            /// <summary>
            /// The list of the sentence attributes.
            /// </summary>
            List<string> SentenceInfo { get; } = new List<string>();
            /// <summary>
            /// Says if the sentence is empty - without any words and sentence attributes.
            /// </summary>
            public bool IsEmpty { get => Positions.Count == 0 && SentenceInfo.Count == 0; }

            /// <summary>
            /// Creates the word attributes set from the conll-u format and adds it to the collections according to its type. 
            /// </summary>
            /// <param name="properties">The set of attributes.</param>
            public void AddWord(string[] properties)
            {
                // Creates the word info set. Can throw the Fromat exception if the properties array is not in a good format.
                WordInfo info = new WordInfo(properties);
                // If the id is integer, it is the basic word.
                if (int.TryParse(info.Id, out int Id))
                {
                    AddBasicWord(info);
                }
                // Else it must be empty node word or the multiword token.
                else
                {
                    if (!TryAddMultiword(info) && !TryAddEmptyWord(info))
                    {
                        throw new FormatException();
                    }
                }
                // Saves the position of the word.
                Positions.Add(info.Id, Positions.Count + 1);
            }

            /// <summary>
            /// Adds the basic word to the list of words according to its parent.
            /// </summary>
            /// <param name="info">The attribute set of the word.</param>
            private void AddBasicWord(WordInfo info)
            {
                int intParent = int.Parse(info.Head);
                if (!Words.ContainsKey(intParent))
                {
                    Words[intParent] = new List<WordInfo>();
                }
                // Adds the word info to its parent group of children.
                Words[intParent].Add(info);
            }

            /// <summary>
            /// Tries to add the word as a multiword token. Returns true, if the word is a multiword token.
            /// </summary>
            /// <param name="info">The set of attributes of the word.</param>
            /// <returns>True, if the word is a multiword token; otherwise, false.</returns>
            private bool TryAddMultiword(WordInfo info)
            {
                // Multiword id must be in a form x-y.
                string[] splittedJoined = info.Id.Split('-');
                if (splittedJoined.Length == 2)
                {
                    // Creates a new multiword token.
                    MultiWord new_joined = new MultiWord(info.Form, info.Id)
                    {
                        Info = info
                    };
                    // Assign the multiword token to all indexes of its subwords.
                    for (int i = int.Parse(splittedJoined[0]); i <= int.Parse(splittedJoined[1]); i++)
                    {
                        Joined.Add(i, new_joined);
                    }
                    return true;
                }
                // The id is not an id of a multiword token.
                return false;
            }

            /// <summary>
            /// Tries to add the word as an empty node word.
            /// </summary>
            /// <param name="info">The set of attributes of the word.</param>
            /// <returns>True, if the word is a multiword token; otherwise, false.</returns>
            private bool TryAddEmptyWord(WordInfo info)
            {
                // The empty node word must be in a form x.y
                string[] splittedEmpty = info.Id.Split('.');
                if (splittedEmpty.Length == 2)
                {
                    // Adds the first part of id as the main word to the dictionary with empty words.
                    int mainWordId = int.Parse(splittedEmpty[0]);
                    if (!EmptyNodes.ContainsKey(mainWordId))
                    {
                        EmptyNodes.Add(mainWordId, new List<EmptyNodeWord>());
                    }
                    // Creates a new empty node word.
                    EmptyNodeWord empty = new EmptyNodeWord(info.Id)
                    {
                        Info = info
                    };
                    // Adds the empty word to the list of empty nodes of the main word.
                    EmptyNodes[mainWordId].Add(empty);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Adds the sentence attribute.
            /// </summary>
            /// <param name="line">The sentence attribute.</param>
            public void AddInfo(string line)
            {
                // Cuts the additional characters and adds the attribute to the set of sentence attributes.
                char[] trimmedChars = { '#', ' ' };
                SentenceInfo.Add(line.Trim(trimmedChars));
            }

            /// <summary>
            /// Recursivelly goes over all words in the sentence which are joined to the root. 
            /// Creates all words and adds them to the <paramref name="allWords"/> according to the order in the sentence.
            /// </summary>
            /// <param name="parentID">The int form of id of the subtree root.</param>
            /// <param name="rootWord">The root of the subtree.</param>
            /// <param name="allWords">The array of all words in the sentence.</param>
            private void AddChildren(BasicWord rootWord, IWord[] allWords)
            {
                int parentID = int.Parse(rootWord.Id);
                // If the parent exists, goes over all its subtrees.
                if (Words.ContainsKey(parentID))
                {
                    // Goes over all its children
                    foreach (var info in Words[parentID])
                    {
                        // Creates a new word
                        BasicWord word = new BasicWord(rootWord, info.Id)
                        {
                            Info = info
                        };
                        // If the word is joined in a multiword token, manages it.
                        AddIfJoined(word, allWords);
                        // Adds the word on the right position to the array of all words.
                        allWords[Positions[info.Id]] = word;
                        // If the word is a main word of empty node words, manages it.
                        AddIfEmpty(word, allWords);
                        // Adds all words from the subtree of the word.
                        AddChildren(word, allWords);
                    }
                }
            }

            /// <summary>
            /// Adds the enhanced children to their parents.
            /// </summary>
            /// <param name="allWords">The array of all words in the sentence.</param>
            private void AddDepsChildren(IWord[] allWords)
            {
                foreach(var word in allWords)
                {
                    foreach(var dep in word.Info.Deps)
                    {
                        // Adds the dep as an child in enhanced representation to its parent, if its parent is not a root.
                        if(dep.Key!="0")
                            allWords[Positions[dep.Key]].ChildrenEnhanced.Add(word);
                    }
                }
            }

            /// <summary>
            /// If the <paramref name="word"/> is joined in a multiword token, manages it. Else does not do anything.
            /// </summary>
            /// <param name="word">The word that is checked to be a part of a multiword.</param>
            /// <param name="allWords">All words in a sentence.</param>
            private void AddIfJoined(BasicWord word, IWord[] allWords)
            {
                int int_id = int.Parse(word.Id);
                // If the word is in the set of joined words
                if (Joined.ContainsKey(int_id))
                {
                    string origId = Joined[int_id].Id;
                    // Adds the word as subword to the multiword.
                    Joined[int_id].AddSubWord(word, null);
                    // If the word is the first in the multiword, adds the multiword to the list of all words.
                    if (Joined[int_id].From == word.Id)
                    {
                        Joined[int_id].Id = origId;
                        allWords[Positions[Joined[int_id].Info.Id]] = Joined[int_id];
                    }
                }
            }

            /// <summary>
            /// If the <paramref name="word"/> is a main word before a group of empty node words, manages it. 
            /// Else does not do anything.
            /// </summary>
            /// <param name="word">The word that is checked to be a main word of a group of empty node words.</param>
            /// <param name="allWords">All words in a sentence.</param>
            private void AddIfEmpty(BasicWord word, IWord[] allWords)
            {
                int int_id = int.Parse(word.Id);
                // If there are some empty node words that belongs to the word
                if (EmptyNodes.ContainsKey(int_id))
                {
                    // Adds the group of empty nodes to the main word.
                    word.EmptyNodes = EmptyNodes[int_id];
                    // Adds all empty words to the list of all words. 
                    foreach (EmptyNodeWord empty in EmptyNodes[int_id])
                    {
                        empty.MainWord = word;
                        allWords[Positions[empty.Info.Id]] = empty;
                    }
                }
            }

            /// <summary>
            /// Checks if all word parts of multiword tokens are present in the sentence.
            /// </summary>
            /// <param name="sentence">The sentence which is being tested.</param>
            /// <returns>True if all parts of multiwords are present; otherwise, false.</returns>
            private bool CheckJoined(ISentence sentence)
            {
                foreach (var word in Joined)
                {
                    // If the sentence does not obtain the word that should be a part of a defined multiword token, returns false.
                    if (sentence.GetWord(word.Key.ToString()) == null)
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Checks if all empty nodes from the file are in the sentence.
            /// </summary>
            /// <param name="sentence">The sentence that is being tested.</param>
            /// <returns>True if the sentence contains all the defined empty node words; otherwise, false.</returns>
            private bool CheckEmpty(ISentence sentence)
            {
                foreach (var word in EmptyNodes)
                {
                    // If the empty node word is not in the sentence, returns false.
                    if ((sentence.GetWord(word.Key.ToString())) == null)
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Returns the complete sentence created from the file.
            /// </summary>
            /// <param name="sentences">The list of all sentences in the file.</param>
            /// <returns>The complete sentence or null, if the sentence is empty.</returns>
            public ISentence Get(ListOfSentences sentences)
            {
                // If the sentence is empty, returns null.
                if (IsEmpty)
                    return null;
                // Creates a root wich has allways the id = "0".
                BasicWord Root = new BasicWord(null, "0");
                IWord[] allWords = new IWord[Positions.Count + 1];
                // Adds the root to the list of words.
                allWords[0] = Root;
                // Tries to add the empty nodes that are directly after the root.
                AddIfEmpty(Root, allWords);
                // Creates and adds all words to the list of all words in the sentence.
                AddChildren(Root, allWords);
                // Manages the dependencies in the enhanced representation of the graph.
                AddDepsChildren(allWords);
                // Creates a new sentence.
                ISentence sentence = new Sentence(Root, allWords.ToList(),  SentenceInfo, sentences);
                // Checks if all the multiword parts and empty node words are present in the sentence. 
                if (!CheckJoined(sentence) || !CheckEmpty(sentence))
                    throw new FormatException();
                return sentence;
            }
        }
        /// <summary>
        ///  The reader where the file is loaded.
        /// </summary>
        private readonly IReader Reader;

        /// <summary>
        /// Creates a new factory which creates sentences while the reader reads the file.
        /// </summary>
        /// <param name="reader">The reader where the conll-u file is loaded.</param>
        public ConlluSentenceFactory(IReader reader)
        {
            Reader = reader;
        }

        /// <summary>
        /// Loads and returns a new created sentence from a file in a conll-u format..
        /// </summary>
        /// <param name="sentences">The list of sentences in the file.</param>
        /// <returns>One created sentence.</returns>
        public ISentence GetSentence(ListOfSentences sentences)
        {
            OneSentenceParts ActualSent = new OneSentenceParts();
            string line;
            // Until the reader reaches the end.
            while ((line = Reader.ReadLine()) != null)
            {
                // If the line is empty, it means the end of the word. Creates a sentence from parts and returns it.
                if (line == "")
                {
                    return ActualSent.Get(sentences);
                }
                // If the line starts by # sign, it means that it is a sentence comment.
                else if (line[0] == '#')
                {
                    ActualSent.AddInfo(line);
                }
                // Else adds a new word to the sentence.
                else
                {
                    string[] props = line.Split('\t');
                    ActualSent.AddWord(props);
                }
            }
            // Returns the last sentence in the file if there is no empty line in the end.
            return ActualSent.Get(sentences);
        }
    }
}
