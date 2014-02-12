// --------------------------------------------------------------------------
// <copyright file="RequestStatus.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models.Request
{
    using Fanex.NexHR.Core.Enums;
    using Resources;

    /// <summary>
    /// Request Status.
    /// </summary>
    public class RequestStatus : EntityBase<byte>
    {
        public new string Name
        {
            get
            {
                switch (Id)
                {
                    case (byte)CurrentRequestStatus.Pending:
                        return Labels.Pending;
                    case (byte)CurrentRequestStatus.Verified:
                        return Labels.Verified;
                    case (byte)CurrentRequestStatus.NotVerified:
                        return Labels.NotVerified;
                    case (byte)CurrentRequestStatus.Approved:
                        return Labels.Approved;
                    case (byte)CurrentRequestStatus.Rejected:
                        return Labels.Rejected;
                    case (byte)CurrentRequestStatus.Processed:
                        return Labels.Processed;
                    case (byte)CurrentRequestStatus.Void:
                        return Labels.Void;
                    default:
                        return string.Empty;
                }
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
