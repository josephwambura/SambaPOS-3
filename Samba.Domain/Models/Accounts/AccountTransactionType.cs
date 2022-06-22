using Samba.Infrastructure.Data;

namespace Samba.Domain.Models.Accounts
{
    public class AccountTransactionType : EntityClass, IOrderable
    {
        private static AccountTransactionType _default;
        public static AccountTransactionType Default
        {
            get { return _default ?? (_default = new AccountTransactionType()); }
        }

        public int SortOrder { get; set; }
        public string UserString { get { return Name; } }
        public int SourceAccountTypeId { get; set; }
        public int TargetAccountTypeId { get; set; }
        public int DefaultSourceAccountId { get; set; }
        public int DefaultTargetAccountId { get; set; }
        public int ForeignCurrencyId { get; set; }

        public bool CanMakeAccountTransaction(Account selectedAccount)
        {
            return DefaultSourceAccountId == selectedAccount.Id || DefaultTargetAccountId == selectedAccount.Id
|| SourceAccountTypeId == selectedAccount.AccountTypeId && DefaultSourceAccountId == 0
|| TargetAccountTypeId == selectedAccount.AccountTypeId && DefaultTargetAccountId == 0;
        }

        public int GetDefaultTransactionType()
        {
            return DefaultSourceAccountId == 0 && DefaultTargetAccountId != 0
                ? SourceAccountTypeId
                : DefaultSourceAccountId != 0 && DefaultTargetAccountId == 0 ? TargetAccountTypeId : 0;
        }
    }
}
