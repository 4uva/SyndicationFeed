using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.Controllers
{
    static class CompareHelper
    {
        static public int ComparePublicationsByDate(Publication p1, Publication p2)
        {
            if (p1 == null && p2 == null)
                return 0; // equals
            else if (p2 == null)
                return 1; // greater
            else if (p1 == null)
                return -1; // less
            else
                return Nullable.Compare(p1.PublishTime, p2.PublishTime);
        }
    }
}
