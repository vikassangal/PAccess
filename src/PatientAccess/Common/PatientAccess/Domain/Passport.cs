using System;
using Extensions;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Passport : Model
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Passport pp = obj as Passport;
            return (pp.Number == this.Number
                && (pp.Country == null && this.Country == null
                || (pp.Country != null
                        && this.Country != null
                        && pp.Country.Code == this.Country.Code)));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties
        public string Number
        {
            get
            {
                return i_Number;
            }
            set
            {
                i_Number = value;
            }
        }

        public Country Country
        {
            get
            {
                return i_Country;
            }
            set
            {
                i_Country = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Passport(string number, Country country)
        {
            this.Number = number;
            if (country != null)
            {
                this.Country = country;
            }
        }

        public Passport(string number)
        {
            this.Number = number;
        }

        public Passport()
        {

        }

        #endregion

        #region Data Elements
        private string i_Number = String.Empty;
        private Country i_Country = new Country();
        #endregion

        #region Constants
        #endregion
    }
}
