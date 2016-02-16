using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace RAM
{
    /// <summary>
    /// Interaction logic for RAMTextBox.xaml
    /// </summary>
    public partial class RAMTextBox : TextBox
    {
        public Brush HeaderBrush = new SolidColorBrush(Colors.Red);
        public Brush HighlightedForegroundBrush = new SolidColorBrush(Colors.White);
        public Brush ForegroundBrush = new SolidColorBrush(Colors.Black);
        public Brush BackgroundBrush = new SolidColorBrush(Colors.White);
        public Brush IPBrush = new SolidColorBrush(Colors.Red);
        public Brush SPBrush = new SolidColorBrush(Colors.Blue);

        private const int COLUMNS = 16;      
        public byte[] ram = new byte[256];
        public int sp = 0;
        public int ip = 0;
        public bool IsHex = true;

        private void CopyCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < COLUMNS; ++i)
            {
                for (int j = 0; j < COLUMNS; ++j)
                {
                    builder.Append(getRamValue(i*COLUMNS+j));
                }
                builder.Append(Environment.NewLine);
            }

            Clipboard.SetText(builder.ToString());
        }

        public RAMTextBox()
        {
            InitializeComponent();
        }      

        // rendering 
        protected override void OnRender(DrawingContext drawingContext)
        {
            // draw background 
            drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight)));
            drawingContext.DrawRectangle(BackgroundBrush, new Pen(), new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            // do the formatting
            FormattedText ft = new FormattedText(
                                    getRAMText(),
                                    System.Globalization.CultureInfo.CurrentCulture,
                                    FlowDirection.LeftToRight,
                                    new Typeface(this.FontFamily.Source),
                                    this.FontSize, ForegroundBrush);

            var left_margin = 4.0 + this.BorderThickness.Left;
            var top_margin = 2.0 + this.BorderThickness.Top;

            const int line_length =(3*(COLUMNS+1))+2;   // Line Number + Columns + newline

            // Set the color of the first line
            ft.SetForegroundBrush(HeaderBrush, 0, line_length);
            // Set the colors of the first 2 chars in every line
            for (int i = 1; i < (COLUMNS + 1); i++)
            {
                ft.SetForegroundBrush(HeaderBrush, i*line_length, 2);
            }

            // Calculate the start positions of SP and IP
            //                line     + header            +   item in line
            int ip_start = (((ip / COLUMNS) + 1) * line_length) + ((ip % COLUMNS) + 1) * 3;
            int sp_start =(((sp / COLUMNS) + 1) * line_length) +((sp % COLUMNS) + 1) * 3;

            // Set foreground and background of the IP
            Geometry geom = ft.BuildHighlightGeometry(new Point(left_margin, top_margin), ip_start, IsHex ? 2 : 1);
            if (geom != null) {
                drawingContext.DrawGeometry(IPBrush, null, geom);
            }
            ft.SetForegroundBrush(HighlightedForegroundBrush, ip_start, IsHex ? 2 : 1);

            // Set foreground and background of the SP
            Geometry geom2 = ft.BuildHighlightGeometry(new Point(left_margin, top_margin), sp_start, IsHex ? 2 : 1);
            if (geom2 != null) {
                drawingContext.DrawGeometry(SPBrush, null, geom2);
            }
            ft.SetForegroundBrush(HighlightedForegroundBrush, sp_start, IsHex ? 2 : 1);

            // draw text
            drawingContext.DrawText(ft, new Point(left_margin, top_margin));

        }

        private string getRAMText() {
            StringBuilder text = new StringBuilder();
            text.Append("   0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F  \r\n");
            // Write Column Header
            for (int i = 0; i < COLUMNS; i++)
            {                
                // Write Row Header
                text.Append(String.Format("{0:X2} ",i*COLUMNS));
                
                // Write the values
                for (int r = 0; r < COLUMNS; r++) {
                    text.Append(getRamValue(i * COLUMNS + r));
                }
                text.Append("\r\n");
            }
            return text.ToString();
        }

        private string getRamValue(int i)
        {
            if (IsHex)
            {
                return String.Format("{0:X2} ", ram[i]);
            }
            else
            {
                // Convert integer value to a ascii character and fill it with blanks on the left
                char c = (char)ram[i];
                if (c <= 31 || c >= 127)
                {
                    c = '.';
                }
                return c.ToString() + "  ";
            }
        }
    }
}
