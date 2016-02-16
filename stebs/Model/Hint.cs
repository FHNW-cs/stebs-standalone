
namespace Stebs.Model
{
    /// <summary>
    /// A Hint which is displayed by clicking the Hint button in the menu
    /// </summary>
    class Hint
    {
        /// <summary>
        /// ID of the Hint
        /// </summary>
        public int ID {
            get;
            set;
        }

        /// <summary>
        /// Title of the Hint
        /// </summary>
        public string Title {
            get;
            set;
        }

        /// <summary>
        /// The Text of the Hint
        /// </summary>
        public string Text {
            get;
            set;
        }
    }
}
