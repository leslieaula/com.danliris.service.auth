namespace Com.DanLiris.Service.Auth.Lib.Interfaces
{
    public interface IMap<TModel, TViewModel>
    {
        TViewModel MapToViewModel(TModel model);

        TModel MapToModel(TViewModel viewModel);
    }
}
