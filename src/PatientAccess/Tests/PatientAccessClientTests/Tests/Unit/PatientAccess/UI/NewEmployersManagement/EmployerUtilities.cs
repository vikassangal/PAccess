using System;
using System.Collections.Generic;
using System.Linq;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.NewEmployersManagement;

namespace Tests.Unit.PatientAccess.UI.NewEmployersManagement
{
    public static class EmployerUtilities
    {
        public static void SetEmployerAddressLine1To(Employer employer, String street)
        {
            ContactPoint employerContactPoint = EmployerHelper.GetFirstContactPointFor(employer);
            Address address = employerContactPoint.Address;
            address.Address1 = street;
        }


        public static void SetEmployerAddressLine2To(Employer employer, String street)
        {
            ContactPoint employerContactPoint = EmployerHelper.GetFirstContactPointFor(employer);
            Address address = employerContactPoint.Address;
            address.Address2 = street;
        }


        public static void SetEmployerAddressCityTo(Employer employer, string city)
        {
            ContactPoint employerContactPoint = EmployerHelper.GetFirstContactPointFor(employer);
            Address address = employerContactPoint.Address;
            address.City = city;
        }


        public static ICollection<ContactPoint> GetContactPointsFrom(Party employer)
        {
            return employer.ContactPoints.OfType<ContactPoint>().ToList();
        }


        public static void SetAllPhoneNumbersTo(Party employer, string phoneNumber)
        {
            var contactPoints = GetContactPointsFrom(employer);

            contactPoints.ToList().ForEach(x => x.PhoneNumber = new PhoneNumber(phoneNumber));
        }


        public static IList<Address> GetAddressesFrom(Party employer)
        {
            return employer.ContactPoints.OfType<ContactPoint>().Select(x => x.Address).ToList();
        }


        public static Address GetAddressOfFirstContactPointFrom(NewEmployerEntry employerEntry)
        {
            return GetAddressOfFirstContactPointFrom(employerEntry.Employer);
        }


        public static Address GetAddressOfFirstContactPointFrom(Party party)
        {
            return party.ContactPoints.OfType<ContactPoint>().ElementAt(0).Address;
        }


        public static ContactPoint GetFirstContactPointFrom(Party party)
        {
            return party.ContactPoints.OfType<ContactPoint>().ElementAt(0);
        }


        public static Employer GetEmployerWithFullAddress()
        {
            var employer = new Employer(01, DateTime.Now, "Some Company", "ABC123XYZ", 1);

            employer.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "Some street address",
                        "Suite 1",
                        "Some City",
                        new ZipCode("99999"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@somecompany.com"),
                    TypeOfContactPoint.NewEmployerContactPointType()));

            return employer;
        }
    }
}