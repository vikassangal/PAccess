using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 
/// </summary>
public delegate void Changed( object sender, string aspect, object oldValue, object newValue );

namespace Extensions
{
    /// <summary>
    /// Base class for all classes that require event publication.  Derived classes
    /// can register listeners.  Listeners will respond to the Change notification
    /// provided.  To notify listeners, raise the Changed event from within a
    /// property Set or any other method where properties of the object may have
    /// changed values.
    /// </summary>
    [Serializable]
    public class Model : object
    {

        #region Fields 

        [NonSerialized]
        private static bool c_IsTrackingEnabled = true;
        private Changed i_ChangedEvent;
        private HybridDictionary i_ChangeTracker;
        private HybridDictionary i_SecondaryProperties;

        #endregion Fields 

        #region Constructors 

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Model()
        {
        }

        #endregion Constructors 

        #region Properties 

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified property key.
        /// </summary>
        /// <value></value>
        public object this[object propertyKey]
        {
            get
            {

                return this.SecondaryProperties[propertyKey];

            }
            set
            {

                this.SecondaryProperties[propertyKey] = value;

            }
        }

        /// <summary>
        /// Provides another method for creating a string representation of an object.
        /// Useful if the derived class has already implemented the ToString().
        /// This method should be used for debugging purposes.
        /// </summary>
        public virtual string DisplayString
        {
            get
            {
                return this.PrintString;
            }
        }

        public static bool IsTrackingEnabled
        {
            get
            {
                return c_IsTrackingEnabled;
            }
            set
            {
                c_IsTrackingEnabled = value;
            }
        }

        /// <summary>
        /// Provides another method for creating a string representation of an object.
        /// Useful if the derived class has already implemented the ToString().
        /// This method should be used as an alternate form of visual display.
        /// </summary>
        public virtual string PrintString
        {
            get
            {
                return this.ToString();
            }
        }

        /// <summary>
        /// Read only property that returns the ChangedEvent delegate.
        /// </summary>
        private Changed ChangedEvent
        {
            get
            {
                return i_ChangedEvent;
            }
        }

        private HybridDictionary ChangeTracker
        {
            get
            {
                // not thread safe
                if(this.i_ChangeTracker == null)
                {
                    this.i_ChangeTracker = new HybridDictionary();
                }
                return this.i_ChangeTracker;
            }
        }

        private HybridDictionary SecondaryProperties
        {
            get
            {

                if( this.i_SecondaryProperties == null )
                {

                    this.i_SecondaryProperties = new HybridDictionary();

                }//if

                return this.i_SecondaryProperties;

            }
        }

        #endregion Properties 

        #region Delegates and Events 

        /// <summary>
        /// Property that allows consumers of the class to add or remove 
        /// listeners to the Changed event.
        /// </summary>
        public event Changed ChangedListeners
        {
            add
            {
                this.i_ChangedEvent += value;
            }
            remove
            {
                this.i_ChangedEvent -= value;
            }
        }

        #endregion Delegates and Events 

        #region Methods 

        /// <summary>
        /// Determines whether [has extended property] [the specified property name].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// 	<c>true</c> if [has extended property] [the specified property name]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasExtendedProperty( string propertyName )
        {
            return this.SecondaryProperties.Contains( propertyName );
        }

        /// <summary>
        /// Provides a default shallow copy implementation.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Provides a deep copy implementation.  This method returns a deep
        /// copy of an entire object graph, event if the other objects within
        /// the graph do not implement the cloning methods.
        /// </summary>
        /// <returns>
        /// An object which represents a deep copy of the object graph.  The caller
        /// must cast this into a concrete type.
        /// </returns>
        public virtual object DeepCopy()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize( stream, this );
            stream.Seek( 0, SeekOrigin.Begin );
            object clone = formatter.Deserialize( stream );
            stream.Close();
            return clone;
        }

        public void ResetChangeTracking()
        {

            this.ChangeTracker.Clear();

        }

        /// <summary>
        /// Forces the changed status for the property regardless of tracking status.
        /// Useful to flage things that need to be seen as changed after a binary deep copy
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void ForceChangedStatusFor(string propertyName)
        {

            this.ChangeTracker[propertyName] = Guid.NewGuid().ToString();

        }

        /// <summary>
        /// Forces status for a property to be unchanged
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void ForceUnChangedStatusFor( string propertyName )
        {

            if( this.ChangeTracker.Contains( propertyName ) )
            {

                this.ChangeTracker.Remove( propertyName );

            }//if

        }

        protected T SetAndTrack<T>( ref T target, T newValue, MethodBase methodInfo )
        {

            // Get the name of the calling method from the first frame of the stack trace
            string propertyName = methodInfo.Name ?? String.Empty;
                        
            // Strip off the "set_" if this is a property setter
            if( propertyName.StartsWith("set_",StringComparison.InvariantCultureIgnoreCase) )
            {

                propertyName = propertyName.Substring(4);

            }//if

            return this.SetAndTrack<T>( ref target, newValue, propertyName );

        }

        private T SetAndTrack<T>(ref T target, T newValue, string propertyName)
        {

            // Go ahead and set the value first
            T oldValue = target;
            target = newValue;

            // Value types are never null, so we can be a little more direct here
            if( typeof( T ).IsValueType )
            {

                if( !target.Equals( oldValue ) )
                {

                    TrackChange( propertyName, oldValue, target );

                }//if

            }//if
                // Reference types take a lot more work to avoid throwing an NRE
            else
            {
                
                if( (( target == null || oldValue == null ) && !ReferenceEquals(target,oldValue) )  ||
                    ( target != null && !target.Equals( oldValue ) ) )
                {

                    TrackChange( propertyName, oldValue, target );

                }//if

            }//else

            return target;

        }
    
        public bool HasChangedFor(string propertyName)
        {

            return this.ChangeTracker.Contains( propertyName );

        }

        protected void RaiseChainedChangedEvent( object sender, string aspect, object oldValue, object newValue )
        {

            Stack senderStack = sender as Stack;

            if( senderStack == null )
            {
                senderStack = new Stack();
                senderStack.Push( sender );
            }

            senderStack.Push( this );

            this.RaiseChangedEvent( senderStack, aspect, oldValue, newValue );

        }

        /// <summary>
        /// Raises the changed event.
        /// </summary>
        /// <param name="aspect">The aspect.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected void RaiseChangedEvent( string aspect, object oldValue, object newValue )
        {
            this.RaiseChangedEvent( null, aspect, oldValue, newValue );
        }

        /// <summary>
        /// Raise a change event for all listeners
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="aspect">The aspect.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected void RaiseChangedEvent( object sender, string aspect, object oldValue, object newValue )
        {
            if( this.ChangedEvent != null )
            {
                this.ChangedEvent( sender ?? this, aspect, oldValue, newValue );
            }
        }

        protected void TrackChange(string propertyName,object oldValue,object newValue)
        {
            if( !IsTrackingEnabled )
            {
                return;
            }

            if( this.ChangeTracker.Contains( propertyName ) )
            {

                if( ( this.ChangeTracker[propertyName] == null && newValue == null ) ||
                    ( this.ChangeTracker[propertyName] != null && this.ChangeTracker[propertyName].Equals( newValue ) ) )
                {

                    this.ChangeTracker.Remove( propertyName );

                } //if

            } //if
            else
            {

                this.ChangeTracker[propertyName] = oldValue;

            } //else

        }

        #endregion Methods 

    }
}