namespace Peradigm.Framework.Domain.Parties
{
    public interface IContactPoint
    {
        bool Validate();
        void Parse( string value );
        string AsString();
        bool Equals( IContactPoint contactPoint );
        int GetHashCode();
    }
}
