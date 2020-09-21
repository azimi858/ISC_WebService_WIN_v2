using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.ServiceModel;
using System.IO;
using System.Collections;
using OfficeOpenXml;
using System.Xml;

using identifier.IdentifierServiceTest;
//using identifier.IdentifierServicBehdad;

using System.Security.Cryptography;


namespace identifier
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		accountInfo userAccountInfo = new accountInfo();
		accountIdentifierInfo accountIdentifierInfoAc = new accountIdentifierInfo();

		int countAcc = 1;


		List<string> RemoveResult = new List<string>();
		List<string> IsEffectiveResult = new List<string>();

		IdentifierServiceClient identifierClient = new IdentifierServiceClient();
		Accountservice.AccountServiceClient accountServiceClient = new Accountservice.AccountServiceClient();

		
		#region [ GetAccountCredential Identifire ]
		private credential GetAccountCredential(string user, string pass)
		{
			credential identifierCredential = new credential();

			if (string.IsNullOrWhiteSpace(userNameTextBox.Text) && string.IsNullOrWhiteSpace(passwordTextBox.Text))
			{
				if (radioButton1.Checked == true)
				{
					if (user == "4001068404006338")
					{
						identifierCredential.username = "987654";
						identifierCredential.password = "1234";
					}
					else if (user == "4001000901017708")
					{
						identifierCredential.username = "9876543";
						identifierCredential.password = "1111";
					}
					else if (user == "4001064004005747")
					{
						identifierCredential.username = "98765432";
						identifierCredential.password = "1111";
					}
					else
					{
						identifierCredential.username = user;
						identifierCredential.password = pass;
					}
				}
				else
				{
					identifierCredential.username = user;
					identifierCredential.password = pass;
				}
			}
			else
			{
				identifierCredential.username = userNameTextBox.Text;
				identifierCredential.password = passwordTextBox.Text;
			}
			return identifierCredential;
		}
		#endregion

		#region [ GetAccountCredential Account Service ]
		private Accountservice.credential GetAccountCredentialAc(string user, string pass)
		{
			Accountservice.credential identifierCredentialAcc = new Accountservice.credential();

			if (string.IsNullOrWhiteSpace(userNameTextBox.Text) && string.IsNullOrWhiteSpace(passwordTextBox.Text))
			{
				if (radioButton1.Checked == true)
				{
					if (user == "4001068404006338")
					{
						identifierCredentialAcc.username = "987654";
						identifierCredentialAcc.password = "1234";
					}
					else if (user == "4001000901017708")
					{
						identifierCredentialAcc.username = "9876543";
						identifierCredentialAcc.password = "1111";
					}
					else if (user == "4001064004005747")
					{
						identifierCredentialAcc.username = "98765432";
						identifierCredentialAcc.password = "1111";

					}

				}
				else
				{
					identifierCredentialAcc.username = user;
					identifierCredentialAcc.password = pass;
				}
			}
			else
			{
				identifierCredentialAcc.username = userNameTextBox.Text;
				identifierCredentialAcc.password = passwordTextBox.Text;
			}



			return identifierCredentialAcc;
		}
		#endregion

        
		#region [ getActiveIdentifiers Botton ]
		private void button3_Click(object sender, EventArgs e)
		{
			listBox1.Items.Clear();

			label10.Text = "";
			try
			{
				string accountNumber = textBox7.Text;
				string[] identsArray = null;

				identsArray = GetActiveIdentifiers(accountNumber);
				label10.Text = identsArray.Length.ToString();

				WriteIdentsWithDetails(identsArray);
			}
			catch (Exception ex)
			{
				listBox1.Text = ex.Message;// " دراین بازه شناسه ای یافت نشد ";
			}

		}
		#endregion

		#region [ Export all account Number to Excel file Botton ]
		private void button10_Click(object sender, EventArgs e)
		{
			countAcc = 0;
			label10.Text = "";
			listBox1.Text = "";

			try
			{
				//if (radioButton1.Checked == true)
				//{
				//    richTextBox5.Text = "";
				//    //richTextBox5.Text = "4001068404006338" + "\n" + "4001000901017708" + "\n" + "4001064004005747";
				//    richTextBox5.Text = "4001068404006338";
				//}

				string acNoandIden = richTextBox5.Text;
				string[] array = acNoandIden.Split(new[] { "\n" }, StringSplitOptions.None);
				richTextBox5.Text = "";

				for (int i = 0; i < array.Length; i++)
				{
					GetAndSaveActiveIdentifire(array[i]);
				}
				MessageBox.Show($"فایل ها در مسیر زیر ذخییره شد{ Environment.NewLine }D:\\Azimi\\excel\\");
			}
			catch (Exception)
			{
				richTextBox5.Text = $"{ richTextBox5.Text }{ Environment.NewLine }";
				//MessageBox.Show("خطا در ورود اطلاعات 001");
			}
		}
		#endregion


		#region [ Call Get & Save methods Method ]
		private void GetAndSaveActiveIdentifire(string accountNumber)
		{
			string[] activeIdentifire = null;
			try
			{
				activeIdentifire = GetActiveIdentifiers(accountNumber);
				if (activeIdentifire[0] != "1")
				{
					string identifiersListCount = (activeIdentifire.Length - 1).ToString("000000");
					richTextBox5.Text = $"{ richTextBox5.Text }{ accountNumber } | { identifiersListCount } | ";

					string resultSave = string.Empty;
					resultSave = SaveExcelFile(accountNumber, activeIdentifire);

					richTextBox5.Text = $"{ richTextBox5.Text }{ resultSave }{ Environment.NewLine }";
				}
				else
				{
					richTextBox5.Text = $"{ richTextBox5.Text }{ accountNumber } | خطای اطلاعات |{ Environment.NewLine }";
				}
			}
			catch (Exception ex)
			{
				richTextBox5.Text = $"{ richTextBox5.Text }{activeIdentifire[0]}{ Environment.NewLine }";
				//MessageBox.Show("008");// " دراین بازه شناسه ای یافت نشد ";
			}
		}
		#endregion

		#region [ Save Identifire to Excel File Method ]
		private string SaveExcelFile(string accountNumber, string[] idenList)
		{
			string result = string.Empty;
			try
			{
				ExcelPackage excel = new ExcelPackage();

				excel.Workbook.Worksheets.Add("sheet1");
				excel.Workbook.Worksheets.Add("sheet2");
				excel.Workbook.Worksheets.Add("sheet3");

				var headerRow = new List<string[]>()
				{
					new string[] { "Identifire", "Create Date", "Create Time", "Expire Date", "Active" }
				};

				// Determine the header range (e.g. A1:D1)
				string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

				// Target a worksheet
				var worksheet = excel.Workbook.Worksheets["sheet1"];

				// Popular header row data
				worksheet.Cells[headerRange].LoadFromArrays(headerRow);


				//string acNoandIden = idenList;
				//string[] array = acNoandIden.Split(new[] { "\n" }, StringSplitOptions.None);

				for (int j = 0; j < idenList.Length - 1; j++)
				{
					string[] accountNoandIden = idenList[j].Split(new[] { " | " }, StringSplitOptions.None);
					int lineCount = accountNoandIden.Length;

					var lineRow = new List<string[]>()
				{
					new string[] { accountNoandIden[0], accountNoandIden[1], accountNoandIden[2], accountNoandIden[3], accountNoandIden[4] }
				};

					string lineRange = "A" + (j + 2) + ":" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + (j + 2);

					worksheet.Cells[lineRange].LoadFromArrays(lineRow);
				}


				FileInfo excelFile = new FileInfo(@"D:\Azimi\excel\" + accountNumber + ".xlsx");
				excel.SaveAs(excelFile);

				countAcc++;

				label1.Text = countAcc.ToString();

				result = "Saved";
			}

			catch (Exception)
			{
				result = "Not Saved";
			}
			return result;
		}
		#endregion

		#region [ Write Identifire Details in richbox Method ]
		private void WriteIdentsWithDetails(string[] accountNumber)
		{
			for (int i = 0; i < accountNumber.Length; i++)
			{
				listBox1.Items.Add(accountNumber[i].ToString());
			}
		}
		#endregion

		#region [ Get Active Identifiers Method ]

		private string[] GetActiveIdentifiers(string accountNumber)
		{
			string[] result = null;
			try
			{
				accountInfo userAccountInfo = null;
				userAccountInfo.accountNumber = "4001068404006338";

				credential identifierCredential = null;
				identifierCredential = GetAccountCredential(accountNumber, accountNumber);

				if (identifierCredential != null)
				{
					//identifierDetail[] identifiersList = null;
					object identifiersList;
					string fromdate = textBox8.Text;
					string toDate = textBox9.Text;

					identifiersList = identifierClient.getActiveIdentifiers(identifierCredential, userAccountInfo, fromdate, toDate);

					int identifiersListCount = ((identifierDetail[])identifiersList).Length;
					result = new string[identifiersListCount];

					for (int i = 0; i < identifiersListCount; i++)
					{
						string identifier = ((identifierDetail[])identifiersList)[i].identifier;
						string startDate = ((identifierDetail[])identifiersList)[i].startDate;
						string startDateDate = startDate.Substring(0, 4) + "/" + startDate.Substring(4, 2) + "/" + startDate.Substring(6, 2);
						string startDateTime = startDate.Substring(9, 2) + ":" + startDate.Substring(11, 2) + ":" + startDate.Substring(13, 2);
						//string lastUpdate = ((identifierDetail[])identifiersList)[i].lastUpdate;
						string endDate = ((identifierDetail[])identifiersList)[i].endDate;
						string endDateDate = endDate.Substring(0, 4) + "/" + endDate.Substring(4, 2) + "/" + endDate.Substring(6, 2);
						bool active = ((identifierDetail[])identifiersList)[i].active;

						result[i] = $"{ identifier } | { startDateDate } | {startDateTime} | { endDateDate } | { active }";
					}
				}
			}

			//FaultException < InvalidCredentialException >
			catch (FaultException<InvalidCredentialException>)
			{
				result = new string[1];
				result[0] = "خطا در نام کاربری / کلمه عبور / شماره حساب ";
			}
			catch (Exception ex)
			{
				listBox1.Text = ex.Message;// " دراین بازه شناسه ای یافت نشد ";
				result = new string[1];
				result[0] = ex.Message;
			}
			return result;
		}
		#endregion

		#region [ Save Result Excel File Botton ]
		private void button9_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(listBox1.Items.ToString()) == false)
				{
					ExcelPackage excel = new ExcelPackage();

					excel.Workbook.Worksheets.Add("sheet1");
					excel.Workbook.Worksheets.Add("sheet2");
					excel.Workbook.Worksheets.Add("sheet3");

					var headerRow = new List<string[]>()
					{
						new string[] { "Identifire", "Create Date", "Create Time", "Expire Date", "Active" }
						};

					// Determine the header range (e.g. A1:D1)
					string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";


					// Target a worksheet
					var worksheet = excel.Workbook.Worksheets["sheet1"];

					// Popular header row data
					worksheet.Cells[headerRange].LoadFromArrays(headerRow);

					int listBoxCount = listBox1.Items.Count;

					//string acNoandIden = listBox1.Items.ToString();
					//string[] array = acNoandIden.Split(new[] { "\n" }, StringSplitOptions.None);

					for (int j = 0; j < listBoxCount; j++)
					{
						string oneIden = listBox1.Items[j].ToString();
						string[] accountNoandIden = oneIden.Split(new[] { " | " }, StringSplitOptions.None);
						int lineCount = accountNoandIden.Length;

						var lineRow = new List<string[]>()
				{
					new string[] { accountNoandIden[0], accountNoandIden[1], accountNoandIden[2], accountNoandIden[3], accountNoandIden[4] }
				};

						string lineRange = "A" + (j + 2) + ":" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + (j + 2);

						worksheet.Cells[lineRange].LoadFromArrays(lineRow);
					}

					string accNumber = textBox7.Text;

					FileInfo excelFile = new FileInfo(@"D:\Azimi\excel\" + accNumber + ".xlsx");
					excel.SaveAs(excelFile);

					MessageBox.Show($"فایل در مسیر زیر ذخییره شد{ Environment.NewLine }D:\\Azimi\\excel\\");
				}
			}
			catch (Exception)
			{
				listBox1.Text = "خطا در ورود اطلاعات";
			}
		}
		#endregion

		#region [ Remove Identifires Botton]
		private void button4_Click(object sender, EventArgs e)
		{
			listBox3.Items.Clear();
			label6.Text = "";
			try
			{
				RemoveResult.Clear();
				string a = richTextBox1.Text;

				string[] array = a.Split(new[] { "\n" },
				StringSplitOptions.None);

				List<string> asdfg = array.ToList();

				for (int item = 0; item < asdfg.Count; item++)
				{
					string elemValue = Convert.ToString(asdfg[item]);
					string result = RemoveGroupIdentsTest(elemValue);
					listBox3.Items.Add(result);
					if (result == "")
					{
						break;
					}
					//RemoveResult.Add(result);
				}
				GetAndSaveActiveIdentifire(textBox1.Text);
				richTextBox5.Text = "";
				//listBox3.Items.Add(String.Join(Environment.NewLine, RemoveResult.ToArray()));
			}
			catch (Exception)
			{
				MessageBox.Show("خطا در ورود اطلاعات 1");
			}
		}
		#endregion

		#region [ isEffectiveIdentifier Botton ]
		private void button2_Click(object sender, EventArgs e)
		{
			listBox3.Items.Clear();
			label6.Text = "";
			try
			{
				IsEffectiveResult.Clear();
				string a = richTextBox1.Text;

				string[] array = a.Split(new[] { "\n" }, StringSplitOptions.None);

				List<string> asdfg = array.ToList();

				for (int item = 0; item < asdfg.Count; item++)
				{
					string elemValue = Convert.ToString(asdfg[item]);
					string result = IsEffectiveIdentifier(elemValue);
					listBox3.Items.Add(result);
					//IsEffectiveResult.Add(result);
				}
				//listBox3.Items.Add(String.Join(Environment.NewLine, IsEffectiveResult.ToArray()));
			}
			catch (Exception)
			{
				MessageBox.Show("خطا در ورود اطلاعات 3");
			}
		}
		#endregion

		#region [ Add Identifires Botton ]
		private void Button5_Click(object sender, EventArgs e)
		{
			listBox3.Items.Clear();
			label6.Text = "";

			credential identifierCredential = null;
			batchIdentifierInfo identifierinfo = null;

			string a = richTextBox1.Text;
			string[] array = a.Split(new[] { "\n" }, StringSplitOptions.None);

			List<string> asdfg = array.ToList();
			List<string> badIden = new List<string>();
			List<string> okIden = new List<string>();

			for (int i = 0; i < asdfg.Count; i++)
			{
				bool invalidElemValue = CheckVerhoeff(asdfg[i]);

				if (invalidElemValue == false)
				{
					badIden.Add(asdfg[i]);
					//listBox1.Items.Add(accountNumber[i].ToString());
					listBox3.Items.Add($"{ asdfg[i] } | نا معتبر | ");
				}
				else
				{
					bool invalidKhazane = CheckKhazaneIden(textBox1.Text, asdfg[i]);

					if (invalidKhazane == true)
					{
						okIden.Add(asdfg[i]);
					}
					else
					{
						listBox3.Items.Add($"{ asdfg[i] } | کد درامدی غلط | ");
					}
				}
			}

			if (okIden.Count > 0)
			{
				identifierCredential = GetAccountCredential(textBox1.Text, textBox1.Text);
				identifierinfo = GetBatchIdentifierInfo(okIden);
				int okIdenCount = okIden.Count;

				try
				{
					object identifierResults = "";
					identifierResults = identifierClient.addIdentifiers(identifierCredential, identifierinfo);

					int existidenCount = ((identifierResult[])identifierResults).Length;

					int idenDone = 0;
					int idenDoplicate = 0;
					int idenNone = 0;

					for (int j = 0; j < existidenCount; j++)
					{
						bool idenResult = ((identifierResult[])identifierResults)[j].result;
						string idenDescription = ((identifierResult[])identifierResults)[j].description;
						string idenidentifier = ((identifierResult[])identifierResults)[j].identifier;

						if (idenResult == true)
						{
							idenDone++;
						}
						else if (idenResult == false)
						{
							listBox3.Items.Add($"{ idenidentifier } | {idenDescription} |");

							idenDoplicate++;
						}
						else
						{
							listBox3.Items.Add($"| خطای سیستمی |");

							idenNone++;
						}
					}

					label6.Text = $" تعداد { idenDone } شناسه اضافه شد";


					GetAndSaveActiveIdentifire(textBox1.Text);
					richTextBox5.Text = "";
				}
				catch (FaultException<InvalidCredentialException>)
				{
					listBox3.Items.Add($" خطای نام کاربری / عدم تعریف رسانه ");
				}
				catch (FaultException<IdentifierIsExistException>)
				{
					listBox3.Items.Add($" قبلا کارسازی شده است ");
				}
				catch (Exception ex)
				{
					listBox3.Items.Add($"خطا در عملیات { Environment.NewLine } { ex.Message }");
				}
			}
		}

		private batchIdentifierInfo GetBatchIdentifierInfo(List<string> asdfg)
		{
			//List<string> asdfg = array.ToList();

			batchIdentifierInfo arg1 = new batchIdentifierInfo();
			identifierAmountPair[] myamountpairlistTes = new identifierAmountPair[asdfg.Count];

			for (int i = 0; i < asdfg.Count; i++)
			{
				identifierAmountPair idenAmoutPair = new identifierAmountPair();

				string idents = Convert.ToString(asdfg[i]);
				idenAmoutPair.identifier = idents;

				myamountpairlistTes[i] = idenAmoutPair;
			}

			arg1.identifierAmountPairList = myamountpairlistTes;
			arg1.accountNumber = textBox1.Text;
			arg1.toDate = textBox6.Text;

			return arg1;
		}
		#endregion

		#region [ AddIdentsBehdad Botton ]
		private void Button1_Click(object sender, EventArgs e)
		{
			string userNO = string.Empty;
			string passNO = string.Empty;

			if (string.IsNullOrWhiteSpace(userNameTextBox.Text) == false && string.IsNullOrWhiteSpace(passwordTextBox.Text) == false)
			{
				userNO = userNameTextBox.Text;
				passNO = passwordTextBox.Text;
			}

			listBox2.Items.Clear();
			listBox2.Text = "";

			string acNoandIden = richTextBox3.Text;
			string[] array = acNoandIden.Split(new[] { "\n" }, StringSplitOptions.None);

			for (int j = 0; j < array.Length; j++)
			{
				string[] accountNoandIden = array[j].Split(new[] { "," }, StringSplitOptions.None);
				string accountNo = accountNoandIden[0];
				string idenNo = accountNoandIden[1];

				if (string.IsNullOrWhiteSpace(userNameTextBox.Text) && string.IsNullOrWhiteSpace(passwordTextBox.Text))
				{
					userNO = accountNo;
					passNO = accountNo;
				}

				/////////////////////////////////////////////////////////////////////////////

				bool invalidElemValue = CheckVerhoeff(idenNo);

				if (invalidElemValue == true)
				{
					bool invalidKhazane = CheckKhazaneIden(accountNo, idenNo);

					if (invalidKhazane == true)
					{
						string result = "";
						result = AddIdents(userNO, passNO, accountNo, idenNo);
						if (result == "Done")
						{
							GetAndSaveActiveIdentifire(accountNo);
							richTextBox5.Text = "";
						}

						listBox2.Items.Add(result.ToString());
					}
					else
					{
						listBox2.Items.Add(idenNo + " | " + "کد درامدی غلط" + " | ");
					}
				}
				else
				{
					listBox2.Items.Add(idenNo + " | " + "نا معتبر" + " | ");
				}
			}
		}
		#endregion

		#region [ getAccountControlType Botton ]
		private void button6_Click(object sender, EventArgs e)
		{
			cleareLable();

			try
			{
				accountInfo userAccountInfo = null;
				userAccountInfo.accountNumber = "4001068404006338";

				Accountservice.credential accountcredential = null;
				accountcredential = GetAccountCredentialAc(textBox10.Text, textBox10.Text);

				string accountNumber = textBox10.Text;

				try
				{
					for (int i = 1; i <= 9; i++)
					{
						string identifierType = i.ToString();

						string AccountControlType = accountServiceClient.getAccountControlType(accountcredential, userAccountInfo, identifierType);
						
						if (string.IsNullOrEmpty(AccountControlType) == false)
						{
							getType.Text = getType.Text + i + " : " + AccountControlType + Environment.NewLine;
						}
					}
					labelGet.Text = $"عملیات با موفقیت انجام شد";
				}
				catch (Exception)
				{

					labelGet.Text = $"خطا در عملیات";
				}


			}
			catch (Exception)
			{
				MessageBox.Show("خطا در ورود اطلاعات");
			}
		}
		#endregion

		#region[ setAccountControlType Botton ]
		private void button7_Click(object sender, EventArgs e)
		{
			cleareLable();
			try
			{
				Accountservice.credential accountcredential = null;
				accountcredential = GetAccountCredentialAc(textBox10.Text, textBox10.Text);

				Accountservice.accountControlCreateModel AccountControlCreateModel = new Accountservice.accountControlCreateModel
				{
					accountNumber = textBox10.Text,
					identifierType = comboBox2.SelectedItem.ToString(),
					controlType = comboBox1.SelectedItem.ToString(),
					toDate = textBox14.Text
				};
				object setRsult = "";
				try
				{
					accountServiceClient.setAccountControlType(accountcredential, AccountControlCreateModel);
					labelSet.Text = $"عملیات با موفقیت انجام شد";
				}
				//catch (FaultException<InvalidCredentialException>)
				catch (FaultException<InvalidCredentialException>)
				{
					labelSet.Text = $"خطا در نام کاربری و رمزعبور یا شماره حساب";
				}
				catch (Exception ex)
				{
					labelSet.Text = $"خطا در عملیات";
				}
			}
			catch (Exception)
			{
				MessageBox.Show("خطا در ورود اطلاعات 2");
			}
		}
		#endregion

		#region[ clearAccountControlType Botton ]
		private void button8_Click(object sender, EventArgs e)
		{
			cleareLable();
			try
			{
				accountInfo userAccountInfo = null;
				userAccountInfo.accountNumber = "4001068404006338";

				Accountservice.credential accountcredential = null;
				accountcredential = GetAccountCredentialAc(textBox10.Text, textBox10.Text);

				string accountNumber = textBox10.Text;
				string identifierType = comboBox3.SelectedItem.ToString();

				try
				{
					accountServiceClient.clearAccountControlType(accountcredential, userAccountInfo , identifierType);
					labelClear.Text = $"عملیات با موفقیت انجام شد";
				}
				catch (Exception)
				{

					labelClear.Text = $"خطا در عملیات";
				}


			}
			catch (Exception)
			{
				MessageBox.Show("خطا در ورود اطلاعات 3");
			}
		}
		#endregion

		public void cleareLable()
		{
			labelGet.Text = "";
			labelSet.Text = "";
			labelClear.Text = "";
			getType.Text = "";
		}

		#region [ check khazane Identifire Method]
		private bool CheckKhazaneIden(string accNo, string idenNo)
		{
			bool result = true;

			string aa = idenNo.Substring(9, 8);
			string bb = idenNo.Substring(9, 6);


			Int64 caseSwitch = Convert.ToInt64(accNo);

			switch (caseSwitch)
			{
				case 4001000901000555:
					if (aa != "31060101")
						result = false;
					break;
				case 4001000901002700:
					if (aa != "31060102")
						result = false;
					break;
				case 4001000901009432:
					if (aa != "21020117")
						result = false;
					break;
				case 4001000902006429:
					if (aa != "21020215")
						result = false;
					break;
				case 4001000901009119:
					if (aa != "21020399")
						result = false;
					break;
				case 4001000901006958:
					if (aa != "21020407")
						result = false;
					break;
				case 4001000902005884:
					if (aa != "21021002")
						result = false;
					break;
				case 4001000901013566:
					if (aa != "21030100")
						result = false;
					break;
				case 4001000901013939:
					if (aa != "16017900")
						result = false;
					break;
				case 4001000901006065:
					if (bb != "310602")
						result = false;
					break;
				case 4001000901006032:
					if (bb != "310605")
						result = false;
					break;
				case 4001000901002452:
					if (aa != "16017400")
						result = false;
					break;
				case 4001000901017708:
					if (aa != "16010113")
						result = false;
					break;
				case 4001000901025842:
					if (aa != "21021800")
						result = false;
					break;
				case 4001011501017816:
					if (bb != "160164")
						result = false;
					break;
				default:
					result = true;
					break;
			}

			return result;
		}
		#endregion


		#region [ Remove Identifire Method]
		private string RemoveGroupIdentsTest(string number)
		{
			string userCreden02 = string.Empty;
			string passCreden02 = string.Empty;

			if (string.IsNullOrWhiteSpace(userNameTextBox.Text) && string.IsNullOrWhiteSpace(passwordTextBox.Text))
			{
				userCreden02 = textBox1.Text;
				passCreden02 = textBox1.Text;
			}
			else
			{
				userCreden02 = userNameTextBox.Text;
				passCreden02 = passwordTextBox.Text;
			}

			string result = "";
			try
			{
				credential identifierCredential = null;
				identifierCredential = GetAccountCredential(userCreden02, passCreden02);

				accountIdentifierInfo accountIdentifierInfoAc = null;
				accountIdentifierInfoAc.accountNumber = textBox1.Text;
				accountIdentifierInfoAc.identifierCode = number;

				if (identifierCredential != null)
				{
					try
					{
						identifierClient.removeIdentifier(identifierCredential,accountIdentifierInfoAc);
						result = "حذف شد";
					}
					catch (FaultException<InvalidCredentialException>)
					{
						result = (number + " | خطای نام کاربری / عدم تعریف رسانه |").ToString();
					}
					catch (FaultException<IdentifierNotFoundException>)
					{
						result = (number + " | شناسه یافت نشد |").ToString();
					}
				}
			}
			catch (Exception)
			{
				MessageBox.Show("خطا در ورود اطلاعات 2");
			}
			return result;
		}
		#endregion

		#region [ IdentifierInfoBehdad Method ]
		private identifierInfo IdentifierInfoBehdad(string account, string iden)
		{
			identifierInfo identifierCredential = new identifierInfo
			{
				accountNumber = account,
				identifier = iden,
				amount = 0,
				toDate = textBox6.Text
			};

			return identifierCredential;
		}
		#endregion

		#region [ Add Ident Method ]
		private string AddIdents(string user, string pass, string number, string identif)
		{
			string result = "";
			credential identifierCredential = null;
			identifierInfo identifierInfo = null;

			identifierCredential = GetAccountCredential(user, pass);
			identifierInfo = IdentifierInfoBehdad(number, identif);

			if (identifierCredential != null)
			{
				try
				{
					identifierClient.addIdentifier(identifierCredential, identifierInfo);
					result = "Done";
				}
				catch (FaultException<InvalidCredentialException>)
				{
					//result = invExc.ToString();
					result = (identif + " | خطای نام کاربری / عدم تعریف رسانه |").ToString();
				}
				catch (FaultException<IdentifierIsExistException>)
				{
					result = (identif + " | قبلا کارسازی شده |").ToString();
				}
			}
			return result;
		}
		#endregion

		#region [ Is Effective Identifire Method]
		private string IsEffectiveIdentifier(string number)
		{
			string userCreden03 = string.Empty;
			string passCreden03 = string.Empty;

			if (string.IsNullOrWhiteSpace(userNameTextBox.Text) && string.IsNullOrWhiteSpace(passwordTextBox.Text))
			{
				userCreden03 = textBox1.Text;
				passCreden03 = textBox1.Text;
			}
			else
			{
				userCreden03 = userNameTextBox.Text;
				passCreden03 = passwordTextBox.Text;
			}

			string result = "";
			try
			{
				credential identifierCredential = null;
				identifierCredential = GetAccountCredential(userCreden03, passCreden03);

				accountIdentifierInfo accountIdentifierInfoAc = null;
				accountIdentifierInfoAc.accountNumber = textBox1.Text;
				accountIdentifierInfoAc.identifierCode = number;

				if (identifierCredential != null)
				{
					try
					{
						identifierClient.isEffectiveIdentifier(identifierCredential, accountIdentifierInfoAc);
						result = "فعال";
					}
					catch (FaultException<InvalidCredentialException>)
					{
						result = (number + " | خطای نام کاربری / عدم تعریف رسانه |").ToString();
					}
					catch (FaultException<IdentifierNotFoundException>)
					{
						result = (number + " | شناسه یافت نشد |").ToString();
					}
				}
			}
			catch (Exception)
			{
				MessageBox.Show("خطا در ورود اطلاعات 4");
			}
			return result;
		}
		#endregion

		#region [ Verhoeff Method ]

		#region[ Check Verhoeff Method ]
		public static bool CheckVerhoeff(string number)
		{
			bool result = false;
			var a = number;

			string aLenght = a.Length.ToString();
			bool isDigits = IsDigitsOnly(number);

			if (aLenght == "30" && isDigits == true)
			{

				string aa = a.Substring(0, 1);
				string bb = a.Substring(3, 27);

				string b = aa + bb;
				string rb = Reverse(b);

				string verhoeff_b = Verhoeff.generateVerhoeff(b);
				string verhoeff_rb = Verhoeff.generateVerhoeff(rb);

				string vNew = verhoeff_b + verhoeff_rb;
				string vExist = a.Substring(1, 2);

				if (vNew == vExist)
				{
					result = true;
				}
			}
			return result;
		}
		#endregion

		#region[ Is Numeric or not Method ]
		public static bool IsDigitsOnly(string str)
		{
			foreach (char c in str)
			{
				if (c < '0' || c > '9')
					return false;
			}
			return true;
		}
		#endregion

		#region[ Reverce Method ]
		public static string Reverse(string s)
		{
			char[] charArray = s.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}
		#endregion

		#region [ Verhoeff Method ]
		public static class Verhoeff
		{
			// The multiplication table
			static int[,] d = new int[,]
			{
			{0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
			{1, 2, 3, 4, 0, 6, 7, 8, 9, 5},
			{2, 3, 4, 0, 1, 7, 8, 9, 5, 6},
			{3, 4, 0, 1, 2, 8, 9, 5, 6, 7},
			{4, 0, 1, 2, 3, 9, 5, 6, 7, 8},
			{5, 9, 8, 7, 6, 0, 4, 3, 2, 1},
			{6, 5, 9, 8, 7, 1, 0, 4, 3, 2},
			{7, 6, 5, 9, 8, 2, 1, 0, 4, 3},
			{8, 7, 6, 5, 9, 3, 2, 1, 0, 4},
			{9, 8, 7, 6, 5, 4, 3, 2, 1, 0}
			};

			// The permutation table
			static int[,] p = new int[,]
			{
			{0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
			{1, 5, 7, 6, 2, 8, 3, 0, 9, 4},
			{5, 8, 0, 3, 7, 9, 6, 1, 4, 2},
			{8, 9, 1, 6, 0, 4, 3, 5, 2, 7},
			{9, 4, 5, 3, 1, 2, 6, 8, 7, 0},
			{4, 2, 8, 6, 5, 7, 3, 9, 0, 1},
			{2, 7, 9, 3, 8, 0, 6, 4, 1, 5},
			{7, 0, 4, 6, 9, 1, 3, 2, 5, 8}
			};

			// The inverse table
			static int[] inv = { 0, 4, 3, 2, 1, 5, 6, 7, 8, 9 };

			public static bool validateVerhoeff(string num)
			{
				int c = 0;
				int[] myArray = StringToReversedIntArray(num);

				for (int i = 0; i < myArray.Length; i++)
				{
					c = d[c, p[(i % 8), myArray[i]]];
				}
				return c == 0;
			}
			public static string generateVerhoeff(string num)
			{
				int c = 0;
				int[] myArray = StringToReversedIntArray(num);

				for (int i = 0; i < myArray.Length; i++)
				{
					c = d[c, p[((i + 1) % 8), myArray[i]]];
				}
				return inv[c].ToString();
			}

			private static int[] StringToReversedIntArray(string num)
			{
				int[] myArray = new int[num.Length];

				for (int i = 0; i < num.Length; i++)
				{
					myArray[i] = int.Parse(num.Substring(i, 1));
				}
				Array.Reverse(myArray);
				return myArray;
			}
		}





		#endregion

		#endregion

        

		#region [ Change Test and Operation ]

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			panel1.Visible = false;
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			panel1.Visible = true;
		}

		private void button13_Click(object sender, EventArgs e)
		{
			textBox1.Text = "4001068404006338";
			textBox7.Text = "4001068404006338";
			textBox10.Text = "4001068404006338";
			textBox5.Text = "4001068404006338";
		}

		private void button12_Click(object sender, EventArgs e)
		{
			textBox1.Text = "4001000901017708";
			textBox7.Text = "4001000901017708";
			textBox10.Text = "4001000901017708";
			textBox5.Text = "4001000901017708";
		}

		private void button11_Click(object sender, EventArgs e)
		{
			textBox1.Text = "4001064004005747";
			textBox7.Text = "4001064004005747";
			textBox10.Text = "4001064004005747";
			textBox5.Text = "4001064004005747";
		}
		#endregion

		#region [ save Transaction Details in Excel Bottom ]
		private void button14_Click(object sender, EventArgs e)
		{
			richTextBox2.Text = "";
			try
			{
				int reqCount = Convert.ToInt32(textBox13.Text);
				if (reqCount > 50)
				{
					MessageBox.Show($"باید از 50 کوچکتر باشد 'recordCount' مقدار ");
					return;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("خطا در ورود اطلاعات");
				return;
			}

			try
			{
				object detailResult = "";

				detailResult = getBankTransactionsDetails();
				if (detailResult.GetType() != typeof(string))
				{
					Accountservice.pagedData myresult;
					myresult = (Accountservice.pagedData)detailResult;


					string detailResultCount = ((Accountservice.pagedData)detailResult).totalCount.ToString();
					int detailResultCountInt = Convert.ToInt32(detailResultCount);
					int textBox12Int = Convert.ToInt32(textBox12.Text);
					int textBox13Int = Convert.ToInt32(textBox13.Text);

					int startRecord = textBox12Int * textBox13Int - (textBox13Int - 1);

					for (int j = 0; j < textBox13Int; j++)
					{
						XmlNode[] xmlNode = ((XmlNode[])((identifier.Accountservice.pagedData)detailResult).currentPageData[j]);
						int asasas = xmlNode.Count();
						string a = "";
						richTextBox2.Text = $"{ richTextBox2.Text }{a.PadLeft(25, '-')}[ { startRecord + (j) } -- {detailResultCountInt} ]{a.PadRight(100, '-')}{ Environment.NewLine }";

                        for (int i = 2; i < asasas; i++)
                        {
                            string titleName = (((XmlNode[])((identifier.Accountservice.pagedData)detailResult).currentPageData[j])[i]).Name;
                            string titleValue = (((XmlNode[])((identifier.Accountservice.pagedData)detailResult).currentPageData[j])[i]).InnerText;

                            //richTextBox2.Text = $"{richTextBox2.Text} { titleName } {/t} : {titleValue} {Environment.NewLine} ";
                            richTextBox2.Text = richTextBox2.Text + titleName.PadRight(27) + "\t" + " : " + titleValue + Environment.NewLine;
                        }

                    }
				}
				else
				{
					richTextBox2.Text = detailResult.ToString();
				}
			}
			catch (Exception exc)
			{
				richTextBox2.Text = "خطا در ورود اطلاعات";
			}
		}
        #endregion


        /////////////////////////////////////////////////////////////////////////////////////////////
        #region [ getBankTransactionsDetails Method ]
        private object getBankTransactionsDetails()
		{
			object result = "";
			try
			{
				Accountservice.accountTransactionFilter AccountTransactionFilter = new Accountservice.accountTransactionFilter
				{
					accountNumber = textBox5.Text,
					fromDateTime = fromDateTime(),
					paymentIdentifier = textBox11.Text,
					toDateTime = toDateTime(),
				};

				Accountservice.paging Paging = new Accountservice.paging
				{
					pageNumber = Convert.ToInt32(textBox12.Text),
					pageNumberSpecified = true,
					recordCount = Convert.ToInt32(textBox13.Text),
					recordCountSpecified = true,
				};

				Accountservice.credential accountcredential = null;
				accountcredential = GetAccountCredentialAc(textBox5.Text, textBox5.Text);

				if (accountcredential != null)
				{
					try
					{
						Accountservice.pagedData PagedData = new Accountservice.pagedData();

						PagedData = accountServiceClient.getBankTransactionsDetails(accountcredential, AccountTransactionFilter, Paging);
						result = PagedData;
						if (PagedData.totalCount == 0)
						{
							result = (" | در این بازه اطلاعاتی موجود نمیباشد |").ToString();
						}
					}
					catch (FaultException<InvalidCredentialException>)
					{
						//result = invExc.ToString();
						result = (" | خطای نام کاربری / عدم تعریف رسانه |").ToString();
					}
					catch (FaultException<IdentifierIsExistException>)
					{
						result = (" | قبلا کارسازی شده |").ToString();
					}
					catch (System.ServiceModel.FaultException<identifier.Accountservice.InvalidCredentialException>)
					{
						result = (" | خطای نام کاربری / عدم تعریف رسانه2 |").ToString();
					}
					catch (Exception ex)
					{
						result = (" | خطای نا شناسخته |").ToString();
					}

				}
			}
			catch (Exception ex)
			{

				MessageBox.Show("خطا در ورود اطلاعات");
			}
			return result;
		}
        #endregion [ getBankTransactionsDetails Method ]

        #region [ fromDateTime ]
        private string fromDateTime()
		{
			string result = "";
			string fromDate = (maskedTextBox1.Text).Replace(@"/", string.Empty);
			string fromTime = (maskedTextBox2.Text).Replace(@":", string.Empty);
            result = $"{fromDate}{fromTime}";
			return result;
		}
        #endregion[ fromDateTime ]

        #region [ toDateTime ]
        private string toDateTime()
		{
			string result = "";
			string toDate = (maskedTextBox3.Text).Replace(@"/", string.Empty);
			string toTime = (maskedTextBox4.Text).Replace(@":", string.Empty);
			result = $"{toDate}{toTime}";
			return result;
		}
        #endregion[ toDateTime ]

        #region [ Get Balance button ]
        private void button15_Click(object sender, EventArgs e)
		{
			richTextBox2.Text = "";
			try
			{
				object balanceResult = "";
				balanceResult = getBalance();

				//if (balanceResult != "error")
				if (balanceResult.GetType() != typeof(string))
				{
					decimal credit = ((identifier.Accountservice.balanceInfo)balanceResult).credit;
					decimal monetary = ((identifier.Accountservice.balanceInfo)balanceResult).monetary;
					string monetaryFormat = string.Format("{0:n0}", monetary);
					string reportDate = ((identifier.Accountservice.balanceInfo)balanceResult).reportDate;
					decimal systemBlock = ((identifier.Accountservice.balanceInfo)balanceResult).systemBlock;
					decimal userBlock = ((identifier.Accountservice.balanceInfo)balanceResult).userBlock;

					string a = "";
					richTextBox2.Text =
						$"credit{ a.PadLeft(10) }\t: { credit }{Environment.NewLine}" +
						$"monetary{ a.PadLeft(10) }\t: { monetaryFormat }{Environment.NewLine}" +
						$"reportDate{ a.PadLeft(10) }\t: { reportDate }{Environment.NewLine}" +
						$"systemBlock{ a.PadLeft(10) }\t: { systemBlock }{Environment.NewLine}" +
						$"userBlock{ a.PadLeft(10) }\t: { userBlock }{Environment.NewLine}";
				}
				else
				{
					//MessageBox.Show("خطا در ورود اطلاعات");
					richTextBox2.Text = balanceResult.ToString();
				}
			}
			catch (Exception exc)
			{
				richTextBox2.Text = "خطا در ورود اطلاعات";
			}
		}
		#endregion [ Get Balance button ]

		#region [ get Balance Method ]
		private object getBalance()
		{
			object result = "";

			try
			{
				Accountservice.credential accountcredential = null;
				accountcredential = GetAccountCredentialAc(textBox5.Text, textBox5.Text);

				accountInfo userAccountInfo = null;
				userAccountInfo.accountNumber = "4001068404006338";
				
				if (accountcredential != null)
				{
					try
					{
						Accountservice.balanceInfo balanceInfo = new Accountservice.balanceInfo();

						balanceInfo = accountServiceClient.getAccountBalance(accountcredential, userAccountInfo);
						result = balanceInfo;
					}

					catch (FaultException<InvalidCredentialException>)
					{
						//result = invExc.ToString();
						result = (" | خطای نام کاربری / عدم تعریف رسانه |").ToString();
					}
					catch (FaultException<IdentifierIsExistException>)
					{
						result = (" | قبلا کارسازی شده |").ToString();
					}
					catch (System.ServiceModel.FaultException<identifier.Accountservice.InvalidCredentialException>)
					{
						result = (" | خطای نام کاربری / عدم تعریف رسانه2 |").ToString();
					}
					catch (Exception ex)
					{
						result = (" | خطای نا شناسخته |").ToString();
					}

				}
			}

			catch (Exception ex)
			{
				MessageBox.Show("خطا در ورود اطلاعات");
			}

			return result;
		}


		#endregion


		static string hCode(string rawData)
		{
			using (SHA256 sha256Hash = SHA256.Create())
			{
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}


		private void openButton_Click(object sender, EventArgs e)
		{
			string userPass = hCode(openTextBox.Text);
			if (userPass == "a56548b31f4a4ccc8390cf1952ddabf59733e33a5eec5a20c005b18200ed8f89" ||
				userPass == "4103fa4e634d4c28f770e5d741295773b1b4e6551198522f6272a7c237c1a4d9" ||
				userPass == "f12f77a0aa76921c972df35550c8990eb2394e6b9931b5cd9080e76662323dce")
			{
				typeGroupBox.Visible = true;
				openTextBox.Text = "";
			}
			else
			{
				typeGroupBox.Visible = false;
				openTextBox.Text = "";
			}
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			typeGroupBox.Visible = false;
			openTextBox.Text = "";
		}

		private void button17_Click(object sender, EventArgs e)
		{
			string userPass2 = hCode(textBox2.Text);
			if (userPass2 == "a56548b31f4a4ccc8390cf1952ddabf59733e33a5eec5a20c005b18200ed8f89" ||
				userPass2 == "4103fa4e634d4c28f770e5d741295773b1b4e6551198522f6272a7c237c1a4d9" ||
				userPass2 == "f12f77a0aa76921c972df35550c8990eb2394e6b9931b5cd9080e76662323dce")
			{
				detailGroupBox.Visible = true;
				textBox2.Text = "";
			}
			else
			{
				detailGroupBox.Visible = false;
				textBox2.Text = "";
			}
		}

		private void button16_Click(object sender, EventArgs e)
		{
			detailGroupBox.Visible = false;
			textBox2.Text = "";
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Accountservice.AccountServiceClient accountClient = new Accountservice.AccountServiceClient();
			try
			{
				ServicePointManager.ServerCertificateValidationCallback += (mender, certificate, chain, sslPolicyErrors) => true;
				var client = new Accountservice.AccountServiceClient();
				client.ClientCredentials.ClientCertificate.Certificate = new X509Certificate2("cert.p12", "changeit", X509KeyStorageFlags.MachineKeySet);
			}

			catch (Exception exc)
			{
			}
		}
	}
}