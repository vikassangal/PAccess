using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.BillingViews
{
    /// <summary>
    /// Summary description for DropdownManager.
    /// </summary>
    public class ComboBoxManager
    {
        #region Public methods
        public ComboBoxManager()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public void ResetComboBoxManager()
        {
            combos.Clear();
        }

        public void AddComboBox( PatientAccessComboBox aComboBox )
        {
            combos.Add( aComboBox.Name, aComboBox );
        }

        public void ComboValueSet( PatientAccessComboBox combo, object comboValue )
        { 
            if( combo != null )
            {
	            ResetAllSelection(); 
            }
        }

        public void ComboValueInitialSet( PatientAccessComboBox combo, object comboValue )
        {
            if (combo != null)
            {
                if (IsManualCode(comboValue))
                {
                    this.ReplaceItem(combo, comboValue);

                    combo.Enabled = true;
                }
                else
                {
                    combo.Items.Add(comboValue);
                    combo.SelectedItem = comboValue;

                    ValidatingCode vc = comboValue as ValidatingCode;
                    if (vc != null)
                    {
                        if (vc.IsValid)
                            combo.Enabled = false;
                        else
                            combo.Enabled = true;
                    }
                }

                RemoveValueFromCombos(comboValue, combo);
            }
        }

        public PatientAccessComboBox GetCombox( string cboName )
        {
            return (PatientAccessComboBox)combos[cboName];
        }
        #endregion

        #region Public Properties
        public ArrayList DataSource
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                LoadCombosData();
            }
        }

        public ArrayList OCCManualCodes
        {
            private get
            {
                return i_OCCManualCodes;
            }
            set
            {
                i_OCCManualCodes = value;
            }                
        }

        public ArrayList ConditionManualCodes
        {
            private get
            {
                return i_ConditionManualCodes;
            }
            set
            {
                i_ConditionManualCodes = value;
            }                
        }
        #endregion

        #region Private methods
//        private void ResetForBlankSelection( )
        private void ResetAllSelection( )
        {
            Hashtable tempHolder = new Hashtable();

            foreach( PatientAccessComboBox combo in combos.Values )
            {
                if( combo.SelectedIndex > 0 )
                {
                    tempHolder.Add( combo.Name, combo.SelectedItem );
                }
            }

            LoadCombosData( this.currentCombo );

            foreach( object key in tempHolder.Keys )
            {
//                ComboValueSet( (ComboBox)combos[key], tempHolder[key] );                
//                ((ComboBox)combos[key]).SelectedItem = tempHolder[key];
                PatientAccessComboBox combo = ((PatientAccessComboBox)combos[key]);
                Object comboValue = tempHolder[key];
                
                this.ReplaceItem( combo, comboValue );
                 
                RemoveValueFromCombos(tempHolder[key], (PatientAccessComboBox) combos[key]);
                 
            }
        }

        private void RemoveValueFromCombos(object comboValue, PatientAccessComboBox excludeCombo)
        {
            Hashtable tempHolder = new Hashtable();

            foreach (var combo in combos.Values.Cast<PatientAccessComboBox>().Where(combo => combo.SelectedIndex > 0))
            {
                tempHolder.Add(combo.Name, combo.SelectedItem);
            }
            if (comboValue.GetType() == typeof (OccurrenceCode))
            {

                if (IsDuplicateOccurenceCodeAllowed(comboValue, tempHolder)) return;
                foreach (PatientAccessComboBox combo in combos.Values)
                {
                    if (excludeCombo != combo && (OccurrenceCode) combo.SelectedItem != (OccurrenceCode) comboValue)
                    {
                        var i = combo.FindString(comboValue.ToString());
                        if (i > 0)
                        {
                            combo.Items.RemoveAt(i);
                        }
                    }
                }
            }
            else if (comboValue.GetType() == typeof (ConditionCode))
            {
                    foreach (PatientAccessComboBox combo in combos.Values)
                    {

                        if (excludeCombo != combo)
                        {
                            var i = combo.FindString(comboValue.ToString());
                            if (i > 0)
                            {
                                combo.Items.RemoveAt(i);
                            }
                        }
                    }
                    
            }
        }

        private static bool IsDuplicateOccurenceCodeAllowed(object comboValue, Hashtable occKeys)
        {
            OccurrenceCode occCode = (OccurrenceCode)comboValue;
            if (occCode == null || !occCode.IsOccurenceCode50())
            {
                return false;
            }
           
            IList occCodesCollection = new ArrayList();
            foreach (var occKey in occKeys.Keys) occCodesCollection.Add((OccurrenceCode)occKeys[occKey]);
            IEnumerable<OccurrenceCode> occCodesSelectedList = occCodesCollection.OfType<OccurrenceCode>().ToList();
            var occCode50Count = 0;
            if (comboValue.GetType() == typeof (OccurrenceCode))
            {
                    occCode50Count +=
                        occCodesSelectedList.Count(
                            occselected =>
                            occselected != null && occselected.IsOccurenceCode50());

                    if (occCode50Count < 2)
                    {
                        return true;
                    }
                }
            
            return false;
        }

        private void LoadCombosData( PatientAccessComboBox currentCombo )
        {
            foreach( PatientAccessComboBox combo in combos.Values )
            {
                if( combo.Name != currentCombo.Name )
                {
                    combo.Items.Clear();
//                    combo.Items.Add( String.Empty );
                    foreach( object row in data )
                    {
                        combo.Items.Add( row );
                    }
                    combo.SelectedIndex = 0;
                    combo.Refresh();
                }
            }            
        }

        private void LoadCombosData()
        {
            //ArrayList arCombos = new ArrayList( combos.Values );
            foreach( PatientAccessComboBox combo in combos.Values )
            {
                combo.Items.Clear();
//                combo.Items.Add( String.Empty );
                foreach( object row in data )
                {
                    combo.Items.Add( row );
                }
                //                combo.SelectedIndex = -1;
                combo.SelectedIndex = 0;
            }            
        }

        private bool IsManualCode( object comboValue )
        {
            if( comboValue as OccurrenceCode != null )
            {
                return IsOCCManualCode( (OccurrenceCode)comboValue );
            }
            else
            {
                return IsConditionManualCode( (ConditionCode)comboValue );
            }            
        }

        private bool IsOCCManualCode( OccurrenceCode comboValue )
        {
            foreach( OccurrenceCode occ in this.OCCManualCodes )
            {
                if( occ.Code == comboValue.Code )
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsConditionManualCode( ConditionCode comboValue )
        {
            foreach( ConditionCode cond in this.ConditionManualCodes )
            {
                if( cond.Code == comboValue.Code )
                {
                    return true;
                }
            }
            return false;
        }

        private void ReplaceItem( PatientAccessComboBox combo, object comboValue )
        {                    
            combo.SelectedIndex = combo.FindString( comboValue.ToString() );
            int i = combo.SelectedIndex;

            if( i > -1 )
            {
//                combo.Items.RemoveAt( i );
//                combo.Items.Insert( i, comboValue );
                combo.Items[i] = comboValue;
            }
            
            combo.SelectedItem = comboValue;            
        }
        #endregion
        
        #region Data Elements
        private Hashtable combos = new Hashtable();
        private ArrayList data = new ArrayList();

        private ArrayList i_OCCManualCodes = new ArrayList();
        private ArrayList i_ConditionManualCodes = new ArrayList();
        public  PatientAccessComboBox currentCombo = new PatientAccessComboBox();

        #endregion
    }
}
