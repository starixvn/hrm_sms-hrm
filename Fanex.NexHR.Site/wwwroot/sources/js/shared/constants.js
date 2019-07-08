export const DateFormat = 'DD MMM, YYYY';
export const DateTimeFormat = 'DD MMM, YYYY HH:mm';
export const OffDayShiftId = -1;
export const RequestType = {
    CheckIn: 1,
    CheckOut: 2,
    LateIn: 3,
    EarlyOut: 4,
    ChangeShift: 5,
    Leave: 6,
    KeepInView: 7,
    Overtime: 8
};
export const RoleSystem = {
    Member: 1,
    Verifier: 2,
    Approver: 3,
    ShiftManager: 4,
    OnBehalfRequest: 5,
    HR: 6,
    SystemAdmin: 7,
    DisciplineManager: 8
};
export const CommandRequest = {
    Accept: 'Accept',
    NotAccept: 'NotAccept',
    Approve: 'Approve',
    Reject: 'Reject',
    Process: 'Process',
    Void: 'Void',
    Reset: 'Reset'
};
export const CurrentRequestStatus =
{
    Pending: 1,
    Verified: 2,
    NotVerified: 3,
    Approved: 4,
    Rejected: 5,
    Processed: 6,
    Void: 7
};