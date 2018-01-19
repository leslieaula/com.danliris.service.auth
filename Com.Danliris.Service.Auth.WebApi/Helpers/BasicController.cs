using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.EntityFrameworkCore;
using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;
using Com.DanLiris.Service.Auth.Lib.Helpers;
using Com.DanLiris.Service.Auth.Lib.Interfaces;

namespace Com.DanLiris.Service.Auth.WebApi.Helpers
{
    public abstract class BasicController<TService, TModel, TViewModel, TDbContext> : Controller
        where TDbContext : DbContext
        where TModel : StandardEntity, IValidatableObject
        where TViewModel : BasicOldViewModel
        where TService : BasicService<TDbContext, TModel>, IMap<TModel, TViewModel>
    {
        private readonly TService Service;
        private string ApiVersion;

        public BasicController(TService Service, string ApiVersion)
        {
            this.Service = Service;
            this.ApiVersion = ApiVersion;
        }

        [HttpGet]
        public IActionResult Get(int Page = 1, int Size = 25, string Order = "{}", [Bind(Prefix = "Select[]")]List<string> Select = null, string Keyword = "")
        {
            try
            {
                Tuple<List<TModel>, int, Dictionary<string, string>, List<string>> Data = Service.ReadData(Page, Size, Order, Select, Keyword);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok<TModel, TViewModel>(Data.Item1, Service.MapToViewModel, Page, Size, Data.Item2, Data.Item1.Count, Data.Item3, Data.Item4);

                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("{_id}")]
        public IActionResult GetById([FromRoute] int _id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var model = Service.ReadDataById(_id);

                if (model == null)
                {
                    Dictionary<string, object> ResultNotFound =
                        new ResultFormatter(ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(ResultNotFound);
                }

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok<TModel, TViewModel>(model, Service.MapToViewModel);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("{_id}")]
        public IActionResult Put([FromRoute] int _id, [FromBody] TViewModel ViewModel)
        {
            try
            {
                this.ValidateViewModel(ViewModel);
                this.Service.Username = User.Identity.Name;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (_id != ViewModel._id)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                        .Fail();
                    return BadRequest(Result);
                }

                TModel model = Service.MapToModel(ViewModel);
                Service.UpdateData(_id, model);

                return NoContent();
            }
            catch (ServiceValidationExeption e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!Service.IsExists(_id))
                {
                    Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                    .Fail();
                    return NotFound(Result);
                }
                else
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                        .Fail();
                    return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] TViewModel ViewModel)
        {
            try
            {
                this.ValidateViewModel(ViewModel);
                this.Service.Username = User.Identity.Name;

                TModel model = Service.MapToModel(ViewModel);
                Service.CreateData(model);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(HttpContext.Request.Path, "/", ViewModel._id), Result);
            }
            catch (ServiceValidationExeption e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpDelete("{_id}")]
        public IActionResult Delete([FromRoute] int _id)
        {
            try
            {
                this.Service.Username = User.Identity.Name;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Service.DeleteData(_id);

                return NoContent();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        void ValidateViewModel(TViewModel ViewModel)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(ViewModel, this.Service.ServiceProvider, null);

            if (!Validator.TryValidateObject(ViewModel, validationContext, validationResults, true))
                throw new ServiceValidationExeption(validationContext, validationResults);
        }
    }
}