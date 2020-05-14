using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SharpGen.Doc;
using SharpGen.Extension.MicrosoftDocs;
using SharpGenTools.Sdk.Documentation;
using Xunit;
using Xunit.Abstractions;

namespace SharpGen.UnitTests.MicrosoftDocs
{
    public class DocProviderTests : TestBase
    {
        public DocProviderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task IUIAnimationInterpolator2_SetInitialValueAndVelocity()
        {
            var context = new DocumentationContext(Logger);
            var provider = new DocsProvider();
            var docItem = await provider.FindDocumentationAsync("IUIAnimationInterpolator2::SetInitialValueAndVelocity", context);
            Assert.Equal("nf-uianimation-iuianimationinterpolator2-setinitialvalueandvelocity", docItem.ShortId);
            Assert.Collection(
                docItem.Names,
                x => Assert.Equal("IUIAnimationInterpolator2::SetInitialValueAndVelocity", x)
            );
            Assert.Equal("Sets the initial value and velocity of the transition for the given dimension.", docItem.Summary);
            Assert.Equal("Windows Animation always calls <b>SetInitialValueAndVelocity</b> before calling the other methods of  <a href=\"https://docs.microsoft.com/windows/desktop/api/uianimation/nn-uianimation-iuianimationinterpolator2\">IUIAnimationInterpolator2</a> at different offsets. However, <b>SetInitialValueAndVelocity</b> can be called multiple times with different parameters. Interpolators can cache internal state to improve performance, but they must update this cached state each time <b>SetInitialValueAndVelocity</b> is called and ensure that the results of subsequent calls to these methods reflect the updated state.", docItem.Remarks);
            Assert.Equal("Returns <b>S_OK</b> if successful; otherwise an <b>HRESULT</b> error code. See <a href=\"https://docs.microsoft.com/windows/desktop/UIAnimation/uianimation-error-codes\">Windows Animation Error Codes</a> for a list of error codes.", docItem.Return);
            Assert.Collection(
                docItem.Items,
                item => AssertItem(
                    "initialValue [in]",
                    "The initial value.",
                    item
                ),
                item => AssertItem(
                    "initialVelocity [in]",
                    "The initial velocity.",
                    item
                ),
                item => AssertItem(
                    "cDimension [in]",
                    "The dimension in which to set the initial value or velocity of the transition.",
                    item
                )
            );
        }

        [Fact]
        public async Task RegisterClassExW()
        {
            var context = new DocumentationContext(Logger);
            var provider = new DocsProvider();
            var docItem = await provider.FindDocumentationAsync("RegisterClassExW", context);
            Assert.Equal("nf-winuser-registerclassexw", docItem.ShortId);
            Assert.Collection(
                docItem.Names,
                x => Assert.Equal("RegisterClassExW", x),
                x => Assert.Equal("RegisterClassEx", x)
            );
            Assert.Equal(
                "Registers a window class for subsequent use in calls to the <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-createwindowa\">CreateWindow</a> or <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-createwindowexa\">CreateWindowEx</a> function.",
                docItem.Summary
            );
            Assert.Equal(
                "<para>If you register the window class by using\n<b>RegisterClassExA</b>, the application tells the system that the windows of the created class expect messages with text or character parameters to use the ANSI character set; if you register it by using\n<b>RegisterClassExW</b>, the application requests that the system pass text parameters of messages as Unicode. The <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-iswindowunicode\">IsWindowUnicode</a> function enables applications to query the nature of each window. For more information on ANSI and Unicode functions, see <a href=\"https://docs.microsoft.com/windows/desktop/Intl/conventions-for-function-prototypes\">Conventions for Function Prototypes</a>.</para><para>All window classes that an application registers are unregistered when it terminates.</para><para>No window classes registered by a DLL are unregistered when the DLL is unloaded. A DLL must explicitly unregister its classes when it is unloaded.</para><h4>Examples</h4><para>For an example, see <a href=\"https://docs.microsoft.com/windows/desktop/winmsg/using-window-classes\">Using Window Classes</a>.</para><div class=\"code\"></div>",
                docItem.Remarks
            );
            Assert.Equal(
                "<para>Type: <b>ATOM</b></para><para>If the function succeeds, the return value is a class atom that uniquely identifies the class being registered. This atom can only be used by the <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-createwindowa\">CreateWindow</a>, <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-createwindowexa\">CreateWindowEx</a>, <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-getclassinfoa\">GetClassInfo</a>, <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-getclassinfoexa\">GetClassInfoEx</a>, <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-findwindowa\">FindWindow</a>, <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-findwindowexa\">FindWindowEx</a>, and <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-unregisterclassa\">UnregisterClass</a> functions and the <b>IActiveIMMap::FilterClientWindows</b> method.</para><para>If the function fails, the return value is zero. To get extended error information, call <a href=\"https://docs.microsoft.com/windows/desktop/api/errhandlingapi/nf-errhandlingapi-getlasterror\">GetLastError</a>.</para>",
                docItem.Return
            );
            Assert.Collection(
                docItem.Items,
                item => AssertItem(
                    "Arg1 [in]",
                    "<para>Type: <b>const WNDCLASSEX*</b></para><para>A pointer to a <a href=\"https://docs.microsoft.com/windows/desktop/api/winuser/ns-winuser-wndclassexa\">WNDCLASSEX</a> structure. You must fill the structure with the appropriate class attributes before passing it to the function.</para>",
                    item
                )
            );
        }

        [Fact]
        public async Task EmptyString()
        {
            var context = new DocumentationContext(Logger);
            var provider = new DocsProvider();
            var docItem = await provider.FindDocumentationAsync(string.Empty, context);
            Assert.Null(docItem);
        }

        [Fact]
        public async Task ShortString()
        {
            var context = new DocumentationContext(Logger);
            var provider = new DocsProvider();
            var docItem = await provider.FindDocumentationAsync("AB", context);
            Assert.Null(docItem);
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        private static void AssertItem(string expectedTerm, string expectedDescription, IDocSubItem actual)
        {
            Assert.Equal(expectedTerm, actual.Term);
            Assert.Equal(expectedDescription, actual.Description);
        }
    }
}