using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMA.Utils.smtp;
using PMA.Utils.ftp;
using PMA.Utils;
using System.IO;
using System.Diagnostics;
using PMA.ProcessMemoryAnalyzer;
using System.ServiceProcess;
using PMA.Info;
using PMA.ConfigManager;


namespace PMA.PMAConsoleApp
{
    public class Program
    {

        PMAConfigManager configManager = PMAConfigManager.GetConfigManagerInstance;
        
        const int SPACE_LENGTH = 20;

        PMAInfo pmaInfo;
        Emails emailsInfo;
        SmtpInfo smtpInfo;
        FTPInfo ftpInfo; 

        static PMATaskHandler pmaTaskHandler;
        
        static void Main(string[] args)
        {

            //File.WriteAllText("C:\\log.txt", "enter the text top");
            string command = string.Empty;
            Program p = new Program();
            int option;
            try
            {
                p.DeserilizeObjects();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Invalid or no config Found, Creating Default");
                p.SerializeDefaultObject();
            }
            //p.SerializedInfo();
            if (args.Length == 0)
            {
                while (command != "exit")
                {
                    try
                    {
                        p.DeserilizeObjects();
                        option = p.CreateUserOptions();
                        switch (option)
                        {
                            case 0:
                                {
                                    CreatePMASettings(p);
                                    break;
                                }
                            case 1:
                                {
                                    CreateSMTPSettings(p);
                                    break;
                                }

                            case 2:
                                {
                                    CreateEmailSettings(p);
                                    break;
                                }

                            case 3:
                                {
                                    StopPMAService();
                                    break;
                                }

                            case 4:
                                {
                                    StartPMAService();
                                    break;
                                }

                            case 5:
                                {
                                    RestoreSettings(p);
                                    break;
                                }

                            case 6:
                                {
                                    ShowSettings(p);
                                    break;
                                }
                            case 7:
                                {
                                    RunTest();
                                    break;
                                }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception :");
                        Console.WriteLine(ex.Message);
                    }
                    p.SerializedInfo();

                }
            }
            else
            {
                //File.AppendAllText("C:\\log.txt", "enter the text");
                RestoreSettings(p);
                StartPMAService();
            }
        }

        private static void ConfigureApplication()
        {
            throw new NotImplementedException();
        }

        private static void ShowSettings(Program p)
        {
            Console.WriteLine();
            Console.WriteLine("Show Settings :");
            Console.WriteLine();
            Console.WriteLine("PMA Settings :");
            Console.WriteLine(p.pmaInfo.Serialize());
            Console.WriteLine();
            Console.WriteLine("SMTP Settings :");
            Console.WriteLine(p.smtpInfo.Serialize());
            Console.WriteLine();
            Console.WriteLine("Email Settings :");
            Console.WriteLine(p.emailsInfo.Serialize());
        }

        private static void RestoreSettings(Program p)
        {
            Console.WriteLine();
            Console.WriteLine("Restoring Defaults :");
            p.SerializeDefaultObject();
        }

        private static void StartPMAService()
        {
            Console.WriteLine();
            Console.WriteLine("Starting PMA Service :");
            ServiceController service = new ServiceController("PMAService");
            service.Start();
        }

        private static void StopPMAService()
        {
            Console.WriteLine();
            Console.WriteLine("Stoping PMA Service :");
            ServiceController service = new ServiceController("PMAService");
            service.Stop();
        }

        private static void CreateEmailSettings(Program p)
        {
            Console.WriteLine();
            Console.WriteLine("Email Settings :");

            string email = string.Empty;
            p.emailsInfo = new Emails();
            p.emailsInfo.AttachmentPath = "";
            Console.WriteLine("Subject Prefix :");
            p.emailsInfo.Subject = Console.ReadLine();
            Console.WriteLine("Mail Body Draft :");
            p.emailsInfo.BodyContent = Console.ReadLine();
            p.emailsInfo.EmailTo = new List<string>();
            while (email != "!")
            {
                email = Console.ReadLine();
                if (email != "!")
                    p.emailsInfo.EmailTo.Add(email);
            }
            email = string.Empty;
            p.emailsInfo.EmailCC = new List<string>();
            while (email != "!")
            {
                email = Console.ReadLine();
                if (email != "!")
                    p.emailsInfo.EmailCC.Add(email);
            }
        }

        private static void CreateSMTPSettings(Program p)
        {
            Console.WriteLine();
            Console.WriteLine("SMTP Settings :");

            p.smtpInfo = new SmtpInfo();
            Console.WriteLine("Enter User Name : ");
            p.smtpInfo.UserName = Console.ReadLine();
            Console.WriteLine("Enter User Password : ");
            p.smtpInfo.Password = Console.ReadLine();
            Console.WriteLine("Enter port : ");
            p.smtpInfo.Port = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter SMTP Name : ");
            p.smtpInfo.SmtpServer = Console.ReadLine(); ;
            Console.WriteLine("Enable SSL : ");
            p.smtpInfo.SSL = bool.Parse(Console.ReadLine());
            Console.WriteLine("Timeout : ");
            p.smtpInfo.TimeOut = int.Parse(Console.ReadLine());
        }

        private static void CreatePMASettings(Program p)
        {
            Console.WriteLine();
            Console.WriteLine("PMA Settings :");

            string serviceName = string.Empty;
            p.pmaInfo = new PMAInfo();
            Console.WriteLine("Enter Mailing Time(DD/MM/YYYY HH:MM) :");
            p.pmaInfo.MailingTime = Console.ReadLine();
            Console.WriteLine("Enter Mailing Report Interval (Hours) :");
            p.pmaInfo.ReportsIntervalHours = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter Client Name :");
            p.pmaInfo.ClientName = Console.ReadLine();

            
        }

        private static void RunTest()
        {

            if (pmaTaskHandler == null)
            {
                pmaTaskHandler = new PMATaskHandler();
            }
            pmaTaskHandler.RunTask();
            
            //PMATaskHandler.CreateAllProcessCSVReport("G:\\PMAService\\Memlog\\IVP_23 April 2010_01-20-54.txt");
            //PMATaskHandler.CreateAllProcessCSVReport("D:\\My Applications\\ProcessMemoryAnalyzer\\PMATestApp\\bin\\Debug\\Memlog\\IVP_29 April 2010_20-14.txt");


        }

        private int CreateUserOptions()
        {
            int i = -1;
            while (i == -1)
            {
                Console.WriteLine("Press 0 to Edit/Create PMA Setting");
                Console.WriteLine("Press 1 to Edit/Create SMTP Setting");
                Console.WriteLine("Press 2 to Edit/Create Email Setting");
                Console.WriteLine("Press 3 to Stop PMA Service");
                Console.WriteLine("Press 4 to Start PMA Service");
                Console.WriteLine("Press 5 to Restore Default Setting Files");
                Console.WriteLine("Press 6 to Show Settings");
                //Console.WriteLine("Press 7 to runTest code");

                try
                {
                    i = int.Parse(Console.ReadKey().KeyChar.ToString());
                    if (i < 0 && i > 6)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid Option");
                    i = -1;
                }

            }
            return i;

        }

        
        private void SerializeDefaultObject()
        {
            //PMAInfo
            pmaInfo = new PMAInfo();
            pmaInfo.MailingTime = DateTime.Now.ToString("d/M/yyyy HH:mm");
            pmaInfo.ReportsIntervalHours = 12;
            pmaInfo.ClientName = System.Environment.MachineName;
            pmaInfo.DisposeLogFile = false;
            pmaInfo.TriggerSeed = 20;
            pmaInfo.UseFTP = true;
            pmaInfo.UseSMTP = false;

            // EMAIL INFO
            emailsInfo = new Emails();
            emailsInfo.EmailTo = new List<string>();
            emailsInfo.EmailTo.Add("msareen@test.in");
            emailsInfo.EmailCC = new List<string>();
            emailsInfo.EmailCC.Add("smalhotra@ivp.in");
            emailsInfo.AttachmentPath = "";
            emailsInfo.Subject = "Server Report";
            emailsInfo.BodyContent = "Please Find the Report Attached";

            // SMTP Info
            smtpInfo = new SmtpInfo();
            smtpInfo.ProtectPassword = true;
            smtpInfo.UserName = "productitsupport@cosmostech.in";
            smtpInfo.Password = "'$e{ye";
            smtpInfo.Port = 587;
            smtpInfo.SmtpServer = "smtp.gmail.com";
            smtpInfo.SSL = true;
            smtpInfo.TimeOut = 100000;

            //FTP Info
            ftpInfo = new FTPInfo();
            ftpInfo.FTPServer = "ftp://202.54.213.231";
            ftpInfo.FTPServerFolder = "PerformanceReports";
            ftpInfo.Password = "#!!!Vbbb";
            ftpInfo.Port = 21;
            ftpInfo.ProtectPassword = true;
            ftpInfo.SSL = false;
            ftpInfo.TimeOut = 100000;
            ftpInfo.UserName = "ftpuser";
            

            SerializedInfo();
        }

        

        
        private void SerializedInfo()
        {
            try
            {
                File.WriteAllText(Path.Combine(configManager.CurrentAppConfigDir , PMAInfo.PMA_INFO_FILE), pmaInfo.Serialize());
                File.WriteAllText(Path.Combine(configManager.CurrentAppConfigDir, Emails.EMAILS_INFO_FILE), emailsInfo.Serialize());
                File.WriteAllText(Path.Combine(configManager.CurrentAppConfigDir, SmtpInfo.SMTP_INFO_FILE), smtpInfo.Serialize());
                File.WriteAllText(Path.Combine(configManager.CurrentAppConfigDir, FTPInfo.FTP_INFO_FILE), ftpInfo.Serialize());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeserilizeObjects()
        {
            try
            {
                pmaInfo = PMAInfo.Deserialize(File.ReadAllText(Path.Combine(configManager.CurrentAppConfigDir, PMAInfo.PMA_INFO_FILE)));
                emailsInfo = Emails.Deserialize(File.ReadAllText(Path.Combine(configManager.CurrentAppConfigDir, Emails.EMAILS_INFO_FILE)));
                smtpInfo = SmtpInfo.Deserialize(File.ReadAllText(Path.Combine(configManager.CurrentAppConfigDir, SmtpInfo.SMTP_INFO_FILE)));
                ftpInfo = FTPInfo.Deserialize(File.ReadAllText(Path.Combine(configManager.CurrentAppConfigDir, FTPInfo.FTP_INFO_FILE)));
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }


        
    }
}
