using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Utilitarios.Service
{
    public class UtilitarioMapperServicecopy
    {
        public TDestination Map<TDestination>(object source)
    where TDestination : new()
        {
            try
            {
                if (source == null)
                    throw new ArgumentNullException(nameof(source));

                var destination = new TDestination();
                var sourceProps = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var destProps = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var sourceProp in sourceProps)
                {
                    var destProp = destProps.FirstOrDefault(p =>
                                                            p.Name == sourceProp.Name &&
                                                            p.CanWrite);
                    if (destProp != null)
                    {
                        var value = sourceProp.GetValue(source);

                        if (value != null &&
                            sourceProp.PropertyType.IsEnum &&
                            destProp.PropertyType == typeof(string))
                        {
                            destProp.SetValue(destination, ObterDisplayName((Enum)value));
                        }
                        else if (value != null)
                        {
                            // int -> Enum
                            if (destProp.PropertyType.IsEnum &&
                                value is int intValue)
                            {
                                destProp.SetValue(
                                    destination,
                                    Enum.ToObject(destProp.PropertyType, intValue));
                            }

                            // int -> Enum?
                            else if (Nullable.GetUnderlyingType(destProp.PropertyType)?.IsEnum == true &&
                                     value is int nullableEnumValue)
                            {
                                var enumType = Nullable.GetUnderlyingType(destProp.PropertyType)!;

                                destProp.SetValue(
                                    destination,
                                    Enum.ToObject(enumType, nullableEnumValue));
                            }

                            // Enum -> int
                            else if (sourceProp.PropertyType.IsEnum &&
                                     destProp.PropertyType == typeof(int))
                            {
                                destProp.SetValue(destination, (int)value);
                            }

                            else
                            {
                                destProp.SetValue(destination, value);
                            }
                        }
                    }
                }

                return destination;
            }
            catch (Exception ex)
            {
                throw new Exception($"Falha no Mapper. Origem: {source.GetType().Name}",ex);
            }
        }

        public List<TDestination> Map<TDestination>(IEnumerable<object> sources)
          where TDestination : new()
        {
            try
            {
                if (sources == null)
                    throw new ArgumentNullException(nameof(sources));

                return sources.Select(source => Map<TDestination>(source)).ToList();
            }
            catch (Exception)
            {

                throw new Exception("Falha no Mapper");
            }

        }

        private static string ObterDisplayName(Enum valor)
        {
            var member = valor.GetType().GetMember(valor.ToString()).FirstOrDefault();

            return member?
                .GetCustomAttribute<DisplayAttribute>()?
                .Name
                ?? valor.ToString();
        }
    }
}
