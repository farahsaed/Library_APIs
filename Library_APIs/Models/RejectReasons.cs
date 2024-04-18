using System.ComponentModel;

namespace Library_APIs.Models
{
    public enum RejectReasons
    {

        [Description("Copyrigts restrictions")]CopyrigtsRestrictions,
        [Description("Limited access right")] LimitedAccessRight,
        [Description("Book is Unlimited")] BookIsUnlimited,
        [Description("Resource Allocation")] ResourceAllocation,
        [Description("Unsuitability Of Requested Material")] UnsuitabilityOfRequestedMaterial,
        [Description("Other")] Other

    }
}
