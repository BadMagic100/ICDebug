using Modding;
using System;

namespace ICDebug
{
    public class ICDebugMod : Mod
    {
        private static ICDebugMod? _instance;

        internal static ICDebugMod Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(ICDebugMod)} was never constructed");
                }
                return _instance;
            }
        }

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public ICDebugMod() : base("ICDebug")
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            

            Log("Initialized");
        }
    }
}
