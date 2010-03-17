namespace Metsys.WebOp.Runner
{
    public interface ICommand
    {
        void Execute(string rootPath);
    }
}