using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Web.UI.Design.WebControls;
using System.Windows.Forms;

//base regular expression functionality found here

namespace Extensions.UI.Winforms
{
    public delegate string PrePasteEdit(string s);
    /// <summary>
    /// The MaskedEditTextBox class is derived from the base TextBox               
    /// class.  It accepts or rejects input, based on a supplied regular
    /// expression, in one of two ways: either each keypress is masked (i.e.
    /// all input is validated as it is entered), or the input is checked   
    /// all at once when the field is left.  The choice of which mode to use
    /// depends on the regular expression employed.    
    /// </summary>
    public class MaskedEditTextBox : TextBox
    {
        #region Event Handlers
        //property to control event subscription
        public event EventHandler TextInvalid
        {
            add  //add EventHandler delegate (value) to textInvalid
            {
                textInvalid += value;
            }
            remove  //remove EventHandler delegate (value) to textInvalid
            {
                textInvalid -= value;
            }
        }
        
        #endregion       
       
        #region Public Methods
        /// <summary>
        /// override reset() to init masked control specific var.
        /// </summary>
        public override void ResetText()
        {            
            //base.ResetText();
            base.Text = Mask;  
            RealTextLength = 0;
            ClipText = string.Empty;
            SelectionStart = RealTextLength;
        }
        #endregion  

        #region Public Properties
        /// <summary>
        /// Public property used to provide read-only access to lastValidValue
        /// </summary>
        [Browsable( false )]
        public string LastValidValue
        {
            get
            {
                return lastValidValue;
            }
        }

        #region Expression property comments
        /// <summary>
        /// By applying the Editor attribute we can specify a class dervied from
        /// System.Drawing.Design.UITypeEditor; this class will define the user 
        /// interface presented when the property is modified from Visual Studio .NET.
        /// 
        /// The Description attribute is used to provide context help in the 
        /// properties windows of Visual Studio .NET.
        /// 
        /// The DefaultValue attribute can be used to specify a default value for the 
        /// property (in this case, we can use it to specify the default regular 
        /// expression to be used).
        /// 
        /// The Expression property itself is used to store the regular expression 
        /// that will be used to evaluate/mask the input into this control after the control
        /// is left.
        /// </summary>
        #endregion
        [Editor(typeof( RegexTypeEditor ), typeof( UITypeEditor )),
        Description( "Regular expression without masks. Used to validate the control after it is left." ),
        DefaultValue( MATCH_EVERYTHING_REGEX )]
        public string ValidationExpression
        {
            //return the private expression
            get 
            { 
                return ( expression.ToString().Length > 0 ) ? expression.ToString() : MATCH_EVERYTHING_REGEX;
            }
			
            //set the private expression
            set 
            { 
                expression = new Regex( value ); 
            }
        }

        /// <summary>
        /// Like Expression above this property is used to provide access to a private
        /// regular expression field, keyPressExpression.  This private field is used to
        /// evalute/mask individual character input.
        /// </summary>
        [Editor( typeof(RegexTypeEditor ), typeof( UITypeEditor ) ),         
        Description( "Regular expression without masks. Used to validate the current text after the latest keypress is processed." ), 
        DefaultValue( MATCH_EVERYTHING_REGEX )]
        public string KeyPressExpression
        {
            get
            { 
                return ( keyPressExpression.ToString().Length > 0 ) ? keyPressExpression.ToString() : MATCH_EVERYTHING_REGEX;
            }

            set
            { 
                keyPressExpression = new Regex( value );
            }
        }

        /// <summary>
        /// Public property used to expose the mask mode of the control.
        /// </summary>
        [Browsable( false )]
        [DefaultValue( MaskMode.Both )]
        public MaskMode Mode
        {
            get 
            {
                return mode; 
            }

            set 
            { 
                mode = value; 
            }
        }

        /// <summary>
        /// determine how to place the cursor when the control becomes active.
        /// </summary>
        [Browsable( true ),
        Description( "Determine the cursor location and selection state when the control becomes active.")]
        public EntrySelectionStyle EntrySelectionStyle
        {
            private get
            {
                return i_EntrySelectionStyle;
            }
            set
            {
                i_EntrySelectionStyle = value;
            }
        }

        /// <summary>
        /// set or get text without masks. 
        /// </summary>
        [Browsable( true ),
        Description( "A string with masks. Used to format the text presentation. Use space for non-mask positions.")]
        public string Mask
        {
            get
            {
                return i_Mask;
            }
            set
            {
                string unmaskedText = UnMaskedText;
                i_Mask = value;
                UnMaskedText = unmaskedText;
            }
        }  

        /// <summary>
        ///   
        /// </summary>
        [Browsable( false )]
        [DefaultValue( true )]
        public bool IsValid
        {
            get
            {
                if( DesignMode )
                {
                    return true;
                }
                
                return i_IsValid;
            }
            private set
            {
                i_IsValid = value;
            }
        }        
    

        /// <summary>
        /// set or get text with masks.
        /// </summary>
        [Browsable( false )]
        [DefaultValue( "" )]
        public override string Text 
        {
            get
            {
                if( DesignMode )
                {
                    return String.Empty;
                }
                return base.Text;
            }
            set
            {                
                CollectLiteralLoc();  //could move to one loc. for init purpose
                        
                if( string.IsNullOrEmpty(value) )
                {
                    base.Text = Mask;
                    RealTextLength = 0;
                }
                else
                {
                    string textForValidation = value == null? string.Empty : value;

                    string textWithoutLiterals = RemoveLocMatchLiterals( textForValidation );

                    if( IsTempValid )
                    {                       
                        Match patternMatch = expression.Match( textWithoutLiterals ); 
                                             
                        if(( patternMatch.Success ) && ( patternMatch.Length == textWithoutLiterals.Length ) )
                        {
                            IsValid = true;
                            base.Text = CastLiteral( textWithoutLiterals );
                            TempClipText = textWithoutLiterals;
                            lastValidValue = textWithoutLiterals;
                        }
                        else
                        {
                            IsValid = false;
                            base.Text = AllowValidAssignOnly? Mask : CastLiteral( textWithoutLiterals );                   
                            TempClipText = AllowValidAssignOnly? string.Empty : textWithoutLiterals;
                        }
                    }    
                    else
                    {
                        IsValid = false;
                        base.Text = AllowValidAssignOnly? Mask : CastLiteral( textWithoutLiterals );                  
                        TempClipText = AllowValidAssignOnly? string.Empty : textWithoutLiterals;
                    }
                    RealTextLength = TempClipText.Length;                        
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Description( "Set or get text without masks." )]
        [Browsable( false )]
        [DefaultValue( "" )]
        public string UnMaskedText
        {
            get
            {
                if( DesignMode )
                {
                    return string.Empty;
                }
                if( CharacterCasing.ToString() == "Upper" )
                {
                    return TempClipText.ToUpper(); 
                }
                if( CharacterCasing.ToString() == "Lower" )
                {
                    return TempClipText.ToLower();                  
                }
                
                return TempClipText;
            }
            set
            {                 
                if( !DesignMode )
                {
                    ClipText = value;  
                }
            }
        }
        #endregion        

        #region Private and Protected Methods
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // MaskedEditTextBox
            // 
            Enter += new EventHandler(MaskedEditTextBox_Enter);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected override void OnKeyDown( KeyEventArgs e )                                     
        {
            if( ReadOnly  ) return;
            
            string stringForValidate = string.Empty;
            NewCursorLoc = OrigCursorLoc = SelectionStart;

            CollectLiteralLoc();  // consider to move out later

            if( e.KeyCode == Keys.Delete )  //right delete
            {
                e.Handled = true;               

                stringForValidate = GetStringForValidationRightDel( e );
                
                if( IsTempValid && IsValidOnKeyPress( stringForValidate.Substring(0, TempTextLength ) ) )
                {
                    SuccessWrapUp( stringForValidate );             
                }
                else
                {
                    NewCursorLoc = OrigCursorLoc;  
                }  
            }
            
        }

        /// <summary>
        /// Override of the OnKeyPress method of the TextBox base class, this function
        /// will test each character as it is typed to determine if the regular expression
        /// would be satisfied by the resulting value of this.Text.  If so, then the
        /// value is allowed, otherwise the input is ignored.
        /// </summary>
        /// <param name="e">Expected KeyPressEventArgs object provided by the framework</param>
        protected override void OnKeyPress( KeyPressEventArgs e ) 
        {
            string stringForValidate = string.Empty;
            NewCursorLoc = OrigCursorLoc = SelectionStart;
            
            CollectLiteralLoc();  // consider to move out later

            if( mode == MaskMode.Leave || 
                e.KeyChar == CTRL_C_COPY )
            {
                //allows further processing by base class
                e.Handled = false;

                //send all event args to base class, including the above property that
                //indicates to the base class to process the input
                base.OnKeyPress( e );

                return; //exit
            }   

            e.Handled = true;

            if( !ReadOnly )
            {
                if( e.KeyChar == CTRL_Z_UNDO )
                {
                    RestoreData();
                    return;
                }  


                if( IsLiteral( e.KeyChar ) )
                {
                    return;     
                }

                stringForValidate = GetStringForValidation( e );

                // TLG 08/14/2006 added null check; set TempTextLength variable in above method overload

                if( stringForValidate != null
                    && stringForValidate.Length >= TempTextLength
                    && IsTempValid 
                    && IsValidOnKeyPress( stringForValidate.Substring(0, TempTextLength ) ) )
                {                     
                    SuccessWrapUp( stringForValidate );             
                }
                else
                {
                    NewCursorLoc = OrigCursorLoc;  
                }  
            }         
        }

        /// <summary>
        /// Override of the OnMouseDown method of the TextBox base class so that the mouse can go to the end of 
        /// real text.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown( MouseEventArgs e ) 
        {                       
            base.OnMouseDown( e );
            if( e.Button.ToString() == "Left" && SelectionStart > RealTextLength + GetLiteralCount( RealTextLength ) )
            {
                SetSelectionToEndOfText();
            }
        }

        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            bool handledMessage = false;
            switch ( keyData )
            {
                case Keys.Control | Keys.V:
                    {
                        handledMessage = ProcContextMenu( "PASTE" );
                    }
                    break;
            }
            if ( handledMessage )
            {
                return true;
            }
            else
            {
                return base.ProcessCmdKey( ref msg, keyData );
            }
        }

        /// <summary>
        /// Override WndProc method to implement different behaviors for the TextBox ContextMenu.        
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc( ref Message m )
        {
            bool handledMessage = false; // used to decide if call to base WndProc needed.


            switch ( m.Msg )
            {               
                case WM_CUT :
                {
                    handledMessage = ProcContextMenu( "CUT");
                }
                    break;
                case WM_PASTE :
                {
                    handledMessage = ProcContextMenu( "PASTE");
                }
                    break;
                case WM_CLEAR : //same as delete cmd
                {
                    handledMessage = ProcContextMenu( "CLEAR");
                }
                    break;
                case WM_UNDO :
                {
                    handledMessage = ProcContextMenu( "UNDO");
                }
                    break;

            }
            if(handledMessage)
            {
                m.Result = new IntPtr( 0 ); //IntPtr.Zero
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// Override of the OnLeave method of the TextBox base class, this function will test the current
        /// value of the control and determine if it matches the appropriate regular
        /// expression (i.e. this.expression).  If so, then everything proceeds as
        /// normal, otherwise the value is changed back to a previously stored value
        /// (i.e. this.lastValidValue) depending on the setting in property AllowValidAssignOnly.
        /// </summary>
        /// <param name="e">The expected EventArgs object supplied by the framework.</param>
        protected override void OnLeave( EventArgs e ) 
        {
            if( mode == MaskMode.KeyPress || ReadOnly ) { base.OnLeave( e ); return; }

            //determine if there is a match
            string testString = TempClipText;

            Match patternMatch = expression.Match( testString );
           
            if( TempClipText.Length == 0 || 
                patternMatch.Success && patternMatch.Length == TempClipText.Length )           
            {
                //store valid value
                lastValidValue = TempClipText;

                //ClipText = TempClipText;//chg

                IsTempValid = true;

                IsValid = true;

            }             
            else //otherwise notify owner
            {
                //if the owner has subscribed then call textInvalid delegate(s)
                //thus letting owner decide what to do
                if(textInvalid != null)
                {
                    textInvalid( this, e );
                }
                    //otherwise, just revert to last valid value if the text value is not empty                
                else if( TempClipText != String.Empty )
                {
                    if( AllowValidAssignOnly )
                    {                    
                        ClipText = TempClipText = lastValidValue; // reset to previous valid value
                        IsTempValid = true;
                        IsValid = true;
                    }
                    else
                    {
                        IsTempValid = patternMatch.Success? true : false;
                        IsValid = false;                                            
                    }
                }                
            }
            //call TextBox control's OnLeave
            base.OnLeave( e );
        }


        internal bool ProcContextMenu( string command )
        {            
            string stringForValidate = string.Empty;
            NewCursorLoc = OrigCursorLoc = SelectionStart;

            if( command == "COPY" )
            {
                base.Copy();
                return true;
            }

            if( !ReadOnly )
            {
                if( command == "UNDO" )
                {
                    RestoreData();
                    return true;
                }            

                stringForValidate = GetStringForValidation( command );

                if (prePasteEdit != null)
                {
                    stringForValidate = prePasteEdit(stringForValidate);
                    TempTextLength = stringForValidate.Length;
                }
                if( IsTempValid && 
                    IsValidOnKeyPress( stringForValidate.Substring(0, TempTextLength ) ) )
                {                     
                    SuccessWrapUp( stringForValidate );             
                }
                else
                {
                    NewCursorLoc = OrigCursorLoc;  
                }   
            }

            return true;
        }


        private void SuccessWrapUp( string stringForValidation ) 
        {
            BackupData();

            RealTextLength = TempTextLength;            
            TempClipText = stringForValidation.Substring( 0, RealTextLength );

            base.Text = CastLiteral( stringForValidation );
            SelectionStart = OrigCursorLoc = NewCursorLoc;
            SelectionLength = 0;
            ScrollToCaret();

        }  

        private bool IsValidOnKeyPress( string stringForValidate ) 
        {            
            Match patternMatch = keyPressExpression.Match( stringForValidate );

            return ( stringForValidate.Length == 0 ) ||
                ( ( patternMatch.Success ) && (patternMatch.Length == stringForValidate.Length ) );
        }

        private bool IsValidOnWholeExpression( string stringForValidation ) 
        {
            Match patternMatch = expression.Match( stringForValidation );

            return ( patternMatch.Success ) && 
                ( patternMatch.Length == stringForValidation.Length );
        }

        private string GetStringForValidation( KeyPressEventArgs e ) 
        {
            string strForValidation = Text == null? string.Empty : Text;

            int selectionLen = SelectionLength;

            if( e.KeyChar == BACKSPACE ) 
            {
                strForValidation = LeftDelete( strForValidation, selectionLen );    
            }               
            else if( e.KeyChar == CTRL_X_CUT )
            {
                strForValidation = ProcCtrlX( strForValidation, selectionLen );  
            }
            else if( e.KeyChar == CTRL_V_PASTE )
            {             
                strForValidation = ProcCtrlV( strForValidation, selectionLen );  
            }
            else  
            {
                strForValidation = InsertChar( strForValidation, selectionLen, e.KeyChar );           
            }
                        
            strForValidation = RemoveLiterals( strForValidation );

            return strForValidation;
        }

        /// <summary>
        /// Overload GetStringForValidation specially for ContextMenu command process
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string GetStringForValidation( string command ) 
        {
            string strForValidation = Text == null? string.Empty : Text;

            int selectionLen = SelectionLength;
            
            switch( command )
            {
                case "CUT" :
                    strForValidation = ProcCtrlX( strForValidation, selectionLen );  
                    break;
                case "PASTE" :
                    strForValidation = ProcCtrlV( strForValidation, selectionLen );  
                    break;
                case "CLEAR" :
                    strForValidation = LeftDelete( strForValidation, selectionLen );  
                    break;
                default:
                    break;
            }
                                    
            strForValidation = RemoveLiterals( strForValidation );
			TempTextLength = strForValidation.Length;
            return strForValidation;
        }

        private string GetStringForValidationRightDel( KeyEventArgs e )
        {
            string strForValidation = Text;            
            int endLocOfMaskedText = RealTextLength + GetLiteralCount( RealTextLength );
            
            TempTextLength = RealTextLength;

            if( SelectionLength == 0 )
            {                
                if( OrigCursorLoc < endLocOfMaskedText )
                {
                    int nonLiteralLoc = GetFirstRightNonLiteralLoc( strForValidation, OrigCursorLoc );
               
                    if( nonLiteralLoc != endLocOfMaskedText )
                    {
                        strForValidation = strForValidation.Substring( 0, nonLiteralLoc ) + 
                            strForValidation.Substring( nonLiteralLoc + 1 );
                        TempTextLength = RealTextLength - 1;
                    }                    
                }               
            }
            else
            {
                strForValidation = strForValidation.Substring( 0, OrigCursorLoc ) + 
                    strForValidation.Substring( OrigCursorLoc + SelectionLength );

                TempTextLength = CalcTempTextLength(  SelectionLength, 0 );
            }          
            
            strForValidation = RemoveLiterals( strForValidation );
                            
            return strForValidation;            
        }


        private string LeftDelete( string strForProc, int selectionLen ) 
        {
            string processedString = strForProc;

            if( OrigCursorLoc == 0 ) 
            {                
                processedString = processedString.Substring( selectionLen );  

                TempTextLength = CalcTempTextLength(  selectionLen, 0 );
            }
            else if( selectionLen == 0 )
            {                
                int firstLeftNonLiteralLoc = GetFirstLeftNonLiteralLoc( processedString, OrigCursorLoc - 1 );
                //NewCursorLoc = GetFirstLeftNonLiteralLoc( processedString, OrigCursorLoc - 1 );
                NewCursorLoc = firstLeftNonLiteralLoc == -1? 0 : firstLeftNonLiteralLoc;

                processedString = processedString.Substring( 0, NewCursorLoc ) + 
                    processedString.Substring( OrigCursorLoc );    
                if( OrigCursorLoc > RealTextLength + GetLiteralCount( RealTextLength ) )
                {
                    TempTextLength = RealTextLength;
                }
                else
                {
                    TempTextLength = firstLeftNonLiteralLoc == -1? RealTextLength : RealTextLength -1;
                }
               
            }
            else //both CursorLoc and selectionLen !=0
            {
                processedString = processedString.Substring( 0, NewCursorLoc ) + 
                    processedString.Substring( NewCursorLoc + selectionLen );   
                
                TempTextLength = CalcTempTextLength(  selectionLen, 0 );
            }
    
            return processedString;

        }
    
        private string InsertChar( string strForProc, int selectionLen, char keyChar ) 
        {
  
            return ProcessInserting( strForProc, keyChar.ToString(), selectionLen );

        } 

        private string ProcCtrlX( string strForProc, int selectionLen ) 
        {
            string processedString = strForProc;           

            base.Copy();  //copy the selection to clipboard.
            processedString = processedString.Substring( 0, OrigCursorLoc ) + 
                processedString.Substring( OrigCursorLoc + selectionLen );                 

            TempTextLength = CalcTempTextLength(  selectionLen, 0 );

            return processedString;
        }

        private string ProcCtrlV( string strForProc, int selectionLen ) 
        {
            string processedString = strForProc;
            string pastedString = string.Empty;

            IDataObject iData = Clipboard.GetDataObject();
 
            // Determines whether the data is in a text format.
            if( iData != null && iData.GetDataPresent(DataFormats.Text)) 
            {
                pastedString = (String)iData.GetData(DataFormats.Text); 
            }
            else 
            {
                return processedString;
            }

            return ProcessInserting( processedString, pastedString, selectionLen );
        }

        private string ProcessInserting( string strForProc, string insertString, int selectionLen )
        {
            string processedString = strForProc;
            
            int endLocOfMaskedText = RealTextLength + GetLiteralCount( RealTextLength );
            if( RealTextLength + literalsLocArrList.Count + insertString.Length - selectionLen <= MaxLength 
                && SelectionStart <= endLocOfMaskedText )
            {
                int firstNonLiteralLoc = GetFirstRightNonLiteralLoc( strForProc, OrigCursorLoc );
                
                if( selectionLen == 0 )
                {
                    processedString = processedString.Substring( 0, firstNonLiteralLoc ) + insertString +
                        processedString.Substring( firstNonLiteralLoc );                        
                }
                else
                {                                                           
                    processedString = processedString.Substring( 0, firstNonLiteralLoc ) + insertString +
                        processedString.Substring( SelectionStart + SelectionLength );                       
                }
                TempTextLength = CalcTempTextLength( selectionLen, insertString.Length );  

                NewCursorLoc = firstNonLiteralLoc + insertString.Length + 
                    GetLiteralCount( firstNonLiteralLoc, insertString.Length ) ;
            }
            else
            {
                TempTextLength = RealTextLength;
            }   
     
            return processedString;
        
        }

        private int GetFirstLeftNonLiteralLoc( string strForProc, int startCursorLoc ) 
        {
            int firstNonLiteralLoc = -1;
                        
            for( int i = startCursorLoc; i >= 0; i-- )
            {
                if( !literalsLocArrList.Contains( i ) )
                {
                    firstNonLiteralLoc = i;
                    break;
                }
            }    

            return firstNonLiteralLoc;
        }

        private int GetFirstRightNonLiteralLoc( string strForProc, int startCursorLoc ) 
        {
            int firstNonLiteralLoc = startCursorLoc;
                        
            for( int i = startCursorLoc; i <= MaxLength -1; i++ )
            {
                if( !literalsLocArrList.Contains( i ) )
                {
                    firstNonLiteralLoc = i;
                    break;
                }
            }    

            return firstNonLiteralLoc;
        }

        private bool IsLiteral( char keyChar )  
        { 
            return Mask.IndexOf( keyChar ) > 0; 
        }

        private string RemoveLiterals( string strForValidation ) 
        {            
            string literal = string.Empty;
            string processedString = strForValidation;

            IsTempValid = true;                        

            foreach( int literalLoc in literalsLocArrList ) 
            {                
                literal = Mask.Substring( literalLoc, 1 );
                processedString = processedString.Replace( literal, string.Empty );
            }    

            return processedString;
        }

        private string RemoveLocMatchLiterals( string strForValidation )       
        {            
            string literal = string.Empty;
            string processedString = strForValidation;
            int literalLoc = 0;
            IsTempValid = true;                        
            
            if( strForValidation.Length > MaxLength || strForValidation.Length < Mask.Length )            
            {
                IsTempValid = false;
                return strForValidation;
            }

            for( int i = literalsLocArrList.Count - 1 ; i >=0 ; i-- )            
            {
                literalLoc = (int)literalsLocArrList[ i ]; 
                literal = Mask.Substring( literalLoc, 1 );

                if( literal == processedString.Substring( literalLoc, 1 ) )
                {
                    processedString = processedString.Substring( 0, literalLoc ) +
                        processedString.Substring( literalLoc + 1 ) ;
                }
                else
                {
                    IsTempValid = false;
                    return processedString;                        
                }
            }                                   

            return processedString;         
        }

        private string CastLiteral( string inputString ) 
        {
            string spc = new string( ' ', Mask.Length ); 
            string castString = inputString + spc;

            foreach( int literalLoc in literalsLocArrList ) 
            {
                castString = castString.Substring( 0, literalLoc ) + Mask.Substring( literalLoc, 1 ) + 
                    castString.Substring( literalLoc ); 
            }  
         
            if( ( TempTextLength + GetLiteralCount( TempTextLength ) ) <= Mask.Length )
            {
                return castString.TrimEnd();
            }
            
            return castString.Substring( 0, castString.Length - Mask.Length );
        }

        private void MaskFuncInit() 
        {
            Mask = DEFAULT_MASK;  
            base.Text = Mask;  
            RealTextLength = 0;
            ClipText = string.Empty;
            SelectionStart = RealTextLength;

            CollectLiteralLoc();  
        }

        private void CollectLiteralLoc() 
        {
            //            if( literalsLocArrList.Count == 0 )  //because the mask could be changed dynamically
            //            {   
            literalsLocArrList.Clear();
            for( int i = 0; i <= Mask.Length-1; i++ )
            {
                if( Mask.Substring( i, 1 ) != " " )
                {                
                    literalsLocArrList.Add( i );
                }
            }
            //            }
        }

        //get literal count for certain real text length where the literals aren't included.
        private int GetLiteralCount( int realTextLength ) 
        {
            int literalCount = 0;

            foreach( int literalLoc in literalsLocArrList ) 
            {
                if( literalLoc <= realTextLength + literalCount )
                {
                    literalCount += 1;
                }
            }  
            return literalCount;                                
        }

        // overload GetLiteralCount to get # of mask literals between start point in a string 
        // with mask literals and a pasted string length which doesn't have mask literals. 
        private int GetLiteralCount( int startLoc, int NonLiteralStringLen ) 
        {
            int literalCount = 0;

            foreach( int literalLoc in literalsLocArrList ) 
            {
                if( literalLoc >= startLoc && literalLoc <= startLoc + NonLiteralStringLen + literalCount ) //use < ??
                {
                    literalCount += 1;
                }
            }  
            return literalCount;                                
        }


        //get literal count for certain masked text section selected where the literals are included. add 050105
        private int GetLiteralCountForMaskedText( int startLoc, int selectionLen ) 
        {
            int literalCount = 0;
            int endLocOfMaskedText = RealTextLength + GetLiteralCount( RealTextLength );
            int endLocOfSelectedText = 0;

            if( startLoc < endLocOfMaskedText )
            {
                if( ( startLoc + selectionLen ) >=  endLocOfMaskedText )
                {
                    endLocOfSelectedText = endLocOfMaskedText;
                }
                else
                {
                    endLocOfSelectedText = startLoc + selectionLen;
                }

                foreach( int literalLoc in literalsLocArrList ) 
                {
                    if( literalLoc >= startLoc && literalLoc < endLocOfSelectedText )
                    {
                        literalCount += 1;
                    }
                }  
            }

            return literalCount;                                
        }

        private void MaskedEditTextBox_Enter( object sender, EventArgs e )
        {
            if( EntrySelectionStyle == EntrySelectionStyle.SelectionAtEnd )
            {
                SetSelectionToEndOfText();
            }
            else if( EntrySelectionStyle == EntrySelectionStyle.SelectionAtStart )
            {
                SetSelectionToStartOfText();
            }
            else if( EntrySelectionStyle == EntrySelectionStyle.SelectAllText )
            {
                SelectAllText();
            }
        }

        private void SetSelectionToStartOfText()
        {
            SelectionStart = 0;
            SelectionLength = 0;
        }

        private void SelectAllText()
        {
            SelectionStart = 0;
            SelectionLength = TextLength;
        }

        private void SetSelectionToEndOfText()
        {
            SelectionStart = RealTextLength + GetLiteralCount( RealTextLength );
            SelectionLength = 0;
        }

        private void BackupData()
        {
            lastTextInfo.unMaskedText = UnMaskedText;       
            lastTextInfo.cursorLoc = SelectionStart;
            lastTextInfo.selectionLen = SelectionLength;
            lastTextInfo.fillByUndo = false;
        }

        private void RestoreData()
        {
            LastTextInfo temp = new LastTextInfo();

            temp.unMaskedText = UnMaskedText;
            temp.cursorLoc = SelectionStart;
            temp.selectionLen = SelectionLength;

            //050203
            // add null check (SZ 08/24/2007)
            RealTextLength = 
                lastTextInfo.unMaskedText == null ? 0 : lastTextInfo.unMaskedText.Length;   
            
            TempClipText = lastTextInfo.unMaskedText; //050203

            UnMaskedText = lastTextInfo.unMaskedText;       
            SelectionStart = lastTextInfo.cursorLoc;
            SelectionLength = lastTextInfo.selectionLen;

            lastTextInfo.unMaskedText = temp.unMaskedText;       
            lastTextInfo.cursorLoc = temp.cursorLoc;
            lastTextInfo.selectionLen = temp.selectionLen;

        }

        private int CalcTempTextLength(  int selectionLen, int pastedStringLen ) 
        {
            int tempTextLength = 0;

            int endLocOfSel = SelectionStart; 
            endLocOfSel = endLocOfSel + selectionLen;

            int endLocOfText = RealTextLength;
            endLocOfText = endLocOfText + GetLiteralCount( RealTextLength );

            if( endLocOfSel <= endLocOfText )
            {
                tempTextLength = RealTextLength + pastedStringLen - selectionLen + GetLiteralCountForMaskedText( SelectionStart, selectionLen ) ; //chg 050104
            }
            else if( SelectionStart ==  endLocOfText ) 
            {
                tempTextLength = RealTextLength + pastedStringLen + GetLiteralCountForMaskedText( SelectionStart, selectionLen ) ; //chg 050311
            }
            else if( SelectionStart >  endLocOfText )
            {
                tempTextLength = RealTextLength;
            }
            else  //overlap
            {
                tempTextLength = RealTextLength + pastedStringLen - ( endLocOfText - SelectionStart ) 
                    + GetLiteralCountForMaskedText( SelectionStart, selectionLen ); //chg 050104
            }

            return tempTextLength;
        }
        #endregion

        #region Private Properties
        private int NewCursorLoc
        {
            get
            {
                return i_NewCursorLoc;
            }
            set
            {
                i_NewCursorLoc = value;
            }        
        }

        private int OrigCursorLoc
        {
            get
            {
                return i_OrigCursorLoc;
            }
            set
            {
                i_OrigCursorLoc = value;
            }        
        }

        private int RealTextLength
        {
            get
            {
                return i_RealTextLength;
            }
            set
            {
                i_RealTextLength = value;
            }        
        }

        private int TempTextLength
        {
            get
            {
                return i_TempTextLength;
            }
            set
            {
                i_TempTextLength = value;
            }        
        }    
    
        /// <summary>
        /// hold temporary valid text on keypress
        /// </summary>
        private string TempClipText
        {
            get
            {
                return i_TempClipText;
            }
            set
            {
                i_TempClipText = value;
            }        
        }    

        /// <summary>
        ///  Currently make it as private. Can be used later if developer wants a choice to only allow valid assignment.     
        /// </summary>
        [Browsable( false )]
        private bool AllowValidAssignOnly
        {
            get
            {
                return i_AllowValidAssignOnly;
            }
            
        }

        /// <summary>
        ///   
        /// </summary>
        [Browsable( false )]
        private bool IsTempValid
        {
            get
            {
                return i_IsTempValid;
            }
            set
            {
                i_IsTempValid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable( false )]
        [DefaultValue( "" )]
        private string ClipText
        {
            set
            {
                CollectLiteralLoc();  //could move from other place to here.

                i_ClipText = value == null? string.Empty : value;
                
                if ( i_ClipText.Length > MaxLength - literalsLocArrList.Count )           
                {
                    i_ClipText = i_ClipText.Substring( 0, MaxLength - literalsLocArrList.Count );
                }
      
                RealTextLength = i_ClipText.Length;           
                TempTextLength = i_ClipText.Length; //050127

                if ( i_ClipText == string.Empty )
                {
                    base.Text = Mask;   
                    lastValidValue = i_ClipText;   
                    IsValid = true;
                }
                else
                {                                                          
                    if( IsValidOnWholeExpression( i_ClipText ) )                    
                    {
                        base.Text = CastLiteral( i_ClipText ); 

                        lastValidValue = i_ClipText; 
                        IsValid = true; 
                    }                                  
                    else 
                    {
                        if( AllowValidAssignOnly )
                        {
                            i_ClipText = lastValidValue;  
                            IsValid = true;
                            RealTextLength = i_ClipText.Length; //050127  
                            TempTextLength = i_ClipText.Length; //050127
                        }
                        else
                        {                           
                            base.Text = CastLiteral( i_ClipText ); 
                            IsValid = false;
                        }
                    }
                }
                TempClipText = i_ClipText;

            }
        }  

        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Default constructor
        /// </summary>
        public MaskedEditTextBox()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            //initialize mask mode
            //mode = MaskMode.Both;

            //initialize expression
            expression = new Regex( MATCH_EVERYTHING_REGEX );

            //initialize keyPressExpression
            keyPressExpression = new Regex( MATCH_EVERYTHING_REGEX );
            
            MaskFuncInit(); 
        }

        /// <summary>
        /// Another constructor for the class, this method allows the developer to specify
        /// the mask mode and expressions to be used.
        /// </summary>
        /// <param name="evalMode">The mask mode (i.e. per character, on leave, or both) to be employ</param>
        /// <param name="leaveExpression">The expression used to evalute input on leave</param>
        /// <param name="keystrokeExpression">The expression used to evaluate input as characters are typed</param>
        public MaskedEditTextBox( MaskMode evalMode, string leaveExpression, string keystrokeExpression )
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            //set mask mode
            Mode = evalMode;

            //initialize appropriate expressions
            switch ( evalMode )
            {
                case MaskMode.KeyPress :
                    keyPressExpression = new Regex( keystrokeExpression );
                    break;
                case MaskMode.Leave :
                    expression = new Regex( leaveExpression );
                    break;
                case MaskMode.Both :
                    keyPressExpression = new Regex( keystrokeExpression );
                    expression = new Regex( leaveExpression );
                    break;
            }
            MaskFuncInit(); 
        }
        #endregion

        #region Data Elements
        /// <summary>
        /// Supplies the list of modes to be used by the control.
        /// </summary>
        public enum MaskMode
        {
            KeyPress,
            Leave,
            Both
        }

        private ArrayList literalsLocArrList = new ArrayList();            
        private ArrayList inputArrList = new ArrayList();  //not used

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        /// <summary>
        /// Private field used to contain a last known good value.
        /// </summary>
        private string lastValidValue = string.Empty; 
        
        private EntrySelectionStyle i_EntrySelectionStyle;
        
        private string i_Mask;
        private string i_ClipText;
        private string i_TempClipText;

        private int i_NewCursorLoc;        
        private int i_OrigCursorLoc;
        private int i_RealTextLength;  //accepted text length without mask literals
        private int i_TempTextLength;  //latest text without mask literals
        private bool i_AllowValidAssignOnly = false;
        private bool i_IsTempValid = true;
        private bool i_IsValid = true;

        private LastTextInfo lastTextInfo = new LastTextInfo(); //( string.Empty, 0, 0, false );
        
        /// <summary>
        /// The regular expression that input will be validated against after this
        /// textbox is left.
        /// </summary>
        private Regex expression = new Regex( MATCH_EVERYTHING_REGEX );

        /// <summary>
        /// The regular expression that input will be validated against on a per
        /// character basis as it is typed by the user.
        /// </summary>
        private Regex keyPressExpression = new Regex( MATCH_EVERYTHING_REGEX );

        /// <summary>
        /// Private field used to contain the mask mode of the control.
        /// </summary>
        private MaskMode mode = MaskMode.Both;

        #endregion

        #region Constants        
        //used to contain delegates to call when the text is invalid
        private EventHandler textInvalid;
        public PrePasteEdit prePasteEdit;
        private const char 
            BACKSPACE       = (char)8,
            CTRL_Z_UNDO     = (char)26,
            CTRL_X_CUT      = (char)24,
            CTRL_C_COPY     = (char)3,
            CTRL_V_PASTE    = (char)22;

        private const string
            MATCH_EVERYTHING_REGEX = "^.*";

        private static readonly string DEFAULT_MASK = string.Empty;

        private const int
            WM_KEYDOWN = 0x0100,
            WM_KEYUP   = 0x0101,
            WM_CUT     = 0x0300,
            WM_COPY    = 0x0301,
            WM_PASTE   = 0x0302,
            WM_CLEAR   = 0x0303,
            WM_UNDO    = 0x0304;

        #endregion

    }

    public enum EntrySelectionStyle
    {
        SelectionAtEnd,
        SelectionAtStart,
        SelectAllText,
        Default
    }

    //used for UNDO process
    struct LastTextInfo
    {
        public string unMaskedText;            
        public int    cursorLoc;
        public int    selectionLen;
        public bool   fillByUndo;
    }
}