using System.Linq;

namespace ReMod.Core.VRChat
{
    public class VRCUiManagerEx
    {
        private static VRCUiManager _uiManagerInstance;

        public static VRCUiManager Instance
        {
            get
            {
                if (_uiManagerInstance == null)
                {
                    _uiManagerInstance = (VRCUiManager)typeof(VRCUiManager).GetMethods().First(x => x.ReturnType == typeof(VRCUiManager)).Invoke(null, new object[0]);
                }

                return _uiManagerInstance;
            }
        }
    }
}
