using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAPI
{
    static class DataStore
    {

        public static List<Person> People { get; }

        public static List<Government> Governments { get;  }

        public static List<PersonalAccount> PersonalAccounts { get; }

        public static List<BusinessAccount> BusinessAccounts { get; }

        public static List<Account> Accounts
        {
            get
            {
                List<Account> AllAccounts = new List<Account>();
                foreach (PersonalAccount TempPersonalAccount in _PersonalAccounts)
                {
                    AllAccounts.Add(TempPersonalAccount);
                }

                foreach (BusinessAccount TempBusinessAccount in _BusinessAccounts)
                {
                    AllAccounts.Add(TempBusinessAccount);
                }

                return AllAccounts;
            }
        }

        public static List<Bank> Banks { get; }

        public static List<Business> Businesses
        {
            get
            {
                List<Business> TempAllBusinesses = new List<Business>();
                foreach (Business TempBusiness in Businesses)
                {
                    TempAllBusinesses.Add(TempBusiness);
                }

                foreach (Bank TempBank in Banks)
                {
                    TempAllBusinesses.Add(TempBank);
                }

                return TempAllBusinesses;

                //return _Businesses;
            }
        }

        public static List<Transaction> Transactions { get; }
    }
}
