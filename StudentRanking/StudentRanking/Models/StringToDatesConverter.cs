using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentRanking.Models
{
    public class StringToDatesConverter
    {
        public DateTime getDateByString(String date)
        {
            String[] comp = date.Split('-');
            return new DateTime(Convert.ToInt32(comp[0]),
                                Convert.ToInt32(comp[1]),
                                Convert.ToInt32(comp[2]));

        }

    }
}