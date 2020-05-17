using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        private static void AssertItem(IDocSubItem actual, string expectedTerm, string expectedDescription,
                                       params string[] attributes)
        {
            Assert.Equal(expectedTerm, actual.Term);
            Assert.Equal(expectedDescription, actual.Description);
            Assert.Equal(attributes, actual.Attributes);
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
        public async Task IUIAnimationInterpolator2_SetInitialValueAndVelocity()
        {
            var context = new DocumentationContext(Logger);
            var provider = new DocsProvider();
            var docItem =
                await provider.FindDocumentationAsync("IUIAnimationInterpolator2::SetInitialValueAndVelocity", context);
            Assert.Equal("nf-uianimation-iuianimationinterpolator2-setinitialvalueandvelocity", docItem.ShortId);
            Assert.Collection(
                docItem.Names,
                x => Assert.Equal("IUIAnimationInterpolator2::SetInitialValueAndVelocity", x)
            );
            Assert.Equal("Sets the initial value and velocity of the transition for the given dimension.",
                         docItem.Summary);
            Assert.Equal(
                "Windows Animation always calls <b>SetInitialValueAndVelocity</b> before calling the other methods of  <i>IUIAnimationInterpolator2</i> at different offsets. However, <b>SetInitialValueAndVelocity</b> can be called multiple times with different parameters. Interpolators can cache internal state to improve performance, but they must update this cached state each time <b>SetInitialValueAndVelocity</b> is called and ensure that the results of subsequent calls to these methods reflect the updated state.",
                docItem.Remarks);
            Assert.Equal(
                "Returns <b>S_OK</b> if successful; otherwise an <b>HRESULT</b> error code. See <i>Windows Animation Error Codes</i> for a list of error codes.",
                docItem.Return);
            Assert.Collection(
                docItem.Items,
                item => AssertItem(item, "initialValue", "The initial value.", "in"),
                item => AssertItem(item, "initialVelocity", "The initial velocity.", "in"),
                item => AssertItem(item, "cDimension",
                                   "The dimension in which to set the initial value or velocity of the transition.",
                                   "in")
            );
            Assert.Empty(docItem.SeeAlso);
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
                "Registers a window class for subsequent use in calls to the <i>CreateWindow</i> or <i>CreateWindowEx</i> function.",
                docItem.Summary
            );
            Assert.Equal(
                "<para>If you register the window class by using <b>RegisterClassExA</b>, the application tells the system that the windows of the created class expect messages with text or character parameters to use the ANSI character set; if you register it by using <b>RegisterClassExW</b>, the application requests that the system pass text parameters of messages as Unicode. The <i>IsWindowUnicode</i> function enables applications to query the nature of each window. For more information on ANSI and Unicode functions, see <i>Conventions for Function Prototypes</i>.</para><para>All window classes that an application registers are unregistered when it terminates.</para><para>No window classes registered by a DLL are unregistered when the DLL is unloaded. A DLL must explicitly unregister its classes when it is unloaded.</para><h4>Examples</h4><para>For an example, see <i>Using Window Classes</i>.</para>",
                docItem.Remarks
            );
            Assert.Equal(
                "<para>Type: <b>ATOM</b></para><para>If the function succeeds, the return value is a class atom that uniquely identifies the class being registered. This atom can only be used by the <i>CreateWindow</i>, <i>CreateWindowEx</i>, <i>GetClassInfo</i>, <i>GetClassInfoEx</i>, <i>FindWindow</i>, <i>FindWindowEx</i>, and <i>UnregisterClass</i> functions and the <b>IActiveIMMap::FilterClientWindows</b> method.</para><para>If the function fails, the return value is zero. To get extended error information, call <i>GetLastError</i>.</para>",
                docItem.Return
            );
            Assert.Collection(
                docItem.Items,
                item => AssertItem(
                    item,
                    "Arg1",
                    "<para>Type: <b>const WNDCLASSEX*</b></para><para>A pointer to a <i>WNDCLASSEX</i> structure. You must fill the structure with the appropriate class attributes before passing it to the function.</para>",
                    "in"
                )
            );
            Assert.Empty(docItem.SeeAlso);
        }

        [Fact]
        public async Task Search_IUIAnimationVariable_SetRoundingMode()
        {
            var provider = new DocsProvider();
            var result = await provider.SearchDocumentation("IUIAnimationVariable::SetRoundingMode");
            Assert.Equal(new[] {"IUIAnimationVariable::SetRoundingMode"}, result.Names);
            Assert.Equal("https://docs.microsoft.com/en-us/windows/win32/api/uianimation/nf-uianimation-iuianimationvariable-setroundingmode", result.PublishedUrl);
        }

        [Fact]
        public async Task Search_IUIAnimationVariable_SetLowerBound()
        {
            var provider = new DocsProvider();
            var result = await provider.SearchDocumentation("IUIAnimationVariable::SetLowerBound");
            Assert.Equal(new[] {"IUIAnimationVariable::SetLowerBound"}, result.Names);
            Assert.Equal("https://docs.microsoft.com/en-us/windows/win32/api/uianimation/nf-uianimation-iuianimationvariable-setlowerbound", result.PublishedUrl);
        }

        [Fact]
        public async Task Process_IUIAnimationVariable_GetFinalValue()
        {
            var context = new DocumentationContext(Logger);
            var docItem = await DocsProvider.ParseDocumentationFromSourceUrl("https://raw.githubusercontent.com/MicrosoftDocs/sdk-api/docs/sdk-api-src/content/uianimation/nf-uianimation-iuianimationvariable-getfinalvalue.md", context);
            Assert.Equal(
                "<para>Gets the final value of the animation variable.</para><para>\nThis is the value after all currently scheduled animations have completed.</para>",
                docItem.Summary
            );
            Assert.Equal(
                "The result can be affected by the lower and upper bounds determined by <i>IUIAnimationVariable::SetLowerBound</i> and <i>IUIAnimationVariable::SetUpperBound</i>, respectively.",
                docItem.Remarks
            );
            Assert.Equal(
                "<para>If the method succeeds, it returns S_OK. Otherwise, it returns an <b>HRESULT</b> error code. See <i>Windows Animation Error Codes</i> for a list of error codes.</para><table> <tbody><tr> <th>Return code</th> <th>Description</th> </tr> <tr> <td width=\"40%\"> <dl> <dt><b>UI_E_VALUE_NOT_DETERMINED</b></dt> </dl> </td> <td width=\"60%\"> The final value of the animation variable cannot be determined at this time. </td> </tr> </tbody></table>",
                docItem.Return
            );
            Assert.Collection(
                docItem.Items,
                item => AssertItem(
                    item,
                    "finalValue",
                    "The final value of the animation variable.",
                    "out"
                )
            );
            Assert.Empty(docItem.SeeAlso);
        }

        [Fact]
        public async Task ShortString()
        {
            var context = new DocumentationContext(Logger);
            var provider = new DocsProvider();
            var docItem = await provider.FindDocumentationAsync("AB", context);
            Assert.Null(docItem);
        }
    }
}