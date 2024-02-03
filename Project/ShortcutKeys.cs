using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ConlluVisualiser
{   
    /// <summary>
    /// Class that saves and processes key shortcuts defined by the user
    /// </summary>
    class ShortcutKeys
    {
        /// <summary>
        /// One shortcut key with the actions
        /// </summary>
        private class ShortcutKey
        {
            /// <summary>
            /// The delimiter that separates the parts describing one action of the shorcut key in the file.
            /// </summary>
            private const char delimAct = '\t';
            /// <summary>
            /// The delimiter that separate a key nad value in the <seealso cref="feats"/> or <seealso cref="misc"/> actions. 
            /// </summary>
            private const char delimFeatsMisc = '=';
            /// <summary>
            /// The delimiter that separate a key nad value in the <seealso cref="feats"/> or <seealso cref="misc"/> actions. 
            /// </summary>
            private readonly Dictionary<string, ItemInWordInfo> actions = new Dictionary<string, ItemInWordInfo>();
            /// <summary>
            /// The set of the actions which are applied on the active word's <seealso cref="WordInfo.Feats"/>.
            /// </summary>
            private readonly Dictionary<string, ItemInWordInfo> feats = new Dictionary<string, ItemInWordInfo>();
            /// <summary>
            /// The set of the actions which are applied on the active word's <seealso cref="WordInfo.Misc"/>.
            /// </summary>
            private readonly Dictionary<string, ItemInWordInfo> misc = new Dictionary<string, ItemInWordInfo>();
            /// <summary>
            /// The set of the actions which are applied on the active word's <seealso cref="WordInfo.Deps"/>.
            /// </summary>
            private readonly Dictionary<string, ItemInWordInfo> deps = new Dictionary<string, ItemInWordInfo>();
            /// <summary>
            /// The key shortcut which runs the word change according to defined actions.
            /// </summary>
            public Keys Shortcut { get; private set; }
            /// <summary>
            /// Creates the shorcut key on <paramref name="shortcut"/> with actions from <paramref name="acts"/>
            /// </summary>
            /// <param name="acts">The list of actions which are applied on the active word when the shorcut key is pressed.</param>
            /// <param name="shortcut">The shortcut key which runs the actions</param>
            public ShortcutKey(List<ItemInWordInfo> acts, Keys shortcut)
            {
                // Saves all actions according the attribute that they change
                foreach(var ac in acts)
                {
                    // Feats actions are saved into the feats dictionary
                    if (ac.Name == nameof(WordInfo.Feats))
                    {
                        AddDictAction(ac, feats);
                    }
                    // Misc actions are saved into the misc dictionary
                    else if ( ac.Name == nameof(WordInfo.Misc))
                    {
                        AddDictAction(ac, misc);
                    }
                    // All remaining actions are saved into the actions dictionary
                    else
                    {
                        actions.Add(key: ac.Name, value: ac);
                    }
                }
                Shortcut = shortcut;
            }
            /// <summary>
            /// Adds the dictionary action into the given dictionary. Splits it by the delimiter. If the value is in the bad format, does not do anything.
            /// </summary>
            /// <param name="action">The action which should be saved.</param>
            /// <param name="dict">The dictionary where the action is saved to.</param>
            private void AddDictAction(ItemInWordInfo action, Dictionary<string, ItemInWordInfo> dict)
            {
                // Splits by the delimiter that separates the dictionary item values
                string[] splitted = action.Value.Split(delimFeatsMisc);
                // The value is not in the good format - will not be saved
                if (splitted.Length != 2)
                    dict.Add(key: "", value: new ItemInWordInfo("", "", action.Overwrite));
                // Creates the new item so that its key and value are the key and value in the Feats attribute
                else dict.Add(key: splitted[0], value: new ItemInWordInfo(splitted[0], splitted[1], action.Overwrite));
            }

            /// <summary>
            /// Creates the new shortcut key according to the <paramref name="lines"/> which are loaded from the file.
            /// If the file is in the bad format, notifies the user.
            /// </summary>
            /// <param name="lines">The set of lines which define one shorcut key.</param>
            /// <returns>New shortcut key defined in the file if the file has a good format. Otherwise, null.</returns>
            public static ShortcutKey MakeKeyFromFile(List<string> lines)
            {
                try
                {
                    // On the first line, there must be a valid shortcut key. If not, an exception is thrown.
                    if (!Enum.TryParse(lines[0], out Keys key))
                    {
                        throw new FormatException();
                    }
                    List<ItemInWordInfo> acts = new List<ItemInWordInfo>();
                    // Saves each following line as an action.
                    for(int i = 1; i < lines.Count; i++)
                    {
                        // Splits the parts of one action:
                        // The name of the attribute, the value that it should have and the sign whether the value should be overwritten if there is some.
                        string[] splitted = lines[i].Split(delimAct);
                        acts.Add(new ItemInWordInfo(splitted[0], splitted[1], splitted[2] == bool.TrueString));
                    }
                    // Creates the new shortcut key
                    return new ShortcutKey(acts, key);
                }
                catch
                {
                    // Notifies the user that the file is in the bad format.
                    MessageBox.Show(text: "File is not in a good format.");
                    return null;
                }
            }

            /// <summary>
            /// Processes all the actions on the <seealso cref="BasicWord.Info"/> of the <paramref name="activeWord"/>.
            /// </summary>
            /// <param name="activeWord">The word which should changed according to defined actions</param>
            public void ProcessKey(IWord activeWord)
            {
                activeWord.Info.ApplyChangesFromShortcut(actions, feats, misc);
            }

            /// <summary>
            /// Writes the shortcut key with all actions into the file.
            /// </summary>
            /// <param name="sw">The stream whiter where the output will be stored.</param>
            public void WriteYourself(StreamWriter sw)
            {
                // On the first line, there is a shortcut key
                sw.WriteLine(Shortcut.ToString());
                // Writes all the actions in the format (delimitted by the delimAct):
                // Attribute name, the value that it should have, the sign whether the value should be overwritten if there is some.
                foreach (KeyValuePair<string, ItemInWordInfo> a in actions)
                {
                    string line = a.Value.Name + delimAct + a.Value.Value + delimAct + a.Value.Overwrite;
                    sw.WriteLine(line);
                }
                // Joins the action of dictionary values by the delimiter delimInItem.
                foreach (KeyValuePair<string, ItemInWordInfo> a in feats)
                {
                    string line = nameof(WordInfo.Feats) + delimAct + a.Value.Name + delimFeatsMisc + a.Value.Value + delimAct + a.Value.Overwrite;
                    sw.WriteLine(line);
                }
                foreach (KeyValuePair<string, ItemInWordInfo> a in misc)
                {
                    string line = nameof(WordInfo.Misc) + delimAct + a.Value.Name + delimFeatsMisc + a.Value.Value + delimAct + a.Value.Overwrite;
                    sw.WriteLine(line);
                }
                // Adds the empty line to the streamwriter.
                sw.WriteLine();
            }
        }
        
        /// <summary>
        /// The set of all shortcut keys which hold some actions
        /// </summary>
        private Dictionary<Keys, ShortcutKey> AllKeys { get; set; }

        /// <summary>
        /// Creates an empty class that stores the shortcut keys.
        /// </summary>
        public ShortcutKeys()
        {
            AllKeys = new Dictionary<Keys, ShortcutKey>();
        }

        /// <summary>
        /// Creates a new shortcut key from the <paramref name="actions"/> and <paramref name="shortcut"/>.
        /// Adds it into the <seealso cref="AllKeys"/>. If there exists the same key shortcut with another actions, overwrites it.
        /// </summary>
        /// <param name="actions">The actions the shorcut key will process</param>
        /// <param name="shortcut">The shortcut key which runs the defined changes</param>
        private void AddShortcutKey(List<ItemInWordInfo> actions, Keys shortcut)
        {
            ShortcutKey sck = new ShortcutKey(actions, shortcut);
            // Overwrites the existing key shortcut, if there is the same.
            AllKeys[shortcut] = sck;
        }

        /// <summary>
        /// Allows user to define a new shortcut key with the actions. Opens a new form where the user can define everyting.
        /// </summary>
        public void AddShortcutKey()
        {
            // Opens a new form where the user defines the actions.
            ShortcutsFieldsForm newForm = new ShortcutsFieldsForm();
            if (newForm.ShowDialog() == DialogResult.OK)
            {
                // Returns the state of the form with the user defined actions.
                List<ItemInWordInfo> actions = newForm.GetResult();
                // Returns the concrete shortcut key which will run the actions.
                Keys shortcut = newForm.GetShortcutKeys();
                // Adds the new shortcut key into the set of all shortcut keys.
                AddShortcutKey(actions, shortcut);
            }
        }

        /// <summary>
        /// Specifies whether the set of shorcut keys contains the <paramref name="keyData"/>.
        /// </summary>
        /// <param name="keyData">The key shortcut which is searched for.</param>
        /// <returns>true if the key shortcut is defined; otherwise, false</returns>
        public bool Contains(Keys keyData)
        {
            return AllKeys.ContainsKey(keyData);
        }

        /// <summary>
        /// Processes the actions that are related with the <paramref name="keyData"/> shortcut.
        /// </summary>
        /// <param name="keyData">The shortcut that runs the actions.</param>
        /// <param name="activeWord">The word which should be modified by the key shortcut actions.</param>
        public void ProcessKey(Keys keyData, IWord activeWord)
        {
            if (AllKeys.ContainsKey(keyData))
            {
                AllKeys[keyData].ProcessKey(activeWord);
            }
        }

        /// <summary>
        /// Writes all the shortcut keys into the file to export them. The shortcut keys are separated by an empty line.
        /// </summary>
        /// <param name="file">The file where the shortcut keys will be stored to.</param>
        private void WriteToFile(string file)
        {
            using (StreamWriter sw = new StreamWriter(file))
            {
                foreach(var shortcut in AllKeys)
                {
                    // Writes everything that concerns the concrete shortcut
                    shortcut.Value.WriteYourself(sw);
                }                
            }
        }

        /// <summary>
        /// Saves all the shortcut keys into the file that user chooses.
        /// </summary>
        public void Save()
        {
            // Opens the SaveFileDialog
            SaveFileDialog dialog = new SaveFileDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                // Writes all the shortcut keys
                WriteToFile(dialog.FileName);
            }
        }

        /// <summary>
        /// Loads the shortcut keys from the file that user chooses.
        /// </summary>
        public void Load()
        {
            // Opens the OpenFileDialog and makes user to choose the file with the shortcut keys actions.
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ReadFromFile(dialog.FileName);
            }
        }

        /// <summary>
        /// Reads the file and saves all the shortcut keys with their actions. If some shortcut key definition is in the bad format, notifies the user.
        /// </summary>
        /// <param name="file">The file where the shortcut keys are stored.</param>
        private void ReadFromFile(string file)
        {
            // Creates a streamreader thar reads the whole file
            using (StreamReader sr = new StreamReader(file))
            {
                //Reads one shortcut key definition in each iteration. The definitions are delimitted by a new line.
                while (!sr.EndOfStream)
                {
                    List<string> lines = new List<string>();
                    string line = sr.ReadLine();
                    // After each definition of one shortcut key, there is an empty line.
                    while(line != null && line != "")
                    {
                        // Adds the line that concerns the shortcut key
                        lines.Add(line); 
                        // Reads the next line
                        line = sr.ReadLine();
                    }                   
                    if (lines.Count > 0)
                    {
                        // If something was defined, creates the new shortcut key for it. If the definition is in the bad format, returns null.
                        ShortcutKey skey = ShortcutKey.MakeKeyFromFile(lines);
                        if (skey != null)
                        {
                            AllKeys[skey.Shortcut] = skey;
                        }
                    }
                }
            }
        }
    }
}
