using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GadgetBox
{
    class GadgetBox : Mod
    {
        internal static GadgetBox Instance;
		internal static string AnyGoldBar;

        public GadgetBox()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
			AnyGoldBar = Name + ":AnyGoldBar";
		}
        
        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }

		public override void AddRecipeGroups()
		{
			RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Language.GetTextValue("ItemName.GoldBar"), new int[]
			{
				ItemID.GoldBar,
				ItemID.PlatinumBar
			});
			RecipeGroup.RegisterGroup(AnyGoldBar, group);
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

        //public static void RedundantFunc()
        //{
        //    var something = System.Linq.Enumerable.Range(1, 10);
        //}
    }

    internal enum MessageType
    {
        // for future use
    }
}
