using ESLTracker.Utils.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels
{
    public class ViewModelLocator
    {
      //  AggregateCatalog catalog;

        public ViewModelLocator()
        {
          //  catalog = new AggregateCatalog();
         //   catalog.Catalogs.Add(new AssemblyCatalog(typeof(ViewModelLocator).Assembly));
         //  // CompositionContainer _container = new CompositionContainer(catalog);
        }

        public object this[string key]
        {
            get
            {
               // var typename = catalog.Parts.Where(p => p.ExportDefinitions.First().ContractName.EndsWith("."+key)).Select( p=> p.ExportDefinitions.First().ContractName).Single();
                return MasserContainer.Container.GetInstance(Type.GetType("ESLTracker.ViewModels."+key));
            }
        }
    }
}
