using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConlluVisualiser
{
    /// <summary>
    /// Class that collects the word attributes and do operations with them.
    /// </summary>
    public class WordInfo
    {
        /// <summary>
        /// The Id of the word. Numeric for the basic word, 
        /// decimal for the empty node word and the range for the multiword token.
        /// </summary>
        public string Id { get; set; } = "";
        /// <summary>
        /// Word form or punctuation symbol.
        /// </summary>
        public string Form { get; set; } = "";
        /// <summary>
        /// Lemma or stem of word form.
        /// </summary>
        public string Lemma { get; private set; } = "";
        /// <summary>
        /// Universal part-of-speech tag.
        /// </summary>
        public string Upos { get; private set; } = "";
        /// <summary>
        /// Language-specific part-of-speech tag.
        /// </summary>
        public string Xpos { get; private set; } = "";
        /// <summary>
        /// List of morphological features
        /// </summary>
        public Dictionary<string, string> Feats { get; private set; } = new Dictionary<string, string>();
        /// <summary>
        /// Head of the current word, which is either a value of ID or zero (0).
        /// </summary>
        public string Head { get; set; } = "";
        /// <summary>
        /// Universal dependency relation to the HEAD (root iff HEAD = 0) or a defined language-specific subtype of one.
        /// </summary>
        public string Deprel { get; private set; } = "";
        /// <summary>
        /// Enhanced dependency graph in the form of a list of head-deprel pairs.
        /// </summary>
        public Dictionary<string, string> Deps { get; private set; } = new Dictionary<string, string>();
        /// <summary>
        /// Any other annotation.
        /// </summary>
        public Dictionary<string, string> Misc { get; private set; } = new Dictionary<string, string>();
        /// <summary>
        /// The sentence where the word lies.
        /// </summary>
        private ISentence Sentence { get; set; }
        /// <summary>
        /// The delimiter that separates property and value in <seealso cref="Feats"/> and <seealso cref="Misc"/> attribute.
        /// </summary>
        public const char delimFeatsMisc = '=';
        /// <summary>
        /// The delimiter that separates head and deprel in <seealso cref="Deps"/> attribute.
        /// </summary>
        public const char delimDeps = ':';
        /// <summary>
        /// The character in the file that means that the field is empty.
        /// </summary>
        public const string emptyField = "_";
        /// <summary>
        /// The delimiter thar separates the whole pairs in the attributes with multiple values.
        /// </summary>
        public const char delimPairs = '|';
        private const char delimAttributes = '\t';
        /// <summary>
        /// Creates class with no data.
        /// </summary>
        public WordInfo()
        {
        }
        /// <summary>
        /// Reads and saves the word information from line in the file in CoNLL-U format.
        /// The sentence dependencies problems are not resolved here. (Unique ID, existing Head, Deprel...)
        /// </summary>
        /// <param name="properties">
        /// The attributes from the line in the file. 
        /// The array must have a specific format according to the specification.
        /// </param>
        public WordInfo(string[] properties)
        {
            if(properties.Length < 10)
            {
                return;
            }
            // Form and Lemma can have the value "_", that signalizes the empty field in other cases.
            // Id can not be empty.
            Id = properties[0];
            Form = properties[1];
            Lemma = properties[2];
            // If the value is "_", it means empty value. The empty string is saved.
            Upos = ImportValue(properties[3]);
            Xpos = ImportValue(properties[4]);
            // Separates the pairs and saves them to the proper dictionary.
            if (!SaveDictValues(properties[5], dict: Feats, delim: delimFeatsMisc))
                throw new FormatException();
            Head = ImportValue(properties[6]);
            Deprel = ImportValue(properties[7]);
            if (!SaveDictValues(properties[8], dict: Deps, delim: delimDeps))
                throw new FormatException();
            // The word can not be a parent of itself.
            if (Deps.ContainsKey(Id) || Head == Id)
                throw new FormatException();
            SaveDictValues(properties[9], dict: Misc, delim: delimFeatsMisc);
        }

        /// <summary>
        /// Saves the text of the dictionary attribute from the file to the right dictionary.
        /// Separates it to the properties and values of the pairs.
        /// </summary>
        /// <param name="valueInFile">The whole value of the attribute in the file.</param>
        /// <param name="dict">The dictionary where the separated values should be stored to.</param>
        /// <param name="delim">The delimiter by which the property and value are separated.</param>
        /// <return>True if the value has a good format; otherwise, false.</return>
        private bool SaveDictValues(string valueInFile, Dictionary<string, string> dict, char delim)
        {
            // Prepares the dictionary to be empty.
            dict.Clear();
            // If the value is empty, does not save anything.
            if (IsNoEmpty(valueInFile))
            {
                // Separates the pairs.
                string[] arr = valueInFile.Split(delimPairs);
                // Separates the property and value in each pair and saves it to the dictionary.
                foreach (var value in arr)
                {
                    if (IsNoEmpty(value))
                    {
                        string[] oneProp = value.Split(new[] { delim }, 2);
                        // If the value is in the bad format, returns false.
                        if (oneProp.Length < 2)
                        {
                            dict.Add(oneProp[0], "");
                            return false;
                        }
                        // If there is an item with the same property, it will be ovewritten. 
                        // There can not be two property two times with different values.
                        dict[oneProp[0]] = oneProp[1];
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Returns true if the value is not empty (or does not obtains the sign that means it is empty in the file).
        /// </summary>
        /// <param name="value">The value that is tested.</param>
        /// <returns>True if the value is not empty according to the Conllu format; otherwise, false.</returns>
        private bool IsNoEmpty(string value)
        {
            return value != emptyField && value != "" ? true : false;
        }
        /// <summary>
        /// Changes the attributtes according to the state. The state must be in the valid format.
        /// </summary>
        /// <param name="state">The list of (attribute, value) pairs.</param>
        public void SaveChanges(Dictionary<string, string> state)
        {
            // Joins the attribute values with the fields in this class of the same name.
            var changed = from attr in state
                          join field in GetType().GetProperties() on attr.Key equals field.Name
                          select new { field, attr };
            // Goes over all the fields and saves the state.
            foreach (var change in changed)
            {         
                // Prepares and saves the Feats attribute.
                if(change.attr.Key == nameof(Feats))
                {
                    SaveDictValues(change.attr.Value, Feats, delimFeatsMisc);
                }
                // Prepares and saves the Misc attribute.
                else if (change.attr.Key == nameof(Misc))
                {
                    SaveDictValues(change.attr.Value, Misc, delimFeatsMisc);
                }
                // Prepares and saves the Deps attribute.
                else if (change.attr.Key == nameof(Deps))
                {
                    Dictionary<string, string> newDeps = new Dictionary<string, string>();
                    SaveDictValues(change.attr.Value, newDeps, delimDeps);
                    if (Sentence != null && change.attr.Value != "")
                    {
                        IWord word = Sentence.GetWord(Id);
                        // Removes the old deps
                        foreach (var dep in Deps)
                        {
                            Sentence.GetWord(dep.Key).ChildrenEnhanced.Remove(word);
                        }
                        // Saves the new deps
                        foreach (var dep in newDeps)
                        {
                            Sentence.GetWord(dep.Key).ChildrenEnhanced.Add(word);
                        }
                    }
                    Deps = newDeps;
                }
                else
                {
                    // Ensures that the parent of the word is the word with Id from head attribute.
                    if (change.field.Name == nameof(Head) && Sentence != null && change.attr.Value!= "")
                    {
                        ITreeWord word = (ITreeWord)Sentence.GetWord(Id);
                        ITreeWord parent = (ITreeWord)Sentence.GetWord(change.attr.Value);
                        parent.AddChild(word, true);
                    }
                    // Changes all the attributtes to the new (or the same old) value.
                    change.field.SetValue(this, change.attr.Value);
                }
            }
        }

        /// <summary>
        /// Shows the word info form and allows user to change attribute values.
        /// </summary>
        /// <param name="s">The sentence where the word lies.</param>
        public void ShowWordInfo(ISentence s)
        {
            Sentence = s;
            WordFieldsForm f = new WordFieldsForm(this, new ConlluValidator(s));
            // Initializes the OK button to save the changes after click.
            f.InitButton(text: "Save changes", handlers: (e, sender) => { SaveChanges(f.GetState()); f.Close(); });
            f.ShowDialog();
        }

        /// <summary>
        /// Applies the shortcut key action to the word info.
        /// </summary>
        /// <param name="actions">The dictionary of the not dictionary attributes and their 
        /// <seealso cref="ItemInWordInfo"/> values.</param>
        /// <param name="feats">The dictionary of the names of the features 
        /// and their <seealso cref="ItemInWordInfo"/> values.</param>
        /// <param name="misc">The dictionary of the names of the misc properties 
        /// and their <seealso cref="ItemInWordInfo"/> values.</param>
        public void ApplyChangesFromShortcut(Dictionary<string, ItemInWordInfo> actions,
                                    Dictionary<string, ItemInWordInfo> feats,
                                    Dictionary<string, ItemInWordInfo> misc)
        {
            // Gets all the fields in this class
            var fields = GetType().GetProperties();
            foreach (var field in fields)
            {
                string name = field.Name;
                // Saves the changes into the Feats dictionary.
                if (name == nameof(Feats))
                {
                    Feats = SaveDictShortcut(feats, Feats);
                }
                // Saves the changes into the Misc dictionary.
                else if (name == nameof(Misc))
                {
                    Misc = SaveDictShortcut( misc, Misc);
                }
                // Saves all other changed fields.
                // There are not the fields Id, Head and Deps, so the graph structure can not be malformerd.
                else if (actions.ContainsKey(name))
                {
                    // Action that is connected with this field
                    ItemInWordInfo ac = actions[name];
                    // If it should be overwritten or if the value was empty, the new value is setted.
                    if (ac.Overwrite || !IsNoEmpty((string)field.GetValue(this)))
                    {
                        field.SetValue(this, ac.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Creates and returns the dictionary according to the <paramref name="changes"/>.
        /// </summary>
        /// <param name="changes">The values from shortcut key action.</param>
        /// <param name="orig">The original values in the proper dictionary.</param>
        /// <returns>The new medified dictionary.</returns>
        private Dictionary<string, string> SaveDictShortcut(Dictionary<string, ItemInWordInfo> changes, 
            Dictionary<string, string> orig)
        {
            // Creates and initializes the new dictionary.
            Dictionary<string, string> Changed = new Dictionary<string, string>();
            // If the actions dictionary does not contain the empty record
            // or the empty record should not overwrite all values, the original values are saved.
            if (!changes.ContainsKey("") || changes[""].Overwrite == false)
            {
                // Saves the original values to the new dictionary.
                Changed = orig;
            }
            // Overwrites or adds the changed values.
            foreach (var pair in changes)
            {
                // If the key is empty, does not do anything.
                // If the value should overwrite the original value 
                // or if the original dictionary does not obtain the key, the changed value is saved.
                if (IsNoEmpty(pair.Key) && (pair.Value.Overwrite || !orig.ContainsKey(pair.Key)))
                {
                    Changed[pair.Value.Name] = pair.Value.Value;
                }
                // If the property was saved in original dictionary and all original pairs are removed,
                // the pair value is overwritten and rested even if its overwrite tag was false.
                else if (orig.ContainsKey(pair.Key) && !Changed.ContainsKey(pair.Key))
                {
                    Changed[pair.Value.Name] = pair.Value.Value;
                }
            }
            return Changed;
        }

        /// <summary>
        /// Exports the value to save the attribute to the file. The empty value is saved as '_'.
        /// </summary>
        /// <param name="val">The value that will be saved.</param>
        /// <returns>The new value that is written to the file.</returns>
        private string ExportValue(string val)
        {
            return val == "" ? emptyField : val;
        }

        /// <summary>
        /// Modifies the value from the file to save it. If the value is '_', makes it empty.
        /// Otherwise, the value is not changed.
        /// </summary>
        /// <param name="val">The value from file.</param>
        /// <returns>The original value, if it is not the sign for empty field('_'); otherwise, "".</returns>
        private string ImportValue(string val)
        {
            return val == emptyField ? "" : val;
        }

        /// <summary>
        /// Creates the export value of the díctionary attribute.
        /// </summary>
        /// <param name="dict">The dictionary whose value is created.</param>
        /// <param name="delim">The delimiter thar separates property and value in the pair.</param>
        /// <returns>The string value of the dictionary.</returns>
        private string CreateStringFromDict(Dictionary<string, string> dict, char delim)
        {
            StringBuilder sb = new StringBuilder();
            // Adds all pairs into the stringbuilder.
            // On the first position there is extra delimiter that separates the pairs.
            foreach (var pair in dict)
            {
                sb.Append(delimPairs + pair.Key + delim + pair.Value);
            }
            // Removes the first extra delimiter.
            if(dict.Count > 0)
            {
                sb.Remove(0, 1);
            }
            return sb.ToString();
        }

        /// <summary>
        /// The enumeration of the attributes of the word.
        /// </summary>
        /// <returns>Enumeration of the attribute fields.</returns>
        public IEnumerable<KeyValuePair<string, string>> Properties()
        {
            yield return new KeyValuePair<string, string>(nameof(Id), Id);        
            yield return new KeyValuePair<string, string>(nameof(Form), Form);
            yield return new KeyValuePair<string, string>(nameof(Lemma), Lemma);
            yield return new KeyValuePair<string, string>(nameof(Upos), Upos);
            yield return new KeyValuePair<string, string>(nameof(Xpos), Xpos);
            yield return new KeyValuePair<string, string>(nameof(Feats), CreateStringFromDict(Feats, delimFeatsMisc));
            yield return new KeyValuePair<string, string>(nameof(Head), Head);
            yield return new KeyValuePair<string, string>(nameof(Deprel), Deprel);
            yield return new KeyValuePair<string, string>(nameof(Deps), CreateStringFromDict(Deps, delimDeps));
            yield return new KeyValuePair<string, string>(nameof(Misc), CreateStringFromDict(Misc, delimFeatsMisc));
        }

        /// <summary>
        /// Saves the word into the file in the CoNLL-U format.
        /// </summary>
        /// <param name="file">The file where the word is saved to.</param>
        public void SaveWord(StreamWriter file)
        {
            StringBuilder sb = new StringBuilder();
            // Writes all the attributes.
            bool first = true;
            foreach (var prop in Properties())
            {
                if (first)
                    first = false;
                else sb.Append(delimAttributes);
                sb.Append(ExportValue(prop.Value));
            }
            // Adds the new line character to the end.            
            //byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            // Writes the bytes to the file.
            //file.Write(bytes, 0, bytes.Length);
            file.WriteLine(sb.ToString());
        }
    }
}
