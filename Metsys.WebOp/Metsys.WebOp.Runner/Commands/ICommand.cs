namespace Metsys.WebOp.Runner
{
    public interface ICommand
    {
        void Execute(string rootPath);
        void AddParameter(string parameter);
    }
}