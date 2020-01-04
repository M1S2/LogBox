using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LogBox
{
    /// <summary>
    /// Represents a view mode that displays data items in columns for a System.Windows.Controls.ListView control with auto sized columns based on the column content     
    /// </summary>
    /// see: https://stackoverflow.com/questions/560581/how-to-autosize-and-right-align-gridviewcolumn-data-in-wpf
    public class AutoSizedGridView : GridView
    {
        protected override void PrepareItem(ListViewItem item)
        {
            foreach (GridViewColumn column in Columns)
            {
                // Setting NaN for the column width automatically determines the required width enough to hold the content completely.
                // If the width is NaN, first set it to ActualWidth temporarily.
                if (double.IsNaN(column.Width)) { column.Width = column.ActualWidth; }

                // Finally, set the column width to NaN. This raises the property change event and re computes the width.
                column.Width = double.NaN;
            }
            base.PrepareItem(item);
        }
    }
}
