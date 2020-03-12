using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;

namespace DML
{
    public class GroupEventItems
    {
        public Group Group { get; set; }
        public IPagedList<Event> Events { get; set; }
    }
}
