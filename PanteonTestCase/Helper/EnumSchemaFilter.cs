using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace PanteonTestCase.Common
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                model.Enum.Clear();
               


                var list = Enum.GetNames(context.Type).ToList();
                foreach (var item in list)
                {
                    try
                    {
                        var displayAttr = context.Type.GetMember(item).First().GetCustomAttribute<DisplayAttribute>();
                        var name = displayAttr == null ? item : displayAttr.Name;
                        model.Enum.Add(new OpenApiString($"{Convert.ToInt64(Enum.Parse(context.Type, item))} = {name}"));
                    }
                    catch (Exception)
                    {
                        model.Enum.Add(new OpenApiString(item));
                    }
                }
            }
        }
    }
}