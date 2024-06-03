using System.ComponentModel.DataAnnotations.Schema;

namespace DocsManager.Models.docs;

[Table("Templates", Schema = "docs")]
public class Template
{
    public int TemplateId { get; set; }
    public string HtmlString { get; set; }
    public string TemplateModel { get; set; }
}