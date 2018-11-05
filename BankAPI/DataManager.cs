using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace BankAPI
{
    public static class DataManager
    {
        //Improved Debug
        //public static string ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0}BankAPI\BankDB.mdf;Integrated Security=True", Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 25)); 
        //Debug
        //public static string ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0}:\Coding\Bank\Bank\BankAPI\BankDB.mdf;Integrated Security=True", Environment.CurrentDirectory.Substring(0,1));

        public static string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\benba\\Documents\\Visual Studio 2017\\Projects\\Bank\\BankAPI\\BankDB.mdf\";Integrated Security=True";

        

        /// <summary>
        /// This is the original "LoadDb" method. It downloads the id's for everything in the database, then uses the
        /// corresponding class' "DbSelect" method to download the rest of the data. With many (thousands) of records,
        /// this was very slow. My hypothesis is that since this process involves opening, closing and disposing of 
        /// thousands of connections, this could be quite taxing. Therfore I have decided to redesign the method.
        /// Classes will now have a protected constructor method which takes all of it's properties. The method will
        /// get all of the properties for each object usint one Sql query for each and a single connection for all!
        /// I think this will be much more efficient and therfore significantly quicker. Let's see!
        /// </summary>
        /// <returns></returns>
        
        public static bool LoadDb()
        {
            DefaultLog.Info("Loading all data from database");
            //bool Success = true;

            string CurrentAction = "";

            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    CurrentAction = "opening connection";
                    conn.Open();
                    DefaultLog.Info("Connection opened");


                    CurrentAction = "getting ALL People from database";

                    SqlCommand GetPeople = new SqlCommand("SELECT * FROM t_People;", conn);
                    using (SqlDataReader reader = GetPeople.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Guid TempId = new Guid(reader[0].ToString());
                            Guid TempGovernmentId = new Guid(reader[1].ToString());
                            string TempForename = reader[2].ToString();
                            string TempSurname = reader[3].ToString();
                            
                            Person TempPerson = new Person(TempId, TempForename, TempSurname, TempGovernmentId);
                            DataStore.People.Add(TempPerson);
                        }
                    }
                    DefaultLog.Info("ALL People downloaded from database");

                    CurrentAction = "getting ALL Governments from database";
                    SqlCommand GetGovernments = new SqlCommand("SELECT * FROM t_Governments;", conn);
                    using (SqlDataReader reader = GetGovernments.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Guid TempId = new Guid(reader[0].ToString());
                            
                            string TempName = reader[2].ToString();

                            Guid TempPresidentId = new Guid(reader[0].ToString());

                            Person TempPresident = null;


                            CurrentAction = "getting ALL Governments from database - Getting President from BankAPI.People";
                            int PresidentIndex = DataStore.People.FindIndex(f => f.Id.ToString() == TempPresidentId.ToString());

                            if (PresidentIndex >= 0)
                            {
                                DefaultLog.Info("The Government's President has already been loaded from the database. " +
                                    "Using this instance of President.");

                                TempPresident = DataStore.People[PresidentIndex];
                            }
                            else
                            {

                                /*throw new Exception("The Government's President has not already been loaded from the database. " +
                                    "All People *should* already have been loaded.");*/
                                //Got rid of this because not all Governments have a 
                                //President
                                DefaultLog.Info("The Government either has no President OR the Government's President has not " +
                                    "already been loaded from the database. ");
                            }

                            Government TempGovernment;
                            if (TempPresident != null)
                            {
                                TempGovernment = new Government(TempId, TempName, TempPresident);
                            }
                            else
                            {
                                TempGovernment = new Government(TempId, TempName);
                            }
                            
                            DataStore.Governments.Add(TempGovernment);
                        }
                    }
                    DefaultLog.Info("ALL Governments downloaded from database");

                    CurrentAction = "getting businesses";
                    SqlCommand GetBusinesses = new SqlCommand("SELECT * FROM t_Businesses;", conn);
                    using (SqlDataReader reader = GetBusinesses.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Guid TempId = new Guid(reader[0].ToString());

                            Guid TempGovernmentId = new Guid(reader[3].ToString());
                            Government TempGovernment;

                            string TempOwnerId = reader[2].ToString();
                            Person TempOwner;





                            string TempName = reader[1].ToString();


                            CurrentAction = "getting ALL Businesses from database - Getting Business's Government from BankAPI.Governments";
                            int GovernmentIndex = DataStore.Governments.FindIndex(f => f.Id.ToString() == TempGovernmentId.ToString());

                            if (GovernmentIndex >= 0)
                            {
                                DefaultLog.Info("The Business's Government has already been loaded from the database. " +
                                    "Using this instance of Government.");

                                TempGovernment = DataStore.Governments[GovernmentIndex];
                            }
                            else
                            {
                                throw new Exception("The Business's Government has not already been loaded from the database. " +
                                    "All Governments *should* already have been loaded.");
                            }

                            CurrentAction = "getting ALL Businesses from database - Getting Business's owner from BusinessAPI.People";
                            int BusinessOwnerIndex = DataStore.People.FindIndex(f => f.Id.ToString() == TempOwnerId.ToString());

                            if (BusinessOwnerIndex >= 0)
                            {
                                DefaultLog.Info("The Business's owner has already been loaded from the database. " +
                                    "Using this instance of owner.");

                                TempOwner = DataStore.People[BusinessOwnerIndex];
                            }
                            else
                            {
                                throw new Exception("The Business's owner has not already been loaded from the database. " +
                                    "All People *should* already have been loaded.");
                            }
                            DataStore.Businesses.Add(new Business(TempId, TempGovernment, TempOwner, TempName));
                        }
                    }
                    DefaultLog.Info("Businesses downloaded (excluding banks which were already downloaded)");


                    DefaultLog.Debug("BUSINESS ID'S START");
                    foreach(Business MyBusiness in DataStore.Businesses)
                    {
                        DefaultLog.Debug("BusinessId  = " + MyBusiness.Id);
                    }
                    DefaultLog.Debug("BUSINESS ID'S END");

                    CurrentAction = "getting banks";
                    SqlCommand GetBanks = new SqlCommand("SELECT * FROM t_Banks;", conn);
                    using (SqlDataReader reader = GetBanks.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Guid TempId = new Guid(reader[0].ToString());
                            Business TempMyBusiness;
                            decimal TempSavingsInterestRate = Convert.ToDecimal(reader[1]);
                            decimal TempLoanInterestRate = Convert.ToDecimal(reader[2]);

                            DefaultLog.Debug("Searching for business with Id = " + TempId);

                            CurrentAction = "getting ALL Banks from database - Getting Bank's Business from BankAPI.Businesses";
                            int BusinessIndex = DataStore.Businesses.FindIndex(f => f.Id.ToString() == TempId.ToString());
                            //DefaultLog.Debug("TempId = " + TempId);
                            if (BusinessIndex >= 0)
                            {
                                DefaultLog.Info("The Bank's Business has already been loaded from the database. " +
                                    "Using this instance of Business.");

                                TempMyBusiness = DataStore.Businesses[BusinessIndex];
                            }
                            else
                            {
                                DefaultLog.Debug("BusinessIndex = " + BusinessIndex);
                                throw new Exception("The Bank's Business has not already been loaded from the database. " +
                                    "All Businesses *should* already have been loaded.");
                            }
                            DataStore.Banks.Add(new Bank(TempMyBusiness, TempSavingsInterestRate, TempLoanInterestRate));
                            DefaultLog.Info("Removing Bank's Business from _Businesses");
                            DataStore.Businesses.Remove(TempMyBusiness);
                        }
                    }
                    DefaultLog.Info("ALL Banks downloaded from database");

                    

                    CurrentAction = "getting ALL PersonalAccounts from database";
                    SqlCommand GetPersonalAccounts = new SqlCommand("SELECT * FROM t_Accounts WHERE IsBusiness = 'false';",
                        conn);
                    using (SqlDataReader reader = GetPersonalAccounts.ExecuteReader())
                    {

                        DefaultLog.Debug("ALL BANKS START");
                        foreach(Bank MyBank in DataStore.Banks)
                        {
                            DefaultLog.Debug("Id = " + MyBank.Id);
                        }
                        DefaultLog.Debug("ALL BANKS END");

                        while (reader.Read())
                        {
                            Guid TempPersonalAccountId;

                            Guid TempBankId;
                            Bank TempBank;

                            Guid TempOwnerId;
                            Person TempOwner;

                            int TempBal;
                            bool TempIsActive;
                            //bool TempIsBusiness = false; N/A, defined by datatype

                            TempPersonalAccountId = new Guid(reader[0].ToString());
                            TempBankId = new Guid(reader[1].ToString());
                            TempOwnerId = new Guid(reader[2].ToString());
                            TempBal = Convert.ToInt32(reader[3]);
                            TempIsActive = Convert.ToBoolean(reader[4]);
                            //_IsBusiness = Convert.ToBoolean(reader[5]); N/A, defined by datatype

                            DefaultLog.Debug("TempBankId = " + TempBankId);

                            /////
                            CurrentAction = "getting ALL PersonalAccounts from database - Getting PersonalAccount's Bank from BankAPI.Banks";
                            int BankIndex = DataStore.Banks.FindIndex(f => f.Id.ToString() == TempBankId.ToString());
                            /*Console.WriteLine(BankIndex);
                            //Console.WriteLine(BankAPI.Banks[BankIndex].Name);
                            Console.ReadLine();*/

                            if (BankIndex >= 0)
                            {
                                DefaultLog.Info("The PersonalAccount's Bank has already been loaded from the database. " +
                                    "Using this instance of Bank.");

                                TempBank = DataStore.Banks[BankIndex];
                            }
                            else
                            {
                                throw new Exception("The PersonalAccount's Bank has not already been loaded from the database. " +
                                    "All Banks *should* already have been loaded.");
                            }

                            CurrentAction = "getting ALL PersonalAccounts from database - Getting PersonalAccount's Owner from BankAPI.People";
                            int BusinessOwnerIndex = DataStore.People.FindIndex(f => f.Id.ToString() == TempOwnerId.ToString());

                            if (BusinessOwnerIndex >= 0)
                            {
                                DefaultLog.Info("The PersonalAccount's owner has already been loaded from the database. " +
                                    "Using this instance of owner.");

                                TempOwner = DataStore.People[BusinessOwnerIndex];
                            }
                            else
                            {
                                throw new Exception("The PersonalAccount's owner has not already been loaded from the database. " +
                                    "All People *should* already have been loaded.");
                            }
                            //                                                            
                            DataStore.PersonalAccounts.Add(new PersonalAccount(TempPersonalAccountId, TempBank, TempOwner, TempBal,
                                TempIsActive));
                        } 
                    }
                    DefaultLog.Info("ALL PersonalAccounts downloaded from database");

                    CurrentAction = "getting ALL BusinessAccounts from database";
                    SqlCommand GetBusinessAccounts = new SqlCommand("SELECT * FROM t_Accounts WHERE IsBusiness = 'true';",
                        conn);
                    using (SqlDataReader reader = GetBusinessAccounts.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Guid TempBusinessAccountId;

                            Guid TempBankId;
                            Bank TempBank;

                            Guid TempBusinessId;
                            Business TempBusiness;

                            int TempBal;
                            bool TempIsActive;
                            //bool TempIsBusiness = true; N/A, defined by datatype

                            TempBusinessAccountId = new Guid(reader[0].ToString());
                            TempBankId = new Guid(reader[1].ToString());
                            TempBusinessId = new Guid(reader[2].ToString());
                            TempBal = Convert.ToInt32(reader[3]);
                            TempIsActive = Convert.ToBoolean(reader[4]);
                            //_IsBusiness = Convert.ToBoolean(reader[5]); N/A, defined by datatype

                            /////
                            CurrentAction = "getting ALL BusinessAccounts from database - Getting BusinessAccount's Bank from BankAPI.Banks";
                            int BankIndex = DataStore.Banks.FindIndex(f => f.Id.ToString() == TempBankId.ToString());

                            if (BankIndex >= 0)
                            {
                                DefaultLog.Info("The BusinessAccount's Bank has already been loaded from the database. " +
                                    "Using this instance of Bank.");

                                TempBank = DataStore.Banks[BankIndex];
                            }
                            else
                            {
                                throw new Exception("The BusinessAccount's Bank has not already been loaded from the database. " +
                                    "All Banks *should* already have been loaded.");
                            }

                            CurrentAction = "getting ALL BusinessAccounts from database - Getting BusinessAccount's Business from BankAPI.Businesses";
                            int BusinessOwnerIndex = DataStore.Businesses.FindIndex(f => f.Id.ToString() == TempBusinessId.ToString());

                            if (BusinessOwnerIndex >= 0)
                            {
                                DefaultLog.Info("The BusinessAccount's Business has already been loaded from the database. " +
                                    "Using this instance of Business.");

                                TempBusiness = DataStore.Businesses[BusinessOwnerIndex];
                            }
                            else
                            {
                                throw new Exception("The BusinessAccount's Business has not already been loaded from the database. " +
                                    "All Businesses *should* already have been loaded.");
                            }
                            //                                                            
                            DataStore.BusinessAccounts.Add(new BusinessAccount(TempBusinessAccountId, TempBank, TempBusiness, TempBal,
                                TempIsActive));
                        }
                    }
                    DefaultLog.Info("All BusinessAccounts downloaded from database");
                }  
            }
            catch (Exception e)
            {
                DefaultLog.Error("There was an exception while " + CurrentAction);
                DefaultLog.Error("Exception = " + e);
                return false;
            }
            DefaultLog.Info("All data loaded from database successfully!");
            DefaultLog.Info(String.Format("{0} personal accounts, {1} business accounts, " +
                "{3} businesses of which {2} are banks, " +
                "{4} people & {5} governments", DataStore.PersonalAccounts.Count(), DataStore.BusinessAccounts.Count(), DataStore.Banks.Count(), 
                DataStore.Businesses.Count(), DataStore.People.Count(), DataStore.Governments.Count()));
            return true;
        }

        public static bool UpdateDb()
        {
            throw new NotImplementedException();
        }
    }

    
}