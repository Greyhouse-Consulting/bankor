using System.Threading.Tasks;

namespace BankOr.Core.Models
{
    public class AccountModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}