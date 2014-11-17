// Add usres to the contest

public partial class AddUsers : System.Web.UI.Page
{	
	protected void Page_Init(object sender, EventArgs e)
	{	
		string strFullAddress = "" + DateTime.Today.ToString("ddMMMyyyy").ToUpper() + ".csv";//holds the full address of the file
		int[] arrOdds = new int[8] {0, 0, 0, 0, 0, 1, 1, 1};//holds all of the oods and the total of the odds, as well as the Number of User and Prizes in order for for the displaying the results to the users
		int intNumberOfMessages = 0;//holds the number of users entred that have been sent messages
		WebClient wcRequest = new WebClient();//holds the object to control of the ftp	
		FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(strFullAddress);//holds the object to contorl the ftop
		
		//sets the access rights for this ftp
		ftpRequest.Credentials = new NetworkCredential("", "");
		wcRequest.Credentials = new NetworkCredential("", "");
					
		try
		{				
			byte[] bytNewFileData = wcRequest.DownloadData(strFullAddress);//holds the byte of the file
			string strFileContent = Encoding.UTF8.GetString(bytNewFileData);//holds the content of the file
			string[] arrPhoneNumberCells = strFileContent.Substring(strFileContent.IndexOf("\r"),  strFileContent.Length - strFileContent.IndexOf("\r")).Split(new string[]{"\r"}, StringSplitOptions.None);//holds the phone numbers and language	
			System.Random ranNumber = new System.Random((int)System.DateTime.Now.Ticks);//holds the object that will random gen numbers
									
			//renames the file on the ftp for backup
			ftpRequest.Method = WebRequestMethods.Ftp.Rename;
			ftpRequest.RenameTo = "BK_SPIN_TO_WIN_" + DateTime.Now.ToString("ddMMMyyyy_hhmm").ToUpper() + ".csv";
			
			//tells the User that the file has been delete
			Response.Write("File Rename: " + ((FtpWebResponse)ftpRequest.GetResponse()).StatusDescription + "<br/>");
		
			//goes around for each phone number that will that will be added to the database
			foreach (string strPhoneNumberParts in arrPhoneNumberCells)
			{
				string[] arrPhoneNumberParts = strPhoneNumberParts.Split(',');//holds the split parts of the phone numbers and language

				//checks if this row is has the phone number and lang to use
				if(arrPhoneNumberParts.Length == 2)
					//plays the odds and adds the user to the database
					arrOdds = General.playingTheOdd(ranNumber, arrOdds, arrPhoneNumberParts[0],arrPhoneNumberParts[1]);
			}//end of for loop
		}//end of try
		catch (WebException ex)
		{
			//checks if this is not a file not found error
			//as it should skip finds that are not found and go on to the
			//next file
			if((int)ex.Status != 7)
			{
				Response.Write("FTP Error: " + ex.ToString());
				
				//sends a email to dev to see why this has program stop
				General.sendErrorMessage("Spin to Win FTP Error", "Hi,<br/><br/>FTP Error has happen in adding user to the contest<br/><br/>Details<br/>" + ex.ToString());
			}//end of if
		}//end of catch
		catch (Exception ex)
		{			
			Response.Write("Error: " + ex.Message + " " + ex.StackTrace);
			
			//sends a email to dev to see why this has program stop
			General.sendErrorMessage("Spin to Win Error", "Hi,<br/><br/>Error has happen in adding user to the contest<br/><br/>Details<br/>" + ex.Message + " " + ex.StackTrace);
		}//end of catch
		
		Response.Write("Added " + arrOdds[0] + " Users<br/>Total $5 Prizes:" + arrOdds[2] + "<br/>Total $10 Prizes:" + arrOdds[3] + "<br/>Total $20 Prizes:" + arrOdds[4] + "<br/>Total Prizes Awarded: " + arrOdds[1] + "<br/>");
					
		DataTable dtUserPhone = DAL.queryDbTable("SELECT * FROM  where  = 0 Order by ");//holds all of the users who has not been sent out yet
				
		//checks if there is any users that the need to have a SMS to get sent out
		if(dtUserPhone != null && dtUserPhone.Rows.Count > 0)
		{
			//deletes all xml files from TempXML in the root directory to make room for more files
			foreach (string strFileTemp in Directory.GetFiles(Server.MapPath("~/TempXML")))
			{
				File.Delete(strFileTemp);
			}//end foreach loop
			
			Response.Write("Delete Root Temp XML Files<br/>");
			
			//deletes all xml files from TempXML in the AddUsers directory to make room for more files
			foreach (string strFileTemp in Directory.GetFiles(Server.MapPath("TempXML")))
			{
				File.Delete(strFileTemp);
			}//end foreach loop
			
			Response.Write("Delete Add Users Temp XML Files<br/>");
			
			//goes around for each phone number that was not sent out and sends out the information
			foreach (DataRow drUserPhone in dtUserPhone.Rows)
			{
				//sends out the entry to the user
				General.CreateXML(drUserPhone[""].ToString().Trim()," = 0");
				
				//update thw users entry that the message was sent out
				DAL.updateSpinSendMessageColoumn(drUserPhone[""].ToString(), true);
				
				//adds to the total number of uses that was sent out
				intNumberOfMessages++;
				
				//checks if the intNumberOfUsers is greater then 1000 if so then 
				//break from the loop as the maxumiun send outs has been reach
				if(intNumberOfMessages >= 1000)
					break;
			}//end of foreach()
		}//end of if
		
		Response.Write("Message Sent " + intNumberOfMessages + " Users");
	}//end of Page_Init()
}//end of Page