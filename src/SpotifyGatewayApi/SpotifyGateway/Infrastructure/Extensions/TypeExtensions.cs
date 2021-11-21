using System;
using System.ComponentModel;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class TypeExtensions
    {
        public static bool TryConvert<T>(this object value, out T result)
        {
            try
            {
                if (typeof(T).IsEnum)
                {
                    result = (T)Enum.Parse(typeof(T), value.ToString());
                    return true;
                }

                if (value is IConvertible && value.TryChangeType(out result) || value.TryWithConverter(out result))
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                result = default;

                return false;
            }
        }

        public static bool TryConvert(this object value, Type type, out object result)
        {
            try
            {
                if (type.IsEnum && Enum.TryParse(type, value.ToString(), out result))
                {
                    return true;
                }

                return value is IConvertible && value.TryChangeType(out result, type) || value.TryWithConverter(out result, type);
            }
            catch (Exception)
            {
                result = default;

                return false;
            }
        }

        public static bool TryGetProperty<T>(this object src, string propName, out T result)
        {
            try
            {
                var value = src.GetType().GetProperty(propName)?.GetValue(src);

                return value.TryConvert(out result);
            }
            catch (Exception)
            {
                result = default;

                return false;
            }
        }

        private static bool TryChangeType<T>(this object value, out T result, Type type = null)
        {
            type ??= typeof(T);

            try
            {
                result = (T)Convert.ChangeType(value, type);
                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }

        private static bool TryWithConverter<T>(this object value, out T result, Type type = null)
        {
            type ??= typeof(T);

            try
            {
                result = (T)TypeDescriptor.GetConverter(type).ConvertTo(value, type);
                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }
    }
}