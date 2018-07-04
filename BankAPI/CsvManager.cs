using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenLog;
using BenCsv;

namespace BankAPI
{
    class CsvManager
    {

        

        public bool CanReadFile(string pDir)
        {
            if (Csv.CanReadFile(pDir))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        List<Person> ReadPeople(string pDir)
        {
            
            if (Csv.CanReadFile(pDir))
            {
                List<string[]> values = Csv.ReadFile(pDir);
                List<Person> MyPeople = new List<Person>();

                string[] columnHeadings = values[0];

                int colFname = 999999999;
                int colSname = 999999999;
                int colGovId = 999999999;

                int headingsFound = 0;
                int currentCol = 0;
                foreach (string heading in columnHeadings)
                {

                    switch (heading.ToLower())
                    {
                        case "fname":
                            colFname = currentCol;
                            headingsFound++;
                            break;
                        case "sname":
                            colSname = currentCol;
                            headingsFound++;
                            break;
                        case "govid":
                            colGovId = currentCol;
                            headingsFound++;
                            break;
                        default:
                            throw new InvalidDataException(string.Format("{0} is not a valid heading for a CSV file of type 'Person'", heading));
                    }

                    currentCol++;
                }


                foreach (string[] myString in values)
                {
                    string valFname;
                    string valSname;
                    Guid valGovId;

                    if (colFname == 999999999)
                    {
                        throw new KeyNotFoundException("There was no column for \"fname\"");
                    }
                    else
                    {
                        valFname = myString[colFname];
                    }

                    if (colSname == 999999999)
                    {
                        throw new KeyNotFoundException("There was no column for \"sname\"");
                    }
                    else
                    {
                        valSname = myString[colSname];
                    }

                    if (colGovId == 999999999)
                    {
                        throw new KeyNotFoundException("There was no column for \"govId\"");
                    }
                    else
                    {
                        try
                        {
                            valGovId = new Guid(myString[colGovId]);
                        }
                        catch
                        {
                            throw new InvalidDataException(String.Format("valGovId \"{0}\" is invalid!", myString[colGovId]));
                        }
                        
                    }

                    Government MyGovernment = DataStore.Governments.Find(Government => Government.Id.ToString() == valGovId.ToString());

                    if(MyGovernment == null)
                    {
                        throw new KeyNotFoundException("Could not find a government in DataStore.Governments which has an Id = " + valGovId);
                    }
                    else
                    {
                        MyPeople.Add(new Person(valFname, valSname, MyGovernment));
                    }
                }
                return MyPeople;

            }
            else
            {
                throw new ArgumentException("The pDir (directory) argument is not a valid CSV file.");
            }
            
        }
    }
}
