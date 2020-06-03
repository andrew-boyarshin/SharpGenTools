using System;
using System.Runtime.InteropServices;
using SharpGen.Runtime;
using Xunit;

namespace Docs
{
    public class DocsTests
    {
        [SkippableFact]
        public void BasicCreation()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            using var manager = new IUIAnimationManager();
            Assert.Equal(UiAnimationManagerStatus.Idle, manager.Status);
            manager.CreateStoryboard(out var storyboard);
            Assert.Equal(UiAnimationStoryboardStatus.Building, storyboard.Status);
            storyboard.Schedule(0, out var result);
            Assert.Equal(UiAnimationSchedulingResult.Succeeded, result);
            manager.Update(0, out var result1);
            Assert.Equal(UiAnimationUpdateResult.NoChange, result1);
            manager.Shutdown();
        }
    }
}
