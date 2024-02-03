using ConlluVisualiser;
namespace Finder
{
    /// <summary>
    /// Provides the searching in the list of the sentnce in the file.
    /// </summary>
    public interface IFinder
    {
        /// <summary>
        /// Starts searching from the <paramref name="first"/> sentence in parameter.
        /// </summary>
        /// <param name="first">The sentence where the searching starts.</param>
        void Find(ISentence first);
    }
}
