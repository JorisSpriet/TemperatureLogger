namespace UitlezenLogger
{

    public interface IView
    {
        object DataContext { get; set; }
    }

    public interface IView<T> : IView where T : IViewModel
    {

    }
}