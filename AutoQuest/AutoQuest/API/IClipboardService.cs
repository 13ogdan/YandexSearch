using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoQuest.API
{
    public interface IClipboardService
    {
        void SaveToClipboard(string text);
    }
}
