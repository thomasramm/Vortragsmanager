using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Vortragsmanager.UserControls
{
    public class CalendarYearShortViewModel
    {
        public CalendarYearShortViewModel()
        {
            Calendar = new List<CalendarYearShortItem>(53);

            var start = Core.Helper.GetConregationDay(new DateTime(Core.Helper.DisplayedYear, 1, 1));
            while (start.Year == Core.Helper.DisplayedYear)
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
