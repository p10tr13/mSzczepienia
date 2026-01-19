using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.DTO.Queries
{
    public class GetVaccinationRecordOverviewQuery
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;

        public string sortingField { get; set; } = "vaccinationDate";
        public string sortingDirection { get; set; } = "desc";
    }

}
