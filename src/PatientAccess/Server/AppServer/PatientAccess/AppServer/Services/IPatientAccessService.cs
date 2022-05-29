using System;
using PatientAccess.HDI;

namespace PatientAccess.Services
{
    public interface IPatientAccessService :IDisposable
    {
        void CancelAsync(object userState);
        ConnectionSpecification getConnectionSpecFor(ConnectionSpecRequest arg_0_0);
        void getConnectionSpecForAsync(ConnectionSpecRequest arg_0_0, object userState);
        void getConnectionSpecForAsync(ConnectionSpecRequest arg_0_0);
        event getConnectionSpecForCompletedEventHandler getConnectionSpecForCompleted;
        FUSNote1[] getFUSNotesFor(ReadFUSNotesRequest arg_0_3);
        void getFUSNotesForAsync(ReadFUSNotesRequest arg_0_3);
        void getFUSNotesForAsync(ReadFUSNotesRequest arg_0_3, object userState);
        event getFUSNotesForCompletedEventHandler getFUSNotesForCompleted;
        bool getPBARAvailabilityFor(PBARAvailabilityRequest arg_0_1);
        void getPBARAvailabilityForAsync(PBARAvailabilityRequest arg_0_1, object userState);
        void getPBARAvailabilityForAsync(PBARAvailabilityRequest arg_0_1);
        event getPBARAvailabilityForCompletedEventHandler getPBARAvailabilityForCompleted;
        bool postFUSNoteFor(PostFUSNoteRequest arg_0_2);
        void postFUSNoteForAsync(PostFUSNoteRequest arg_0_2, object userState);
        void postFUSNoteForAsync(PostFUSNoteRequest arg_0_2);
        event postFUSNoteForCompletedEventHandler postFUSNoteForCompleted;
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
    }
}
