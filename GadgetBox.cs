using System;
using Terraria.ModLoader;

namespace GadgetBox
{
    class GadgetBox : Mod
    {
        public static GadgetBox Instance;

        public GadgetBox()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }
        
        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }

        internal ModPacket GetPacket(MessageType type, int capacity)
        {
            ModPacket packet = GetPacket(capacity + 1);
            packet.Write((byte)type);
            return packet;
        }

        internal static void Log(object message)
            => ErrorLogger.Log(string.Format("[{0}][{1}] {2}", Instance, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), message));

        internal static void Log(string message, params object[] formatData)
            => ErrorLogger.Log(string.Format("[{0}][{1}] {2}", Instance, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), string.Format(message, formatData)));

        public static void RedundantFunc()
        {
            var something = System.Linq.Enumerable.Range(1, 10);
        }
    }

    internal enum MessageType
    {
        // for future use
    }
}
