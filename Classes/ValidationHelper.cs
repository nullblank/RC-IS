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
        // Element Validation function.
        // Returns list of empty or otherwise null elemnents for user to fill out.
        // Helper function
        public List<ControlInfo> GetEmptyControls(List<ControlInfo> controls)
        {
            List<ControlInfo> emptyControls = new List<ControlInfo>();

            foreach (var controlInfo in controls)
            {
                if (controlInfo.Control is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
                {
                    emptyControls.Add(controlInfo);
                }
                else if (controlInfo.Control is DataGrid dataGrid && dataGrid.Items.Count == 0)
                {
                    emptyControls.Add(controlInfo);
                }
                else if (controlInfo.Control is ComboBox comboBox && (comboBox.SelectedIndex == 0 || comboBox.SelectedIndex == -1))
                {
                    emptyControls.Add(controlInfo);
                }
            }

            return emptyControls;
        }
    }
}
