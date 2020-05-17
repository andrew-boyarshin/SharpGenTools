using System;
using System.Runtime.InteropServices;
using SharpGen.Runtime;

namespace Docs
{
    public partial class IUIAnimationManager
    {
        private static readonly Guid ManagerGuid = new Guid("4C1FC63A-695C-47E8-A339-1A194BE3D0B8");

        public IUIAnimationManager()
        {
            ComActivationHelpers.CreateComInstance(ManagerGuid, ComContext.InprocServer, typeof(IUIAnimationManager).GUID, out var ptr).CheckError();
            NativePointer = ptr;
        }
    }
}
