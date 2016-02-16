using System;
using System.Collections.Generic;
using System.Windows;

namespace Stebs.Model
{
    /// <summary>
    /// Reads the XML-File which contains the hints
    /// </summary>
    class Hints
    {
        /// <summary>
        /// List of Hint-objects
        /// </summary>
        private List<Hint> hintsList = new List<Hint>();

        /// <summary>
        /// Constructor
        /// Parses the XML and fills the list with the hints
        /// </summary>
        public Hints() {

            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(Properties.Settings.Default.HintsFile);

            Hint hint = new Hint();

            // Read an node of the XML
            while(reader.Read()) {
                reader.MoveToContent();
                
                // Check if the node is an element
                if (reader.NodeType == System.Xml.XmlNodeType.Element) {

                    switch (reader.Name) {
                        case "id":
                            // create an new Hint-instance
                            hint = new Hint();
                            reader.Read();
                            if (reader.NodeType == System.Xml.XmlNodeType.Text)
                                hint.ID = Convert.ToInt32(reader.Value);
                            break;

                        case "title":
                            reader.Read();
                            if (reader.NodeType == System.Xml.XmlNodeType.Text)
                                hint.Title = reader.Value;
                            break;

                        case "text":
                            reader.Read();
                            if (reader.NodeType == System.Xml.XmlNodeType.Text)
                                hint.Text = reader.Value;

                            // Add the hint to the list
                            hintsList.Add(hint);
                            break;

                        default:
                            continue;
                    }
                }
            }
            
        }

        /// <summary>
        /// Displays a random hint
        /// </summary>
        public void Show() {

            Random rand = new Random();
            int randomNr = 0;
            int lastRandNr = 0;

            MessageBoxResult res = MessageBoxResult.Yes;
            do {
                // Get the next random id
                do {
                    randomNr = rand.Next(hintsList.Count);
                } while(lastRandNr == randomNr);

                lastRandNr = randomNr;

                // Display the hint
                res = MessageBox.Show(hintsList[randomNr].Text + "\n\nWould you like to see another hint?", hintsList[randomNr].Title, MessageBoxButton.YesNo, MessageBoxImage.Information);
            } while(res == MessageBoxResult.Yes);
        }
    }
}
