using System;
using System.Data;

namespace Extensions.DB2Persistence
{
    /// <summary>
    /// SafeReader is a Decorator for IDataReader implementations.
    /// This class provides methods for safely accessing values from
    /// the underlying IDataReader without throwing exceptions.  For example,
    /// accessing a Guid field that has a value of DBNull will return
    /// an instance of Guid with 0s for the byte values instead of throwing 
    /// an exception.
    /// 
    /// This class also provides type safe access methods for fields based
    /// on the name as well as the ordinal value.
    /// </summary>
    [Serializable]
    public class SafeReader : IDataReader
    {
        #region Constants

        private const int INITIAL_POSITION = 0,
                         VERSION_LENGTH = 8;
        #endregion

        #region Implementation of IDataReader
        /// <summary>
        /// Move the underlying reader to the next result set if one exists.
        /// </summary>
        /// <returns>
        /// True if there is another result set to view.
        /// </returns>
        public bool NextResult()
        {
            return this.Data.NextResult();
        }

        /// <summary>
        /// Close the underlying reader and database connection.
        /// </summary>
        public void Close()
        {
            this.Data.Close();
        }

        /// <summary>
        /// Read the next record from the record set.
        /// </summary>
        /// <returns>
        /// True if another record can be read.
        /// </returns>
        public bool Read()
        {
            return this.Data.Read();
        }

        /// <summary>
        /// Return the underlying schema for the source reader.
        /// </summary>
        /// <returns>
        /// An instance of DataTable with the schema information.
        /// </returns>
        public DataTable GetSchemaTable()
        {
            return this.Data.GetSchemaTable();
        }

        /// <summary>
        /// Get the level of nesting for the current row.
        /// </summary>
        public int Depth
        {
            get
            {
                return this.Data.Depth;
            }
        }

        /// <summary>
        /// Gets a value that determines if the underlying reader is closed.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return this.Data.IsClosed;
            }
        }

        /// <summary>
        /// Gets the number of rows affected by the execution.
        /// </summary>
        public int RecordsAffected
        {
            get
            {
                return this.Data.RecordsAffected;
            }
        }
        #endregion

        #region Implementation of IDisposable
        /// <summary>
        /// Performs reader specific clean up of resources.
        /// </summary>
        public void Dispose()
        {
            this.Data.Dispose();
        }
        #endregion

        #region Implementation of IDataRecord
        /// <summary>
        /// Answer the number of columns in the current row.
        /// </summary>
        public int FieldCount
        {
            get
            {
                return this.Data.FieldCount;
            }
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a boolean.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Boolean field value.
        /// </returns>
        public bool GetBoolean( int fieldIndex )
        {
            bool data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = true;
                }
                else
                {
                    data = this.Data.GetBoolean( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = true;
            }
            return data;
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a byte.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Byte field value.
        /// </returns>
        public byte GetByte( int fieldIndex )
        {
            byte data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = 0;
                }
                else
                {
                    data = this.Data.GetByte( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = 0;
            }
            return data;
        }

        /// <summary>
        /// Read a stream of bytes from the specified column offset into an array.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <param name="fieldOffset">
        /// Starting position within the column.
        /// </param>
        /// <param name="buffer">
        /// Array for output
        /// </param>
        /// <param name="bufferOffset">
        /// Starting position within the buffer.
        /// </param>
        /// <param name="length">
        /// Number of bytes to read.
        /// </param>
        /// <returns>
        /// Number of bytes read into the array.
        /// </returns>
        public long GetBytes( int fieldIndex, long fieldOffset, byte[] buffer, int bufferOffset, int length )
        {
            return this.Data.GetBytes( fieldIndex, fieldOffset, buffer, bufferOffset, length );
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a char.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Char field value.
        /// </returns>
        public char GetChar( int fieldIndex )
        {
            char data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = (char)0;
                }
                else
                {
                    data = this.Data.GetChar( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = (char)0;
            }
            return data;
        }

        /// <summary>
        /// Read a stream of characters from the specified column offset into an array.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <param name="fieldOffset">
        /// Starting position within the column.
        /// </param>
        /// <param name="buffer">
        /// Array for output
        /// </param>
        /// <param name="bufferoffset">
        /// Starting position within the buffer.
        /// </param>
        /// <param name="length">
        /// Number of bytes to read.
        /// </param>
        /// <returns>
        /// Number of bytes read into the array.
        /// </returns>
        public long GetChars( int fieldIndex, long fieldOffset, char[] buffer, int bufferoffset, int length )
        {
            return this.Data.GetChars( fieldIndex, fieldOffset, buffer, bufferoffset, length );
        }

        /// <summary>
        /// Return a IDataReader that refers to the nested records located within 
        /// the specified ordinal.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// IDataReader positioned on the nested result set.
        /// </returns>
        public IDataReader GetData( int fieldIndex )
        {
            return this.Data.GetData( fieldIndex );
        }

        /// <summary>
        /// Answer the column data type name of the specified ordinal.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Data type name of the specified ordinal.
        /// </returns>
        public string GetDataTypeName( int fieldIndex )
        {
            return this.Data.GetDataTypeName( fieldIndex );
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a DateTime
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// DateTime field value.
        /// </returns>
        public DateTime GetDateTime( int fieldIndex )
        {
            DateTime data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = new DateTime();
                }
                else
                {
                    data = this.Data.GetDateTime( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = new DateTime();
            }
            return data;
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a decimal
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Decimal field value.
        /// </returns>
        public decimal GetDecimal( int fieldIndex )
        {
            decimal data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = 0;
                }
                else
                {
                    data = this.Data.GetDecimal( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = 0;
            }
            return data;
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a double.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Double field value.
        /// </returns>
        public double GetDouble( int fieldIndex )
        {
            double data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = 0;
                }
                else
                {
                    data = this.Data.GetDouble( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = 0;
            }
            return data;
        }
        /// <summary>
        /// Answer the column Type of the specified ordinal as a System.Type
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// System.Type of the field.
        /// </returns>
        public Type GetFieldType( int fieldIndex )
        {
            return this.Data.GetFieldType( fieldIndex );
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a float.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Float field value.
        /// </returns>
        public float GetFloat( int fieldIndex )
        {
            float data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = 0;
                }
                else
                {
                    data = this.Data.GetFloat( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = 0;
            }
            return data;
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a System.Guid.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// System.Guid field value.
        /// </returns>
        public Guid GetGuid( int fieldIndex )
        {
            Guid data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = new Guid( 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
                }
                else
                {
                    data = this.Data.GetGuid( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = new Guid( 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
            }
            return data;
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a short(Int16).
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Short field value.
        /// </returns>
        public short GetInt16( int fieldIndex )
        {
            short data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = 0;
                }
                else
                {
                    data = this.Data.GetInt16( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = 0;
            }
            return data;
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as an int(Int32).
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Int32 field value.
        /// </returns>
        public int GetInt32( int fieldIndex )
        {
            int data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = 0;
                }
                else
                {
                    data = this.Data.GetInt32( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = 0;
            }
            return data;
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a long(Int64).
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Long field value.
        /// </returns>
        public long GetInt64( int fieldIndex )
        {
            long data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = 0;
                }
                else
                {
                    data = this.Data.GetInt64( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = 0;
            }
            return data;
        }

        /// <summary>
        /// Answer the column name of the specified ordinal.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Field name.
        /// </returns>
        public string GetName( int fieldIndex )
        {
            string data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = String.Empty;
                }
                else
                {
                    data = this.Data.GetName( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = String.Empty;
            }
            return data;
        }

        /// <summary>
        /// Answer the ordinal for the specified column name.
        /// </summary>
        /// <param name="fieldName">
        /// Name of the desired column index.
        /// </param>
        /// <returns>
        /// Ordinal for the specified column name.
        /// </returns>
        public int GetOrdinal( string fieldName )
        {
            return this.Data.GetOrdinal( fieldName );
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as a string.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// String field value.
        /// </returns>
        public string GetString( int fieldIndex )
        {
            string data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = String.Empty;
                }
                else
                {
                    data = this.Data.GetString( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = String.Empty;
            }
            return data;
        }

        /// <summary>
        /// Answer the column value of the specified ordinal as an object.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// Object field value.
        /// </returns>
        public object GetValue( int fieldIndex )
        {
            object data;
            try
            {
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = new Object();
                }
                else
                {
                    data = this.Data.GetValue( fieldIndex );
                }
            }
            catch( Exception )
            {
                data = new Object();
            }
            return data;
        }

        /// <summary>
        /// Get all the attribute fields in the collection for the current row.
        /// </summary>
        /// <param name="values">
        /// Object array to store the values in the row.
        /// </param>
        /// <returns>
        /// An object array with row values.
        /// </returns>
        public int GetValues( object[] values )
        {
            return this.Data.GetValues( values );
        }

        /// <summary>
        /// Determine if the value at the specified ordinal is NULL in the 
        /// underlying data source.
        /// </summary>
        /// <param name="fieldIndex">
        /// Column position of the desired value.
        /// </param>
        /// <returns>
        /// True if the value in the specified ordinal is NULL.
        /// </returns>
        public bool IsDBNull( int fieldIndex )
        {
            return this.Data.IsDBNull( fieldIndex );
        }

        /// <summary>
        /// Get the value of the specified field name.  If the value is
        /// null, return a new instance of Object.
        /// </summary>
        public object this[string fieldName]
        {
            get
            {
                return this[this.GetOrdinal( fieldName )];
            }
        }

        /// <summary>
        /// Get the value of the specified ordinal.  If the value is null, return a
        /// new instance of Object.
        /// </summary>
        public object this[int fieldIndex]
        {
            get
            {
                object data;
                if( this.Data.IsDBNull( fieldIndex ) )
                {
                    data = new Object();
                }
                else
                {
                    data = this.Data[fieldIndex];
                }
                return data;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Answer the column value of the specified column name as a boolean.
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// Boolean field value.
        /// </returns>
        public bool GetBoolean( string fieldName )
        {
            return this.GetBoolean( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as a byte.
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// Byte field value.
        /// </returns>
        public byte GetByte( string fieldName )
        {
            return this.GetByte( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Read a stream of bytes from the specified column offset into an array.
        /// </summary>
        /// <param name="fieldName">
        /// Column name of the desired value.
        /// </param>
        /// <param name="fieldOffset">
        /// Starting position within the column.
        /// </param>
        /// <param name="buffer">
        /// Array for output
        /// </param>
        /// <param name="bufferOffset">
        /// Starting position within the buffer.
        /// </param>
        /// <param name="length">
        /// Number of bytes to read.
        /// </param>
        /// <returns>
        /// Number of bytes read into the array.
        /// </returns>
        private long GetBytes( string fieldName, long fieldOffset, byte[] buffer, int bufferOffset, int length )
        {
            return this.Data.GetBytes( this.GetOrdinal( fieldName ), fieldOffset, buffer, bufferOffset, length );
        }

        /// <summary>
        /// Answer the column value of the specified column name as a char.
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// Char field value.
        /// </returns>
        public char GetChar( string fieldName )
        {
            return this.GetChar( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Read a stream of characters from the specified column offset into an array.
        /// </summary>
        /// <param name="fieldName">
        /// Column name of the desired value.
        /// </param>
        /// <param name="fieldOffset">
        /// Starting position within the column.
        /// </param>
        /// <param name="buffer">
        /// Array for output
        /// </param>
        /// <param name="bufferOffset">
        /// Starting position within the buffer.
        /// </param>
        /// <param name="length">
        /// Number of bytes to read.
        /// </param>
        /// <returns>
        /// Number of bytes read into the array.
        /// </returns>
        public long GetChars( string fieldName, long fieldOffset, char[] buffer, int bufferOffset, int length )
        {
            return this.Data.GetChars( this.GetOrdinal( fieldName ), fieldOffset, buffer, bufferOffset, length );
        }

        /// <summary>
        /// Return a IDataReader that refers to the nested records located within 
        /// the specified ordinal.
        /// </summary>
        /// <param name="fieldName">
        /// Column name of the desired value.
        /// </param>
        /// <returns>
        /// IDataReader positioned on the nested result set.
        /// </returns>
        public IDataReader GetData( string fieldName )
        {
            return this.GetData( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as a DateTime.
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// DateTime field value.
        /// </returns>
        public DateTime GetDateTime( string fieldName )
        {
            return this.GetDateTime( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as a decimal.
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// Decimal field value.
        /// </returns>
        public decimal GetDecimal( string fieldName )
        {
            return this.GetDecimal( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as a double.
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// Double field value.
        /// </returns>
        public double GetDouble( string fieldName )
        {
            return this.GetDouble( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as a float.
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// Float field value.
        /// </returns>
        public float GetFloat( string fieldName )
        {
            return this.GetFloat( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as a System.Guid.
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// System.Guid field value.
        /// </returns>
        public Guid GetGuid( string fieldName )
        {
            return this.GetGuid( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as a short(Int16).
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// Short field value.
        /// </returns>
        public short GetInt16( string fieldName )
        {
            return this.GetInt16( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as an int(Int32).
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// Int field value.
        /// </returns>
        public int GetInt32( string fieldName )
        {
            return this.GetInt32( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as a long(Int64).
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// Long field value.
        /// </returns>
        public long GetInt64( string fieldName )
        {
            return this.GetInt64( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column name of the specified ordinal.
        /// </summary>
        /// <param name="fieldName">
        /// Column name of the desired value.
        /// </param>
        /// <returns>
        /// Field name.
        /// </returns>
        public string GetName( string fieldName )
        {
            return this.GetName( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as a byte array.
        /// Used if a column represents a timestamp or rowversion type.
        /// </summary>
        /// <returns>Timestamp value</returns>
        public byte[] GetRowVersion( string fieldName )
        {
            byte[] verstionstamp = new byte[VERSION_LENGTH];
            try
            {
                this.GetBytes( fieldName,
                               INITIAL_POSITION,
                               verstionstamp,
                               INITIAL_POSITION,
                               VERSION_LENGTH );
            }
            catch( Exception )
            {
                verstionstamp = new byte[VERSION_LENGTH];
            }
            return verstionstamp;
        }

        /// <summary>
        /// Answer the column value of the specified column name as a string.
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// String field value.
        /// </returns>
        public string GetString( string fieldName )
        {
            return this.GetString( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Answer the column value of the specified column name as an object.
        /// </summary>
        /// <param name="fieldName">
        /// Column named of the desired value.
        /// </param>
        /// <returns>
        /// Object field value.
        /// </returns>
        public object GetValue( string fieldName )
        {
            return this.GetValue( this.GetOrdinal( fieldName ) );
        }

        /// <summary>
        /// Determine if the value at the specified ordinal is NULL in the 
        /// underlying data source.
        /// </summary>
        /// <param name="fieldName">
        /// Column name of the desired value.
        /// </param>
        /// <returns>
        /// True if the value in the specified ordinal is NULL.
        /// </returns>
        public bool IsDBNull( string fieldName )
        {
            return this.Data.IsDBNull( this.GetOrdinal( fieldName ) );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        /// <summary>
        /// The underlying IDataReader data source for this decorator.
        /// </summary>
        private IDataReader Data
        {
            get
            {
                return i_Data;
            }
            set
            {
                i_Data = value;
            }
        }
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Construct a SafeReader decorator on the given IDataReader.
        /// </summary>
        /// <param name="dataReader">
        /// An instance that supports the IDataReader interface.
        /// </param>
        public SafeReader( IDataReader dataReader )
        {
            this.Data = dataReader;
        }
        #endregion

        #region Data Elements
        private IDataReader i_Data;
        #endregion
    }
}
