using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using BenLog;

namespace CSVImportLib
{
    public class CSVConverter
    {
        string ClassPrefix = "CSV: ";
        string DefaultClass = "CSV: Imported Learners";

        public List<Pupil> Pupils { get { return _Pupils; } }
        private List<Pupil> _Pupils;

        public string FileName { get {
                string[] x = _Dir.Split(new Char[] { '\\' });

                return x.Last();

            }
        }

        private int NoOfColumns;

        private List<string> MyHeadings;

        public string NewFileName
        {
            get
            {
                string[] x = _Dir.Split(new Char[] { '\\' });

                string y = x.Last().Substring(0, x.Last().Length - 4);

                return y + ".ctf";
            }
        }

        public string Dir { get { return _Dir; } }
        private string _Dir;

        public List<Pupil> InvalidPupils { get { return _InvalidPupils; } }
        private List<Pupil> _InvalidPupils;

        public List<string> MissingHeadings { get { return _MissingHeadings; } }
        private List<string> _MissingHeadings;

        public bool ValidationFailed { get { if (HasMissingHeadings || HasInvalidPupils) { return true; } else { return false; } } }

        public bool HasValidFileExtension;

        public bool HasMissingHeadings { get { if (MissingHeadings.Count == 0) { return false; } else { return true; } } }

        public bool HasInvalidPupils { get { if (InvalidPupils.Count() == 0) { return false; } else { return true; } } }


        private int ClassCol = 999;
        private int ForenameCol = 999;
        private int SurnameCol = 999;
        private int DOBCol = 999;
        private int UPNCol = 999;
        private int GenderCol = 999;
        private int ParentEmailCol = 999;

        private bool ValidateFileType(string pDir)
        {
            DefaultLog.Info(String.Format("Checking file extension for \"{0}\"", pDir));
            string MyFileExtension = System.IO.Path.GetExtension(pDir);
            if (MyFileExtension.ToLower() == ".csv")
            {
                DefaultLog.Info("File extension is \".csv\"! Import can be attempted!");
                HasValidFileExtension = true;
                return true;
            }
            else
            {
                DefaultLog.Info(String.Format("File extension is \"{0}\"! File extension MUST be \".csv\". Import can NOT be attempted!"));
                HasValidFileExtension = false;
                return false;
            }
        }

        public CSVConverter(string pDir)
        {
            DefaultLog.Info("Initialising new instance of CSVConverter..."); //V1.3 Changed from 'CSVImporter' to 'CSVConverter'
            _Dir = pDir;

            if (ValidateFileType(pDir))
            {
                GetHeadings();

                FindMissingHeadings();

                GetPupils();
            }
            
            //FindIncompleteRecords(); 1.3 - This is now done whilst data is being loaded from the file.
            DefaultLog.Info("CSVConverter initialised successfully!"); //V1.3 Changed from 'CSVImporter' to 'CSVConverter'
        }

        

        /// <summary>
        /// Finds missing headers and adds them to the 'MissingHeadings' list.
        /// </summary>
        /// <returns>
        /// Returns true if headings are missing.
        /// Returns false if headings are not missing.
        /// </returns>
        private bool FindMissingHeadings()
        {
            DefaultLog.Info("Finding missing headers for: " + _Dir);

            _MissingHeadings = new List<string>();

            if (!MyHeadings.Contains("Forename"))
            {
                MissingHeadings.Add("Forename");
            }

            if (!MyHeadings.Contains("Surname"))
            {
                MissingHeadings.Add("Surname");
            }

            /*if (!MyHeadings.Contains("DOB")) V1.3 These are no longer mandetory
            {
                MissingHeadings.Add("DOB");
            }

            if (!MyHeadings.Contains("UPN"))
            {
                MissingHeadings.Add("UPN");
            }

            if (!MyHeadings.Contains("Parent Email"))
            {
                MissingHeadings.Add("Parent Email");
            }*/

            if (MissingHeadings.Count() != 0)
            {
                DefaultLog.Info(String.Format("{0} missing headings were found. Returning true", MissingHeadings.Count()));
                /*foreach (string Header in Headings)
                {
                    DefaultLog.Info(Header);

                }*/
                return true;
            }
            else
            {
                DefaultLog.Info("No missing headings were found. Returning false");
                return false;
            }
        }

        /// <summary>
        /// Finds incomplete records and adds them to the 'IncompleteRecords' list.
        /// An incomplete record is a record which does not have data in all fields which the 2BAP websuite requires.
        /// </summary>
        /// <returns>
        /// Returns true if one or more records are incomplete.
        /// Returns false if all records are complete.
        /// </returns>
        
            /*private bool FindIncompleteRecords()
        {
            DefaultLog.Info("Finding incomplete records...");

            _IncompletePupils = new List<Pupil>();

            int i = 0;

            foreach (Pupil MyPupil in Pupils)
            {
                i++;
                if (MyPupil.FName == "" ||
                    MyPupil.SName == "" ||
                    MyPupil.DOB == DateTime.MinValue ||
                    MyPupil.UPN == "" ||
                    MyPupil.Gender.ToString() == ""
                    )
                {
                    _IncompletePupils.Add(MyPupil);
                }
            }

            if (IncompletePupils.Count != 0)
            {
                DefaultLog.Info(String.Format("The following {0} pupil records were incomplete: ", IncompletePupils.Count()));
                foreach (Pupil MyPupil in IncompletePupils)
                {
                    DefaultLog.Info(
                        String.Format("Forename: {0} | Surname: {1} | Gender: {2} | DOB: {3} | UPN: {4}",
                        MyPupil.FName,
                        MyPupil.SName,
                        MyPupil.Gender,
                        MyPupil.DOB,
                        MyPupil.UPN));
                }
            }
            if (HasIncompletePupils)
            {
                DefaultLog.Info(String.Format("{0} incomplete records were found. Returning true", IncompletePupils.Count()));
                return true;
            }
            else
            {
                DefaultLog.Info("No incomplete records were found. Returning false");
                return false;
            }
        }
        */

        public void WritePupilToLog(Pupil pPupil)
        {
            DefaultLog.Info(
                        String.Format("LineNo: {0} | Forename: {0} | Surname: {1} | Gender: {2} | DOB: {3} | UPN: {4} | Parent Email: {5}",
                        pPupil.LineNo,
                        pPupil.FName,
                        pPupil.SName,
                        pPupil.Gender,
                        pPupil.DOB,
                        pPupil.UPN,
                        pPupil.ParentEmail));
        }

        public void WritePupilsToLog(List<Pupil> pPupils)
        {
            foreach(Pupil TempPupil in pPupils)
            {
                WritePupilToLog(TempPupil);
            }
            
        }

        public void GetPupils()
        {
            DefaultLog.Info("Getting pupils from CSV: " + _Dir);

            _Pupils = new List<Pupil>();
            _InvalidPupils = new List<Pupil>();
            
            using (StreamReader reader = new StreamReader(_Dir))
            {
                int i = 1;
                reader.ReadLine(); //This is to get it past the headers
                while (!reader.EndOfStream)
                {
                    var Line = reader.ReadLine();
                    var Values = Line.Split(',');

                    //V1.1 - This prevents crash when there is a blank line (no commas)


                    if (i > 0)
                    {
                        if (Values.Count() == NoOfColumns)
                        {

                            /*var Line = reader.ReadLine();
                            var Values = Line.Split(',');*/

                            //DefaultLog.Debug("i = " + i);

                            //This...

                            //...is a simpler way of putting this...
                            /*
                            string Line = reader.ReadLine().ToString();
                            string[] Values = Line.Split(',');
                            */

                            //string MyDefaultClass = ClassPrefix + DefaultClass; //1.3 Added default class
                            string TempClass = "";
                            if (ClassCol != 999)
                            {
                                string MyTempClass = Values[ClassCol].ToString();

                                if (MyTempClass == "" || MyTempClass == " ")
                                {
                                    TempClass = DefaultClass;
                                }
                                else
                                {
                                    TempClass = ClassPrefix + MyTempClass;
                                }
                            }
                            else
                            {
                                TempClass = DefaultClass;
                            }

                            string TempFname = "";
                            if (ForenameCol != 999)
                            {
                                TempFname = Values[ForenameCol].ToString();
                            }

                            string TempSname = "";
                            if (SurnameCol != 999)
                            {
                                //DefaultLog.Debug("BEN!!! Line " + (i + 1) + " SurnameCol = " + SurnameCol);
                                TempSname = Values[SurnameCol].ToString();
                            }

                            DateTime TempDOB = DateTime.MinValue;
                            if (DOBCol != 999)
                            {
                                if (Values[DOBCol].ToString() != "")
                                {
                                    try
                                    {
                                        TempDOB = Convert.ToDateTime(Values[DOBCol].ToString());
                                    }
                                    catch
                                    {
                                        DefaultLog.Error(String.Format("Failed to convert date \"{0}\" on line {1} to DateTime. Value will be set to DateTime min", Values[DOBCol].ToString(), (i + 1)));       
                                    }

                                }
                            }

                            string TempUPN = "";
                            if (UPNCol != 999)
                            {
                                TempUPN = Values[UPNCol].ToString();
                            }

                            Char TempGender = '\0';
                            //Char TempGender = 'D';
                            if (GenderCol != 999)
                            {
                                //V1.3 - Removed 'if' statement so that multi-character genders (Male/Female rather than M/F) can be read
                                if (Values[GenderCol].ToString().Length == 1)
                                {
                                    //TempGender = Convert.ToChar(Values[GenderCol]);

                                    //V1.2 - This means that if someone puts for example 'Male', the app won't crash,
                                    //it'll just convert it to 'M'. This *shouldn't* affect original functionality
                                    //so I didn't add an if statement.
                                    TempGender = Convert.ToChar((Values[GenderCol]).Substring(0, 1).ToUpper());
                                }
                            }

                            string TempParentEmail = "";
                            if (ParentEmailCol != 999)
                            {
                                TempParentEmail = Convert.ToString(Values[ParentEmailCol]);
                            }




                            if (TempClass == "" &&
                                TempFname == "" &&
                                TempSname == "" &&
                                TempDOB.ToString() == DateTime.MinValue.ToString() &&
                                TempUPN == "" &&
                                Convert.ToString(TempGender) == Convert.ToString('\0') &&
                                TempParentEmail == ""
                                )
                            {
                                DefaultLog.Info(String.Format("Line {0} has commas but contains no data. Ignoring line {0}", (i + 1)));
                            }
                            else
                            {
                                Pupil TempPupil = new Pupil(
                                i + 1,
                                TempFname,
                                TempSname
                                );

                                if(TempClass != "" && TempClass != " ")
                                {
                                    TempPupil.ClassName = TempClass;
                                }

                                if (TempDOB != DateTime.MinValue)
                                {
                                    TempPupil.DOB = TempDOB;
                                }

                                if (TempUPN != "" && TempUPN != " ")
                                {
                                    TempPupil.UPN = TempUPN;
                                }

                                if (TempGender.ToString() == "M" || TempGender.ToString() == "F" || TempGender.ToString() == "O")
                                {
                                    TempPupil.Gender = TempGender;
                                }

                                if (TempParentEmail != "" && TempParentEmail != " ")
                                {
                                    TempPupil.ParentEmail = TempParentEmail;
                                }
                                

                                if (TempFname == "" ||
                                TempSname == "" /*||
                                TempDOB == DateTime.MinValue ||
                                TempUPN == "" ||
                                TempGender.ToString() == ""*/
                                )
                                {
                                    _InvalidPupils.Add(TempPupil);
                                }
                                else
                                {
                                    _Pupils.Add(TempPupil);
                                }
                            }
                        }
                        else
                        {
                            DefaultLog.Info(String.Format("Bad record at line {0}. Number of cells on row ({1}) " +
                                "does not match number of columns ({2}). Adding line {0} to 'InvalidPupils' list", (i + 1), Values.Count(), NoOfColumns));

                            InvalidPupils.Add(new Pupil(i + 1));
                        }
                    }
                    else
                    {
                        DefaultLog.Error("This code should not be reached. This is where header reading code used to be! Crashing app...");
                        throw new Exception();
                    }
                    i++;
                }
            }
            if (HasInvalidPupils)
            {
                DefaultLog.Info("Pupils have been loaded successfully but some pupils are invalid: ");
                WritePupilsToLog(InvalidPupils);
            }
            else
            {
                DefaultLog.Info("Got pupils successfully!");
            }
            
        }

        private void GetHeadings()
        {
            NoOfColumns = 0;

            MyHeadings = new List<string>();

            using (StreamReader reader = new StreamReader(_Dir))
            {

                /*while (!reader.EndOfStream) //Don't want to be reading the whole file, maybe change this to an if?
                {*/
                    var Line = reader.ReadLine();
                    var Values = Line.Split(',');

                    /*var Line = reader.ReadLine();
                    var Headings = Line.Split(',');*/

                    int CurrentCol = 0;

                    foreach (var MyHeading in Values)
                    {
                        NoOfColumns++;

                        switch (MyHeading.ToLower())
                        {
                            case "class":
                                ClassCol = CurrentCol;
                                MyHeadings.Add("Class");
                                DefaultLog.Info("Class column: " + ClassCol);
                                CurrentCol++;
                                break;
                            case "forename":
                                ForenameCol = CurrentCol;
                                MyHeadings.Add("Forename");
                                DefaultLog.Info("Forename column: " + ForenameCol);
                                CurrentCol++;
                                break;
                            case "surname":
                                SurnameCol = CurrentCol;
                                MyHeadings.Add("Surname");
                                DefaultLog.Info("Surname column: " + SurnameCol);
                                CurrentCol++;
                                break;
                            case "dob":
                                DOBCol = CurrentCol;
                                MyHeadings.Add("DOB");
                                DefaultLog.Info("DOB column: " + DOBCol);
                                CurrentCol++;
                                break;
                            case "upn":
                                UPNCol = CurrentCol;
                                MyHeadings.Add("UPN");
                                DefaultLog.Info("UPN column: " + UPNCol);
                                CurrentCol++;
                                break;
                            case "gender":
                                GenderCol = CurrentCol;
                                MyHeadings.Add("Gender");
                                DefaultLog.Info("Gender column: " + GenderCol);
                                CurrentCol++;
                                break;
                            case "parent email":
                                ParentEmailCol = CurrentCol;
                                MyHeadings.Add("Parent Email");
                                DefaultLog.Info("Parent email column: " + ParentEmailCol);
                                CurrentCol++;
                                break;
                            default:
                                //V1.1 - Added this because if an unrecognised column was detected, it would be
                                //ignored BUT the CurrentCol counter wouldn't increment.
                                DefaultLog.Info(String.Format("Column {0} has an unrecognised heading: \"{1}\". This will be ignored.", CurrentCol, MyHeading));
                                CurrentCol++;
                                break;
                        //}
                    }
                }
            }
        }

        public string WritePupilsToCTF()
        {
            string MyDir = Dir.Substring(0, Dir.Length - 3) + "ctf";
            WritePupilsToCTF(MyDir);
            return MyDir;
        }

        public void WritePupilsToCTF(string Directory)
        {
            using (StreamWriter MyFile = new StreamWriter(Directory))
            {
                /*if (FindIncompleteRecords() == true)
                {
                    throw new Exception("Validation of pupils failed when writing CTF. You MUST validate before attempting import!");
                }*/
                //MyFile.Write("Some text");

                string DocumentName = "Common Transfer File";
                string CTFversion = "15.0";
                string MyDateTime = DateTimeToCTF(DateTime.Now);
                string DocumentQualifier = "partial";
                string DataDescriptor = "EExBA";
                //string SupplierID = "SIMS";
                //string SourceLEA = "383";
                //string SourceEstab = "2358";
                //string SchoolName = "Yeadon Westfield Infant School";
                //string AcademicYear = "2015";
                //string DestLEA = "LAD";
                //string DestEstab = "DERS";

                //ClassName

                MyFile.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                MyFile.WriteLine("<CTfile>");
                MyFile.WriteLine("<Header>");
                MyFile.WriteLine("<DocumentName>{0}</DocumentName>", DocumentName);
                MyFile.WriteLine("<CTFversion>{0}</CTFversion>", CTFversion);
                MyFile.WriteLine("<DateTime>{0}</DateTime>", MyDateTime);
                MyFile.WriteLine("<DocumentQualifier>{0}</DocumentQualifier>", DocumentQualifier);
                MyFile.WriteLine("<DataDescriptor>{0}</DataDescriptor>", DataDescriptor);
                //MyFile.WriteLine("<SupplierID>{0}</SupplierID>",SupplierID);
                /*MyFile.WriteLine("<SourceSchool>");
                MyFile.WriteLine("<LEA>{0}</LEA>",SourceLEA);
                MyFile.WriteLine("<Estab>{0}</Estab>",SourceEstab);
                MyFile.WriteLine("<SchoolName>{0}</SchoolName>",SchoolName);
                MyFile.WriteLine("<AcademicYear>{0}</AcademicYear>",AcademicYear);
                MyFile.WriteLine("</SourceSchool>");
                MyFile.WriteLine("<DestSchool>");
                MyFile.WriteLine("<LEA>{0}</LEA>", DestLEA);
                MyFile.WriteLine("<Estab>{0}</Estab>", DestEstab);
                MyFile.WriteLine("</DestSchool>");*/
                MyFile.WriteLine("</Header>");
                MyFile.WriteLine("<CTFpupilData>");

                foreach (Pupil MyPupil in Pupils)
                {
                    MyFile.WriteLine("<Pupil>");
                    if(MyPupil.UPN != null)
                    {
                        MyFile.WriteLine("<UPN>{0}</UPN>", MyPupil.UPN);
                    }

                    MyFile.WriteLine("<Surname>{0}</Surname>", MyPupil.SName);
                    MyFile.WriteLine("<Forename>{0}</Forename>", MyPupil.FName);

                    if(MyPupil.DOB != null && MyPupil.DOB != DateTime.MinValue)
                    {
                        MyFile.WriteLine("<DOB>{0}</DOB>", DateToCTF(MyPupil.DOB));
                    }
                    
                    if(MyPupil.Gender.ToString() == "M" || MyPupil.Gender.ToString() == "F" || MyPupil.Gender.ToString() == "O")
                    {
                        MyFile.WriteLine("<Gender>{0}</Gender>", MyPupil.Gender);
                    }

                    if (MyPupil.ClassName != "")
                    {
                        MyFile.WriteLine("<BasicDetails>");
                        MyFile.WriteLine("<NCyearActual>{0}</NCyearActual>", MyPupil.ClassName);
                        MyFile.WriteLine("</BasicDetails>");
                    }


                    if (MyPupil.ParentEmail != "")
                    {
                        MyFile.WriteLine("<Email>{0}</Email>", MyPupil.ParentEmail);
                        //MyFile.WriteLine("</Pupil>"); V1.3 Moved down 2 lines so that pupil tag is closed even if no parent
                                                        //email address is provided
                    }
                    MyFile.WriteLine("</Pupil>"); //V1.3 - See above
                }

                MyFile.WriteLine("</CTFpupilData>");
                MyFile.WriteLine("</CTfile>");
            }
        }

        public static string DateToCTF(DateTime pDateTime)
        {
            string year = pDateTime.ToString().Substring(6, 4);
            string month = pDateTime.ToString().Substring(3, 2);
            string day = pDateTime.ToString().Substring(0, 2);

            
            return String.Format("{0}-{1}-{2}", year, month, day);
        }

        public static string DateTimeToCTF(DateTime pDateTime)
        {
            string date = DateToCTF(pDateTime);
            string time = pDateTime.ToString().Substring(11, 8);

            return date + "T" + time;
        }
    }

    public class ColHead
    {
        public ColHead(int pColumn, string pValue)
        {
            _Value = pValue;
            _Column = pColumn;
        }

        public int Column { get { return _Column; } }
        public string Value { get { return _Value; } }

        private int _Column;
        private string _Value; 
    }

    public class Pupil
    {
        public int LineNo { get; set; }
        public string ClassName { get; set; }
        public string FName { get; set; }
        public string SName { get; set; }
        public DateTime DOB { get; set; }
        public string UPN { get; set; }
        public Char Gender { get; set; }
        public string ParentEmail { get; set; }

        internal Pupil(int pLineNo)
        {
            LineNo = pLineNo;
        }

        /*public Pupil(int pLineNo, string pClassName, string pFName, string pSName, DateTime pDOB, string pUPN, char pGender, string pParentEmail)
        {
            LineNo = pLineNo;
            ClassName = pClassName;
            FName = pFName;
            SName = pSName;
            DOB = pDOB;
            UPN = pUPN;
            Gender = pGender;
            ParentEmail = pParentEmail;
        }

        public Pupil(int pLineNo, string pFName, string pSName, DateTime pDOB, string pUPN, char pGender, string pParentEmail)
        {
            LineNo = pLineNo;
            FName = pFName;
            SName = pSName;
            DOB = pDOB;
            UPN = pUPN;
            Gender = pGender;
            ParentEmail = pParentEmail;
        }

        public Pupil(int pLineNo, string pClassName, string pFName, string pSName, DateTime pDOB, string pUPN, char pGender)
        {
            LineNo = pLineNo;
            ClassName = pClassName;
            FName = pFName;
            SName = pSName;
            DOB = pDOB;
            UPN = pUPN;
            Gender = pGender;
        }

        public Pupil(int pLineNo, string pFName, string pSName, DateTime pDOB, string pUPN, char pGender)
        {
            LineNo = pLineNo;
            FName = pFName;
            SName = pSName;
            DOB = pDOB;
            UPN = pUPN;
            Gender = pGender;
        }*/

        public Pupil(int pLineNo, string pFName, string pSName)
        {
            LineNo = pLineNo;
            FName = pFName;
            SName = pSName;
        }
    }
}
