using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll
{
    public static class TaskExt
    {
        public static TResult GetResult<TResult>(this Task<TResult> task)
        {
            task.Wait();
            return task.Result;
        }
    }
}
