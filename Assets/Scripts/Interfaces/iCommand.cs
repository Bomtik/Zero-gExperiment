namespace Interfaces.Command
{
    public interface ICommand
    {
        void Execute();
        void Execute(int value);
    }
}