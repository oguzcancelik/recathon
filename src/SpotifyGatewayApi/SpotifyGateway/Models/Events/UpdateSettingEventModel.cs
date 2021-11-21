using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.Events
{
    public class UpdateSettingEventModel
    {
        public UpdateSettingEventModel(SettingsClass settingsClass, string propertyName, object value)
        {
            SettingsClass = settingsClass;
            PropertyName = propertyName;
            Value = value;
        }

        public SettingsClass SettingsClass { get; set; }

        public string PropertyName { get; set; }

        public object Value { get; set; }
    }
}