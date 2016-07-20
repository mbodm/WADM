using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MBODM.WPF
{
    public sealed class DisplayNameExtension : MarkupExtension
    {
        private Type type;
        private string property;

        public DisplayNameExtension(Type type, string property)
        {
            this.type = type;
            this.property = property;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                var propertyInfo = type.GetProperty(property);

                var attributesArray = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);

                return (attributesArray.First() as DisplayNameAttribute).DisplayName;
            }
            catch
            {
                return null;
            }
        }
    }
}
