using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankAPI;
using BenLog;

namespace BankConsoleTest
{
    class BankConsoleTest
    {
        static void Main(string[] args)
        {
            DefaultLog.ClearLog();

            Text.Title("-----< Loading >-----");
            Console.Write("Loading items from database. Please wait... ");

            DateTime LoadStart = DateTime.Now;

            if (BankAPI.DataManager.LoadDb())
            {
                DateTime LoadEnd = DateTime.Now;
                Text.Pass();
                var Difference = (LoadEnd - LoadStart).TotalSeconds;
                Console.WriteLine("Loaded in {0} seconds", Difference);
            }
            else
            {
                Text.Fail();
            }
            Text.Title("-----< /Loading >-----");

            
            string GovernmentName = "MyGovernment";
            string NewGovernmentName = "MyNewGovernment";


            Government MyGovernment = null;
            Government MyNewGovernment = null;
            bool GovernmentSuccess = TestGovernment();
            Console.WriteLine();

            Person MyPerson = null;
            Person MyNewPerson = null;
            bool PersonSuccess = TestPerson();
            Console.WriteLine();

            Business MyBusiness = null;
            Business MyNewBusiness = null;
            bool BusinessSuccess = TestBusiness();
            Console.WriteLine();

            Bank MyBank = null;
            Bank MyNewBank = null;
            bool BankSuccess = TestBank();
            Console.WriteLine();

            PersonalAccount MyPersonalAccount = null;
            PersonalAccount MyNewPersonalAccount = null;
            bool PersonalAccountSuccess = TestPersonalAccount();
            Console.WriteLine();

            BusinessAccount MyBusinessAccount = null;
            BusinessAccount MyNewBusinessAccount = null;
            bool BusinessAccountSuccess = TestBusinessAccount();
            Console.WriteLine();

            Console.ReadLine();

            bool TestGovernment() {
                Text.Title("-----< class Government >-----");

                bool Success = true;


                Console.Write("Test 1 - Create new instance of class ");
                bool Test1 = true;
                MyGovernment = null;
                try
                {
                    MyGovernment = new Government(GovernmentName);
                }
                catch
                {
                    Test1 = false;
                    Success = false;
                }
                if (Test1)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }


                Console.Write("Test 2 - Insert into database ");
                if (Test1)
                {
                    if (MyGovernment.DbInsert())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                        //Console.ReadLine();
                    }
                }
                else
                {
                    Text.Skipped();
                }


                Console.Write("Test 3 - Change properties ");
                bool Test3 = true;
                try
                {
                    MyGovernment.Name = NewGovernmentName;
                }
                catch
                {
                    Success = false;
                    Test3 = false;
                }

                if (Test3)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }


                Console.Write("Test 4 - Update database ");
                if (Test3)
                {
                    if (MyGovernment.DbUpdate())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                        //Console.ReadLine();
                    }
                }
                else
                {
                    Text.Skipped();
                }

                Console.Write("Test 5 - Create new instance of class with the same Id ");

                bool Test5 = true;

                MyNewGovernment = null;
                try
                {
                    MyNewGovernment = new Government(MyGovernment.Id);
                }
                catch
                {
                    Success = false;
                    Test5 = false;
                }

                if (Test5)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }

                Console.Write("Test 6 - Select from database ");
                bool Test6 = true;
                if (Test5)
                {
                    if (MyNewGovernment.DbSelect())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Test6 = false;
                        Success = false;
                        Text.Fail();
                        //Console.ReadLine();
                    }
                }
                else
                {
                    Test6 = false;
                    Text.Skipped();
                }

                /*Console.Write("Test 7 - Validate from database ");
                if (Test6)
                {
                    try 
                    {
                        if(MyGovernment.Id.ToString() != MyNewGovernment.Id.ToString())
                        {
                            throw new Exception();
                        }

                        Text.Pass();
                    }
                    catch
                    {
                        Success = false;
                        Text.Fail();
                        //Console.ReadLine();
                    }
                }
                else
                {
                    Text.Skipped();
                }*/

                Text.Title("-----<\\ class Government >-----");
                return Success;
            }

            bool TestPerson()
            {
                bool Success = true;

                Text.Title("-----< class Person >-----");
                bool Test1 = true;
                Console.Write("Test 1 - Create new instance of class ");
                try
                {
                    MyPerson = new Person("Joe", "Bloggs", MyGovernment);
                }
                catch
                {
                    Test1 = false;
                }

                if (Test1)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }


                Console.Write("Test 2 - Insert into database ");
                if (Test1)
                {


                    if (MyPerson.DbInsert())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                    }
                }
                else
                {
                    Text.Skipped();
                }

                Console.Write("Test 3 - Change properties ");

                bool Test3 = true;

                try
                {
                    MyPerson.Forename = "Jon";
                    MyPerson.Surname = "Doe";
                }
                catch
                {
                    Success = false;
                    Test3 = false;
                }

                if (Test3)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }

                Console.Write("Test 4 - Update database ");
                if (Test3)
                {
                    if (MyPerson.DbUpdate())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                    }
                }
                else
                {
                    Text.Skipped();
                }

                Console.Write("Test 5 - Create new instance of class with the same Id ");
                bool Test5 = true;
                try
                {
                    MyNewPerson = new Person(MyPerson.Id);
                }
                catch
                {
                    Test5 = false;
                    Success = false;
                }

                if (Test5)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }

                Console.Write("Test 6 - Select from database ");
                if (Test5)
                {
                    if (MyNewPerson.DbSelect())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                    }
                }
                else
                {
                    Text.Skipped();
                }
                Text.Title("-----<\\ class Person >-----");
                return Success;
            }

            bool TestBusiness()
            {
                Text.Title("-----< class Business >-----");
                bool Success = true;

                Console.Write("Test 1 - Create new instance of class ");
                bool Test1 = true;
                try
                {
                    MyBusiness = new Business(MyGovernment, MyPerson, "MyBusiness");
                }
                catch
                {
                    Test1 = false;
                }

                if (Test1)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }

                Console.Write("Test 2 - Insert into database ");
                if (Test1)
                {
                    if (MyBusiness.DbInsert())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                    }
                }
                else
                {
                    Text.Skipped();
                }



                Console.Write("Test 3 - Change properties ");
                bool Test3 = true;
                try
                {
                    MyBusiness.Name = "Changed business name";
                    //MyBusiness.MyGovernment
                }
                catch
                {
                    Success = false;
                    Test3 = false;
                }

                if (Test3)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }

                Console.Write("Test 4 - Update database ");
                if (Test3)
                {
                    if (MyBusiness.DbUpdate())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                    }

                }
                else
                {
                    Text.Skipped();
                }


                Console.Write("Test 5 - Create new instance of class with the same Id ");
                bool Test5 = true;
                try
                {
                    MyNewBusiness = new Business(MyBusiness.Id);
                }
                catch
                {
                    Success = false;
                    Test5 = false;
                }

                if (Test5)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }

                Console.Write("Test 6 - Select from database ");
                if (Test5)
                {
                    if (MyNewBusiness.DbSelect())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                    }
                }
                else
                {
                    Text.Skipped();
                }
                Text.Title("-----<\\ class Business >-----");
                return Success;
            }

            bool TestBank() {

                bool Success = true;

                Text.Title("-----< class Bank >-----");



                Console.Write("Test 1 - Create new instance of class ");
                bool Test1 = true;
                try
                {
                    decimal TempSavingsInterestRate = Convert.ToDecimal(0.03);
                    decimal TempLoanInterestRate = Convert.ToDecimal(0.05);
                    MyBank = new Bank(MyGovernment, "MyBank", MyPerson, TempSavingsInterestRate, TempLoanInterestRate);
                }
                catch
                {
                    Test1 = false;
                    Success = false;
                }

                if (Test1)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }

                Console.Write("Test 2 - Insert into database ");
                if (Test1)
                {
                    if (MyBank.DbInsert())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                    }
                }
                else
                {
                    Text.Skipped();
                }


                Console.Write("Test 3 - Change properties ");
                bool Test3 = true;
                try
                {
                    MyBank.Name = "Changed bank name";
                }
                catch
                {
                    Success = false;
                    Test3 = false;
                }

                if (Test3)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }

                Console.Write("Test 4 - Update database ");
                if (Test3)
                {
                    if (MyBank.DbUpdate())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                    }

                }
                else
                {
                    Text.Skipped();
                }

                Console.Write("Test 5 - Create new instance of class with the same Id ");
                bool Test5 = true;
                try
                {
                    MyNewBank = new Bank(MyBank.Id);
                }
                catch
                {
                    Success = false;
                    Test5 = false;
                }

                if (Test5)
                {
                    Text.Pass();
                }
                else
                {
                    Text.Fail();
                }

                Console.Write("Test 6 - Select from database ");
                if (Test5)
                {
                    if (MyNewBank.DbSelect())
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Success = false;
                        Text.Fail();
                    }
                }
                else
                {
                    Text.Skipped();
                }

                Text.Title("-----<\\ class Bank >-----");

                return Success;
            }
            

            bool TestPersonalAccount() {
                


                    bool Success = true;

                    Text.Title("-----< class PersonalAccount >-----");
                if (BankSuccess)
                {
                    Console.Write("Test 1 - Create new instance of class ");
                    bool Test1 = true;
                    try
                    {
                        MyPersonalAccount = new PersonalAccount(MyPerson, MyBank);
                    }
                    catch
                    {
                        Test1 = false;
                        Success = false;
                    }

                    if (Test1)
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Text.Fail();
                    }

                    Console.Write("Test 2 - Insert into database ");
                    if (Test1)
                    {
                        if (MyPersonalAccount.DbInsert())
                        {
                            Text.Pass();
                        }
                        else
                        {
                            Text.Fail();
                        }
                    }
                    else
                    {
                        Text.Skipped();
                    }



                    Console.Write("Test 3 - Change properties ");
                    bool Test3 = true;
                    try
                    {
                        //MyPersonalAccount.
                    }
                    catch
                    {
                        Success = false;
                        Test3 = false;
                    }

                    if (Test3)
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Text.Fail();
                    }

                    Console.Write("Test 4 - Update database ");
                    if (Test3)
                    {
                        if (MyPersonalAccount.DbUpdate())
                        {
                            Text.Pass();
                        }
                        else
                        {
                            Text.Fail();
                        }
                    }
                    else
                    {
                        Text.Skipped();
                    }

                    Console.Write("Test 5 - Create new instance of class with the same Id ");
                    bool Test5 = true;
                    try
                    {
                        MyNewPersonalAccount = new PersonalAccount(MyPersonalAccount.Id);
                    }
                    catch
                    {
                        Test5 = false;
                        Success = false;
                    }

                    if (Test5)
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Text.Fail();
                    }

                    Console.Write("Test 6 - Select from database ");
                    if (Test5)
                    {
                        if (MyNewPersonalAccount.DbSelect())
                        {
                            Text.Pass();
                        }
                        else
                        {
                            Success = false;
                            Text.Fail();
                        }
                    }
                    else
                    {
                        Text.Skipped();
                    }
                }
                else
                {
                    Text.Skipped("Skipped due to errors whilst testing \"Bank\" class. ");
                    return false;
                }
                Text.Title("-----<\\ class PersonalAccount >-----");

                    return Success;
                
        }

            bool TestBusinessAccount()
            {
                


                    bool Success = true;

                    Text.Title("-----< class BusinessAccount >-----");
                if (BankSuccess)
                {
                    Console.Write("Test 1 - Create new instance of class ");
                    bool Test1 = true;
                    try
                    {
                        MyBusinessAccount = new BusinessAccount(MyBusiness, MyBank);
                    }
                    catch
                    {
                        Test1 = false;
                        Success = false;
                    }

                    if (Test1)
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Text.Fail();
                    }

                    Console.Write("Test 2 - Insert into database ");
                    if (Test1)
                    {
                        if (MyBusinessAccount.DbInsert())
                        {
                            Text.Pass();
                        }
                        else
                        {
                            Text.Fail();
                        }
                    }
                    else
                    {
                        Text.Skipped();
                    }



                    Console.Write("Test 3 - Change properties ");
                    bool Test3 = true;
                    try
                    {
                        //MyBusinessAccount.
                    }
                    catch
                    {
                        Success = false;
                        Test3 = false;
                    }

                    if (Test3)
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Text.Fail();
                    }

                    Console.Write("Test 4 - Update database ");
                    if (Test3)
                    {
                        if (MyBusinessAccount.DbUpdate())
                        {
                            Text.Pass();
                        }
                        else
                        {
                            Text.Fail();
                        }
                    }
                    else
                    {
                        Text.Skipped();
                    }

                    Console.Write("Test 5 - Create new instance of class with the same Id ");
                    bool Test5 = true;
                    try
                    {
                        MyNewBusinessAccount = new BusinessAccount(MyBusinessAccount.Id);
                    }
                    catch
                    {
                        Test5 = false;
                        Success = false;
                    }

                    if (Test5)
                    {
                        Text.Pass();
                    }
                    else
                    {
                        Text.Fail();
                    }

                    Console.Write("Test 6 - Select from database ");
                    if (Test5)
                    {


                        if (MyNewBusinessAccount.DbSelect())
                        {
                            Text.Pass();
                        }
                        else
                        {
                            Success = false;
                            Text.Fail();
                        }
                    }
                    else
                    {
                        Text.Skipped();
                    }
                    

                }
                else
                {
                    Text.Skipped("Skipped due to errors whilst testing \"Bank\" class. ");
                    Success = false ;
                }
                Text.Title("-----<\\ class BusinessAccount >-----");

                return Success;
            }
            /*LogReader MyNewReader = new LogReader(@"F:\Coding\Bank\BankConsoleTest\bin\Debug\log.txt");
            MyNewReader.DisplayParsedLog();*/
            
    }
    }
    
    public static class Text
    {
        public static void Skipped()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Skipped");
            Console.ResetColor();
        }

        public static void Skipped(string pString)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(pString); 
            Console.ResetColor();
        }

        public static void Pass()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Pass");
            Console.ResetColor();
        }

        public static void Fail()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Fail");
            Console.ResetColor();
        }

        public static void Title(string pString)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(pString);
            Console.ResetColor();
        }

        public static void WriteNewInst(string pClass, string pInstanceName)
        {
            WriteKey("new");
            Console.Write(" ");
            WriteClass(pClass);
            WriteNormal("()");
            Console.Write(" called ");
            WriteNormal(pInstanceName);
        }

        public static void WriteClassVar(string pClass, string pInstanceName)
        {
            WriteClass(pClass);
            Console.Write(" ");
            WriteNormal(pInstanceName);
        }

        public static void WriteKey(string pClass)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(pClass);
            Console.ResetColor();
        }

        public static void WriteClass(string pClass)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(pClass);
            Console.ResetColor();
        }

        public static void WriteNormal(string pString)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(pString);
            Console.ResetColor();
            
        }
    }
}
