using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Egate_Payroll.Model
{
    public class shift_settings
    {
        public long Id { get; set; }

        public long CutoffId { get; set; }

        public long OnDuty { get; set; }

        public long OffDuty { get; set; }

        public long OvertimeOffset { get; set; }

        public long IsDeductOvertimeOffset { get; set; }

        public virtual cutoff cutoff { get; set; }
    }
}
