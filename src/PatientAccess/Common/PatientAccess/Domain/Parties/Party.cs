using System;
using System.Collections;
using System.Reflection;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public abstract class Party : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        public virtual Guarantor CopyAsGuarantor()
        {
            Guarantor result = new Guarantor();
            this.Populate( result );

			if( this.GetType() == typeof(Insured) ) 
			{
				foreach( ContactPoint cp in result.ContactPoints )
				{
					if(cp != null &&  cp.TypeOfContactPoint.Oid == TypeOfContactPoint.NewPhysicalContactPointType().Oid )
					{
						cp.TypeOfContactPoint = TypeOfContactPoint.NewMailingContactPointType();
						break;
					}
				}
			}

            // SR 54716 - County code should be displayed and saved only for Patient
            // Mailing Address, and hence should not be copied forward to Guarantor
            foreach (ContactPoint cp in result.ContactPoints)
            {
                if (cp != null && cp.TypeOfContactPoint.Oid == TypeOfContactPoint.NewMailingContactPointType().Oid)
                {
                    cp.Address.County = null;
                    break;
                }
            }

            return result;
        }

        public virtual Insured CopyAsInsured()
        {
            Insured result = new Insured();
            this.Populate( result );

            if( this.GetType() == typeof(Employer) )
            {
                return result;
            }

            if( this.GetType() != typeof(Insured) 
                && result.primContactPoints != null
                && result.primContactPoints.Count > 0)
            {
                // 06/09/2006 - Insured is to get the Mailing contact point from the patient
                // remove any old physical contact point

                result.RemoveContactPoint( TypeOfContactPoint.NewPhysicalContactPointType() );

                foreach( ContactPoint cp in result.primContactPoints )
                {
                    if( cp.TypeOfContactPoint.Description == TypeOfContactPoint.NewMailingContactPointType().Description )
                    {
                        // SR 54716 - County code should be displayed and saved only for Patient
                        // Mailing Address, and hence should not be copied forward to Guarantor
                        cp.Address.County = null;
                        cp.TypeOfContactPoint = TypeOfContactPoint.NewPhysicalContactPointType();
                        cp.EmailAddress = new EmailAddress();
                        break;
                    }
                }
            }

            return result;
        }

        public virtual Patient CopyAsPatient()
        {
            Patient result = new Patient();
            this.Populate( result );
            return result;
        }

        public virtual void AddContactPoints( IList contactPoints )
        {
            foreach( ContactPoint contactPoint in contactPoints )
            {
                this.AddContactPoint( contactPoint );
            }
        }

        public virtual  void AddContactPoint( ContactPoint aContactPoint )
        {
            if( !this.primContactPoints.Contains( aContactPoint ) )
            {
                this.primContactPoints.Add( aContactPoint );
            }
        }

        public virtual void RemoveContactPoint( ContactPoint aContactPoint )
        {
            if( this.primContactPoints.Contains( aContactPoint ) )
            {
                this.primContactPoints.Remove( aContactPoint );
            }
        }

        public virtual void RemoveContactPoint( TypeOfContactPoint aType )
        {
            foreach( ContactPoint cp in this.primContactPoints  )
            {
                if( cp.TypeOfContactPoint.Description == aType.Description )
                {
                    this.primContactPoints.Remove( cp );
                    break;
                }
            }
        }

        public virtual void RemoveRelationship( RelationshipType aType )
        {
            foreach( Relationship r in this.primRelationships  )
            {
                if( r.Type == aType )
                {
                    this.primRelationships.Remove( r );
                    break;
                }
            }
        }
    

        public Relationship AddRelationship( Relationship relationship )
        {
            if( !this.primRelationships.Contains( relationship ) )
            {
                this.primRelationships.Add( relationship );
            }
            return relationship;
        }

        public ArrayList FindRelationships( RelationshipType type )
        {
            ArrayList relationships = new ArrayList();

            foreach( Relationship r in this.primRelationships )
            {
                if( r.Type.Oid == type.Oid )
                {
                    relationships.Add( r );
                }
            }
            return relationships;
        }

        public ArrayList RelationshipOf( string typeName )
        {
            ArrayList relationships = new ArrayList();
            foreach( Relationship r in this.primRelationships )
            {
                if( r.Type.Description == typeName )
                {
                    relationships.Add( r );
                }
            }
            return relationships;
        }

        public RelationshipType RelationshipWith( Party aParty )
        {
            RelationshipType aRelationshipType = null;
            foreach( Relationship r in this.Relationships)
            {
                if( aParty.GetType() == r.ReciprocatingPartyType )
                {
                    aRelationshipType = r.Type;
                }
            }
            return aRelationshipType;
        }

        public Relationship RemoveRelationship( Relationship relationship )
        {
            if( this.primRelationships.Contains( relationship ) )
            {
                this.primRelationships.Remove( relationship );
            }
            return relationship;
        }

        public virtual ContactPoint ContactPointWith( TypeOfContactPoint aTypeOfContactPoint )
        {
            bool found = false;
            ContactPoint contactPoint = null;

            foreach( ContactPoint contact in this.primContactPoints )
            {
                if( contact != null
                    && contact.TypeOfContactPoint == aTypeOfContactPoint ) 
                {
                    found = true;
                    contactPoint = contact;
                    break;
                }
            }
            if( !found )
            {
                contactPoint = new ContactPoint( aTypeOfContactPoint );
                AddContactPoint( contactPoint );
            }
            return contactPoint;
        }
        #endregion

        #region Properties
    
        public virtual ICollection ContactPoints
        {
            get
            {
                return (ICollection)primContactPoints.Clone();
            }
        }

        public EmailAddress EmailAddress
        {
            get
            {
                return i_EmailAddress;
            }
            set
            {
                i_EmailAddress = value;
            }
        }
        
 
        public ICollection Relationships
        {
            get
            {
                return (ICollection)this.primRelationships.Clone();
            }
        }
        #endregion

        #region Private Methods
        private void Populate( object targetObject )
        {
            ((Party)targetObject).i_Relationships.Clear();
            ((Party)targetObject).i_primContactPoints.Clear();

            Type sourceType  = this.GetType();
            Type targetType  = targetObject.GetType();

            foreach( PropertyInfo targetProperty in targetType.GetProperties( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ) )
            {
                if( targetProperty.CanWrite && targetProperty.GetIndexParameters().Length == 0 )
                {                    
                    PropertyInfo sourceProperty = sourceType.GetProperty( targetProperty.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
                    
                    if( sourceProperty != null )
                    {
                        if( targetProperty.PropertyType != null
                            && (
                                (targetProperty.PropertyType.ToString().Length < 13 
                                ||
                                targetProperty.PropertyType.ToString().Substring(0,13) != PATIENT_ACCESS)
                                && sourceProperty.Name != PRIM_CONTACTS
                                && sourceProperty.Name != PRIM_RELATIONSHIPS)
                          )
                        {
                            object sourceValue = sourceProperty.GetValue( this, null );
                            targetProperty.SetValue( targetObject, sourceValue, null );
                        }
                        else
                        {
                            if( targetProperty.Name == "Name" )
                            {
                                object o = sourceProperty.GetValue(this,null);
                                Name oldValue = (Name)o;
                                Name newValue = (Name)oldValue.Clone();

                                targetProperty.SetValue( targetObject, newValue, null );
                            }
                            else if ( targetProperty.Name == "DriversLicense")
                            {
                                object o = sourceProperty.GetValue(this,null);
                                DriversLicense oldValue = (DriversLicense)o;
                                DriversLicense newValue = (DriversLicense)oldValue.Clone();

                                targetProperty.SetValue( targetObject, newValue, null );
                            }
                            else if (targetProperty.Name == "Passport")
                            {
                                object o = sourceProperty.GetValue(this, null);
                                Passport oldValue = (Passport)o;
                                Passport newValue = (Passport)oldValue.Clone();

                                targetProperty.SetValue(targetObject, newValue, null);
                            }
                            else if ( targetProperty.Name == "Sex")
                            {
                                object o = sourceProperty.GetValue(this,null);
                                Gender oldValue = (Gender)o;
                                Gender newValue = (Gender)oldValue.Clone();

                                targetProperty.SetValue( targetObject, newValue, null );
                            }
                            else if ( targetProperty.Name == "SocialSecurityNumber")
                            {
                                object o = sourceProperty.GetValue(this,null);
                                SocialSecurityNumber oldValue = (SocialSecurityNumber)o;
                                SocialSecurityNumber newValue = (SocialSecurityNumber)oldValue.Clone();

                                targetProperty.SetValue( targetObject, newValue, null );
                            }
                            else if ( targetProperty.Name == "Employment")
                            {
                                object o = sourceProperty.GetValue(this,null);
                                Employment oldValue = (Employment)o;
                                Employment newValue = (Employment)oldValue.Clone();

                                targetProperty.SetValue( targetObject, newValue, null );
                            }
                            else if( targetProperty.Name == "primContactPoints" )
                            {
                                object o = sourceProperty.GetValue(this,null);
                                ArrayList oldCPs = (ArrayList)o;

                                ArrayList newCPs = new ArrayList();                           

                                if( oldCPs != null && oldCPs.Count > 0 )
                                {
									foreach( object obj in oldCPs )
									{
										ContactPoint oldValue = (ContactPoint)obj;
										ContactPoint newValue = (ContactPoint)oldValue.Clone();
										newCPs.Add(newValue);                                
									}
                                }
                            
                                targetProperty.SetValue( targetObject, newCPs, null );
                            }
                        }                                                                            
                    }
                }
            }
        }
        #endregion

        #region Private Properties
     

        protected ArrayList primRelationships
        {
            get
            {
                return i_Relationships;
            }
            set
            {
                i_Relationships = value;
            }
        }

        protected virtual ArrayList primContactPoints
        {
            get
            {
                return i_primContactPoints;
            }
            set
            {
                i_primContactPoints = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public Party()
            : base( NEW_OID, NEW_VERSION )
        {
        }

        public Party( long oid, DateTime version )
            : base( oid, version )
        {
        }
        #endregion

        #region Data Elements
   
        private ArrayList    i_Relationships         = new ArrayList();
        private EmailAddress i_EmailAddress          = new EmailAddress();
        private ArrayList    i_primContactPoints     = new ArrayList();
        #endregion

        #region Constants

        private const string PATIENT_ACCESS     = "PatientAccess",
                             PRIM_CONTACTS      = "primContactPoints",
                             PRIM_RELATIONSHIPS = "primRelationships";

        #endregion
    }
}
