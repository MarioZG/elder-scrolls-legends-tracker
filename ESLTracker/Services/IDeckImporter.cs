using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Services
{
    public interface IDeckImporter
    {
        void CancelImport();
        Task ImportFromText(string importData);

    }
}
