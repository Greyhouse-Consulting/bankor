namespace Bancor.Core.Events.Account
{
    public class NewStakeholderEvent : AccountEvent
    {
        public int StakeholderId { get; }
        public Stakeholder.Type TypeOfStakeholder { get; }


        public NewStakeholderEvent(int stakeholderId, Stakeholder.Type typeOfStakeholder, string description) : base(description)
        {
            StakeholderId = stakeholderId;
            TypeOfStakeholder = typeOfStakeholder;
        }


    }
}