using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Galaxy {

	public interface IExtendableObject {
		Properties ExtendedProperties { get; }
	}

	public class Properties {

		public bool ContainsKey(string key) {
			return properties.ContainsKey(key);
		}

		private Dictionary<string, object> properties = new Dictionary<string, object>();
		public void SetValue<T>(string property, T value) {
			property = property.ToLower();
			if (!properties.ContainsKey(property))
				properties.Add(property, value);
			else
				properties[property] = value;
		}

		public T GetValue<T>(string property) {
			property = property.ToLower();
			if (properties.ContainsKey(property)) {
				T convertedValue = default(T);
				try {
					Convert.ChangeType(properties[property], typeof(T));
				} catch {}
				return convertedValue;
			}
			return default(T); 
		}

	}
}
