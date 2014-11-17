// Genral Functions of the site

public class General
{
	#region "public functions"	
	
	//adds user from find users as both the only files phone number and database phone numbers 
	//need to have a loop to add muliple enties with the same phone number
	public static int[] addMulipleEnties(System.Random ranNumber, int intDiffBetweenFilesDatabase, int[] arrOdds, string strPhoneNumber, string strLang, string strFileCreatedDate)
	{
		try
		{
			int intSpecialPINumber = DAL.getCountSpecialPIN("", strPhoneNumber.Trim());//holds the SpecialPIN for this phone number
			
			//checks if the phone number is in the database if not then add it to the table
			if(intSpecialPINumber == 0)
				//adds the users number to the list of people who did not get their emails
				DAL.addUpdateSpecialPIN(0, strPhoneNumber, DateTime.Now.ToString("MMddtt"));
			
			//goes around until both the phone number in the file and database are equal
			for (int intIndex = 0;intIndex < intDiffBetweenFilesDatabase;intIndex++)
			{
				//plays the odds and adds the user to the database
				arrOdds = playingTheOdd(ranNumber,arrOdds,strPhoneNumber, strLang, strFileCreatedDate);
			}//end of for loop
			
			//updates all of the phone number for this day as it is all site and ready to go
			DAL.updateRowValid(strFileCreatedDate, strPhoneNumber, true);
						
			return arrOdds;
		}//end of try
		catch (Exception ex)
		{
			throw ex;
		}//end of catch
	}//end of addMulipleEnties()
	
	//creates a pin number
	public static int createPin(System.Random ranNumber)
	{
		int intPin = ranNumber.Next(999999, 9999999);//holds the random number of the pin
		DataTable dtlSpinToWin = DAL.getColData(" Where  = '" + intPin + "'", "");//holds the spin to win: generted pin

		//goes around checks if this random number has been used if so then create aother random number
		//until a number that has not been used is display
		while(dtlSpinToWin != null)
		{
			//creates another random and get it from the database
			//in order to check if it is in the database
			intPin = ranNumber.Next(999999, 9999999);
			dtlSpinToWin = DAL.getColData(" Where  = '" + intPin + "'", "");
		}//end of if
				
		return intPin;
	}//end of createPin()
	
	public static void CreateXML(string phoneNumber, string strSQLAND = "ISNULL([],'') = ''")
    {
        try
        {
            xiamSMS objxiamSMS = new xiamSMS();

            DataTable dataTableSmsRequest = DAL.queryDbTable("SELECT * FROM  WHERE [] = '" + phoneNumber + "' AND " + strSQLAND + " OR [] = '1" + phoneNumber + "' AND " + strSQLAND + " ORDER BY ;");

            if (dataTableSmsRequest != null && dataTableSmsRequest.Rows.Count > 0)
            {
                xiamSMSSubmitRequest[] xiamSMSSubmitRequests = new xiamSMSSubmitRequest[dataTableSmsRequest.Rows.Count];
                int i = 0;
				
                foreach (DataRow dataRowSmsRequest in dataTableSmsRequest.Rows)
                {
                    xiamSMSSubmitRequest objxiamSMSSubmitRequest = new xiamSMSSubmitRequest();
                    objxiamSMSSubmitRequest.id = dataRowSmsRequest[""].ToString();
                    objxiamSMSSubmitRequest.from = "";
                    objxiamSMSSubmitRequest.to = dataRowSmsRequest[""].ToString();

                    //content
                    xiamSMSSubmitRequestContent objxiamSMSSubmitRequestContent = new xiamSMSSubmitRequestContent();
                    objxiamSMSSubmitRequestContent.type = "text";
					
                    if (dataRowSmsRequest["LanguageCode"].ToString().Trim() == "f")
                        objxiamSMSSubmitRequestContent.Value = "chatr ROUE CHANCEUSE: Merci d'avoir reapprovisionner. Ton NIP de concours: " + dataRowSmsRequest[""].ToString() + "  . Consulte: http://chatrrouechanceuse.com/. Desabonnement: chatrsansfil.com/stop";
                    else
                        objxiamSMSSubmitRequestContent.Value = "chatr SPIN to WIN: Thanks for topping up. Your Contest PIN is " + dataRowSmsRequest[""].ToString() + "  . Go to http://chatrspintowin.com/ to see if you're a winner. To opt-out go to chatrwireless.com/stop";

                    objxiamSMSSubmitRequest.content = objxiamSMSSubmitRequestContent;

                    //sendOnGroup
                    xiamSMSSubmitRequestSendOnGroup objxiamSMSSubmitRequestSendOnGroup = new xiamSMSSubmitRequestSendOnGroup();
                    objxiamSMSSubmitRequestSendOnGroup.value = "RG_PROD";
                    objxiamSMSSubmitRequestSendOnGroup.Value = " ";
                    objxiamSMSSubmitRequest.sendOnGroup = objxiamSMSSubmitRequestSendOnGroup;

                    //requestDeliveryReport
                    xiamSMSSubmitRequestRequestDeliveryReport objxiamSMSSubmitRequestRequestDeliveryReport = new xiamSMSSubmitRequestRequestDeliveryReport();
                    objxiamSMSSubmitRequestRequestDeliveryReport.value = "yes";
                    objxiamSMSSubmitRequestRequestDeliveryReport.Value = " ";
                    objxiamSMSSubmitRequest.requestDeliveryReport = objxiamSMSSubmitRequestRequestDeliveryReport;

                    xiamSMSSubmitRequests[i] = objxiamSMSSubmitRequest;
					
                    i++;
                }//end of foreach
				
                objxiamSMS.submitRequest = xiamSMSSubmitRequests;
				
				string xiamSMSXml = CreateUserXml(objxiamSMS);
	            WriteXmlToFile(xiamSMSXml, phoneNumber);
	            ConsumeWebService(xiamSMSXml, phoneNumber);
            }//end of if
        }//end of try
        catch (Exception ex)
        {
            throw ex;
        }//end of catch
    }//end of CreateXML()
	
	//plays the odds and adds the user to the database
	public static int[] playingTheOdd(System.Random ranNumber, int[] arrOddTotal, string strPhoneNumber, string strLang, string strDateEntryCreatedDate = "")
	{
		string strPin = createPin(ranNumber).ToString();//holds the pin that is being created this person
		string strPrizeAmount = "";//holds the amount of the prize
		bool boolIsWinner = false;//holds if this user is a winner
		
		/*
		
			arrOdds - Table of Contents
			
			intNumberOfUsers - holds the number of users entred
			intNumberPrizes - holds the number of winning prizes sent out
			intSessionTotalOdd5 - holds this session total odds of winning the 5 dollar prize
			intSessionTotalOdd10 - holds this session total odds of winning the 10 dollar prize
			intSessionTotalOdd20 - holds this session total odds of winning the 20 dollar prize
			intOdd5 - holds the odds of winning the 5 dollar prize
			intOdd10 - holds the odds of winning the 10 dollar prize
			intOdd20 - holds the odds of winning the 20 dollar prize
		
		*/
				
		//checks if this user is the winner of the five dollor prize
		if(arrOddTotal[5] >= 5)
		{
			DataTable dtTotalGift = DAL.queryDbTable("SELECT * FROM  where Value = '5'");//holds total of gifts avaiable
			DataTable dtGiveOutTotalGift = DAL.queryDbTable("SELECT * FROM  where  = '5'");//holds current number gifts given out
			
			//checks if there is ehough prizes to give out if so then set the amount and boolIsWinner
			if(dtTotalGift.Rows.Count >= dtGiveOutTotalGift.Rows.Count)
			{
				//sets the amount for this prize and boolIsWinner for it to display
				//for this user
				strPrizeAmount = "5";
				boolIsWinner = true;
				
				//adds to the arrOddTotal[1] total
				arrOddTotal[1]++;
			
				//adds to the session total for this odd
				arrOddTotal[2]++;
		
				//resets the odds for the next round
				arrOddTotal[5] = 1;
			}//end of if
			else
				//sets this arrOddTotal to -1 to tell that there is no more gifts for this odd
				arrOddTotal[5] = -1;
		}//end of if
		else
			arrOddTotal[5]++;
			
		//checks if this user is the winner of the ten dollor prize
		if(arrOddTotal[6] >= 183 && string.IsNullOrEmpty(strPrizeAmount))
		{
			DataTable dtTotalGift = DAL.queryDbTable("SELECT * FROM  where Value = '10'");//holds total of gifts avaiable
			DataTable dtGiveOutTotalGift = DAL.queryDbTable("SELECT * FROM  where  = '10'");//holds current number gifts given out
			
			//checks if there is ehough prizes to give out if so then set the amount and boolIsWinner
			if(dtTotalGift.Rows.Count >= dtGiveOutTotalGift.Rows.Count)
			{
				//sets the amount for this prize and boolIsWinner for it to display
				//for this user
				strPrizeAmount = "10";
				boolIsWinner = true;
				
				//adds to the arrOddTotal[1] total
				arrOddTotal[1]++;
				
				//adds to the session total for this odd
				arrOddTotal[3]++;
				
				//resets the odds for the next round
				arrOddTotal[6] = 1;
			}//end of if
			else
				//sets this arrOddTotal to -1 to tell that there is no more gifts for this odd
				arrOddTotal[6] = -1;
		}//end of if
		else
			arrOddTotal[6]++;
			
		//checks if this user is the winner of the twenty dollor prize
		if(arrOddTotal[7] >= 183 && string.IsNullOrEmpty(strPrizeAmount))
		{
			DataTable dtTotalGift = DAL.queryDbTable("SELECT * FROM  where Value = '20'");//holds total of gifts avaiable
			DataTable dtGiveOutTotalGift = DAL.queryDbTable("SELECT * FROM  where  = '20'");//holds current number gifts given out
			
			//checks if there is ehough prizes to give out if so then set the amount and boolIsWinner
			if(dtTotalGift.Rows.Count >= dtGiveOutTotalGift.Rows.Count)
			{
				//sets the amount for this prize and boolIsWinner for it to display
				//for this user
				strPrizeAmount = "20";
				boolIsWinner = true;
				
				//adds to the arrOddTotal[1] total
				arrOddTotal[1]++;
				
				//adds to the session total for this odd
				arrOddTotal[4]++;
				
				//resets the odds for the next round
				arrOddTotal[7] = 1;
			}//end of if
			else
				//sets this arrOddTotal to -1 to tell that there is no more gifts for this odd
				arrOddTotal[7] = -1;
		}//end of if
		else
			arrOddTotal[7]++;

		//adds all of those phones to the win a gift 
		DAL.addUpdateSpin(0, strPhoneNumber.Trim(), strLang.Trim(), strPin, false, boolIsWinner, "", "", boolIsWinner, false, -1, "", strPrizeAmount, "", "", "", strDateEntryCreatedDate);
		
		//adds to the total number of users added to the database
		arrOddTotal[0]++;
		
		return arrOddTotal;
	}//end of playingTheOdd()
	
	//searches the file content for a phone number and returns the number of phone numbers in this file
	public static int searchContentNumberOfPhoneNumbers(string strPhoneNumber, ArrayList alSearch)
	{
		try
		{
			int intSearchResults = 0;//holds the number of phone numbers that are the same as strPhoneNumber
						
			//goes around until strPhoneNumber is found and counts the number of times it is in the csv file
			foreach (CSVContent csvDetails in alSearch)
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
	
	public static void sendErrorMessage(string strEmailSubject, string strEmailBody)
	{
		MailMessage mmError = new MailMessage(new MailAddress(""));//holds the message of email
		SmtpClient smtpMail = new SmtpClient();//holds the Smtp info for sending out e-mails
		
		//sets the properties for the email
		mmError.CC.Add(new MailAddress(""));
		mmError.Subject = strEmailSubject;
		mmError.Body = strEmailBody;
		mmError.Priority = MailPriority.Normal;
		mmError.IsBodyHtml = true;
		
		//sets the settings of the email and send it out
		smtpMail.Host = DotNetNuke.Entities.Host.HostSettings.GetHostSetting("SMTPServer");
		smtpMail.Send(mmError);
	}//end of sendErrorMessage()

	#endregion
	
	#region "private functions"	
	
	private static void WriteXmlToFile(string userProfileDetailXml, string spinToWinId)
    {
        try
        {
            StreamWriter sr;
            sr = File.CreateText(HttpContext.Current.Server.MapPath(".\\TempXML\\") + spinToWinId + ".xml");
            sr.WriteLine(userProfileDetailXml.Replace("utf-16", "utf-8").Replace("<xiamSMS>", "\r\n<!DOCTYPE xiamSMS SYSTEM \"xiamSMSMessage.dtd\">\r\n<xiamSMS>"));
            sr.Close();
        }//end of try
        catch (Exception ex)
        {
            throw ex;
        }//end of catch
    }//end of WriteXmlToFile()

    private static void ConsumeWebService(string xiamSMSXml, string spinToWinId)
    {
        try
        {
            string url = "http://websvcs2.jumptxt.com/smsxml/collector";

            // declare ascii encoding
            ASCIIEncoding encoding = new ASCIIEncoding();

            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.Load(HttpContext.Current.Server.MapPath(".\\TempXML\\" + spinToWinId + ".xml"));
            string postData = xmlDoc.InnerXml;
            // convert xmlstring to byte using ascii encoding
            byte[] data = encoding.GetBytes(postData);

            // declare httpwebrequet wrt url defined above
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);

            //webrequest header information
            webrequest.Headers.Add("HTTP_ACCEPT", "text/xml; charset=UTF-8");
            webrequest.Headers.Add("X-XIAM-Provider-ID", "132");
            webrequest.Headers.Add("HTTP_USER_AGENT", System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToString());
            webrequest.Headers.Add("HTTP_HOST", "websvcs2.jumptxt.com");
            webrequest.Headers.Add("HTTP_CONTENT_LENGTH", data.Length.ToString());

            // set method as post
            webrequest.Method = "POST";

            // get stream data out of webrequest object
            Stream newStream = webrequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            // declare & read response from service
            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();

            // set utf8 encoding
            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            // read response stream from response object
            StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            // read string from stream data
            string strResult = loResponseStream.ReadToEnd();
            // close the stream object
            loResponseStream.Close();
            // close the response object
            webresponse.Close();
        }//end of try
        catch (Exception ex)
        {
            throw ex;
        }//end of catch
    }//end of ConsumeWebService()

    private static string CreateUserXml(xiamSMS objxiamSMS)
    {
        MemoryStream stream = null;
        TextWriter writer = null;
		
        try
        {
            stream = new MemoryStream(); // read xml in memory
            writer = new StreamWriter(stream, Encoding.Unicode);
            // get serialise object
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer serializer = new XmlSerializer(typeof(xiamSMS));
            serializer.Serialize(writer, objxiamSMS, ns); // read object
            int count = (int)stream.Length; // saves object in memory stream
            byte[] arr = new byte[count];
            stream.Seek(0, SeekOrigin.Begin);
            // copy stream contents in byte array
            stream.Read(arr, 0, count);
            UnicodeEncoding utf = new UnicodeEncoding(); // convert byte array to string
			
            return utf.GetString(arr).Trim();
        }//end of try
        catch (Exception ex)
        {
            throw ex;
        }//end of catch
        finally
        {
            if (stream != null) 
				stream.Close();
            if (writer != null) 
				writer.Close();
        }//end of finally
    }//end of try

	#endregion
}//end of class General