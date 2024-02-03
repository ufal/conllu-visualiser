namespace ConlluVisualiser
{
    /// <summary>
    /// Implements Commander design pattern.
    /// It is the mean how to change the active sentence or the active word inside the main form.
    /// Used when there is not necessary to have access to whole form.
    /// </summary>
    public class CurrentState
    {
        /// <summary>
        /// The form that can be changed.
        /// </summary>
        private readonly AppForm MainForm;

        /// <summary>
        ///  The word that is currently active.
        /// </summary>
        public IWord ActiveWord { get; set; }
        
        /// <summary>
        /// Creates the commander of the <paramref name="form"/>.
        /// </summary>
        /// <param name="form">The form that can be changed by this way.</param>
        public CurrentState(AppForm form)
        {
            MainForm = form;
        }

        /// <summary>
        /// Makes the new word active. If the <paramref name="drawInVisualiser"/> is true,
        /// changes the graph of the sentence.
        /// </summary>
        /// <param name="new_active">The new active word</param>
        /// <param name="drawInVisualiser">Whether the change of the active word should be visualised in graph.</param>
        public void ChangeActiveWord(IWord new_active, bool drawInVisualiser = true)
        {
            //If there is some active word
            if (ActiveWord != null)
            {
                // Make the old word not active
                ActiveWord.IsActive = false;
                // If the change should be drawn, draw the word as not active
                if (drawInVisualiser)
                {
                    MainForm.SentenceVisualiser.DrawOneWord(ActiveWord);
                }
            }
            ActiveWord = new_active;
            if (new_active != null)
            {
                // Make the new word active
                new_active.IsActive = true;
                // If the change should be drawn, draw the word as active
                if (drawInVisualiser)
                {
                    MainForm.SentenceVisualiser.DrawOneWord(ActiveWord);
                }
            }
        }
        /// <summary>
        /// Changes the active sentence in the main form.
        /// </summary>
        /// <param name="sentence">The new active sentence.</param>
        public void ChangeActiveSentence(ISentence sentence)
        {
            MainForm.ChangeActiveSentence(sentence);
            
        }

        /// <summary>
        /// Actualizes the visualised graph of the sentence.
        /// </summary>
        public void VisualiseNewGraph()
        {
            MainForm.VisualiseNewGraph();
        }
    }
}
