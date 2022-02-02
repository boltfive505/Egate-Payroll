using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomControls.Modal
{
    public interface IModalCommand
    {
        Action<ModalResult, object> ExecuteResult { get; set; }
    }
}
