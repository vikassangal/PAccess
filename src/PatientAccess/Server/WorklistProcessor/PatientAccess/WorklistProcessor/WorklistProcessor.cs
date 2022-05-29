using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.WorklistProcessor
{
    /// <summary>
    /// This is the main driver/class/main program for the worklist Processor
    /// </summary>
    class WorklistProcessor
    {

		#region Constants 


		#endregion Constants 

		#region Fields 

        private static ILog c_log = LogManager.GetLogger( typeof( WorklistProcessor ) );
        private static string i_targetFacilityList = string.Empty;

		#endregion Fields 

		#region Properties 

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        /// <value>The facility code.</value>
        [CommandLineSwitch("HSPS","This is the list of facilities to process")]
        public static string FacilityIdList
        {
            get
            {
                return i_targetFacilityList;
            }
            set
            {
                i_targetFacilityList = value;
            }
        }

		#endregion Properties 

		#region Methods 

        static void Main(string[] args)
        {

            Console.WriteLine("Beginning Worklist Processor " + DateTime.Now);

            WorklistProcessor worklistProcessor = new WorklistProcessor();

            if( args.Length >= 1 )
            {
                try
                {
                    Parser parser = new Parser(Environment.CommandLine, worklistProcessor);

                    parser.Parse();

                    worklistProcessor.ProcessHospitals(FacilityIdList);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("An Error was encountered during Processing: {0}",
                        ex.Message );
                }
                finally
                {
                }
            }
            Console.WriteLine("Completed Worklist Processor " + DateTime.Now);
        }

        private void ProcessHospitals(string hosptialList)
        {
            string[] hosptials = hosptialList.Split(',');

            foreach (string hospital in hosptials)
            {
                ProcessWorklistsFor(hospital);
            }
        }

        private void ProcessWorklistsFor(string hospital)
        {

            IFacilityBroker facilitybroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            IWorklistBroker worklistBroker = BrokerFactory.BrokerOfType< IWorklistBroker >();

            if (hospital.ToUpper().Equals("ALL"))
            {
                ArrayList allFacilities = facilitybroker.AllFacilities() as ArrayList;
                foreach (Facility facility in allFacilities)
                {
                    Console.WriteLine("Beginning Processing worklists for " + facility.Code + " " + DateTime.Now);
                    
                    worklistBroker.ProcessAllWorklistsFor(facility);
                    
                    Console.WriteLine("Completed Processing worklists for " + facility.Code + " " + DateTime.Now);
                }
            }
            else
            {
                Int32 hspID = -1;
                Facility facility;

                if (Int32.TryParse(hospital, out hspID))
                {
                    facility = facilitybroker.FacilityWith(hspID);
                }
                else
                {
                    facility = facilitybroker.FacilityWith(hospital);
                }

                if (facility != null)
                {
                    Console.WriteLine("Beginning Processing worklists for " + facility.Code + " " + DateTime.Now);
                    
                    worklistBroker.ProcessAllWorklistsFor(facility);

                    Console.WriteLine("Completed Processing worklists for " + facility.Code + " " + DateTime.Now);

                }
            }
        }

		#endregion Methods 

    }

}
