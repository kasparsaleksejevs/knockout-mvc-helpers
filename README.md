# knockout-mvc-helpers
Project to ease the development and reduce the code amount of Knockout-MVC5 applications through HtmlHelpers and code generation

# Usage

1. Define data class that will correspond to the table data row. You can add DisplayAttribute (or DescriptionAttribute - for legacy projects) with column title.

```C#
public class MyRow
{
    [Display(Name = "Title for Int")]
    public int IntValue { get; set; }
    
    [Display(Name = "Title for String")]
    public string StringValue { get; set; }
}
```

2. Create a table using html helper for IEnumerable data rows using strongly-typed linq-based syntax:

```C#
@Html.TableFor(Model.Rows).Columns(col =>
{
    col.Bind(m => m.IntValue);
    col.Bind(m => m.StringValue);
    col.Bind(m => m.StringValue + ":" + m.IntValue).Title("Combined column");
})
```

and that's it! This will generate a simple table with headers and rows for your model. 

3. [optional] Customize the generated table using helper methods, like, apply custom css classes for whole table or columns, add custom footer or appply formatting for the cell values.


# Work in Progress
- KnockMvc.TypeScriptGenerator - library to generate TypeScript viewmodels and WEB API proxy methods from C# classes/enums and WEB API methods. Currently viewmodel generation is working with few TODOs, proxy generation at initial stages
- KnockMvc - auto-generated Knockout binding for tables
