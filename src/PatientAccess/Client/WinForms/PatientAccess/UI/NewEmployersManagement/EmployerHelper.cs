using System;
using System.Collections;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.NewEmployersManagement
{
    static class EmployerHelper
    {
        public static ContactPoint GetFirstContactPointFor(Employer employer)
        {
            IEnumerator enumerator = employer.ContactPoints.GetEnumerator();
            enumerator.MoveNext();
            ContactPoint contactPoint = (ContactPoint)enumerator.Current;
            enumerator.Reset();

            return contactPoint;
        }

        public static bool EmployerAddressHasStreetAndCity(Employer employer)
        {
            bool hasAddress = false;
            bool hasCityAndStreet = false;
            bool hasContactPoints = (employer.ContactPoints.Count != 0);

            if (hasContactPoints)
            {
                ContactPoint employerContactPoint = GetFirstContactPointFor(employer);

                hasAddress = employerContactPoint.Address != null;
                if (hasAddress)
                {
                    Address address = employerContactPoint.Address;
                    hasCityAndStreet = AddressHasCityAndStreet(address);
                }
            }

            return (hasContactPoints && hasAddress && hasCityAndStreet);
        }


        public static bool AddressHasCityAndStreet(Address address)
        {
            if (address == null) return false;

            string city = address.City;

            bool hasCity = !IsNullOrEmptyAfterTrimming(city);

            bool hasStreet = !IsNullOrEmptyAfterTrimming(address.Address1) ||
                             !IsNullOrEmptyAfterTrimming(address.Address2);

            bool hasCityAndStreet = hasCity && hasStreet;

            return hasCityAndStreet;
        }


        private static bool IsNullOrEmptyAfterTrimming(string @string)
        {
            if (@string != null)
            {
                @string.Trim();
            }

            bool hasCity = String.IsNullOrEmpty(@string);
            return hasCity;
        }


        public static string GetFirstAddressFor(NewEmployerEntry employer)
        {
            string address = String.Empty;
            if (EmployerAddressHasStreetAndCity(employer.Employer))
            {
                ContactPoint employerContactPoint = GetFirstContactPointFor(employer.Employer);

                address = employerContactPoint.Address.AsMailingLabel() + Environment.NewLine +
                          employerContactPoint.PhoneNumber.AsFormattedString();
            }
            return address;
        }


        public static bool EmployerHasOneOrMoreAddresses(Employer employer)
        {
            bool hasContactPoints = (employer.ContactPoints.Count != 0);
            bool hasOneNonBlankAddress = false;

            foreach (ContactPoint contactPoint in employer.ContactPoints)
            {
                Address address = contactPoint.Address;
                if (AddressHasCityAndStreet(address))
                {
                    hasOneNonBlankAddress = true;
                    break;
                }
            }
            return hasContactPoints && hasOneNonBlankAddress;
        }
    }
}
