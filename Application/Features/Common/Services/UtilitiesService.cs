using Application.Features.Common.Interfaces;

namespace Application.Features.Common.Services;

public class UtilitiesService : IUtilitiesService
{
    public void TrimStrings<T>(T model)
    {
        if (model == null) return;

        var properties = typeof(T).GetProperties();
        foreach (var prop in properties)
        {
            if (prop.PropertyType == typeof(string) && prop.CanRead && prop.CanWrite)
            {
                string? value = (string?)prop.GetValue(model);
                if (!string.IsNullOrEmpty(value))
                    prop.SetValue(model, value.Trim());
            }
        }
    }
}
