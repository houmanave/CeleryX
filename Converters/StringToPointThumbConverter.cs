using Celery.Controls.Subcontrols;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Celery.Converters
{
    class StringToPointThumbConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(cecoPointFree) ||
                objectType == typeof(cecoPointEnd) ||
                objectType == typeof(cecoPointOrtho) ||
                objectType == typeof(cecoPointControl));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string str = reader.Value as string;
            string[] pointstr = str.Split(',');

            if (objectType == typeof(cecoPointFree))
            {
                return new cecoPointFree(new System.Windows.Point(
                    double.Parse(pointstr[0]),
                    double.Parse(pointstr[1])
                    ));
            }
            else
            if (objectType == typeof(cecoPointEnd))
            {
                return new cecoPointEnd(new System.Windows.Point(
                    double.Parse(pointstr[0]),
                    double.Parse(pointstr[1])
                    ),
                    bool.Parse(pointstr[2])
                    );
            }
            else
            if (objectType == typeof(cecoPointOrtho))
            {
                return new cecoPointOrtho(new System.Windows.Point(
                    double.Parse(pointstr[0]),
                    double.Parse(pointstr[1])
                    ),
                    double.Parse(pointstr[2]),
                    double.Parse(pointstr[3]),
                    bool.Parse(pointstr[4])
                    );
            }
            else
            if (objectType == typeof(cecoPointControl))
            {
                return new cecoPointControl(new System.Windows.Point(
                    double.Parse(pointstr[0]),
                    double.Parse(pointstr[1])
                    ));
            }
            else
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType() == typeof(cecoPointFree))
            {
                cecoPointFree cefr = (cecoPointFree)value;
                writer.WriteValue(cefr.ToString());
            }
            else
            if (value.GetType() == typeof(cecoPointEnd))
            {
                cecoPointEnd ceen = (cecoPointEnd)value;
                writer.WriteValue(ceen.ToString());
            }
            else
            if (value.GetType() == typeof(cecoPointOrtho))
            {
                cecoPointOrtho ceor = (cecoPointOrtho)value;
                writer.WriteValue(ceor.ToString());
            }
            else
            if (value.GetType() == typeof(cecoPointControl))
            {
                cecoPointControl ceco = (cecoPointControl)value;
                writer.WriteValue(ceco.ToString());
            }
            else
            {
                //return;
            }
        }
    }
}
