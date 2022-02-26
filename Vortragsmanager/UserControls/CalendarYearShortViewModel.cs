using System;
using System.Collections.Generic;
using Vortragsmanager.Helper;

namespace Vortragsmanager.UserControls
{
    public class CalendarYearShortViewModel
    {
        public CalendarYearShortViewModel()
        {
            Calendar = new List<CalendarYearShortItem>(53);

            var start = DateCalcuation.GetConregationDay(new DateTime(Helper.Helper.DisplayedYear, 1, 1));
            while (start.Year == Helper.Helper.DisplayedYear)
            {
                var item = new CalendarYearShortItem(start);
                Calendar.Add(item);
                start = start.AddDays(7);
            }
            //dxlc:FlowLayoutControl.IsFlowBreak="True"
        }



        public List<CalendarYearShortItem> Calendar { get; }
    }

    
}
