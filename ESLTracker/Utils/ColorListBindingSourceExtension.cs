using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ESLTracker.Utils
{
    public class ColorListBindingSourceExtension : MarkupExtension
    {
        public ColorListBindingSourceExtension() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Array colorValues = GetConstants(typeof(Color)).ToArray();
            return colorValues;
        }

        static List<Color> GetConstants(Type type)
        {
            MethodAttributes attributes = MethodAttributes.Static | MethodAttributes.Public;
            PropertyInfo[] properties = type.GetProperties();
            List<Color> list = new List<Color>();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo info = properties[i];
                if (info.PropertyType == typeof(Color))
                {
                    MethodInfo getMethod = info.GetGetMethod();
                    if ((getMethod != null) && ((getMethod.Attributes & attributes) == attributes))
                    {
                        object[] index = null;
                        list.Add((Color)info.GetValue(null, index));
                    }
                }
            }
            return list;
        }
    }
}
