using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DIBZ.Common
{
 public class EmailTemplateHelper
    {
        private const string PlaceholderRegexPattern = @"\[([\w\.]+)\]";
        private Dictionary<string, object> _templateParams;

        public EmailTemplateHelper()
        {
            _templateParams = new Dictionary<string, object>();
        }

        public void AddParam(string name, object param)
        {
            _templateParams.Add(name.ToLower(), param);
        }

        public void ClearParams()
        {
            _templateParams.Clear();
        }

        public string FillTemplate(string template)
        {

            if (template == null)
                return null;

            template = template.Replace("[n]", "\r\n");

            var regex = new Regex(PlaceholderRegexPattern);
            return regex.Replace(template,
                match => ConversionHelper.SafeConvertToText(GetValue(match.Value.ToLower()), string.Empty));
        }

        private object GetValue(string valuePath)
        {
            string[] names = valuePath.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            if (names.Length <= 0)
                return null;
            if (names.Length == 1)
            {
                if (_templateParams.ContainsKey(names[0]))
                    return FormatValue(_templateParams[names[0]]);
                else
                    return null;
            }

            object currentObject = null;
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                currentObject = GetPropertyValue(currentObject, name);
            }

            return FormatValue(currentObject);
        }

        private object FormatValue(object value)
        {
            if (value is decimal || value is float || value is double
            || value is decimal? || value is float? || value is double?)
            {
                return string.Format("{0:0.00}", value);
            }

            return value;
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null)
            {
                try
                {
                    return _templateParams[propertyName];
                }
                catch
                {
                    return propertyName;
                }
            }

            var objType = obj.GetType();
            var properties = objType.GetProperties();

            var property = (from p in properties
                            where p.Name.ToLower() == propertyName
                            select p).FirstOrDefault();

            if (property != null)
            {
                var value = property.GetGetMethod().Invoke(obj, null);
                return value;
            }

            var fields = objType.GetFields();

            var field = (from f in fields
                         where f.Name.ToLower() == propertyName
                         select f).FirstOrDefault();

            if (field != null)
            {
                return field.GetValue(obj);
            }

            return null;
        }
    }
}
