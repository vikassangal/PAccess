//-----------------------------------------------------------------------------
// Copyright © 2003-2005 Perot Systems Coproration. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Peradigm.Framework.Domain
{
	/// <summary>
	/// Delegate to raise change events
	/// </summary>
	public delegate void Changed( string Aspect, object OldValue, object NewValue );
	
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
        #region Methods
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
        #endregion

        #region Private Methods
		/// <summary>
		/// Raise a change event for all listeners
		/// </summary>
		/// <param name="aspect">Aspect that is changing</param>
		/// <param name="oldValue">Value before change occurred</param>
		/// <param name="newValue">Current Value</param>
        protected void RaiseChangedEvent( string aspect, object oldValue, object newValue )
        {
            if( this.ChangedEvent != null )
            {
                this.ChangedEvent( aspect, oldValue, newValue );
            }
        }
        #endregion

        #region Properties
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
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Default constructor.
        /// </summary>
		public Model()
		{
		}
        #endregion

        #region Data Elements
        private Changed i_ChangedEvent;
        #endregion
	}
}
