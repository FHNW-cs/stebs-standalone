
namespace Stebs.Model
{
    /// <summary>
    /// A Recent opened file with the path and its title
    /// </summary>
    public class RecentFile
    {
        /// <summary>
        /// The Title of the File
        /// </summary>
        public string Title {
            get;
            set;
        }
        /// <summary>
        /// The Path of the File
        /// </summary>
        public string Path {
            get;
            set;
        }
    }
}
