using Serilog.Templates;

namespace Product.Api.SerilogTemplates
{
    public class DatadogJsonNoTemplateFormatter : ExpressionTemplate
    {
        public DatadogJsonNoTemplateFormatter() : base(@"{ {
        Timestamp: @t,
        level: @l,
        message: @m, 
        Properties: {..@p},
        Renderings: @r}
    }") { }
    }
}
