using Windows.UI.Xaml.Controls;

namespace WiFiConnect
{
    /// <summary>
    /// Class used to represent an item in the navigation menu SplitView
    /// </summary>
    public class NavMenuItem
    {

        /// <summary>
        /// Property used to access and modify the label of the navigation menu item
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Property used to access and modify the symbol of the navigation menu item
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// Property used to access the character value of a symbol
        /// </summary>
        public char SymbolAsChar
        {
            get { return (char)Symbol; }
        }
    }
}
