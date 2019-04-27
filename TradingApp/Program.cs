using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Trades FORMAT "NAME;OPENING PRICE(IF WITH- then its SHORT POSITION; Position SIZE|next..."
            //Current Bid and Ask Price FORMAT "NAME;ASKPrice;BIDPrice|NAME;ASKPrice;BIDPrice"
            var instruments = Trades2("DAX30;-12200,22;0,2|FTSE100;-11000,20;0,1|US100;11000;0,4"); //Remember about Locales when inputing price , or . in floats 
            var currentPrices = Prices("DAX30;11200,20;11210,70|FTSE100;10500,20;10520,40|US100;9500,70;9507,40");
            ShowALlTrades(instruments);
            ShowALlInstruments(currentPrices);
            var PositionsWithPnL = CountPositions(instruments, currentPrices);
            WriteTradesWithPnl(PositionsWithPnL);

            var PnLOfAll = CountPnL(PositionsWithPnL);

            Console.WriteLine("");
            Console.WriteLine("$$$$$$$$$$$$        Your PnL: " + PnLOfAll + "        $$$$$$$$$$$");
            

            Console.ReadKey();
            
        }

        static void WriteTradesWithPnl(List<PositionPnL> positionsPnL)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("*********** Positions with PnL ***********");

            foreach(var position in positionsPnL)
            {
                Console.WriteLine("Instrument: " + position.Name + " Opening Price: " + position.OpeningPrice + " Current Price: " + position.CurrentPrice + " Position Size: " + position.PositionSize +  " Position Value: " + position.PositionValue + " PnL: " + position.PnL);

            }
        }
        static void ShowALlTrades(List<InstrumentPosition> instruments)
        {
            Console.WriteLine("*********** ALL YOUR TRADES *************");
            foreach (var i in instruments)
            {
                var Price = Convert.ToString(i.Price);
                var Size = Convert.ToString(i.Size);
                var type = "";
                if (i.Price < 0)
                {
                    type = "sell";
                }
                else
                {
                    type = "buy";
                }
                Console.WriteLine("Instrument: " + i.Name + "  Type: " + type + " Opening Price:" + Price + "  Size:" + Size);
            }
        }

        static void ShowALlInstruments(List<InstrumentPrice> instruments)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("*********** ALL Instruments *************");
            foreach (var i in instruments)
            {
                var BidPrice = Convert.ToString(i.BidPrice);
                var AskPrice = Convert.ToString(i.AskPrice);
                Console.WriteLine("Instrument: " + i.Name + "  AskPrice:  " + AskPrice + "  BidPrice:  " + BidPrice);
            }
        }

        static void ShowAccountBalance(AccountBalance balance)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("*********** Your account *************");
            Console.WriteLine("Balance: " + balance.Total + "  Free Margin:  " + balance.Free  + "  PnL:  " + balance.PnLOfAll);
        }

        public class InstrumentPosition
        {
            public string Name;
            public float Price;
            public float Size;
            

            public InstrumentPosition(string Name, string Price, string Size)
            {
                this.Name = Name;
                this.Price = float.Parse(Price);
                this.Size = float.Parse(Size); ;
            }
        }

        public class AccountBalance
        {
            public float Total;
            public float Free;
            public float PnLOfAll;

            public AccountBalance(float Total, float Free, float PnLOfAll)
            {
                this.Total = Total;
                this.Free = Free;
                this.PnLOfAll = PnLOfAll;
            }
        }

        public class InstrumentPrice
        {
            public string Name;
            public float AskPrice;
            public float BidPrice;

            public InstrumentPrice(string name, string askPrice, string bidPrice)
            {
                this.Name = name;
                this.AskPrice = float.Parse(askPrice);
                this.BidPrice = float.Parse(bidPrice);
            }
        }
        public class PositionPnL
        {
            public string Name { get; set; }
            public float OpeningPrice { get; set; }
            public float CurrentPrice { get; set; }
            public float PnL { get; set; }
            public float PositionSize { get; set; }
            public float PositionValue { get; set; }

            public PositionPnL(string name,float openingPrice, float currentPrice, float pnL, float positionSize, float positionValue)
            {
                Name = name;
                OpeningPrice = openingPrice;
                CurrentPrice = currentPrice;
                PnL = pnL;
                PositionSize = positionSize;
                PositionValue = positionValue;
            }

        }

        public static float CountPnL(List<PositionPnL> positionsPnL)
        {
            float total = positionsPnL.Sum(item => item.PnL);
            return total;
        }

        public static List<PositionPnL> CountPositions(List<InstrumentPosition> positions, List<InstrumentPrice> prices)
        {
            var positionsPnL = new List<PositionPnL>();
            foreach(var position in positions)
            {
                if(position.Price>0)
                {
                    var instrument = prices
                        .Where(p => p.Name == position.Name)
                        .FirstOrDefault();
                    var balance = position.Size * (instrument.BidPrice - position.Price);
                    var positionBalance = new PositionPnL(position.Name, position.Price, instrument.BidPrice, balance, position.Size, position.Size*instrument.BidPrice);
                    positionsPnL.Add(positionBalance);

                }
                else
                {
                    var instrument = prices
                        .Where(p => p.Name == position.Name)
                        .FirstOrDefault();
                    var balance = position.Size * (-1 *( position.Price + instrument.AskPrice));
                    var positionBalance = new PositionPnL(position.Name, position.Price, instrument.AskPrice, balance, position.Size, position.Size * instrument.AskPrice);
                    positionsPnL.Add(positionBalance);
                }
                
            }
            return positionsPnL;
            

        }

        //public CountAll()
        

        public static List<InstrumentPosition> Trades2(String S)
        {
            var instruments = S.Split(';', '|');
            var n = instruments.Count();
            var p = 0;
            var Name = "";
            var Size = "";
            var Price = "";
            var tradeInstruments = new List<InstrumentPosition>();
            foreach (var i in instruments)
            {
                n--;
                if(p==0)
                {
                    Name = i;
                    p++;
                }
                else if (p==1)
                {
                    Price = i;
                    p++;
                }
                else
                {
                    Size = i;
                    var tradeInstrument = new InstrumentPosition(Name, Price, Size);
                    tradeInstruments.Add(tradeInstrument);
                    p = 0;
                }
                
            }
            return tradeInstruments;
            
        }

        public static List<InstrumentPrice> Prices(String S)
        {
            var instruments = S.Split(';', '|');
            var n = instruments.Count();
            var p = 0;
            var Name = "";
            var AskPrice = "";
            var BidPrice = "";
            var tradeInstruments = new List<InstrumentPrice>();
            foreach (var i in instruments)
            {
                n--;
                if (p == 0)
                {
                    Name = i;
                    p++;
                }
                else if (p == 1)
                {
                    AskPrice = i;
                    p++;
                }
                else
                {
                    BidPrice = i;
                    var tradeInstrument = new InstrumentPrice(Name, AskPrice, BidPrice);
                    tradeInstruments.Add(tradeInstrument);
                    p = 0;
                }

            }
            return tradeInstruments;

        }

        public static String ReverseString(String s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new String(arr);

        }

        public static List<InstrumentPrice> Trades(string S)
        {
            var SArray = S.ToArray();
            var Commas = 0;
            var LastComma = 0;
            var Instruments = new List<InstrumentPrice>();
            var LastPipe = 0;
            var NameS = "";
            var PriceS = "";
            var SizeS = "";
            StringBuilder Name = new StringBuilder();
            StringBuilder Price = new StringBuilder();
            StringBuilder Size = new StringBuilder();


            for (var i = 0; i < SArray.Length; i++)
            {
                if (Commas == 0)
                {
                    if (SArray[i] == ',')
                    {
                        for (var n = i-1; n >= LastComma; n--)
                        {
                            var p = 0;
                            Name.Insert(p, SArray[n]);
                            var namess = SArray[n];
                            p++;
                        }
                        NameS = Name.ToString();
                        NameS = ReverseString(NameS);
                        Commas++;
                        LastComma = i;
                    }
                }
                else if (Commas == 1)
                {
                    if (SArray[i] == ',')
                    {
                        for (var n = i-1; n > LastComma; n--)
                        {
                            var p = 0;
                            Price.Insert(p, SArray[n]);
                            p++;
                        }
                        PriceS = Name.ToString();
                        PriceS = ReverseString(PriceS);
                        Commas++;
                        LastComma = i;
                    }
                    else
                    {
                        
                    }
                }
                else if (Commas == 2)
                {
                    if (SArray[i] == '|')
                    {
                        for (var n = i-1; n > LastComma; n--)
                        {
                            var p = 0;
                            Size.Insert(p, SArray[n]);
                            p++;
                        }
                        SizeS = Name.ToString();
                        SizeS = ReverseString(SizeS);
                        Commas = 0;
                        LastPipe = i;
                        var Instrument = new InstrumentPrice(NameS, PriceS, SizeS);
                        Instruments.Add(Instrument);
                    }
                }
                    
                    
            }
            return Instruments;

        }
    }
}


