using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Noloc
{
    public class NolocRender : XmlDocObjectRenderer<NolocInline>
    {
        protected override void Write(XmlDocRenderer renderer, NolocInline obj)
        {
            renderer.Write(obj.Text);
        }
    }
}
