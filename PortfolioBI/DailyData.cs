using System;

namespace SecurityPriceAnalysis
{
    internal class DailyData
    {
        public DateTime Date { get; internal set; }
        public decimal Open { get; internal set; }
        public decimal High { get; internal set; }
        public decimal Low { get; internal set; }
        public decimal Close { get; internal set; }
        public decimal AdjustedClose { get; internal set; }
        public long Volume { get; internal set; }
    }
}