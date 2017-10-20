using AutoMapper;
using Empired.Data.Infrastructure;
using Empired.Data.Repositories;
using Empired.Entities;
using Empired.Web.Infrastructure.Core;
using Empired.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Empired.Web.Infrastructure.Extensions;
using Empired.Data.Extensions;

namespace Empired.Web.Controllers
{
    [Authorize(Roles="Admin")]
    [RoutePrefix("api/patients")]
    public class PatientController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Patient> _customersRepository;

        public PatientController(IEntityBaseRepository<Patient> customersRepository, 
            IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _customersRepository = customersRepository;
        }

        public HttpResponseMessage Get(HttpRequestMessage request, string filter)
        {
            filter = filter.ToLower().Trim();

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var customers = _customersRepository.GetAll()
                    .Where(c => c.Email.ToLower().Contains(filter) ||
                    c.FirstName.ToLower().Contains(filter) ||
                    c.LastName.ToLower().Contains(filter)).ToList();

                var customersVm = Mapper.Map<IEnumerable<Patient>, IEnumerable<PatientViewModel>>(customers);

                response = request.CreateResponse<IEnumerable<PatientViewModel>>(HttpStatusCode.OK, customersVm);

                return response;
            });
        }

        [Route("details/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var customer = _customersRepository.GetSingle(id);

                PatientViewModel customerVm = Mapper.Map<Patient, PatientViewModel>(customer);

                response = request.CreateResponse<PatientViewModel>(HttpStatusCode.OK, customerVm);

                return response;
            });
        }

        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register(HttpRequestMessage request, PatientViewModel customer)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    if (_customersRepository.UserExists(customer.Email, customer.IdentityCard))
                    {
                        ModelState.AddModelError("Invalid user", "Email or Identity Card number already exists");
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                    }
                    else
                    {
                        Patient newCustomer = new Patient();
                        newCustomer.UpdateCustomer(customer);
                        _customersRepository.Add(newCustomer);

                        _unitOfWork.Commit();

                        // Update view model
                        customer = Mapper.Map<Patient, PatientViewModel>(newCustomer);
                        response = request.CreateResponse<PatientViewModel>(HttpStatusCode.Created, customer);
                    }
                }

                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, PatientViewModel customer)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    Patient _customer = _customersRepository.GetSingle(customer.ID);
                    _customer.UpdateCustomer(customer);

                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

        [HttpGet]
        [Route("search/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Patient> customers = null;
                int totalCustomers = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    customers = _customersRepository.FindBy(c => c.LastName.ToLower().Contains(filter) ||
                            c.IdentityCard.ToLower().Contains(filter) ||
                            c.FirstName.ToLower().Contains(filter))
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalCustomers = _customersRepository.GetAll()
                        .Where(c => c.LastName.ToLower().Contains(filter) ||
                            c.IdentityCard.ToLower().Contains(filter) ||
                            c.FirstName.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    customers = _customersRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalCustomers = _customersRepository.GetAll().Count();
                }

                IEnumerable<PatientViewModel> customersVM = Mapper.Map<IEnumerable<Patient>, IEnumerable<PatientViewModel>>(customers);

                PaginationSet<PatientViewModel> pagedSet = new PaginationSet<PatientViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalCustomers,
                    TotalPages = (int)Math.Ceiling((decimal)totalCustomers / currentPageSize),
                    Items = customersVM
                };

                response = request.CreateResponse<PaginationSet<PatientViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }
    }
}
