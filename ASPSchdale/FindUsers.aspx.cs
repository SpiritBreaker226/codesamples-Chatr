// FInds users who are already in teh contest

public partial class FindUsers : System.Web.UI.Page
{
	private bool boolFinishedLoad = false;//holds if the page is done loading
	private int intTotalNumberOfUsersForDay = 50000;//holds the total number of users entred for this day
	private string strDateFormatCode = "MMddtt";//holds the date code use in special PINS
	
	//does the check if there is the user is in the database but not all of their enties
	private void compareFilesOnlyUser(System.Random ranNumber, FileDetails fdDetails)
	{
		int[] arrOdds = new int[8] {0, 0, 0, 0, 0, 1, 1, 1};//holds all of the oods and the total of the odds, as well as the Number of User and Prizes in order for for the displaying the results to the users
		
		try
		{	
		}//end of try
		catch (Exception ex)
		{			
			Response.Write("File Only User Error: " + ex.Message + " " + ex.StackTrace);
			Response.Flush();
		}//end of catch
	}//end of compareFilesOnlyUser()
	
	//does the check if there is the user is in the database but not all of their enties
	private void compareUserToDatabase(DataTable dtblDayPhoneNumber, System.Random ranNumber, FileDetails fdDetails, string strFileCreatedDate)
	{
		try
		{
			int[] arrOdds = new int[8] {0, 0, 0, 0, 0, 1, 1, 1};//holds all of the oods and the total of the odds, as well as the Number of User and Prizes in order for for the displaying the results to the users
		}//end of try
		catch (Exception ex)
		{			
			Response.Write("User To Database Error: " + ex.Message + " " + ex.StackTrace);
			Response.Flush();
		}//end of catch
	}//end of compareUserToDatabase()
	
	//keeps the screen alive and kicking
	private void keepAlive()
	{
		//goes around send . every 10 seconds
		while (!boolFinishedLoad)
		{
			Response.Write(".");
			Response.Flush();
			Thread.Sleep(10000);
		}//end of while loop
	}//end of keepAlive()
		
	//Main Task
	private void mainTask()
	{
		string strFilename = "CurrentNumberOfUsersPerDay";//holds the Name of the File
		string strFileUsersPerDayContent = "";//holds the content of the file CurrentNumberOfUsersPerDay
		string strFTPFileResponse = "";//holds ftp response of all of the items that are in the FTP
		int intNumberOfUsersForDay = 0;//holds the number of users entred for this day
		FileStream fsFile = null;//holds the file object
		WebClient wcRequest = new WebClient();//holds the object to control of the ftp	
		FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("");//holds the object to contorl the ftop
		System.Random ranNumber = new System.Random((int)System.DateTime.Now.Ticks);//holds the object that will random gen numbers
		
		//sets the access rights for this ftp
		ftpRequest.Credentials = new NetworkCredential("", "");
		wcRequest.Credentials = new NetworkCredential("", "");

		try
		{			
			//gets all of the files on the ftp
			ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
			
			//reads the items and their details
			using(StreamReader srFTPFiles = new StreamReader(ftpResponse.GetResponseStream(), Encoding.Default))
			{
				//reads all of the items and their details in to strFTPFileResponse
    	        strFTPFileResponse = srFTPFiles.ReadToEnd();
			}//end of using

			//reads the item with their details into an array
			string[] arrFileDetails = strFTPFileResponse.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);//holds the each item with their details for each line
			string[] arrCSVFilesContent = new string[arrFileDetails.Length];//holds the array of the strings soin order to sort
			DateTime[] arrFiles = new DateTime[arrFileDetails.Length];//holds the array of the strings soin order to sort
			ArrayList alFD = new ArrayList();//holds the array of a FileDetails object that has the details of the file
			int intSpecialPINumber = DAL.getCountSpecialPIN(DateTime.Now.ToString(strDateFormatCode), "");//holds the count of the phone numbers that where already process for this time period
				
			//goes around for each day that the contest has been on and counts the number of actully rows
			//for that day and writing them to a csv file
			foreach (string strFileDetail in arrFileDetails)
			{
				int intIndexFileDetails = strFileDetail.IndexOf(':');//holds the location of : as it is used to get the name of the file and the time and date of the file as well
				string strFileName = strFileDetail.Substring(intIndexFileDetails + 3).Trim();//holds the file name			
				//checks if this is a file or a folder if a file then prcess it
				if(strFileName.IndexOf('.') > -1)
				{
					int intMonthNumber = 0;//holds which month number to use for the sorting
					int intYearNumber = 0;//holds which year number to use for the sorting
					int intMonthDay = Convert.ToInt32(strFileDetail.Substring(intIndexFileDetails - 5, 3).Trim());//holds which day number to use for the sorting
					byte[] bytNewFileData = wcRequest.DownloadData("" + strFileName);//holds the byte of the file
					string strFileContent = Encoding.UTF8.GetString(bytNewFileData);//holds the content of the file
					string[] arrPhoneNumberCells = strFileContent.Substring(strFileContent.IndexOf("\r"),  strFileContent.Length - strFileContent.IndexOf("\r")).Split(new string[]{"\r"}, StringSplitOptions.None);//holds the phone numbers and language
					ArrayList alCSV = new ArrayList();//holds the array of a CSVContent object that has both a phone number and lang
					ArrayList alOnlyFile = new ArrayList();//holds the array of a CSVContent object that has both a phone number and lang for only files
					
					//checks if wich month/year it is and puts the number the current number for intMonthNumber
					//and intYearNumber in order to sort it currently
					switch(strFileDetail.Substring(intIndexFileDetails - 10, 5).Trim())
					{
						case "Nov":
							intMonthNumber = 11;
							intYearNumber = 2012;
						break;
						case "Dec":
							intMonthNumber = 12;
							intYearNumber = 2012;
						break;
						case "Jan":
							intMonthNumber = 1;
							intYearNumber = 2013;
						break;
						case "Feb":
							intMonthNumber = 2;
							intYearNumber = 2013;
						break;
					}//end of switch
					
					DateTime dtFileCreatedDate = new DateTime(intYearNumber,intMonthNumber,intMonthDay);//holds the file creation date
					int intAllNumberPhoneNos = DAL.getCountPhoneNumberDay(dtFileCreatedDate.ToString("yyyy-MM-dd"), "");//holds the all of the entries for this day
									
					//goes around for each phone number that will that will be added to the database
					//and counts the number of lines in order to tell if this is need now
					foreach (string strPhoneNumberParts in arrPhoneNumberCells)
					{
						string[] arrPhoneNumberParts = strPhoneNumberParts.Split(',');//holds the split parts of the phone numbers and language
							
						//checks if this row is has the phone number and lang to use
						if(arrPhoneNumberParts.Length == 2)
						{						
							//adds to the total number of users added to the database
							intNumberOfUsersForDay++;
						}//end of if
					}//end of foreach loop
												
					//checks if there is a row and if both the file and the database entry are the same
					//if so then set the boolFileEqDatabase to true
					if((intNumberOfUsersForDay - intAllNumberPhoneNos) > 0 && intMonthNumber != 11)
					{				
						Response.Write("Date: " + dtFileCreatedDate.ToString("yyyy-MM-dd") + "<br/>Files: " + intNumberOfUsersForDay + "<br/>Database: " + intAllNumberPhoneNos + "<br/>Difference Between File &amp; Database: " + (intNumberOfUsersForDay - intAllNumberPhoneNos) + "<br/><br/>");
						Response.Flush();
																	
						FileDetails fdDetails = null;//holds the details of the file
						string strFileCreatedDate = dtFileCreatedDate.ToString("yyyy-MM-dd");//holds the when the file was created
						int[] arrOdds = new int[8] {0, 0, 0, 0, 0, 1, 1, 1};//holds all of the oods and the total of the odds, as well as the Number of User and Prizes in order for the displaying  results to the users
						
						//goes around for each phone number that will that will be added to the database
						foreach (string strPhoneNumberParts in arrPhoneNumberCells)
						{
							string[] arrPhoneNumberParts = strPhoneNumberParts.Split(',');//holds the split parts of the phone numbers and language
							
							Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;Start Check<br/><br/>");
							Response.Flush();
								
							//checks if this row is has the phone number and lang to use
							if(arrPhoneNumberParts.Length == 2)
							{
								int intNumberPhoneNo = DAL.getCountPhoneNumberDay(dtFileCreatedDate.ToString("yyyy-MM-dd"), arrPhoneNumberParts[0].Trim());//holds the all of the phone numbers for this day
																
								Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;Phone No: " + arrPhoneNumberParts[0] + "<br/><br/>");
								Response.Flush();
								
								//checks if there is a phone number of this day
								if(intNumberPhoneNo == 0)
								{
									int intNumberOnlyFile = General.searchContentNumberOfPhoneNumbers(arrPhoneNumberParts[0], alOnlyFile);//holds the number of this phone number in alOnlyFile	
									
									 Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;Number in the Database Phone No: " + intNumberPhoneNo + "<br/>&nbsp;&nbsp;&nbsp;&nbsp;Number in the Only File CSV: " + intNumberOnlyFile + "<br/>");
									 Response.Flush();
									
									//check if only one item can be display if not then add it to the alOnlyFile
									if(intNumberOnlyFile == 0)
									{						
										//adds the content of the file int 
										alOnlyFile.Add(new CSVContent(arrPhoneNumberParts[0],arrPhoneNumberParts[1]));
										
										CSVContent csvDetails = new CSVContent(arrPhoneNumberParts[0],arrPhoneNumberParts[1]);//holds the content of the file
										
										string strPhoneNumber = csvDetails.displayPhoneNumber();//holds the Phone Number for this file content
										Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;Phone No: " + strPhoneNumber + " on " + dtFileCreatedDate.ToString("MMM dd, yyyy") + 
										"<br/> &nbsp;&nbsp;&nbsp;&nbsp;Number of Enties in the File: " + 1 + "<br/><br/>");
										Response.Flush();
						
										//adds the phone number for this user
										arrOdds = General.addMulipleEnties(ranNumber, 1, arrOdds, strPhoneNumber, csvDetails.displayLang(), dtFileCreatedDate.ToString("yyyy-MM-dd"));
		
										//checks if there are still gifts avaiable if there is no gifts for anyone of those
										//odds then do not add the user to the database
										if(arrOdds[5] == -1 || arrOdds[6] == -1 || arrOdds[7] == -1)
										{
											string strMessage = "Hi,<br/><br/>There is no more gifts to give out for Database Compare on " + dtFileCreatedDate.ToString("MMM dd, yyyy") + "<br/><br/>";//holds the message that will be sent out
											
											//checks this odd if it is out if so then tell us
											if(arrOdds[5] == -1)
												strMessage += "There is no more $5 gifts<br/><br/>";
																
											//checks this odd if it is out if so then tell us
											if(arrOdds[6] == -1)
												strMessage += "There is no more $10 gifts<br/><br/>";
											
											//checks this odd if it is out if so then tell us
											if(arrOdds[7] == -1)
												strMessage += "There is no more $20 gifts<br/><br/>";
																	
											//sends a email
											General.sendErrorMessage("No Gifts Avaiable", strMessage);
											
											break;
										}//end of if
										
										//checks if not eought gifts if so then make sure there is a breaks to exit
										if(arrOdds[5] == -1 || arrOdds[6] == -1 || arrOdds[7] == -1)
											break;
									}//end of if
									
									//checks if not eought gifts if so then make sure there is a breaks to exit
									if(arrOdds[5] == -1 || arrOdds[6] == -1 || arrOdds[7] == -1)
										break;
								}//end of if
								
								//checks if not eought gifts if so then make sure there is a breaks to exit
								if(arrOdds[5] == -1 || arrOdds[6] == -1 || arrOdds[7] == -1)
									break;
																		
								//adds the content of the file int 
								alCSV.Add(new CSVContent(arrPhoneNumberParts[0],arrPhoneNumberParts[1]));
							}//end of if
														
							Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;End Check<br/>-----------------------------------------<br/><br/>");
							Response.Flush();

							//checks if not eought gifts if so then make sure there is a breaks to exit
							if(arrOdds[5] == -1 || arrOdds[6] == -1 || arrOdds[7] == -1)
							{
								Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;Database Compare Odd Break on " + dtFileCreatedDate.ToString("MMM dd, yyyy") + "<br/><br/>");
								Response.Flush();
								break;
							}//end of if
						}//end of foreach loop
												
						//sorts the file content by phone number in order to search it better
						alCSV.Sort();
						alOnlyFile.Sort();
						
						Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;Number of All CSV: " + alCSV.Count + "<br/>&nbsp;&nbsp;&nbsp;&nbsp;Number of Only File CSV: " + alOnlyFile.Count + "<br/><br/>");
						Response.Flush();
														
						//sets the month and day to the what will display in the csv in order to sort it currently
						alFD.Add(new FileDetails(dtFileCreatedDate,"",intNumberOfUsersForDay,alCSV,alOnlyFile));
						
						//gets the file details from the array
						fdDetails = (FileDetails)alFD[alFD.Count - 1];

						Response.Write("Find Users the File Only<br/>Added " + arrOdds[0] + " Users<br/>Total $5 Prizes:" + arrOdds[2] + "<br/>Total $10 Prizes:" + arrOdds[3] + "<br/>Total $20 Prizes:" + arrOdds[4] + "<br/>Total Prizes Awarded: " + arrOdds[1] + "<br/><br/>");
						Response.Flush();
												
						DataTable dtblDayPhoneNumber = DAL.getColData(" Where CONVERT(VARCHAR(26), , 23) = '" + strFileCreatedDate + "' Order by ","distinct , ");//holds all of the phone numbers for a day that are uniqe AND IsRowValid = 0
						string strCreatedFileDate = dtFileCreatedDate.ToString("yyyyMMdd");//holds the date of creation
																					
						//checks if there is a phone number of this day
						if(dtblDayPhoneNumber != null)
						{
							//resets the odds for this seciton
							arrOdds = new int[8] {0, 0, 0, 0, 0, 1, 1, 1};
							
							//goes around for each phone number that is for this 
							foreach(DataRow drblDayPhoneNumber in dtblDayPhoneNumber.Rows)
							{
								int intFileNumberPhoneNumber = fdDetails.searchContentNumberOfPhoneNumbers(drblDayPhoneNumber[""].ToString());//holds the number of phone numbers in file for this one Phone Number
								int intNumberPhoneNo = DAL.getCountPhoneNumberDay(dtFileCreatedDate.ToString("yyyy-MM-dd"), drblDayPhoneNumber[""].ToString());//holds the all of the phone numbers for this day
								int intDiffBetweenFilesDatabase = (intFileNumberPhoneNumber - intNumberPhoneNo);//holds the difference between the file and the database for this phone number on this day
								
								Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;Phone No: " + drblDayPhoneNumber[""].ToString() + " on " + strFileCreatedDate + 
								"<br/> &nbsp;&nbsp;&nbsp;&nbsp;Number That is in the File: " + intFileNumberPhoneNumber + 
								"<br/> &nbsp;&nbsp;&nbsp;&nbsp;Number That is in the Database: " + intNumberPhoneNo + 
								"<br/> &nbsp;&nbsp;&nbsp;&nbsp;Difference Between Files &amp; Database: " + intDiffBetweenFilesDatabase + "<br/><br/>");
								Response.Flush();
								
								//checks if the phone number is in the database
								if(intNumberPhoneNo > 0)
								{														
									//checks if there is a difference between the file and the database
									//if so then add the phone number to the add special pin and adds
									//as may as there it is need for this phone number on this day		
									if(intDiffBetweenFilesDatabase > 0)				
										//adds the phone number for this user
										arrOdds = General.addMulipleEnties(ranNumber, intDiffBetweenFilesDatabase, arrOdds, drblDayPhoneNumber[""].ToString(), drblDayPhoneNumber[""].ToString(), strFileCreatedDate);
									else
										//updates all of the phone number for this day as it is all site and ready to go
										DAL.updateRowValid(strFileCreatedDate, drblDayPhoneNumber[""].ToString(), true);
								}//end of if
								else
								{
									Response.Write("Phone No: " + drblDayPhoneNumber[""].ToString() + " on " + strFileCreatedDate + "<br/>");
									Response.Flush();
								}//end of if
																
								//checks if there are still gifts avaiable if there is no gifts for anyone of those
								//odds then do not add the user to the database
								if(arrOdds[5] == -1 || arrOdds[6] == -1 || arrOdds[7] == -1)
								{
									string strMessage = "Hi,<br/><br/>There is no more gifts to give out for Files Only on " + dtFileCreatedDate.ToString("MMM dd, yyyy") + "<br/><br/>";//holds the message that will be sent out
									
									//checks this odd if it is out if so then tell us
									if(arrOdds[5] == -1)
										strMessage += "There is no more $5 gifts<br/><br/>";
														
									//checks this odd if it is out if so then tell us
									if(arrOdds[6] == -1)
										strMessage += "There is no more $10 gifts<br/><br/>";
									
									//checks this odd if it is out if so then tell us
									if(arrOdds[7] == -1)
										strMessage += "There is no more $20 gifts<br/><br/>";
															
									//sends a email
									General.sendErrorMessage("No Gifts Avaiable", strMessage);
									
									Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;For Files Only Odd Break on " + dtFileCreatedDate.ToString("MMM dd, yyyy") + "<br/><br/>");
									Response.Flush();
									
									break;
								}//end of if
							}//end of foreach
											
							Response.Write("<br/>Find Users in that where in the Database but the number of PINs where lower" + " on " + strFileCreatedDate + "<br/>Added " + arrOdds[0] + " Users<br/>Total $5 Prizes:" + arrOdds[2] + "<br/>Total $10 Prizes:" + arrOdds[3] + "<br/>Total $20 Prizes:" + arrOdds[4] + "<br/>Total Prizes Awarded: " + arrOdds[1] + "<br/><br/>");
							Response.Flush();
						}//end of if
					}//end of if
								
					//resets intNumberOfUsersForDay for the next row
					intNumberOfUsersForDay = 0;
				}//end of if
			}//end of foreach loop
						
			Response.Write("<br/><br/>End of Task");
			Response.Flush();
			
			//turns off the keep alive			
			boolFinishedLoad = true;
		}//end of try
		catch (WebException ex)
		{
			//checks if this is not a file not found error
			//as it should skip finds that are not found and go on to the next file
			if((int)ex.Status != 7)
			{
				Response.Write("FTP Error: " + ex.ToString());
				Response.Flush();
			}//end of if
		}//end of catch
		catch (Exception ex)
		{			
			Response.Write("Find User Error: " + ex.Message + " " + ex.StackTrace);
			Response.Flush();
		}//end of catch
	}//end of mainTask()
	
	protected void Page_Init(object sender, EventArgs e)
	{
		Task[] tasks = new Task[2];//holds the task that will be running on the page
		
		//this will keep alive the page as the main task is going on
		tasks[0] = Task.Factory.StartNew(() => keepAlive());
							
		//gets all of the file content and checks if all of the users are in the database
		tasks[1] = Task.Factory.StartNew(() => mainTask());
		
		//waits until all task are done
		Task.WaitAll(tasks);
	}//end of Page_Init()
}//end of Page