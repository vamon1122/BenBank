using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace BankAPI
{
    public abstract class Account
    {
        public Guid Id { get { return _Id; } }
        protected Guid _Id;

        public int Bal { get { return _Bal; } }
        protected int _Bal;

        /*public Guid MyOwnerId { get { return _MyOwnerId; } }
        protected Guid _MyOwnerId;*/

        public Bank MyBank { get { return _MyBank; } }
        protected Bank _MyBank;

        public bool IsActive { get; set; }
        protected bool _IsActive;

        public bool IsBusiness { get; set; }
        protected bool _IsBusiness;

        protected abstract Guid OwnerId();

        //Not necessary in an abstract class
        /*public Account(Person pPerson, Bank pBank, bool pIsBusiness)
        {

        }*/

        protected void AddFunds(int pAmmount)
        {
            _Bal = _Bal + pAmmount;
        }

        protected void TakeFunds(int pAmmount)
        {
            _Bal = _Bal - pAmmount;
        }

        public abstract bool DbInsert();

        public abstract bool DbUpdate();

        public abstract bool DbSelect();



    }

    public class BusinessAccount : Account
    {
        public Business MyBusiness { get { return _MyBusiness; } }
        private Business _MyBusiness;

        protected override Guid OwnerId()
        {
            return MyBusiness.Id;
        }

        internal BusinessAccount(Guid pId, Bank pBank, Business pBusiness, int pBal, bool pIsActive)
        {
            _IsBusiness = true;
            _Id = pId;
            _MyBank = pBank;
            _MyBusiness = pBusiness;
            _Bal = pBal;
            _IsActive = pIsActive;
        }

        public BusinessAccount(Business pBusiness, Bank pBank)
        {
            DefaultLog.Info("Starting create new instance of 'BusinessAccount' class " + MyInfo());

            _MyBusiness = pBusiness;
            _MyBank = pBank;
            _Id = Guid.NewGuid();
            _Bal = 0;
            _IsActive = true;
            _IsBusiness = true;
            //BankAPI.AddBusinessAccount(this);

            DefaultLog.Info("Successfully created new instance of 'BusinessAccount' class" + MyInfo());
        }

        public BusinessAccount(Guid pId)
        {
            DefaultLog.Info("Starting create new instance of 'BusinessAccount' class " + MyInfo());

            _Id = pId;
            //BankAPI.AddBusinessAccount(this);

            DefaultLog.Info("Successfully created new instance of 'BusinessAccount' class" + MyInfo());
        }

        public override bool DbInsert()
        {
            DefaultLog.Info("Starting create business account on database " + MyInfo());
            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand InsertAccount = new SqlCommand("INSERT INTO t_Accounts  " +
                        "VALUES (@AccountId, @BankId, @OwnerId, @Bal, @IsActive, @IsBusiness);", conn);
                    InsertAccount.Parameters.Add(new SqlParameter("@IsBusiness", _IsBusiness));
                    InsertAccount.Parameters.Add(new SqlParameter("@IsActive", _IsActive));
                    InsertAccount.Parameters.Add(new SqlParameter("@AccountId", _Id));
                    InsertAccount.Parameters.Add(new SqlParameter("@BankId", _MyBank.Id));
                    InsertAccount.Parameters.Add(new SqlParameter("@OwnerId", OwnerId()));
                    InsertAccount.Parameters.Add(new SqlParameter("@Bal", _Bal));


                    InsertAccount.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while creating business account on database " + MyInfo());
                DefaultLog.Error("SQL exception : " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while creating business account on database " + MyInfo());
                DefaultLog.Error("Generic exception : " + e);
                return false;
            }

            DefaultLog.Info("Successfully created business account on database " + MyInfo());
            return true;
        }

        public override bool DbUpdate()
        {
            DefaultLog.Info("Starting update business account on database " + MyInfo());
            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand UpdateAccount = new SqlCommand("UPDATE t_Accounts SET Bal = @Bal, IsActive = @IsActive " +
                        "WHERE AccountId = @Id;", conn);
                    UpdateAccount.Parameters.Add(new SqlParameter("@IsActive", _IsActive));
                    UpdateAccount.Parameters.Add(new SqlParameter("@Bal", _Bal));
                    UpdateAccount.Parameters.Add(new SqlParameter("@Id", _Id));

                    UpdateAccount.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while updating business account on database " + MyInfo());
                DefaultLog.Error("SQL exception : " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while updating business account on database " + MyInfo());
                DefaultLog.Error("Generic exception : " + e);
                return false;
            }

            DefaultLog.Info("Successfully updated business account on database " + MyInfo());

            return true;
        }

        public override bool DbSelect()
        {
            DefaultLog.Info("Starting get business account from database " + MyInfo());
            try
            {
                Guid TempBankId = Guid.Empty;
                Guid TempBusinessId = Guid.Empty;

                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Accounts WHERE AccountId = @Id;", conn);
                    SelectAccount.Parameters.Add(new SqlParameter("Id", _Id));

                    using (SqlDataReader reader = SelectAccount.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TempBankId = new Guid(reader[1].ToString());
                            TempBusinessId = new Guid(reader[2].ToString());
                            _Bal = Convert.ToInt32(reader[3]);
                            _IsActive = Convert.ToBoolean(reader[4]);
                            _IsBusiness = Convert.ToBoolean(reader[5]);
                        }
                    }
                }

                int BankIndex = DataManager.Banks.FindIndex(f => f.Id.ToString() == TempBankId.ToString());

                if (BankIndex >= 0)
                {
                    DefaultLog.Info("The business account's bank has already been loaded from the database. " +
                        "Using this instance of bank." + MyInfo());

                    _MyBank = DataManager.Banks[BankIndex];
                }
                else
                {
                    DefaultLog.Info("The business account's bank has not already been loaded from the database. " +
                        "Bank will be loaded" + MyInfo());
                    _MyBank = new Bank(TempBankId);
                    _MyBank.DbSelect();
                }

                int BusinessIndex = DataManager.Businesses.FindIndex(f => f.Id.ToString() == TempBusinessId.ToString());

                if (BusinessIndex >= 0)
                {
                    DefaultLog.Info("The business account's business has already been loaded from the database. " +
                        "Using this instance of business.");

                    _MyBusiness = DataManager.Businesses[BusinessIndex];
                }
                else
                {
                    DefaultLog.Info("The business account's business has not already been loaded from the database. " +
                        "Business will be loaded");
                    _MyBusiness = new Business(TempBusinessId);
                    MyBusiness.DbSelect();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while getting business account from database " + MyInfo());
                DefaultLog.Error("SQL exception : " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while getting business account from database " + MyInfo());
                DefaultLog.Error("Generic exception : " + e);
                return false;
            }
            DefaultLog.Info("Successfully got business account from database database " + MyInfo());
            return true;
        }

        private string MyInfo()
        {
            string MyString = String.Format("[ Class = BusinessAccount : Account | Id = {0} | Owner name = ", _Id);

            if (MyBusiness != null)
            {
                MyString += MyBusiness.Name + " ]";
            }
            else
            {
                MyString += "??? ]";
            }

            return MyString;
        }
    }

    public class PersonalAccount : Account
    {
        public Person MyPerson { get { return _MyPerson; } }
        private Person _MyPerson;

        protected override Guid OwnerId()
        {
            return MyPerson.Id;
        }

        internal PersonalAccount(Guid pId, Bank pBank, Person pOwner, int pBal, bool pIsActive)
        {
            _IsBusiness = false;
            _Id = pId;
            _MyBank = pBank;
            _MyPerson = pOwner;
            _Bal = pBal;
            _IsActive = pIsActive;
        }

        public PersonalAccount(Person pPerson, Bank pBank)
        {
            DefaultLog.Info("Starting create new instance of 'PersonalAccount' class " + MyInfo());

            _MyPerson = pPerson;
            _MyBank = pBank;
            _Id = Guid.NewGuid();
            _Bal = 0;
            _IsActive = true;
            _IsBusiness = false;
            //BankAPI.AddPersonalAccount(this);

            DefaultLog.Info("Successfully created new instance of 'PersonalAccount' class" + MyInfo());
        }

        public PersonalAccount(Guid pId)
        {
            DefaultLog.Info("Starting create new instance of 'PersonalAccount' class " + MyInfo());

            _Id = pId;
            //BankAPI.AddPersonalAccount(this);

            DefaultLog.Info("Successfully created new instance of 'PersonalAccount' class" + MyInfo());
        }

        public override bool DbInsert()
        {
            DefaultLog.Info("Starting create personal account on database " + MyInfo());
            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand InsertAccount = new SqlCommand("INSERT INTO t_Accounts  " +
                        "VALUES (@AccountId, @BankId, @OwnerId, @Bal, @IsActive, @IsBusiness);", conn);
                    InsertAccount.Parameters.Add(new SqlParameter("@IsBusiness", _IsBusiness));
                    InsertAccount.Parameters.Add(new SqlParameter("@IsActive", _IsActive));
                    InsertAccount.Parameters.Add(new SqlParameter("@AccountId", _Id));
                    InsertAccount.Parameters.Add(new SqlParameter("@BankId", _MyBank.Id));
                    InsertAccount.Parameters.Add(new SqlParameter("@OwnerId", OwnerId()));
                    InsertAccount.Parameters.Add(new SqlParameter("@Bal", _Bal));


                    InsertAccount.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while creating personal account on database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while creating personal account on database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }

            DefaultLog.Info("Successfully created personal account on database " + MyInfo());
            return true;
        }

        public override bool DbUpdate()
        {
            DefaultLog.Info("Starting update personal account on database " + MyInfo());
            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand UpdateAccount = new SqlCommand("UPDATE t_Accounts SET Bal = @Bal, IsActive = @IsActive " +
                        "WHERE AccountId = @Id;", conn);
                    UpdateAccount.Parameters.Add(new SqlParameter("@IsActive", _IsActive));
                    UpdateAccount.Parameters.Add(new SqlParameter("@Bal", _Bal));
                    UpdateAccount.Parameters.Add(new SqlParameter("@Id", _Id));

                    UpdateAccount.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while updating personal account on database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while updating personal account on database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }
            DefaultLog.Info("Successfully updated personal account on database " + MyInfo());
            return true;
        }

        public override bool DbSelect()
        {
            DefaultLog.Info("Starting get personal account from database " + MyInfo());
            try
            {
                Guid TempBankId = Guid.Empty;
                Guid TempOwnerId = Guid.Empty;

                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Accounts WHERE AccountId = @Id;", conn);
                    SelectAccount.Parameters.Add(new SqlParameter("Id", _Id));

                    using (SqlDataReader reader = SelectAccount.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TempBankId = new Guid(reader[1].ToString());
                            TempOwnerId = new Guid(reader[2].ToString());
                            _Bal = Convert.ToInt32(reader[3]);
                            _IsActive = Convert.ToBoolean(reader[4]);
                            _IsBusiness = Convert.ToBoolean(reader[5]);
                        }
                    }
                }

                int BankIndex = DataManager.Banks.FindIndex(f => f.Id.ToString() == TempBankId.ToString());


                if (BankIndex >= 0)
                {
                    DefaultLog.Info("The personal account's bank has already been loaded from the database. " +
                        "Using this instance of bank." + MyInfo());

                    _MyBank = DataManager.Banks[BankIndex];
                }
                else
                {
                    DefaultLog.Info("The personal account's bank has not already been loaded from the database. " +
                        "Bank will be loaded" + MyInfo());
                    _MyBank = new Bank(TempBankId);
                    _MyBank.DbSelect();
                }

                int OwnerIndex = DataManager.People.FindIndex(f => f.Id.ToString() == TempOwnerId.ToString());

                if (OwnerIndex >= 0)
                {
                    DefaultLog.Info("The personal account's owner has already been loaded from the database. " +
                        "Using this instance of owner." + MyInfo());

                    _MyPerson = DataManager.People[OwnerIndex];
                }
                else
                {
                    DefaultLog.Info("The personal account's owner has not already been loaded from the database. " +
                        "Owner will be loaded" + MyInfo());
                    _MyPerson = new Person(TempBankId);
                    _MyPerson.DbSelect();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while getting personal account from database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while getting personal account from database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }
            DefaultLog.Info("Successfully got personal account from database " + MyInfo());
            return true;
        }


        private string MyInfo()
        {
            string MyString = String.Format("[ Class = PersonalAccount : Account | Id = {0} | Owner name = ", _Id);

            if (MyPerson != null)
            {
                MyString += MyPerson.Name + " ]";
            }
            else
            {
                MyString += "??? ]";
            }

            return MyString;
        }
    }

    public class Bank : Business
    {
        decimal _SavingsInterestRate;
        public decimal SavingsInterestRate { get { return _SavingsInterestRate; } set { _SavingsInterestRate = value; } }

        decimal _LoanInterestRate;
        public decimal LoanInterestRate { get { return _LoanInterestRate; } set { _LoanInterestRate = value; } }


        public Bank(Guid pId, Government pGovernment, string pName, Person pOwner, decimal pSavingsInterestRate, decimal pLoanInterestRate)
        {
            DefaultLog.Info("Starting create new instance of 'Bank' class " + MyInfo());

            _Id = pId;
            _MyGovernment = pGovernment;
            _Name = pName;
            _Owner = pOwner;
            _SavingsInterestRate = pSavingsInterestRate;
            _LoanInterestRate = pLoanInterestRate;
            //BankAPI.AddBank(this);

            DefaultLog.Info("Successfully created new instance of 'Bank' class " + MyInfo());
        }

        public Bank(Government pGovernment, string pName, Person pOwner, decimal pSavingsInterestRate, decimal pLoanInterestRate)
        {
            DefaultLog.Info("Starting create new instance of 'Bank' class " + MyInfo());

            _Id = Guid.NewGuid();
            _MyGovernment = pGovernment;
            _Name = pName;
            _Owner = pOwner;
            _SavingsInterestRate = pSavingsInterestRate;
            _LoanInterestRate = pLoanInterestRate;
            //BankAPI.AddBank(this);

            DefaultLog.Info("Successfully created new instance of 'Bank' class " + MyInfo());
        }

        public Bank(Business pMyBusiness, decimal pSavingsInterestRate, decimal pLoanInterestRate)
        {
            DefaultLog.Info("Starting create new instance of 'Bank' class " + MyInfo());

            _Id = pMyBusiness.Id;
            _MyGovernment = pMyBusiness.MyGovernment;
            _Name = pMyBusiness.Name;
            _Owner = pMyBusiness.Owner;
            _SavingsInterestRate = pSavingsInterestRate;
            _LoanInterestRate = pLoanInterestRate;
            //BankAPI.AddBank(this);

            DefaultLog.Info("Successfully created new instance of 'Bank' class " + MyInfo());
        }

        public Bank(Guid pId)
        {
            DefaultLog.Info("Starting create new instance of 'Bank' class " + MyInfo());

            _Id = pId;
            //BankAPI.AddBank(this);

            DefaultLog.Info("Successfully created new instance of 'Bank' class " + MyInfo());
        }

        new public bool DbInsert()
        {
            DefaultLog.Info("Starting create bank on database " + MyInfo());

            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    //DefaultLog.Debug("Creating business...");
                    SqlCommand CreateBusiness = new SqlCommand("INSERT INTO t_Businesses VALUES (@Id, @Name, @OwnerId, " +
                        "@GovernmentId);", conn);
                    CreateBusiness.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateBusiness.Parameters.Add(new SqlParameter("Name", _Name));
                    CreateBusiness.Parameters.Add(new SqlParameter("OwnerId", _Owner.Id));
                    CreateBusiness.Parameters.Add(new SqlParameter("GovernmentId", _MyGovernment.Id));

                    CreateBusiness.ExecuteNonQuery();
                    //DefaultLog.Debug("Business created!");

                    //DefaultLog.Debug("Creating bank...");
                    SqlCommand CreateBank = new SqlCommand("INSERT INTO t_Banks VALUES (@Id, @SavingsInterestRate, @LoanInterestRate);", conn);
                    CreateBank.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateBank.Parameters.Add(new SqlParameter("SavingsInterestRate", _SavingsInterestRate));
                    CreateBank.Parameters.Add(new SqlParameter("LoanInterestRate", _LoanInterestRate));

                    CreateBank.ExecuteNonQuery();
                    //DefaultLog.Debug("Created bank!");
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while creating bank on database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while creating bank on database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }

            DefaultLog.Info("Successfully created bank on database " + MyInfo());
            return true;
        }

        new public bool DbUpdate()
        {
            DefaultLog.Info("Starting update bank on database " + MyInfo());
            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand UpdateBusiness = new SqlCommand("UPDATE t_Businesses SET MyGovernmentId = @MyGovernmentId, " +
                        "Name = @Name WHERE Id = @Id;", conn);
                    UpdateBusiness.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateBusiness.Parameters.Add(new SqlParameter("MyGovernmentId", _MyGovernment.Id));
                    UpdateBusiness.Parameters.Add(new SqlParameter("Name", _Name));

                    /*SqlCommand UpdateBank = new SqlCommand("UPDATE t_Banks SET  " +
                        WHERE Id = @Id;");
                    UpdateBank.Parameters.Add(new SqlParameter("Id", _Id));*/
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while committing changes to database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while committing changes to database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }

            DefaultLog.Info("Successfully updated bank on database " + MyInfo());
            return true;
        }

        new public bool DbSelect()
        {
            DefaultLog.Info("Starting get bank from database " + MyInfo());
            try
            {
                Guid TempOwnerId = Guid.Empty;
                Guid TempGovernmentId = Guid.Empty;

                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand SelectBusiness = new SqlCommand("SELECT * FROM t_Businesses WHERE Id = @Id;", conn);
                    SelectBusiness.Parameters.Add(new SqlParameter("Id", _Id));

                    using (SqlDataReader reader = SelectBusiness.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Name = reader[1].ToString();
                            TempOwnerId = new Guid(reader[2].ToString());
                            TempGovernmentId = new Guid(reader[3].ToString());
                        }
                    }

                    /*SqlCommand SelectBank = new SqlCommand("UPDATE t_Banks SET  " +
                        WHERE Id = @Id;");
                    SelectBank.Parameters.Add(new SqlParameter("Id", _Id));

                    using(SqlDataReader reader = SelectBank.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                        }
                    }   
                 */
                }

                int OwnerIndex = DataManager.People.FindIndex(f => f.Id.ToString() == TempOwnerId.ToString());

                if (OwnerIndex >= 0)
                {
                    DefaultLog.Info("The bank's owner has already been loaded from the database. " +
                        "Using this instance of owner " + MyInfo());

                    _Owner = DataManager.People[OwnerIndex];
                }
                else
                {
                    DefaultLog.Info("The bank's owner has not already been loaded from the database. " +
                        "Owner will be loaded" + MyInfo());
                    _Owner = new Person(TempOwnerId);
                    _Owner.DbSelect();
                }

                int GovernmentIndex = DataManager.Governments.FindIndex(f => f.Id.ToString() == TempGovernmentId.ToString());

                if (GovernmentIndex >= 0)
                {
                    DefaultLog.Info("The bank's government has already been loaded from the database. " +
                        "Using this instance of government." + MyInfo());

                    _MyGovernment = DataManager.Governments[GovernmentIndex];
                }
                else
                {
                    DefaultLog.Info("The bank's government has not already been loaded from the database. " +
                        "Government will be loaded" + MyInfo());
                    _MyGovernment = new Government(TempGovernmentId);
                    _MyGovernment.DbSelect();
                }


            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while getting bank from database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while getting bank from database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }

            DefaultLog.Info("Successfully got bank from database database " + MyInfo());
            return true;
        }

        private string MyInfo()
        {
            string MyString = String.Format("[ Class = Bank : Business | Id = {0} | Owner name: ", _Id);

            if (Owner != null)
            {
                MyString += Owner.Name + " ]";
            }
            else
            {
                MyString += "??? ]";
            }

            return MyString;
        }
    }

    public class Person
    {
        public Guid Id { get { return _Id; } }
        private Guid _Id;

        public string Forename { get { return _Forename; } set { _Forename = value; } }
        private string _Forename;

        public string Surname { get { return _Surname; } set { _Surname = value; } }
        private string _Surname;

        public string Name { get { return String.Format("{0} {1}", Forename, Surname); } }

        public Guid MyGovernmentId { get { return _MyGovernmentId; } }
        private Guid _MyGovernmentId;

        internal Person(Guid pId, string pForename, string pSurname, Guid pGovernmentId)
        {
            _Id = pId;
            _Forename = pForename;
            _Surname = pSurname;
            _MyGovernmentId = pGovernmentId;
        }

        public Person(string pForename, string pSurname, Government pGovernment)
        {
            DefaultLog.Info("Starting create new instance of 'Person' class " + MyInfo());

            _Id = Guid.NewGuid();
            _Forename = pForename;
            _Surname = pSurname;
            _MyGovernmentId = pGovernment.Id;
            //BankAPI.AddPerson(this);

            DefaultLog.Info("Successfully created new instance of 'Person' class" + MyInfo());
        }

        public Person(Guid pId)
        {
            DefaultLog.Info("Starting create new instance of 'Person' class " + MyInfo());

            _Id = pId;
            //BankAPI.AddPerson(this);

            DefaultLog.Info("Successfully created new instance of 'Person' class" + MyInfo());
        }

        public Person()
        {
            DefaultLog.Info("Starting create new instance of 'Person' class " + MyInfo());

            _Id = Guid.NewGuid();
            //BankAPI.AddPerson(this);

            DefaultLog.Info("Successfully created new instance of 'Person' class" + MyInfo());
        }

        public bool DbInsert()
        {
            DefaultLog.Info("Starting create person on database " + MyInfo());
            try
            {
                //DefaultLog.Debug("Try block started...");

                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    //DefaultLog.Debug(String.Format("Connection opened with connection string \"{0}\"", BankAPI.ConnectionString));

                    SqlCommand CreatePerson = new SqlCommand("INSERT INTO t_People VALUES (@PersonId, @GovernmentId, " +
                        "@Forename, @Surname);", conn);
                    CreatePerson.Parameters.Add(new SqlParameter("PersonId", _Id));
                    CreatePerson.Parameters.Add(new SqlParameter("GovernmentId", _MyGovernmentId));
                    CreatePerson.Parameters.Add(new SqlParameter("Forename", _Forename));
                    CreatePerson.Parameters.Add(new SqlParameter("Surname", _Surname));

                    CreatePerson.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while creating person on database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while creating person on database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }
            DefaultLog.Info("Successfully created person on database " + MyInfo());
            return true;
        }

        public bool DbUpdate()
        {
            DefaultLog.Info("Starting update person on database " + MyInfo());
            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand UpdatePerson = new SqlCommand("UPDATE t_People SET Forename = @Forename, " +
                        "Surname = @Surname WHERE PersonId = @PersonId;", conn);
                    UpdatePerson.Parameters.Add(new SqlParameter("PersonId", _Id));
                    UpdatePerson.Parameters.Add(new SqlParameter("Forename", _Forename));
                    UpdatePerson.Parameters.Add(new SqlParameter("Surname", _Surname));

                    UpdatePerson.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while updating person on database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while updating person on database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }
            DefaultLog.Info("Successfully updated person on database " + MyInfo());
            return true;
        }

        public bool DbSelect()
        {
            DefaultLog.Info("Starting get person from database " + MyInfo());
            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_People WHERE PersonId = @Id;", conn);
                    SelectAccount.Parameters.Add(new SqlParameter("Id", _Id));

                    using (SqlDataReader reader = SelectAccount.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _MyGovernmentId = new Guid(reader[1].ToString());
                            _Forename = reader[2].ToString();
                            _Surname = reader[3].ToString();
                        }
                    }

                    /*SqlCommand SelectAccount = new SqlCommand("UPDATE t_Accounts SET  " +
                        WHERE Id = @Id;");
                    SelectAccount.Parameters.Add(new SqlParameter("Id", _Id));*/
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while getting person from database " + MyInfo());
                DefaultLog.Error("SQL exception : " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while getting person from database " + MyInfo());
                DefaultLog.Error("Generic exception : " + e);
                return false;
            }

            DefaultLog.Info("Successfully got person from database database " + MyInfo());

            return true;
        }

        private string MyInfo()
        {
            string MyString = String.Format("[ Person id : {0}   |   Person name: {1} {2} ]", _Id, _Forename, _Surname);
            return MyString;
        }
    }

    public class Government
    {
        public Guid Id { get { return _Id; } }
        protected Guid _Id;

        public string Name { get { return _Name; } set { _Name = value; } }
        protected string _Name;

        public Person President { get { return _President; } set { _President = value; } }
        protected Person _President;

        internal Government(Guid pId, string pName, Person pPresident)
        {
            _Id = pId;
            _Name = pName;
            _President = pPresident;
        }

        internal Government(Guid pId, string pName)
        {
            _Id = pId;
            _Name = pName;
        }

        public Government(string pName)
        {
            DefaultLog.Info("Starting create new instance of 'Government' class " + MyInfo());

            _Id = Guid.NewGuid();
            _Name = pName;
            //BankAPI.AddGovernment(this);

            DefaultLog.Info("Successfully created new instance of 'Government' class" + MyInfo());
        }

        public Government(Guid pId)
        {
            DefaultLog.Info("Starting create new instance of 'Government' class " + MyInfo());

            _Id = pId;
            DbSelect();
            //BankAPI.AddGovernment(this);

            DefaultLog.Info("Successfully created new instance of 'Government' class" + MyInfo());
        }

        public bool DbInsert()
        {
            DefaultLog.Info("Starting create government on database " + MyInfo());

            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand CreateGovernment;

                    if (_President == null)
                    {
                        DefaultLog.Info("No presedent has been assigned. Creating government without presedent " + MyInfo());
                        CreateGovernment = new SqlCommand("INSERT INTO t_Governments (Id, Name) VALUES (@Id, @Name);", conn);

                        CreateGovernment.Parameters.Add(new SqlParameter("Id", Id));
                        CreateGovernment.Parameters.Add(new SqlParameter("Name", Name));
                    }
                    else
                    {
                        DefaultLog.Info("A presedent has been assigned. Creating government with presedent " + MyInfo());
                        CreateGovernment = new SqlCommand("INSERT INTO t_Governments VALUES (@Id, @Name," +
                        "@PresidentId);", conn);

                        CreateGovernment.Parameters.Add(new SqlParameter("Id", Id));
                        CreateGovernment.Parameters.Add(new SqlParameter("Name", Name));
                        CreateGovernment.Parameters.Add(new SqlParameter("PresidentId", President.Id));
                    }



                    CreateGovernment.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while creating government on database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while creating government on database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }

            DefaultLog.Info("Successfully created government on database " + MyInfo());
            return true;
        }

        public bool DbUpdate()
        {
            DefaultLog.Info("Starting update government on database " + MyInfo());
            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand UpdateGovernment = null;

                    if (_President == null)
                    {
                        DefaultLog.Info("President not found!" + MyInfo());
                        UpdateGovernment = new SqlCommand("UPDATE t_Governments SET Name = @Name WHERE Id = @Id;", conn);
                        UpdateGovernment.Parameters.Add(new SqlParameter("Id", Id));
                        UpdateGovernment.Parameters.Add(new SqlParameter("Name", Name));
                    }
                    else
                    {
                        DefaultLog.Info("President found!" + MyInfo());
                        UpdateGovernment = new SqlCommand("UPDATE t_Governments SET PresidentId = @PresidentId, " +
                        "Name = @Name WHERE Id = @Id;", conn);
                        UpdateGovernment.Parameters.Add(new SqlParameter("Id", Id));
                        UpdateGovernment.Parameters.Add(new SqlParameter("PresidentId", President.Id));
                        UpdateGovernment.Parameters.Add(new SqlParameter("Name", Name));
                    }
                    UpdateGovernment.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while updating government on database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while updating government on database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }

            DefaultLog.Info("Successfully updated government on database " + MyInfo());
            return true;
        }

        public bool DbSelect()
        {
            //throw new NotImplementedException();

            DefaultLog.Info("Starting get government from database " + MyInfo());
            try
            {
                Guid TempPresidentId = Guid.Empty;

                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Governments WHERE Id = @Id;", conn);
                    SelectAccount.Parameters.Add(new SqlParameter("Id", _Id));

                    using (SqlDataReader reader = SelectAccount.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Name = reader[1].ToString();


                            if (reader[2].ToString() != "")
                            {
                                DefaultLog.Info("President Id found in database! " + MyInfo());
                                TempPresidentId = new Guid(reader[2].ToString());
                            }
                            else
                            {
                                DefaultLog.Info("No president Id found in database! " + MyInfo());
                            }
                        }
                    }

                    /*SqlCommand SelectAccount = new SqlCommand("UPDATE t_Accounts SET  " +
                        WHERE Id = @Id;");
                    SelectAccount.Parameters.Add(new SqlParameter("Id", _Id));*/
                }

                if (TempPresidentId == Guid.Empty)//Cannot be empty, nulls not allowed
                {
                    DefaultLog.Info("No president Id found. Not getting president " + MyInfo());
                }
                else
                {
                    DefaultLog.Info("President Id found. Getting president " + MyInfo());

                    int Index = DataManager.People.FindIndex(f => f.Id.ToString() == TempPresidentId.ToString());

                    if (Index >= 0)
                    {
                        DefaultLog.Info("The government's president has already been loaded from the database. " +
                            "Using this instance of president " + MyInfo());

                        _President = DataManager.People[Index];
                    }
                    else
                    {
                        DefaultLog.Info("The government's president has not already been loaded from the database. " +
                            "President will be loaded " + MyInfo());
                        _President = new Person(TempPresidentId);
                        _President.DbSelect();
                    }
                }


            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while getting government from database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while getting government from database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }

            DefaultLog.Info("Successfully got government from database database " + MyInfo());

            return true;
        }

        private string MyInfo()
        {
            string MyString = String.Format("[ Class = Government | Id = {0} | Name = ", _Id);

            if (Name != null && Name != "")
            {
                MyString += Name + " ]";
            }
            else
            {
                MyString += "??? ]";
            }

            return MyString;
        }

    }

    public class Business
    {
        public Guid Id { get { return _Id; } }
        protected Guid _Id;

        public string Name { get { return _Name; } set { _Name = value; } }
        protected string _Name;

        public Person Owner { get { return _Owner; } set { _Owner = value; } }
        protected Person _Owner;

        public Government MyGovernment { get { return _MyGovernment; } }
        protected Government _MyGovernment;

        //This is required for derived classes
        protected Business()
        {
            DefaultLog.Info("Starting create new instance of 'Business' class");

            _Id = Guid.NewGuid();
            //BankAPI.AddBusiness(this);

            DefaultLog.Info("Successfully created new instance of 'Business' class" + MyInfo());
        }

        internal Business(Guid pId, Government pGovernment, Person pOwner, string pName)
        {
            _Id = pId;
            _MyGovernment = pGovernment;
            _Owner = pOwner;
            _Name = pName;
        }

        public Business(Guid pId)
        {
            DefaultLog.Info("Starting create new instance of 'Business' class " + MyInfo());

            _Id = pId;
            DbSelect();
            //BankAPI.AddBusiness(this);

            DefaultLog.Info("Successfully created new instance of 'Business' class" + MyInfo());
        }

        public Business(Government pGovernment, Person pOwner, string pName)
        {
            DefaultLog.Info("Starting create new instance of 'Business' class " + MyInfo());

            _Id = Guid.NewGuid();
            _MyGovernment = pGovernment;
            _Owner = pOwner;
            _Name = pName;

            DefaultLog.Info("Successfully created new instance of 'Business' class" + MyInfo());
        }

        public bool DbInsert()
        {
            DefaultLog.Info("Starting create business on database " + MyInfo());
            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand CreateBusiness = new SqlCommand("INSERT INTO t_Businesses VALUES (@Id, " +
                        "@Name, @OwnerId, @GovernmentId);", conn);
                    CreateBusiness.Parameters.Add(new SqlParameter("Id", Id));
                    CreateBusiness.Parameters.Add(new SqlParameter("Name", Name));
                    CreateBusiness.Parameters.Add(new SqlParameter("OwnerId", Owner.Id));
                    CreateBusiness.Parameters.Add(new SqlParameter("GovernmentId", MyGovernment.Id));

                    CreateBusiness.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while creating business on database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while creating business on database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }

            DefaultLog.Info("Successfully created business on database " + MyInfo());
            return true;
        }

        public bool DbUpdate()
        {
            //throw new NotImplementedException();
            DefaultLog.Info("Starting update business on database " + MyInfo());
            try
            {
                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand UpdateBusiness = new SqlCommand("UPDATE t_Businesses SET Name = @Name, " +
                        "OwnerId = @OwnerId, GovernmentId = @GovernmentId WHERE Id = @Id;", conn);
                    UpdateBusiness.Parameters.Add(new SqlParameter("Id", Id));
                    UpdateBusiness.Parameters.Add(new SqlParameter("Name", Name));
                    UpdateBusiness.Parameters.Add(new SqlParameter("OwnerId", _Owner.Id));
                    UpdateBusiness.Parameters.Add(new SqlParameter("GovernmentId", _MyGovernment.Id));

                    UpdateBusiness.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while updating business on database " + MyInfo());
                DefaultLog.Error("Sql exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while updating business on database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }
            DefaultLog.Info("Successfully updated business on database " + MyInfo());
            return true;
        }

        public bool DbSelect()
        {
            //throw new NotImplementedException();

            DefaultLog.Info("Starting get business from database " + MyInfo());
            try
            {
                Guid TempOwnerId = Guid.Empty;
                Guid TempGovernmentId = Guid.Empty;

                using (SqlConnection conn = new SqlConnection(DataManager.ConnectionString))
                {
                    conn.Open();

                    SqlCommand SelectBusiness = new SqlCommand("SELECT * FROM t_Businesses WHERE Id = @Id;", conn);
                    SelectBusiness.Parameters.Add(new SqlParameter("Id", _Id));

                    using (SqlDataReader reader = SelectBusiness.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Name = reader[1].ToString();

                            TempOwnerId = new Guid(reader[2].ToString());
                            TempGovernmentId = new Guid(reader[3].ToString());

                        }
                    }
                }

                DefaultLog.Info("Data loaded from businesses table successfully." + MyInfo());

                if (TempOwnerId == Guid.Empty)//Cannot be empty, nulls not allowed
                {
                    DefaultLog.Info("No owner Id found. Not getting owner " + MyInfo());
                }
                else
                {
                    DefaultLog.Info("Owner Id found. Getting owner." + MyInfo());
                    int Index = DataManager.People.FindIndex(f => f.Id.ToString() == TempOwnerId.ToString());

                    if (Index >= 0)
                    {
                        DefaultLog.Info("Owner has already been loaded from the database. " +
                            "Using this instance of owner " + MyInfo());
                        _Owner = DataManager.People[Index];
                    }
                    else
                    {
                        DefaultLog.Info("The businesses owner has not already been loaded from the database. " +
                            "Owner will be loaded " + MyInfo());
                        _Owner = new Person(TempOwnerId);
                        _Owner.DbSelect();
                    }
                }

                if (TempGovernmentId != Guid.Empty)//Cannot be empty, nulls not allowed
                {
                    DefaultLog.Info("No government Id found. Not getting government " + MyInfo());
                }
                else
                {
                    DefaultLog.Info("Government Id found. Getting government " + MyInfo());
                    int Index = DataManager.Governments.FindIndex(f => f.Id.ToString() == TempGovernmentId.ToString());

                    if (Index >= 0)
                    {
                        DefaultLog.Info("The businesses government has already been loaded from the database. " +
                            "Using this instance of government " + MyInfo());

                        _Owner = DataManager.People[Index];
                    }
                    else
                    {
                        DefaultLog.Info("The businesses government has not already been loaded from the database. " +
                            "Government will be loaded " + MyInfo());
                        _MyGovernment = new Government(TempGovernmentId);
                        _MyGovernment.DbSelect();
                    }
                }
            }
            catch (SqlException e)
            {
                DefaultLog.Error("An SQL exception occured while getting business from database " + MyInfo());
                DefaultLog.Error("SQL exception = " + e);
                return false;
            }
            catch (Exception e)
            {
                DefaultLog.Error("A generic exception occured while getting business from database " + MyInfo());
                DefaultLog.Error("Generic exception = " + e);
                return false;
            }

            DefaultLog.Info("Successfully got business from database " + MyInfo());

            return true;
        }

        private string MyInfo()
        {
            string MyString = String.Format("[ Class = Business | Id = {0} | Name = ", _Id);

            if (Name != null && Name != "")
            {
                MyString += Name + " ]";
            }
            else
            {
                MyString += "??? ]";
            }
            return MyString;
        }
    }

    public class Transaction
    {
        public int Ammount { get; }
        public Account FromAcc { get; }
        public Account ToAcc { get; }
        
        public Transaction(int pAmmount, Account pFromAcc, Account pToAcc)
        {
            Ammount = pAmmount;
            FromAcc = pFromAcc;
            ToAcc = pToAcc;
        }


    }
}