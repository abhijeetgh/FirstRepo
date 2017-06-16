using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Providers.Helper
{    
    /// <summary>
    /// Exposes the properties of multiple objects to make data binding and named-string formatting simpler.
    /// </summary>
    internal class ReplacementParameters : ICustomTypeDescriptor
    {
        private readonly IEnumerable<object> _objects;
        private Dictionary<PropertyDescriptor, object> _properties;

        public ReplacementParameters(IEnumerable<object> objects)
        {
            _objects = objects;
        }

        private Dictionary<PropertyDescriptor, object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    var propertyNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

                    _properties =
                        (from o in _objects
                         from PropertyDescriptor p in TypeDescriptor.GetProperties(o)
                         where propertyNames.Add(p.Name)
                         select new { Property = p, Obj = o }).ToDictionary(x => x.Property, x => x.Obj);
                }
                return _properties;
            }
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            var properties = Properties.Select(p => p.Key);

            if (attributes != null && attributes.Any())
                properties = properties.Where(p => p.Attributes.Contains(attributes));

            return new PropertyDescriptorCollection(properties.ToArray(), true);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return Properties[pd];
        }
    }
}
