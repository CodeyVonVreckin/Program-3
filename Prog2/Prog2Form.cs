// Grading ID: C4811 
// Program 3
// CIS 200 -1
// Due: 11/15/2016



// By: Andrew L. Wright (Students use Grading ID) A
// File: Prog2Form.cs
// This class creates the main GUI for Program 2. It provides a
// File menu with About and Exit items, an Insert menu with Address and
// Letter items, and a Report menu with List Addresses and List Parcels
// items.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;                       
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace UPVApp
{
    public partial class Prog3Form : Form
    {
        private UserParcelView upv; // The UserParcelView

        private BinaryFormatter formatter = new BinaryFormatter();// object for serializing serializables in binary format
        private BinaryFormatter reader = new BinaryFormatter();// object for deserializing serializables in binary format
        private FileStream output; // stream for writing to a file
        private FileStream input; //stream for reading from a file

        // Precondition:  None
        // Postcondition: The form's GUI is prepared for display. A few test addresses are
        //                added to the list of addresses
        public Prog3Form()
        {
            InitializeComponent();

            upv = new UserParcelView();
        }

        // Precondition:  File, About menu item activated
        // Postcondition: Information about author displayed in dialog box
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string NL = Environment.NewLine; // Newline shorthand

            MessageBox.Show($"Program 2{NL}By: Andrew L. Wright{NL}CIS 200{NL}Fall 2016",
                "About Program 2");
        }

        // Precondition:  File, Exit menu item activated
        // Postcondition: The application is exited
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Precondition:  Insert, Address menu item activated
        // Postcondition: The Address dialog box is displayed. If data entered
        //                are OK, an Address is created and added to the list
        //                of addresses
        private void addressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddressForm addressForm = new AddressForm();    // The address dialog box form
            DialogResult result = addressForm.ShowDialog(); // Show form as dialog and store result

            if (result == DialogResult.OK) // Only add if OK
            {
                try
                {
                    upv.AddAddress(addressForm.AddressName, addressForm.Address1,
                        addressForm.Address2, addressForm.City, addressForm.State,
                        int.Parse(addressForm.ZipText)); // Use form's properties to create address
                }
                catch (FormatException) // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Address Validation!", "Validation Error");
                }
            }

            addressForm.Dispose(); // Best practice for dialog boxes
        }

        // Precondition:  Report, List Addresses menu item activated
        // Postcondition: The list of addresses is displayed in the addressResultsTxt
        //                text box
        private void listAddressesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String
            string NL = Environment.NewLine;            // Newline shorthand

            result.Append("Addresses:");
            result.Append(NL); // Remember, \n doesn't always work in GUIs
            result.Append(NL);

            foreach (Address a in upv.AddressList)
            {
                result.Append(a.ToString());
                result.Append(NL);
                result.Append("------------------------------");
                result.Append(NL);
            }

            reportTxt.Text = result.ToString();

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        // Precondition:  Insert, Letter menu item activated
        // Postcondition: The Letter dialog box is displayed. If data entered
        //                are OK, a Letter is created and added to the list
        //                of parcels
        private void letterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LetterForm letterForm; // The letter dialog box form
            DialogResult result;   // The result of showing form as dialog

            if (upv.AddressCount < LetterForm.MIN_ADDRESSES) // Make sure we have enough addresses
            {
                MessageBox.Show("Need " + LetterForm.MIN_ADDRESSES + " addresses to create letter!",
                    "Addresses Error");
                return;
            }

            letterForm = new LetterForm(upv.AddressList); // Send list of addresses
            result = letterForm.ShowDialog();

            if (result == DialogResult.OK) // Only add if OK
            {
                try
                {
                    // For this to work, LetterForm's combo boxes need to be in same
                    // order as upv's AddressList
                    upv.AddLetter(upv.AddressAt(letterForm.OriginAddressIndex),
                        upv.AddressAt(letterForm.DestinationAddressIndex),
                        decimal.Parse(letterForm.FixedCostText)); // Letter to be inserted
                }
                catch (FormatException) // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Letter Validation!", "Validation Error");
                }
            }

            letterForm.Dispose(); // Best practice for dialog boxes
        }

        // Precondition:  Report, List Parcels menu item activated
        // Postcondition: The list of parcels is displayed in the parcelResultsTxt
        //                text box
        private void listParcelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String
            decimal totalCost = 0;                      // Running total of parcel shipping costs
            string NL = Environment.NewLine;            // Newline shorthand

            result.Append("Parcels:");
            result.Append(NL); // Remember, \n doesn't always work in GUIs
            result.Append(NL);

            foreach (Parcel p in upv.ParcelList)
            {
                result.Append(p.ToString());
                result.Append(NL);
                result.Append("------------------------------");
                result.Append(NL);
                totalCost += p.CalcCost();
            }

            result.Append(NL);
            result.Append($"Total Cost: {totalCost:C}");

            reportTxt.Text = result.ToString();

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        // Precondition: File, save as menu item selected
        // Postcondition: Saves list of parcels and Addresses
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result; // creates and shows a dialog box that allows users to save files
            string fileName; //declares a string variable to hold the name of the saved data

            using (SaveFileDialog fileChooser = new SaveFileDialog())
            {
                fileChooser.CheckFileExists = false; // lets user create file
                result = fileChooser.ShowDialog(); // retrieve the results of the dialog box
                fileName = fileChooser.FileName;// returns the specified file name
            }
            if (result == DialogResult.OK)// makes sure the user pressed OK
            {
                if (fileName == string.Empty) // if user selected an invalid file
                    // displays an error message
                    MessageBox.Show("Invalid File Name", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    // saves the file using filestream if the user selected a valid file
                    try
                    {
                        //opens the file with write access
                        output = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                        formatter.Serialize(output, upv); // writes a upv (serialized object) to FileStream                   
                    }
                    catch (IOException)// handles the exception if there is a problem opening the file
                    {
                        // lets the user know if the file could not be opened 
                        MessageBox.Show("error Opening the File", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (SerializationException)// handles exception when there are no Serializables in the file
                    {
                        // lets the user know if there are no Serializables in the file  
                        MessageBox.Show("No more records in File", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }                                                                   
                                   
                                       
                }
            }

        }

        // Precondition: File, open menu item selected
        // Postcondition: opens a previously saved file, Deserializes it and reads the contents to the form
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;// creates and shows a dialog box that allows users to open files
            string fileName;//declares a string variable to hold the name of the file that contains data
            using (OpenFileDialog fileChooser = new OpenFileDialog())
            {
                result = fileChooser.ShowDialog();
                fileName = fileChooser.FileName;// returns the specified file name
            }
            if (result == DialogResult.OK)// makes sure the user pressed OK
            {
                if (fileName == string.Empty)// if user selected an invalid file
                    // displays an error message
                    MessageBox.Show("Invalid File Name", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {                    
                    try
                    {
                        // creates a filestream to obtain rea access to file
                        input = new FileStream(fileName, FileMode.Open, FileAccess.Read);                        
                        upv = (UserParcelView)reader.Deserialize(input);// gets next UserParcelView available in file
                    }
                    catch (IOException)// handles the exception if there is a problem opening the file
                    {
                        // lets the user know if the file could not be opened 
                        MessageBox.Show("error Opening the File", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }// handles exception when there are no Serializables in the file
                    catch (SerializationException)
                    {
                        // lets the user know if there are no Serializables in the file  
                        MessageBox.Show("No more records in File", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

            }
        }     
        // Precondition: Edit, Addresses menu item selected
        // Postcondition: edits existing address object
        private void addressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditAddressForm editAddress = new EditAddressForm(upv.AddressList); // creates a new Edit Address Form
            DialogResult result = editAddress.ShowDialog(); // displays the edit form as a dialog box and stores the results
            
            if (result == DialogResult.OK)// if everything is valid it opens the addressform(empty) need to have the data alrdy included into the new form!
            {
               
                AddressForm addressForm = new AddressForm(); //creates a new Address Form 

                // sets all of the address information(Name, Address 1 & 2, City, State, and ZipCode)
                // on the Address Form for the person who was selected in the Edit Address Dialog Box           
                addressForm.AddressName = upv.AddressAt(editAddress.NameAddress).Name.ToString();
                addressForm.Address1 = upv.AddressAt(editAddress.NameAddress).Address1.ToString();
                addressForm.Address2 = upv.AddressAt(editAddress.NameAddress).Address2.ToString();
                addressForm.City = upv.AddressAt(editAddress.NameAddress).City.ToString();
                addressForm.State = upv.AddressAt(editAddress.NameAddress).State.ToString();
                addressForm.ZipText = upv.AddressAt(editAddress.NameAddress).Zip.ToString();

                DialogResult editResult = addressForm.ShowDialog(); // displays all of the information that was has been set onto the dialog box

                if(editResult == DialogResult.OK) // if all of the text boxes contain valid information
                {
                    // edits the previous Address information to their new (if changed) values
                    upv.AddressAt(editAddress.NameAddress).Name = addressForm.AddressName;
                    upv.AddressAt(editAddress.NameAddress).Address1 = addressForm.Address1;
                    upv.AddressAt(editAddress.NameAddress).Address2 = addressForm.Address2;
                    upv.AddressAt(editAddress.NameAddress).City = addressForm.City;
                    upv.AddressAt(editAddress.NameAddress).State = addressForm.State;
                    upv.AddressAt(editAddress.NameAddress).Zip = Convert.ToInt32(addressForm.ZipText);
                }                         
            }
            
        }        
    }
    
}