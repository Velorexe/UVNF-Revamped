using UVNF.Core.Entities;

namespace UVNF.Editor.Importer.Parser
{
    public interface IUVNFScriptParser
    {
        public UVNFScript CompileLines(UVNFScript script, string[] lines);

        public string ExportLines(UVNFScript script);
    }
}
