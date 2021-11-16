namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Data;

/// <summary>
/// Error codes reported by the DBNETLIB module.
/// </summary>
internal enum ProcessNetLibErrorCode
{
    ZeroBytes = -3,

    Timeout = -2, /* Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding. */

    Unknown = -1,

    InsufficientMemory = 1,

    AccessDenied = 2,

    ConnectionBusy = 3,

    ConnectionBroken = 4,

    ConnectionLimit = 5,

    ServerNotFound = 6,

    NetworkNotFound = 7,

    InsufficientResources = 8,

    NetworkBusy = 9,

    NetworkAccessDenied = 10,

    GeneralError = 11,

    IncorrectMode = 12,

    NameNotFound = 13,

    InvalidConnection = 14,

    ReadWriteError = 15,

    TooManyHandles = 16,

    ServerError = 17,

    SSLError = 18,

    EncryptionError = 19,

    EncryptionNotSupported = 20
}

// this is a specific error class we need to look for see http://technet.microsoft.com/en-us/library/aa937483(v=SQL.80).aspx
