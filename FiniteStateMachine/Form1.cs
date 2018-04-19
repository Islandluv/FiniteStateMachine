using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiniteStateMachine
{
    /************************************************************
     *** Class Name: Form1 : Form
     *** Class Author: Allistair London
     ************************************************************
     *** Purpose of class: Displays windows Form that houses
     *** controls that enable user to select a file, load file
     *** and enter validated words and determine wheter or not
     *** validated words are contained in language.
     ************************************************************
     *** Date: 04/17/2018
     ************************************************************
     ***********************************************************/
    public partial class Form1 : Form
    {
        //GLobal variable
        int sState;  //Sets start state
        string entry = string.Empty;
        int nxState; //Variable to hold nxtState
        int curState; //Variable to hold curState
        string result = ""; //String to hold each line in text file
        List<String> aRule = new List<String>(); //List array to store ruls
        List<String> charSplit = new List<String>(); //List array to hold characters
        List<int> aState = new List<int>(); //List array used to hold accepting states extracted from text file

        public Form1()
        {
            InitializeComponent();
            //txtInput.Focus();
            btnLoadFile.Focus();
        }

        /*********************************************************************
        * Method Name: btnLoadFIle()
        * Method Author: Allistair London
        **********************************************************************
        * Purpose of method: This method ensures that a selected file is 
        * correctly opened.
        * Method Inputs: N/A
        * Return Value: N/A
        * ********************************************************************
        * Date: 04/17/2018
        *********************************************************************/
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            StreamReader inputFile; //StreamReader variable
            OpenFileDialog openFileDialog = new OpenFileDialog(); //Declare a new instance of OpenFileDialog
            

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            string test = ""; //Variable to hold ReadLine to check for Start State
            string accept = ""; //Variable to hold ReadLine to check for Accepting State(s)

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK) //If user selects Ok in file selection process
                {
                    try
                    {
                        inputFile = File.OpenText(openFileDialog.FileName); //Assigne file to variable inputFile
                        //
                        while (!inputFile.EndOfStream)
                        {
                            result = inputFile.ReadLine();
                            //Searches document for embedded Start State
                            //uses Regex.Ismatch to find desired line in file
                            if(Regex.IsMatch(result, "Start State *"))
                            {
                                //Assign  each ReadLine to String variable
                                test = result.ToString();
                                //Use Regex funtion to remove none numeric
                                //afer assigning specific file line to string[] numbers
                                string[] numbers = Regex.Split(test, @"\D+");

                                //Checks each element in numbers array
                                foreach(string i in numbers)
                                {
                                    if(!string.IsNullOrEmpty(i)) //Ensure is not null
                                    {
                                        //Assigns Start State to sState variable
                                        sState = int.Parse(i);
                                    }
                                }
                            }
                            //Searches document for embedded Accepting States
                            //uses Regex.Ismatch to find desired line in file
                            if (Regex.IsMatch(result, "Accepting States *"))
                            {
                                //Assign each ReadLine to String variable
                                accept = result.ToString();

                                //Use Regex funtion to remove none numeric
                                //afer assigning specific file line to string[] accepting
                                string[] accepting = Regex.Split(accept, @"\D+");

                                //Checks each element in accepting array
                                foreach (string t in accepting)
                                {
                                    if (!string.IsNullOrEmpty(t)) //Ensure is not null
                                    {
                                        aState.Add(int.Parse(t));
                                    }
                                }
                            }
                            //If ReadLine contain specific format
                            //add to aRule List Array.
                            if (result.Contains("G("))
                            { aRule.Add(String.Format("{0},{1},{2}", result.Substring(2, 1), result.Substring(4, 1), result.Substring(9, 1))); }
                        }
                        //close file
                        inputFile.Close();
                        MessageBox.Show("File Successfully Loaded"); //Inform user of successful file load
                        txtInput.Focus(); //Reset focus to txtInput
                    }
                    catch (Exception ex)
                    {
                        //Informs user of exception
                        MessageBox.Show(ex.ToString());
                    }
                }
                else
                {
                    //Infomrs user of their action
                    MessageBox.Show("You Cancelled File Selection Process","HELLO THERE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //Reset focus to btnFileLoad
                    btnLoadFile.Focus();
                }
                    
            }
            catch (Exception ex)
            {
                //Informs user of exception
                MessageBox.Show(ex.Message);
            }
        }

        /*********************************************************************
        * Method Name: ValidateChar()
        * Method Author: Allistair London
        **********************************************************************
        * Purpose of method: This method ensures that each validated
        * character in SubString is valid and assigned to correct
        * index for execution
        * Method Inputs: string symbol
        * Return Value: N/A
        * ********************************************************************
        * Date: 04/17/2018
        *********************************************************************/
        private void ValidateChar(string symbol)
        {          
            try
            {
                //Checks each item in aRule array
                foreach (string item in aRule)
                {
                    string[] rules = item.Split(','); //uses , as delimiter

                    if (curState == int.Parse(rules[0]) && symbol == rules[1])
                    {
                        nxState = Convert.ToInt32(rules[2]); //sets nextState variable to index 2 of SubString
                        curState = nxState; //assigns nxtState to curState
                        break; //break out of loop
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); } //Displays exception message
        }

        /*********************************************************************
        * Method Name: btnSubmit()
        * Method Author: Allistair London
        **********************************************************************
        * Purpose of method: This method checks that txtInput receives only
        * valid entries (only accepts alphanumeric). THis method also
        * checks if an entered word is contained in language.
        * Method Inputs: N/A
        * Return Value: N/A
        * ********************************************************************
        * Date: 04/17/2018
        *********************************************************************/
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //if running again, reset current state
            curState = sState; //Sets current state to Start State
            entry = txtInput.Text; //Assign textBox entry to variable entry

            //Checks if a file was loaded
            if(result == "")
            {
                MessageBox.Show("NO FILE SELECTED! Please Upload a TextFile", "STOP", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //Clears txtInput
                txtInput.Clear();
                //Sets focus to btnFileLoad after user is informed that
                //no file was selected
                btnLoadFile.Focus();
            }
            else
            {
                if (txtInput.Text != "") //Check for blank inputBox
                {
                    //Checks for non-alphabetic characters
                    if (!System.Text.RegularExpressions.Regex.IsMatch(txtInput.Text, "^[a-zA-Z ]"))
                    {
                        //Informs user only Alphabetic entry is accepted.
                        //advics user to enter valid LOWERCASE alphabetic entry
                        MessageBox.Show("This input only accepts ALPHABETIC entries. You should enter a LowerCase entry.", 
                            "Hello There",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtInput.Clear();//clear txtInput
                        txtInput.Focus(); //Set focus to txtInput     
                    }
                    //Checks  if all characters are UpperCase
                    else if(entry.All(char.IsUpper))
                    {
                        //Advices user that entry is UpperCase and offers to convert to LowerCase
                        DialogResult alpha = MessageBox.Show("Your entry is all Uppercase. Shall I convert it LowerCase?"
                            , "Hello There", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (alpha == DialogResult.Yes) //If user selects yes
                        {
                            //split string into characters
                            foreach (char c in txtInput.Text.ToLower())
                            {
                                entry = txtInput.Text.ToLower();
                                ValidateChar(c.ToString()); //Calls the ValidateChar() passes in char c.
                            }
                            // After all characters in input are exausted, checks to see if
                            // current state is equal an accepting state in sState List Array
                            if (aState.Contains(curState))
                            {
                                //Displays to user that selected word is In language.
                                lblDisplay.Text = "The word " + entry + " is in this language.";
                                txtInput.Text = "Your previous entry was converted to: " + entry.ToString();
                            }
                            else
                            {
                                //DIsplays to user that entered word is not in language
                                lblDisplay.Text = "The word " + entry + " is not in this language.";
                                txtInput.Text = "Your previous entry was converted to: " + entry.ToString();
                            }
                        }
                        else
                        {
                            txtInput.Clear(); //Clears txtInput
                            txtInput.Focus(); //Resets focus to txtInput if an entry is invalid.
                            return;
                        }

                    }
                    //Checks for a mixture of UpperCase and LowerCase characters
                    //If mixture exists, user will be asked if entry should be converted
                    //to all LowerCase
                    else if(entry.Any(char.IsLower) && entry.Any(char.IsUpper))
                    {
                        DialogResult alpha = MessageBox.Show("Some of the entered Characters are UpperCase. Shall I convert them to LowerCase?"
                           , "Hello There", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (alpha == DialogResult.Yes)
                        {
                            //split string into characters
                            foreach (char c in txtInput.Text.ToLower())
                            {
                                entry = txtInput.Text.ToLower();
                                ValidateChar(c.ToString()); //Calls the ValidateChar() passes in char c.
                            }
                            // After all characters in input are exausted, checks to see if
                            // current state is equal an accepting state in sState List Array
                            if (aState.Contains(curState)) //Checks if current state is equal to any accepting state in sState List Array
                            {
                                //Displays to user that selected word is In language.
                                lblDisplay.Text = "The word " + entry + " is in this language.";
                                txtInput.Text = "Your previous entry was converted to: " + entry.ToString();
                            }
                            else
                            {
                                //DIsplays to user that entered word is not in language
                                lblDisplay.Text = "The word " + entry + " is not in this language.";
                                txtInput.Text = "Your previous entry was converted to: " + entry.ToString();
                            }
                        }
                        else
                        {
                            txtInput.Clear(); //Clears txtInput
                            txtInput.Focus(); //Resets focus to txtInput if an entry is invalid.
                            return;
                        }
                    }
                    //Check if entry is all LowerCase alphabetic
                    else if((System.Text.RegularExpressions.Regex.IsMatch(txtInput.Text, "^[a-z ]")))
                    {
                        //split string into characters
                        foreach (char c in txtInput.Text)
                        {
                            ValidateChar(c.ToString()); //Calls the ValidateChar() passes in char c.
                        }
                        // After all characters in input is exausted, checks to see if
                        // current state is equal to accepting state.
                        // After all characters in input are exausted, checks to see if
                        // current state is equal an accepting state in sState List Array
                        if (aState.Contains(curState))
                        {
                            //Displays to user that selected word is In language.
                            lblDisplay.Text = "The word " + entry + " is in this language.";
                        }
                        else
                        {
                            //DIsplays to user that entered word is not in language
                            lblDisplay.Text = "The word " + entry + " is not in this language.";
                        }
                    }
                }
                else
                {
                    //Informs user txtINput cannot be blank.
                    // Offers user the option to retry if RETRY is selected
                    DialogResult result = MessageBox.Show("Cannot be blank! You must make an Alphanumeric entry", "STOP",
                        MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                    if (result == DialogResult.Retry)
                    {
                        txtInput.Clear(); //Clears txtInput.
                        txtInput.Focus(); //Resets focus to txtInput
                    }
                    else if (result == DialogResult.Cancel) //If CANCEL is selected
                    {
                        //Informs user CANCEL will exit program and gives them a final option of 
                        //cancelling program, or returning to form where a valid word can be entered
                        DialogResult ques = MessageBox.Show("You opted to exit program. Are you sure you want to exit?", "STOP",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (ques == DialogResult.Yes)
                        {
                            this.Close(); //Closes the program if user selects Yes
                        }
                        else
                        {
                            txtInput.Clear(); //clears txtInput.
                            txtInput.Focus(); //sets focus to txtInput.
                            return;
                        }
                    }
                }
            }        
        }
       /*********************************************************************
       * Method Name: ClearState()
       * Method Author: Allistair London
       **********************************************************************
       * Purpose of method: This method resets the form objects to include:
       * txtInput, lblDisplay, the curState variabl (reset to start state)
       * Focus is also placed on txtInput once all objects are cleard
       * Method Inputs: N/A
       * Return Value: N/A
       * ********************************************************************
       * Date: 04/17/2018
       *********************************************************************/
        private void ClearState()
        {
            txtInput.Clear();
            lblDisplay.Text = "";
            curState = sState;
            //validated = string.Empty;
            txtInput.Focus();
        }

       /*********************************************************************
       * Method Name: btnClear()
       * Method Author: Allistair London
       **********************************************************************
       * Purpose of method: This method calls the ClearState()
       * Method Inputs: N/A
       * Return Value: N/A
       * ********************************************************************
       * Date: 04/17/2018
       *********************************************************************/
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearState();
        }

       /*********************************************************************
       * Method Name: btnExit()
       * Method Author: Allistair London
       **********************************************************************
       * Purpose of method: This method call the Close() to exit program 
       * Method Inputs: N/A
       * Return Value: N/A
       * ********************************************************************
       * Date: 04/17/2018
       *********************************************************************/
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*********************************************************************
        * Method Name: Form1_Load()
        * Method Author: Allistair London
        **********************************************************************
        * Purpose of method: This method executes contained code prior
        * to form load. THe contianed instructions offer instructions to
        * users
        * Method Inputs: N/A
        * Return Value: N/A
        * ********************************************************************
        * Date: 04/17/2018
        *********************************************************************/
        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("To Start, Select text file contianing rules and upload to program",
                "HELLO THERE", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
