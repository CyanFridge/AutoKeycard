using EFT;
using EFT.Communications;

namespace AutoKeycard
{
    public class AutoKeycardNotification : Notification
    {
        private readonly string _message;

        public override string Description
        {
            get
            {
                return _message;
            }
        }

        public override ENotificationIconType Icon
        {
            get
            {
                return ENotificationIconType.Default;
            }
        }

        public AutoKeycardNotification(string message)
        {
            Duration = ENotificationDurationType.Default;
            _message = message;
        }
    }
}