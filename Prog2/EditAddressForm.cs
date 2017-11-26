/*Grading ID: C4811
 * CIS 200-01
 * Program 3
 * Due Date: Nov 15, 2016
 * creates a form that allows the user to choose an existing Address to be Edited 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPVApp
{
    public partial class EditAddressForm : Form
    {
        private List<Address> addressList; // List of addresses used to fill combo boxes

        // Precondition:  List addresses is populated with the available
        //                addresses to choose from
        // Postcondition: The form's GUI is prepared for display.
        public EditAddressForm(List<Address> addresses)
        {
            InitializeComponent();
            addressList = addresses;
            // displays the list of names in the combo box 
            foreach (Address a in addressList)
                nameComboBox.Items.Add(a.Name);            
        }

        internal int NameAddress
        {
            // Precondition:  User has selected from name combo box
            // Postcondition: The index of the selected name is returned
            get { return nameComboBox.SelectedIndex; }

            // Precondition:  -1 <= value < addressList.Count
            // Postcondition: The specified index is selected in namecombobox
            set
            {
                if ((value >= -1) && (value < addressList.Count))
                    nameComboBox.SelectedIndex = value;
                else
                    throw new ArgumentOutOfRangeException("AddressIndex", value,
                        "Index must be valid");
            }
        }

        // Precondition: requires the user to select an item from the combo box
        // Postcondition: if no valid input is selected it cancels the event and sets the error message
        private void nameComboBox_Validating(object sender, CancelEventArgs e)
        {
            if(nameComboBox.SelectedIndex == -1) 
            {                
                e.Cancel = true;
                errorProvider1.SetError(nameComboBox, "Please select the address you wish to edit");
                
            }

        }
        // Precondition: Validating was not canceled
        // Postcondition: clears the Error Provider and allows the focus to be changed
        private void nameComboBox_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(nameComboBox, "");
        }

        // Precondition: user clicks the cancel button
        // Postcondition: the form closes
        private void cancelButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // makes sure the user left clicks the button otherwise the click wont be allowed if the combo box has an invalid result
                this.DialogResult = DialogResult.Cancel;
        }

        // Precondition: user clicks the select button
        // Postcondition: if the combo box is left empty keeps the form open 
        //                and the error provider pops up next to the combo box. 
        //                if the combo box is valid then it returns an OK and closes the form
        private void selectButton_Click(object sender, EventArgs e)
        {
            if (ValidateChildren()) // validates the combo box and if it is valid ValidateChildren() will be true 
                this.DialogResult = DialogResult.OK;          
          
        }
    }
}
