using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmbeddedDesignProjectGUI
{
    public partial class Form1 : Form
    {
        private int totalValue;
        // Total value of checked checkboxes
        // Checkbox CheckedChanged event handler
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotalValue();
        }

        private void UpdateTotalValue()
        {
            totalValue = 0;
            if (PC0.Checked) totalValue |= 0b00000001;
            if (PC1.Checked) totalValue |= 0b00000010;
            if (PC2.Checked) totalValue |= 0b00000100;
            if (PC3.Checked) totalValue |= 0b00001000;
            if (PC4.Checked) totalValue |= 0b00010000;
            if (PC5.Checked) totalValue |= 0b00100000;
            if (PC6.Checked) totalValue |= 0b01000000;
            if (PC7.Checked) totalValue |= 0b10000000;

            DisplayTotalValue();
        }

        private void DisplayTotalValue()
        {
            // Get the lower 4 bits (0-3) and upper 4 bits (4-7)
            int lsb = totalValue & 0x0F;
            int msb = (totalValue >> 4) & 0x0F;

            // Display lower 4 bits (0-3) in hexadecimal format on segment 2
            sevenSegment2.Value = lsb.ToString("X");

            // Display upper 4 bits (4-7) in hexadecimal format on segment 1
            sevenSegment1.Value = msb.ToString("X");
        }

        private void refresh_btn_Click(object sender, EventArgs e)
        {
            // Clear the checked state of all checkboxes
            PC0.Checked = false;
            PC1.Checked = false;
            PC2.Checked = false;
            PC3.Checked = false;
            PC4.Checked = false;
            PC5.Checked = false;
            PC6.Checked = false;
            PC7.Checked = false;

            // Reset the total value to zero
            totalValue = 0;

            // Update the display to show zero on both seven segments
            DisplayTotalValue();
        }
    }
}
