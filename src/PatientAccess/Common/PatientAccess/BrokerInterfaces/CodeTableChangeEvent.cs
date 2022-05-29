using System;
using System.Collections;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// 1. This class is a template for event object sent by CIE.
	/// 2. Event represents updated PBAR table row.
	/// 3. It holds 
	/// 3.1 EventType   - PBAR table name.
	/// 3.2 Action      - Action performed on PBAR table (INSERT/UPDATE/DELETE)
	/// 3.3 Row         - Column and value pairs of modified PBAR table row.
	/// 3.4 DateTime    - Date time of event generation.
	/// </summary>
	[Serializable]
	public class CodeTableChangeEvent
	{
        #region Properties

        /// <summary>
        /// Gets / Sets EventType (PBAR table name)
        /// </summary>
        public string EventType
        {
            get
            {
                return i_EventType;
            }
            set
            {
                i_EventType = value;
            }
        }

        /// <summary>
        /// Gets / Sets Action performed on PBAR table -INSERT/UPDATE/DELETE)
        /// </summary>
        public string Action
        {
            get
            {
                return i_Action;
            }
            set
            {
                i_Action = value;
            }
        }

        /// <summary>
        /// Gets / Sets Row i.e Column and Value pairs of modified PBAR table row
        /// </summary>
        public Hashtable Row
        {
            get
            {
                return i_Row;
            }
            set
            {
                i_Row = value;
            }
        }

        public Hashtable OldRow
        {
            get
            {
                return i_OldRow;
            }
            set
            {
                i_OldRow = value;
            }
        }

            /// <summary>
            /// Gets / Sets Event DateTime (Date time of event generation)
            /// </summary>
            public DateTime EventDateTime
        {
            get
            {
                return i_EventDateTime;
            }
            set
            {
                i_EventDateTime = value;
            }
        }

        #endregion
    
        #region Construction and Finalization

        public CodeTableChangeEvent()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #endregion

        #region Data Elements

        private string                  i_EventType;
        private string                  i_Action;
        private Hashtable               i_Row;
        private Hashtable               i_OldRow = new Hashtable();
        private DateTime                i_EventDateTime;

        #endregion
        
	}
}
