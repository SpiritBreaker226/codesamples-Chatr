// Create an .csv in order to display the report on how the contest is doing

public class FileDetails : IComparable
{
	private DateTime dtFileCreatedDate;//holds the datetime of the file
	private string strFileDetails;//holds the details of the file
	private string strFileContent;//holds the content of the file
	private int intNumberOfLines;//holds the number of lines that are in the file of the file
	private bool boolFileEqDatabase;//holds if the number of phone numbers file = database number of phones
	private ArrayList alCSV;//holds the array of a CSVContent object that has both a phone number and lang this a needs to be in here as a file will have more then one line content in it
	private ArrayList alOnlyFile = new ArrayList();//holds the array of a CSVContent object that has both a phone number and lang for only files
	
	#region "Contstutor funcation"
	
	//Default Contstutor
	
	public FileDetails()
	{
		//sets the default for the valuables
		dtFileCreatedDate = new DateTime();
		strFileDetails = "";
		strFileContent = "";
		intNumberOfLines = 0;
		boolFileEqDatabase = false;
		alCSV = new ArrayList();
		alOnlyFile = new ArrayList();
	}//end of Default Contstutor()
	
	//1st Contstutor
	
	public FileDetails(DateTime dtFileCreatedDateValue, string strFileDetailsValue, string strFileContentValue, int intNumberOfLinesValue)
	{
		//sets the values that the user what to use
		dtFileCreatedDate = dtFileCreatedDateValue;
		strFileDetails = strFileDetailsValue;
		strFileContent = strFileContentValue;
		intNumberOfLines = intNumberOfLinesValue;
		boolFileEqDatabase = false;
		alCSV = new ArrayList();
		alOnlyFile = new ArrayList();
	}//end of 1st Contstutor()
	
	//2st Contstutor
	
	public FileDetails(DateTime dtFileCreatedDateValue, string strFileDetailsValue, int intNumberOfLinesValue, ArrayList alCSVContent, ArrayList alOnlyFileContent)
	{
		//sets the values that the user what to use
		dtFileCreatedDate = dtFileCreatedDateValue;
		strFileDetails = strFileDetailsValue;
		strFileContent = "";
		intNumberOfLines = intNumberOfLinesValue;
		boolFileEqDatabase = false;
		alCSV = alCSVContent;
		alOnlyFile = alOnlyFileContent;
	}//end of 2st Contstutor()
	
	#endregion

    #region "public funcation"
		
	//adds the fiiles that are only in the file system to the database
	public int[] addOnlyFilesPhoneNumbersToDatabase(System.Random ranNumber, int[] arrOdds, int intTotalNumberOfUsersForDay)
	{		
		try
		{
			//goes around checking each line to see if there is a
			foreach (CSVContent csvDetails in alOnlyFile)
			{
				string strPhoneNumber = csvDetails.displayPhoneNumber();//holds the Phone Number for this file content
				int intFileNumberPhoneNumber = searchContentNumberOfPhoneNumbers(strPhoneNumber);//holds the number of phone numbers in file for this one Phone Number
											
				//adds the phone number for this user
				arrOdds = General.addMulipleEnties(ranNumber, intFileNumberPhoneNumber, arrOdds, strPhoneNumber, csvDetails.displayLang(), displayFileCreatedDate("yyyy-MM-dd"));
				
				//updates the count of the number of PINS for this time period
				int intSpecialPINumber = DAL.getCountSpecialPIN(DateTime.Now.ToString("MMddtt"), "");//holds the SpecialPIN for this phone number
								
				//checks if there the pins have been  a in a give time period
				if(intSpecialPINumber >= intTotalNumberOfUsersForDay)
					break;
			}//end of foreach
			
			return arrOdds;
		}//end of try
		catch (Exception ex)
		{
			throw ex;
		}//end of catch
	}//end of addOnlyFilesPhoneNumbersToDatabase()
	
	//displays the dtFileCreatedDate to the user
	public string displayFileCreatedDate(string strDateFormat)
	{
		return dtFileCreatedDate.ToString(strDateFormat);
	}//end of displayFileCreatedDate()
	
	//displays the strFileDetails to the user
	public string displayFileDetails()
	{
		return strFileDetails;
	}//end of displayFileDetails()
	
	//displays the strFileContent to the user
	public string displayFileContent()
	{
		return strFileContent;
	}//end of displayFileContent()
		
	//checks if the boolFileEqDatabaseValue is eq to one another and returns the number of lines if there is more out there
	public int IsFileEqDatabase()
	{
		try
		{
			int intNumberPhoneNo = DAL.getCountPhoneNumberDay(dtFileCreatedDate.ToString("yyyy-MM-dd"), "");//holds the all of the entries for this day
	
			//checks if there is a row and if both the file and the database entry are the same
			//if so then set the boolFileEqDatabase to true
			if(intNumberPhoneNo >= intNumberOfLines || intNumberPhoneNo == 0)
				boolFileEqDatabase = true;
				
			//checks if the file is not eq to the database if so then display the number of lines
			if(boolFileEqDatabase == false)
				return intNumberOfLines;
			else
				return -1;
		}//end of try
		catch (Exception ex)
		{			
			throw ex;
		}//end of catch
	}//end of IsFileEqDatabase()
	
	//searches the file content for a phone number and returns the number of phone numbers in this file
	//this is only for not have to use a get funcation for arraylist
	public int searchContentNumberOfPhoneNumbers(string strPhoneNumber)
	{
		try
		{
			int intSearchResults = 0;//holds the number of phone numbers that are the same as strPhoneNumber
						
			//goes around until strPhoneNumber is found and counts the number of times it is in the csv file
			foreach (CSVContent csvDetails in alCSV)
			{
				//checks if the phone number in csv index is the same as the one that the user is looking for
				if (csvDetails.displayPhoneNumber() == strPhoneNumber)
					//adds to the search results
					intSearchResults++;
				//checks if the search results is already been used if so then break
				//in order to make the search a little faster
				else if(intSearchResults > 0)
					break;
			}//end of foreach
			
			return intSearchResults;
		}//end of try
		catch (Exception ex)
		{
			throw ex;
		}//end of catch
	}//end of searchContentNumberOfPhoneNumbers()
			
	#endregion

    #region "overload funcation"
		
	//Compares the two object in order do sorting 
	//it overrides the ArrayList sort
	public int CompareTo(object objComparing)
	{
		//checks if the object is a FileDetails object
		if (objComparing is FileDetails)
		{
		  FileDetails fd2 = (FileDetails)objComparing;//holds the converts objComparing to this FileDetails object
		  
		  //compates the fd2 with this FileDetails object
		  return dtFileCreatedDate.CompareTo(fd2.dtFileCreatedDate);
		}//end of if
		else
		    throw new ArgumentException("This Object is not of type File Details");
	}//end of CompareTo()
	
	#endregion
}//end of class FileDetails

//holds the values of the csv
public class CSVContent : IComparable
{
	private string strPhoneNumber;//holds the phone number of the user
	private string strLang;//holds the default lanueage of the user
	
	//Default Contstutor
	
	public CSVContent()
	{
		//sets the default for the valuables
		strPhoneNumber = "";
		strLang = "";
	}//end of Default Contstutor()
	
	//1st Contstutor
	
	public CSVContent(string strPhoneNumberValue, string strLangValue)
	{
		//sets the values that the user what to use
		strPhoneNumber = strPhoneNumberValue;
		strLang = strLangValue;
	}//end of 1st Contstutor()
	
	//displays the strPhoneNumber to the user
	public string displayPhoneNumber()
	{
		return strPhoneNumber.Trim();
	}//end of displayPhoneNumber()
	
	//displays the strLang to the user
	public string displayLang()
	{
		return strLang.Trim();
	}//end of displayLang()
	
	//Compares the two object in order do sorting 
	//it overrides the ArrayList sort
	public int CompareTo(object objComparing)
	{
		//checks if the object is a CSVContent object
		if (objComparing is CSVContent)
		{
			CSVContent csv2 = (CSVContent)objComparing;//holds the converts objComparing to this CSVContent object
		  
			//compates the csv2 with this CSVContent object
			return string.Compare(strPhoneNumber,csv2.strPhoneNumber);
		}//end of if
		else
			return 0;
		    //throw new ArgumentException("This Object is not of type");
	}//end of CompareTo()
}//end of class CSVContent