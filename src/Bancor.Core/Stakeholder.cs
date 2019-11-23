namespace Bancor.Core
{
    public class Stakeholder
    {

        public enum Type
        {
            Trustee,
            Firm
        }

        public int StakeholderId { get; set; }

        public Type TypeOfStakeholder { get; set; }
    }
}