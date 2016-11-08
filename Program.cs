﻿using BTCeAPI;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCeAPITest
{
    class Program
    {
        private static ILog logger = log4net.LogManager.GetLogger("Console");
        private static FeeInfo fee = BTCeAPI.BTCeAPIWrapper.Instance.DefaultFee;
        private static TickerInfo ticker = null;

        private static object lockFee = new object();
        private static object lockTicket = new object();

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            logger.Info("Application started ...");

            string key = "0NFWV0SF-2RRLZAEE-PT6757VY-LRQ90DMZ-DBEV6LR9";
            string secret = "f7b0aa472c338e6aa9f2971d1d379b8a761e0f545da85f27dfd397016c3f399d";

            if (args.Length > 0)
            {
                for (int index = 0; index < args.Length; index++)
                {
                    switch (args[index].Trim().ToUpper())
                    {
                        case "-K":
                        case "/K":
                            index++;
                            key = args[index];
                            logger.InfoFormat("Key paraneter: {0}", key);
                            break;
                        case "-S":
                        case "/S":
                            index++;
                            secret = args[index];
                            logger.InfoFormat("secret paraneter: {0}", secret);
                            break;

                    }
                }
            }

            BTCeAPIWrapper api = BTCeAPIWrapper.Instance;
            api.TickerTimeout = 2;
            api.FeeTimeout = 4;
            
            api.PriceChanged += new EventHandler(OnUSDBTCPChanged);
            api.PriceChanged += new EventHandler(OnUSDBTCPChanged2);

            api.FeeChanged += new EventHandler(OnBTCeFeeChanged);

            Console.WriteLine("Default Fee: " + fee);

            api.Credential(key, secret);
            api.InfoReceived += new EventHandler(OnInfoReceived);
            api.ActiveOrdersReceived += new EventHandler(OnActiveOrdersReceived);

            Console.ReadLine();

            logger.Info("Application ended ...");
        }

        private static void OnUSDBTCPChanged(Object tickerObject, EventArgs e)
        {
            lock (lockTicket)
            {
                ticker = (tickerObject as TickerInfo);
            }
        }

        private static void OnUSDBTCPChanged2(Object tickerObject, EventArgs e)
        {
            lock (lockTicket)
            {
                ticker = (tickerObject as TickerInfo);
            }
        }

        private static void OnBTCeFeeChanged(Object feeObject, EventArgs e)
        {
            lock (lockFee)
            {
                fee = (feeObject as FeeInfo);
            }
        }

        private static void OnInfoReceived(Object infoObject, EventArgs e)
        {
            Console.WriteLine("Account Info: " + infoObject.ToString());
        }

        private static void OnActiveOrdersReceived(object activeOrdersList, EventArgs e)
        {
            Console.WriteLine("Active Orders: ");
            Console.WriteLine(activeOrdersList.ToString());
        }
    }
}