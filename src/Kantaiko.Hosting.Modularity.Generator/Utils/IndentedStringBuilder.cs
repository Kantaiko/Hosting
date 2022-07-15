using System.Text;

namespace Kantaiko.Hosting.Modularity.Generator.Utils;

internal class IndentedStringBuilder
{
    private const string Indent = "    ";

    private int _indent;

    private readonly StringBuilder _builder;

    public IndentedStringBuilder(int capacity)
    {
        _indent = 0;
        _builder = new StringBuilder(capacity);
    }

    public void Append(char c)
    {
        _builder.Append(c);
    }

    public void AppendLine()
    {
        _builder.AppendLine();
    }

    public void AppendLine(string line)
    {
        _builder.AppendLine(line);
    }

    public void AppendLine(char c)
    {
        _builder.Append(c);
        _builder.AppendLine();
    }

    public void AppendIndented(string text)
    {
        AppendIndent();
        _builder.Append(text);
    }

    public void AppendIndentedLine(string line)
    {
        AppendIndent();
        _builder.AppendLine(line);
    }

    public void AppendIndentedFormat(string format, params object[] args)
    {
        AppendIndent();
        _builder.AppendFormat(format, args);
    }

    public void AppendIndentedFormatLine(string format, params object[] args)
    {
        AppendIndent();
        _builder.AppendFormat(format, args);
        AppendLine();
    }

    public void AppendFormat(string format, params object[] args)
    {
        _builder.AppendFormat(format, args);
    }

    private void AppendIndent()
    {
        _builder.EnsureCapacity(_builder.Capacity + _indent * 4);

        for (var i = 0; i < _indent; i++)
        {
            _builder.Append(Indent);
        }
    }

    private void IncreaseIndent()
    {
        _indent++;
    }

    private void DecreaseIndent()
    {
        _indent--;
    }

    public void AppendOpenBrace()
    {
        AppendIndentedLine("{");
        IncreaseIndent();
    }

    public void AppendCloseBrace()
    {
        DecreaseIndent();
        AppendIndentedLine("}");
    }

    public StringBuilder ToBuilder()
    {
        return _builder;
    }

    public void Clear()
    {
        _builder.Clear();
    }
}
