using Com.Moonlay.Models;

namespace Com.DanLiris.Service.Auth.Lib.Helpers
{
    class MappingEntity<TModel, TViewModel>
        where TModel : StandardEntity
        where TViewModel : BasicOldViewModel
    {
        public void MappingToOldEntity(TModel model, TViewModel viewModel)
        {
            viewModel._id = model.Id;
            viewModel._deleted = model._IsDeleted;
            viewModel._active = model.Active;
            viewModel._createdDate = model._CreatedUtc;
            viewModel._createdBy = model._CreatedBy;
            viewModel._createAgent = model._CreatedAgent;
            viewModel._updatedDate = model._LastModifiedUtc;
            viewModel._updatedBy = model._LastModifiedBy;
            viewModel._updateAgent = model._LastModifiedAgent;
        }

        public void mappingToNewEntity(TModel model, TViewModel viewModel)
        {
            model.Id = viewModel._id;
            model._IsDeleted = viewModel._deleted;
            model.Active = viewModel._active;
            model._CreatedUtc = viewModel._createdDate;
            model._CreatedBy = viewModel._createdBy;
            model._CreatedAgent = viewModel._createAgent;
            model._LastModifiedUtc = viewModel._updatedDate;
            model._LastModifiedBy = viewModel._updatedBy;
            model._LastModifiedAgent = viewModel._updateAgent;
        }
    }
}
