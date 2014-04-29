using Company.Commerce.Service.Validation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.Utility
{
    public class ServiceOperationResult
    {
        #region Properties

        private Boolean _succeeded;

        public Boolean Succeeded
        {
            get
            {
                return Errors.Any() ? false : _succeeded;
            }
            set
            {
                _succeeded = value;
            }
        }

        #endregion

        public ServiceOperationResult(params ValidationError[] errors)
        {
            Errors = new List<ValidationError>();

            Errors.AddRange(errors);
        }

        public List<ValidationError> Errors { get; private set; }
    }

    public class ServiceOperationResult<TData> : ServiceOperationResult
    {
        /// <summary>
        /// Contains any data that might need to be sent.
        /// </summary>
        private TData _data;

        public ServiceOperationResult(params ValidationError[] errors)
            : base(errors) { }

        public TData Data { get; set; }
    }
}
