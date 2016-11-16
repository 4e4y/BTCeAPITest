using BTCeAPI;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BTCeAPITest
{
    class Program
    {
        private static ILog logger = log4net.LogManager.GetLogger("Console");
        private static FeeInfo fee = null;
        private static TickerInfo ticker = null;

        private static object lockFee = new object();
        private static object lockTicket = new object();

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Console.CursorVisible = false;

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

            Console.WindowHeight = 5;
            Console.WindowWidth = 80;
            Console.BufferHeight = 5;
            Console.BufferWidth = 80;
            Console.Title = "BTCeAPI Test Application";

            CreateGUI();

            BTCeAPIWrapper api = BTCeAPIWrapper.Instance;
            api.PriceChangePushIndicator = BTCeAPIWrapper.PUSH_PRICE_CHANGE_BUY | BTCeAPIWrapper.PUSH_PRICE_CHANGE_SELL;
            // api.TickerTimeout = 2;
            // api.FeeTimeout = 40;
            api.PriceChanged += new EventHandler(OnPriceChanged);
            api.FeeChanged += new EventHandler(OnBTCeFeeChanged);

            api.Credential(key, secret);

            /*
            // api.InfoReceived += new EventHandler(OnInfoReceived);
            api.CurrencyAmountChanged += new EventHandler(OnCurrencyAmountChanged);
            api.ActiveOrdersReceived += new EventHandler(OnActiveOrdersReceived);

            api.ActiveOrdersCountChanged += new EventHandler(OnActiveOrdersCountChange);
            api.ActiveOrdersTotalAmountChanged += new EventHandler(OnActiveOrdersTotalAmountChange);
             * */

            Console.ReadLine();

            logger.Info("Application ended ...");
        }

        private static void CreateGUI()
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.Write("*".PadLeft(80, '*'));

            Console.CursorLeft = 0;
            Console.CursorTop = 1;
            //                         1         2         3         4         5         6         7
            //               01234567890123456789012345678901234567890123456789012345678901234567890123456789
            Console.Write(  "* Currency: BTC_USD; BUY:            ; SELL:            ; VOL:                 *");

            Console.CursorLeft = 0;
            Console.CursorTop = 2;
            Console.Write("* Fee:     ;                                                                   *");

            Console.CursorLeft = 0;
            Console.CursorTop = 3;
            Console.Write("*".PadLeft(80, '*'));
        }

        private static void OnPriceChanged(Object tickerObject, EventArgs e)
        {
            TickerInfo ti = (tickerObject as TickerInfo);
            PriceChangedEventArgs args = (e as PriceChangedEventArgs);

            if (ti != null && args != null)
            {
                lock (lockTicket)
                {
                    if (ticker == null)
                    {
                        ticker = ti;

                        Console.CursorLeft = 26;
                        Console.CursorTop = 1;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(string.Format("{0:F6}", ticker.Buy).PadLeft(11, ' '));

                        Console.CursorLeft = 45;
                        Console.CursorTop = 1;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(string.Format("{0:F6}", ticker.Sell).PadLeft(11, ' '));

                        Console.CursorLeft = 63;
                        Console.CursorTop = 1;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(string.Format("{0:F0}/{1:F0}", ticker.VolumeCurrency, ticker.Volume).PadLeft(15, ' '));

                        return;
                    }

                    ticker = ti;
                }

                if (
                    (args.PriceChangedIndicator & BTCeAPIWrapper.PRICE_CHANGED_BUY_UP) > 0 ||
                    ((args.PriceChangedIndicator & BTCeAPIWrapper.PRICE_CHANGED_BUY_DOWN) > 0)
                )
                {
                    Console.CursorLeft = 26;
                    Console.CursorTop = 1;

                    if ((args.PriceChangedIndicator & BTCeAPIWrapper.PRICE_CHANGED_BUY_UP) > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(string.Format("{0:F6}", ticker.Buy).PadLeft(11, ' '));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(string.Format("{0:F6}", ticker.Buy).PadLeft(11, ' '));
                    }
                }

                if (
                    (args.PriceChangedIndicator & BTCeAPIWrapper.PRICE_CHANGED_SELL_UP) > 0 ||
                    ((args.PriceChangedIndicator & BTCeAPIWrapper.PRICE_CHANGED_SELL_DOWN) > 0)
                )
                {
                    Console.CursorLeft = 45;
                    Console.CursorTop = 1;

                    if ((args.PriceChangedIndicator & BTCeAPIWrapper.PRICE_CHANGED_SELL_UP) > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(string.Format("{0:F6}", ticker.Sell).PadLeft(11, ' '));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(string.Format("{0:F6}", ticker.Sell).PadLeft(11, ' '));
                    }
                }

                Console.CursorLeft = 63;
                Console.CursorTop = 1;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(string.Format("{0:F0}/{1:F0}", ticker.VolumeCurrency, ticker.Volume).PadLeft(15, ' '));
            }
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
                Console.WriteLine("New Price: " + ticker);
                logger.Debug(ticker);
            }
        }

        private static void OnBTCeFeeChanged(Object feeObject, EventArgs e)
        {
            FeeInfo feeInfo = (feeObject as FeeInfo);
            if (feeInfo != null)
            {
                lock (lockFee)
                {
                    if (fee == null)
                    {
                        fee = feeInfo;

                        Console.CursorLeft = 7;
                        Console.CursorTop = 2;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(string.Format("{0:F1}%", fee.Fee).PadLeft(4, ' '));

                        return;
                    }
                    fee = feeInfo;

                    Console.CursorLeft = 7;
                    Console.CursorTop = 2;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(string.Format("{0:F1}%", fee.Fee).PadLeft(4, ' '));
                }
            }
        }

        private static void OnInfoReceived(Object infoObject, EventArgs e)
        {
            Console.WriteLine("Account Info: " + infoObject.ToString());
        }

        private static void OnCurrencyAmountChanged(object infoObject, EventArgs e)
        {
            CurrencyEventArgs cea = (e as CurrencyEventArgs);
            if (cea != null &&
                cea.Currencies.Count > 0)
            {
                Console.WriteLine("Changed Currencies: " + cea.Currencies);
            }
        }

        private static void OnActiveOrdersReceived(object activeOrdersList, EventArgs e)
        {
            Console.WriteLine("Active Orders: ");
            Console.WriteLine(activeOrdersList.ToString());
        }

        private static void OnActiveOrdersCountChange(object activeOrdersList, EventArgs e)
        {
            Console.WriteLine("Active Orders Count changed: ");
            Console.WriteLine(activeOrdersList.ToString() + "; New Orders count: " + (e as DecimalEventArgs).Value);
        }

        private static void OnActiveOrdersTotalAmountChange(object activeOrdersList, EventArgs e)
        {
            Console.WriteLine("Active Orders Total Amount changed: ");
            Console.WriteLine(activeOrdersList.ToString() + "; New Total Amount: " + (e as DecimalEventArgs).Value);
        }
    }
}
