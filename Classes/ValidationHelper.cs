using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RC_IS.Classes
{
    internal class ValidationHelper
    {
        public List<string> GetEmptyControls(List<TextBox> textboxes, List<DataGrid> datagrids)
        {
            List<string> emptyControls = new List<string>();

            foreach (var textBox in textboxes)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    emptyControls.Add(textBox.Name); // or any other identifier
                }
            }

            foreach (var dataGrid in datagrids)
            {
                if (dataGrid.Items.Count == 0)
                {
                    emptyControls.Add(dataGrid.Name); // or any other identifier
                }
            }

            return emptyControls;
        }
    }
}
